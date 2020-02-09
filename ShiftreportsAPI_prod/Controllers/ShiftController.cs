using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

using BiomedicaLib.Net;
using shiftreportapp.data;

using ExpenseTrackerLib.Rest;
using ExpensTrackerAPI.App_Code;
using shiftreportapp.Rest;
using ShiftreportLib;
using ShiftreportLib.Helpers;

namespace ExpensTrackerAPI.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ShiftController : ApiController
    {

		//S3 s3 = new S3();

		String ConnectionString
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["ConnectionString"];
            }
        }
        String NotificationEmail
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["NotificationEmail"];
            }
        }
        String PortalUrl
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["PortalUrl"];
            }
        }
        String ReportPath
        {
            get
            {
                return System.Configuration.ConfigurationSettings.AppSettings["ReportsPath"];
            }
        }
		String CashReportPath
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["CashReportsPath"];
			}
		}


		cashier_mst_helper cH = new cashier_mst_helper();

        AppModel Context = new AppModel();
        /// <summary>
        /// Autherise user
        /// </summary>
        /// <param name="data">casheir_id</param>
        /// <returns>casheir_name</returns>
        [HttpPost]
        public HttpResponseMessage cashier_login(AddnewShiftDM data)
        {
            try
            {
                /* Disabled By Adil
				if (data.store_id == null)
					data.store_id = 0;
                String sql = "Select  id ,cashier_name AS Name,store_id from cashier_mst where id='" + data.username + "' and cashier_pw='" + data.password + "'";
                var u = Context.Database.SqlQuery<cashierAuthDM>(sql).FirstOrDefault();
                
                */
                int shift_id = -1;
                int RegisterId_on_duty = -1;
                //Added By Adil
                int cashier_id = Convert.ToInt32(data.username);
                var cashier = Context.cashier_mst2.Where(o => o.id ==cashier_id).FirstOrDefault();

                

                if(cashier == null)
                {
                     throw new AppException(data.username, "-1");
                }
                var vc_store_profile_mst = Context.vc_store_profile_mst2.Where(s => s.store_id == cashier.store_id).FirstOrDefault();

                var plan_no = cH.GetCasherStorePlanNo(cashier.id);

				String corp_pay_stat = Context.Database.SqlQuery<String>("select c.payment_status from corporate_mst c inner join store_profile_mst s on s.corp_id=c.id inner join cashier_mst ca on ca.store_id = s.id and ca.id=" + cashier.id).FirstOrDefault();

				var shift = Context.shift_details_mst2.Where(r => r.cashier_id == cashier.id && r.shift_status.Equals("P"));
 
                foreach (var x in shift)
                {
                    shift_id = x.id;
                    RegisterId_on_duty = (int)x.RegisterId_on_duty;
                }
             
				var plans_mst_d = Context.plan_mst2.Find(plan_no);
				var vc = Context.vc_StoreRegisters2.Where(r => r.store_id == cashier.store_id).ToList();
                int is_mobile_register = -1;
				var rod = vc.Where(o=> o.Id== RegisterId_on_duty).FirstOrDefault();
                if (rod != null)
                {
                    is_mobile_register = (int)rod.IsMobileRegister;
                }

				return Request.CreateResponse(HttpStatusCode.OK, new  
                {
                    casher_id = cashier.id,
                    store_id = cashier.store_id,
                    casher_name = cashier.cashier_name,
                    shift_id=shift_id,
					plan_no= plan_no,
					payment_status= corp_pay_stat,
					plan_type= plans_mst_d.plan_type,
					plan_label= plans_mst_d.plan_label,
					discount= plans_mst_d.discount,
					frequency= plans_mst_d.frequency,
                    plan_mst = plans_mst_d,
                    vc_StoreRegisters =vc,
					RegisterId_on_duty= RegisterId_on_duty,
					is_mobile_register= is_mobile_register,
                    is_tips = vc_store_profile_mst.is_tips,
                    is_gas_station=vc_store_profile_mst.is_gas_station,
					business_type =vc_store_profile_mst.business_type
                });
            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
            }
            catch (Exception err)
            {
                //ErrorLog.WriteToErrorLog(err.Message, err);
				if(err.InnerException==null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@linkedlive.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@linkedlive.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
              
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }
      
        /// <summary>
        /// Update new Shift
        /// </summary>
        /// <param name="data">{casherid,cashare_pw}</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage BusinessShiftsInfo([FromUri] int casheir_id)
        {
            try
            {
                var caheir = Context.cashier_mst2.Find(casheir_id);
                var store = Context.store_profile_mst2.Find(caheir.store_id);
                return Request.CreateResponse(HttpStatusCode.OK, AppHelpers.CreateBuismesinfo(store.id));
            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
            }
            catch (Exception err)
            {
				if (err.InnerException == null)
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.Message);
				}
				else
				{
					MailLogService.SendMail("shiftmanager@shiftReports.com", "henry@shiftreports.com", "Erorr while Casheir Loged in", err.InnerException.Message);
				}
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

		[HttpPost]
		public HttpResponseMessage Cash_Reports(CreateShiftDM data)
		{
			try
			{
				Object result = null;
				string date = DateTime.Now.ToShortDateString();

				ShiftDetails sh = new ShiftDetails();
				var shift = Context.cashier_shift_pending_mst2.Where(r => r.cashier_id == data.casheir_id && r.shift_num == data.shift_no && r.shift_date.Equals(date)).FirstOrDefault();
				if (shift == null)
				{
					result = AppHelpers.CreateNewShiftForStoreid(data.casheir_id, data.shift_no);
					//	Context.Database.ExecuteSqlCommand("exec usp_sr_do_post_creatnewshift_V2 @shift_id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey + ", @cashier_id=" + data.casheir_id);

					Context.Database.ExecuteSqlCommand("exec usp_sr_do_post_creatnewshift_V2 @shift_id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey);



					var shift_session_started = data.shift_session_started;
					//shiftm.shift_session_ended = MilitaryToDateTime(DateTime.Now);// Convert.ToInt32(DateTime.Now.ToString("hhmm"));

					var shift_date = data.shift_date;
					String SQL = "Update a Set a.shift_session_started='" + shift_session_started + "',a.shift_date='" + shift_date + "' from shift_details_mst a where id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;


					Context.Database.ExecuteSqlCommand(SQL);
					// var shiftpe = new shiftreportapp.data.AppModel.cashier_shift_pending_mst();
					//  shiftpe.cashier_id = data.casheir_id;
					//  shiftpe.shift_date = date;
					//  shiftpe.shift_num = data.shift_no;
					//  shiftpe.shift_status = "P";




					//  shiftpe.shift_id = ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;
					// Context.cashier_shift_pending_mst2.Add(shiftpe);
					// Context.SaveChanges();

				}
				else
				{
					result = AppHelpers.GetShiftForStoreid(shift.shift_id);
				}
				//MailLogService.SendMail("shiftManager@shiftreports.com", NotificationEmail, "Casheir Started a new shift", shift.shift_id + " has started a new shift (Shift #:shiftpe.shift_num,Date: shiftpe.shift_date");
				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@linkedlive.com", "Erro When createing a new shift", err.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}

		[HttpPost]
		public HttpResponseMessage CreateShiftReport(CreateShiftDM data)
		{
			try
			{
				Object result = null;
				string date = DateTime.Now.ToShortDateString();

				ShiftDetails sh = new ShiftDetails();
				var shift = Context.cashier_shift_pending_mst2.Where(r => r.cashier_id == data.casheir_id && r.shift_num == data.shift_no && r.shift_date.Equals(date)).FirstOrDefault();
				if (shift == null)
				{
					result = AppHelpers.CreateNewShiftForStoreid(data.casheir_id, data.shift_no);
					

					Context.Database.ExecuteSqlCommand("exec usp_sr_do_post_creatnewshift_V2 @shift_id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey);
                    
                    /* Henry Code disabled by Adil
					using (AppModel Context2 = new AppModel())
					{
						var s = Context2.shift_details_mst2.Find(shift.shift_id);
						s.RegisterId_on_duty = data.RegisterId_on_duty;
						s.Tip_Payout_Status = "Tips_In_Progress";
						Context2.Entry(s).State = System.Data.Entity.EntityState.Modified;
						Context2.SaveChanges();
						Context2.Dispose();
					}
                    */

                    /* Henry Code disabled by Adil
					using(AppModel Context3=new AppModel())
					{
						var v = new vc_shift_register_progress();
						
						v.created_userid = "system";
						v.create_dt = DateTime.Now;
						v.cashier_id_on_duty = data.casheir_id;
						v.store_id = ((shiftreportapp.data.AppModel.ShiftResult)result).store_id;
						v.register_id = data.RegisterId_on_duty;
						v.shift_id = ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;
						Context3.vc_shift_register_progress2.Add(v);
						Context3.SaveChanges();
						Context3.Dispose();
					}
		            */			
        
						var shift_session_started = data.shift_session_started;
					//shiftm.shift_session_ended = MilitaryToDateTime(DateTime.Now);// Convert.ToInt32(DateTime.Now.ToString("hhmm"));

					var shift_date = data.shift_date;
					String SQL = "Update a Set a.shift_session_started='" + shift_session_started + "',a.shift_date='" + shift_date + "' from shift_details_mst a where id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;
					Context.Database.ExecuteSqlCommand(SQL);


                    // Added By ADIL

                    var shiftDetailsMst = Context.shift_details_mst2.Find(((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey);
                    shiftDetailsMst.Tip_Payout_Status = "TIPS_IN_PROGRESS";
                    shiftDetailsMst.Tip_Payout_Amount = 0;
                    shiftDetailsMst.RegisterId_on_duty = data.RegisterId_on_duty;



                    var storeRegister = Context.vc_StoreRegisters2.Find(data.RegisterId_on_duty);
                    if (storeRegister != null)
                    {
                        storeRegister.Cashier_ID_on_duty = data.casheir_id;
                        storeRegister.shift_id = ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;
                        
                    }
                        
                    


                    




                    // save to DB
                    Context.SaveChanges();

                }
				else
				{
					result = AppHelpers.GetShiftForStoreid(shift.shift_id);
				}
                
                //MailLogService.SendMail("shiftManager@shiftreports.com", NotificationEmail, "Casheir Started a new shift", shift.shift_id + " has started a new shift (Shift #:shiftpe.shift_num,Date: shiftpe.shift_date");
                return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@linkedlive.com", "Erro When createing a new shift", err.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}



		[HttpPost]
		public HttpResponseMessage CreateShiftReportV2(CreateShiftDM data)
		{
			try
			{
				Object result = null;
				string date = DateTime.Now.ToShortDateString();

				ShiftDetails sh = new ShiftDetails();
				var shift = Context.cashier_shift_pending_mst2.Where(r => r.cashier_id == data.casheir_id && r.shift_num == data.shift_no && r.shift_date.Equals(date)).FirstOrDefault();
				if (shift == null)
				{
					result = AppHelpers.CreateNewShiftForStoreid(data.casheir_id, data.shift_no);
					

					Context.Database.ExecuteSqlCommand("exec usp_sr_do_post_creatnewshift_V2 @shift_id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey);



					var shift_session_started = data.shift_session_started;
					
					var shift_date = data.shift_date;
					String SQL = "Update a Set a.shift_session_started='" + shift_session_started + "',a.shift_date='" + shift_date + "' from shift_details_mst a where id=" + ((shiftreportapp.data.AppModel.ShiftResult)result).shiftkey;


					Context.Database.ExecuteSqlCommand(SQL);
					

				}
				else
				{
					result = AppHelpers.GetShiftForStoreid(shift.shift_id);
				}
				//MailLogService.SendMail("shiftManager@shiftreports.com", NotificationEmail, "Casheir Started a new shift", shift.shift_id + " has started a new shift (Shift #:shiftpe.shift_num,Date: shiftpe.shift_date");
				return Request.CreateResponse(HttpStatusCode.OK, result);
			}
			catch (AppException ex)
			{
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@linkedlive.com", "Erro When createing a new shift", err.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}
		}


		[HttpPost]
        public HttpResponseMessage SubmitShiftReport(submitDN data)
        {
			String reportfullpath=ReportPath+data.shiftkey + ".json";
            try
            {
               
			   shift_details_mst_helper h = new shift_details_mst_helper();
				if(h.getShiftStatus(Convert.ToInt32(data.shiftkey)).Equals("D"))
				{
					throw new AppErrorException(0x01,"Attempt to resubmit the same shift");
				}
			  
               string filename=reportfullpath;
			   
               System.IO.File.WriteAllText(filename, data.report.ToString());
               System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
               ShiftDetails sh = new ShiftDetails();
                string json=System.IO.File.ReadAllText(filename);
                String msg = "";

				int sid = Convert.ToInt32(data.shiftkey);

				sh.ParseData(Convert.ToInt32(data.shiftkey), json, ConnectionString);
                
				h.DoPostSubmit(data.shiftkey);
				h.setShiftDone(Convert.ToInt32(data.shiftkey));
               AppModel con = new AppModel();
                string email= (from x in con.store_profile_mst2
                                    where x.id == (from y in con.shift_details_mst2
                                                   where y.id==sid
                                                   select y.store_id).FirstOrDefault()
                                    select x.store_email).FirstOrDefault();
                
               MailLogService.SendMail("support@shiftReports.com", email, "Shift Report Summary for Shift ID " + data.shiftkey.ToString(), msg);
               
              
              
			   return Request.CreateResponse(HttpStatusCode.OK, new { result = "success", code = data.shiftkey});
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "App Error", ex.Message);
				
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
			catch(AppErrorException ex)
			{
				System.IO.File.Delete(reportfullpath);
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "App Error", ex.ErrorDisc);
				return Request.CreateResponse(HttpStatusCode.OK, new { result="fail",code=ex.ErrorId});
			}
            catch (Exception err)
            {
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "Error", err.Message);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }


		[HttpPost]
		public HttpResponseMessage SubmitShiftReport_cashier(submitDN data)
		{
			String reportfullpath = CashReportPath + data.shiftkey + ".json";
			try
			{

				shift_details_mst_helper h = new shift_details_mst_helper();
				if (h.getShiftStatus(Convert.ToInt32(data.shiftkey)).Equals("D"))
				{
					throw new AppErrorException(0x01, "Attempt to resubmit the same shift");
				}

				string filename = reportfullpath;


				//
				//s3.UploadString(data.report, "qa1", data.shiftkey, true);


				System.IO.File.WriteAllText(filename, data.report.ToString());
				System.Web.Script.Serialization.JavaScriptSerializer jss = new System.Web.Script.Serialization.JavaScriptSerializer();
				ShiftDetails sh = new ShiftDetails();
				//string json = System.IO.File.ReadAllText(filename);
				string json = data.report;
				String msg = "";

				int sid = Convert.ToInt32(data.shiftkey);

				sh.ParseData_cash_app(Convert.ToInt32(data.shiftkey), json, ConnectionString);

				h.DoPostSubmit(data.shiftkey);
			    h.setShiftDone(Convert.ToInt32(data.shiftkey));
				AppModel con = new AppModel();
				string email = (from x in con.store_profile_mst2
								where x.id == (from y in con.shift_details_mst2
											   where y.id == sid
											   select y.store_id).FirstOrDefault()
								select x.store_email).FirstOrDefault();
				if (email != null)
					MailLogService.SendMail("support@shiftReports.com", email, "Shift Report Summary for Shift ID " + data.shiftkey.ToString(), "");



				return Request.CreateResponse(HttpStatusCode.OK, new { result = "success", code = data.shiftkey });
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
			catch (AppErrorException ex)
			{
				System.IO.File.Delete(reportfullpath);
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "App Error", ex.ErrorDisc);
				return Request.CreateResponse(HttpStatusCode.OK, new { result = "fail", code = ex.ErrorId });
			}
			catch (Exception err)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "henry@shiftreports.com", "Error", err.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
			}



		}


       
    }

    public class cashierAuthDM
    {
        public int id { set; get; }
		
		public string Name { set; get; }

        public int Shift_id { get; set; }

        public string shift_text { get; set; }

        public int store_id { get; set; }

    }
    public class CreateShiftDM
    {

        public int shift_no { get; set; }
        public int casheir_id { get; set; }
		public String shift_session_started { set; get; }
		public String shift_date { set; get; }

		public int RegisterId_on_duty { set; get; }

		public string Tip_Payout_Status { set; get; }

        public bool is_mobile_register { get; set; }



	}
    public class submitDN
    {
        public string shiftkey { set; get; }
        //public shiftreportapp.data.AppModel.cartons_of_cigarrets cartons_of_cigarrets { set; get; }
        //public cig_rackrowtray_close_mst cig_rackrowtray_close_mst { set; get; }
        public string report { set; get; }
    }
   
 
  
	
}
