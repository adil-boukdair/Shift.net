using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using ExpensTrackerAPI.App_Code;
using System.Web.Http;
using System.Web.Http.Cors;
using PayPal.Api;
using System.IO;
using BiomedicaLib.Net;
using System.Data.Entity;
using shiftreportapp.data;
using ShiftreportLib.Helpers;
using ShiftreportLib;

namespace ExpensTrackerAPI.Controllers
{
	[EnableCors(origins: "*", headers: "*", methods: "*")]
	public class BusinessController : ApiController
	{

		String html_root_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_root_path"];
			}
		}


		String html_folder_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_folder_path"];
			}
		}

		String S3_path
		{
			get
			{
				return html_root_path + html_folder_path;
			}
		}

		[HttpGet]
		public HttpResponseMessage login([FromUri] String manager_id, [FromUri] String manager_pw)
		{
			try
			{

				AppModel Context = new shiftreportapp.data.AppModel();
				shift_store_times_Context stContext = new shift_store_times_Context();
				int store_id = 0;
				string access_level = "";
				Boolean isValid = true;
				// Add decription later
				var db = Context.Database;

				var data1 = new List<store_sales_report_mst>();
				var data2 = new List<corp_sales_report_mst>();
				var data3 = new List<store_cash_diff_report_mst>();
				var data4 = new List<corp_cash_diff_report_mst>();
				var data8 = new List<store_profile_mst_col>();
				var mngr = new List<shiftreportapp.data.AppModel.storelist_dtls>().ToList().FirstOrDefault();
				var c = new List<shiftreportapp.data.AppModel.storelist_dtls>().ToList().FirstOrDefault();
				string mngr_name = "";
				var corp = new List<shiftreportapp.data.AppModel.corporate_mst>().ToList().FirstOrDefault();
				int managerCorpID = 0;

				var check = Context.managers_mst2.Where(m => m.manager_id == manager_id && m.manager_pw == manager_pw).FirstOrDefault();
				if (check != null)
				{
					managerCorpID = Convert.ToInt32(Context.managers_mst2.Find(Convert.ToInt32(manager_id)).corp_id);
					if (managerCorpID != null)
					{
						int corpID = Convert.ToInt32(managerCorpID);
						corp = Context.corporate_mst2.Find(corpID);
						mngr_name = Context.managers_mst2.Find(Convert.ToInt32(manager_id)).manager_name;
						if (corp.firsttimelogin_flag == 1)
						{
							//c = db.SqlQuery<shiftreportapp.data.AppModel.storelist_dtls>("select top 1 access_level,a.corp_id,b.store_id from storelist_dtls a inner join shift_details_mst b on a.store_id = b.store_id where a.managerID='" + select top 1 access_level,corp_id,store_id from storelist_dtls a where a.managerID=1114 order by a.store_id desc + "' order by a.store_id desc").FirstOrDefault();


							var Cc = db.SqlQuery<login_storelist_dtls_dm>("select top 1 access_level,corp_id,store_id from storelist_dtls a where a.managerID='" + manager_id + "' order by a.store_id desc").FirstOrDefault();

							if (Cc != null)
							{
								store_id = Cc.store_id;
								access_level = Cc.access_level;
							}
							// mngr = db.SqlQuery<shiftreportapp.data.AppModel.storelist_dtls>("Select b.* from managers_mst a inner join storelist_dtls b on a.id=b.managerID where a.manager_id='" + manager_id + "' and a.manager_pw='" + manager_pw + "'").ToList().FirstOrDefault();

							data1 = db.SqlQuery<store_sales_report_mst>("select Convert(float,total_day_sales) as total_day_sales,date_of_sales from store_sales_report_mst where store_id=" + store_id).ToList();


							data2 = db.SqlQuery<corp_sales_report_mst>("select * from corp_sales_report_mst where corp_id=" + managerCorpID).ToList();

							data3 = db.SqlQuery<store_cash_diff_report_mst>("select * from store_cash_diff_report_mst where store_id=" + store_id).ToList();

							data4 = db.SqlQuery<corp_cash_diff_report_mst>("Select * from corp_cash_diff_report_mst where corp_id=" + managerCorpID).ToList();

							var data6 = stContext.shift_store_times.Where(r => r.store_id == store_id).ToList();

							var data7 = db.SqlQuery<shift_checklist_mst_login_resp>("Select id,q_type,q_uuid,q_text from shift_checklist_store_mst where store_id=" + store_id).ToList();
							data8 = db.SqlQuery<store_profile_mst_col>("Select top 1 a.id,a.store_name,a.store_address1,a.store_address2,a.store_zip,a.store_city,a.store_state,a.managers_num,b.is_tips from store_profile_mst a,vc_store_profile_mst b where a.id=b.store_id and a.id in ( select store_id from storelist_dtls where corp_id=" + managerCorpID + " and managerID=" + manager_id.ToString() + ") order by a.id desc").ToList();

						}


						// mngr_name = db.SqlQuery<String>("Select manager_name from managers_mst where id=" + ).ToList().FirstOrDefault();

						// var str = db.SqlQuery<shiftreportapp.data.AppModel.store_profile_mst>("Select * from store_profile_mst where id=" + mngr.store_id).FirstOrDefault();
						//var data1= new List<store_sales_report_mst_col>();


					}
				}
				else
				{
					throw new AppException(manager_id, "-1");
				}

				var data9 = Context.plan_mst2.Find(corp.plan_id);

                // Venuecash check if corporate have at least one payout method

                int corpPayoutMethodsCount = Context.vc_CorpPayoutMethods2.Where(p => p.corp_id == managerCorpID).Count();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{

                    corpPayoutMethodsCount= corpPayoutMethodsCount,

                    managers_mst = new
					{
						corp_id = managerCorpID,
						manager_name = mngr_name
					},
					storelist_dtls = new
					{
						access_level = access_level,
						store_id = store_id
					},
					corporate_mst = new
					{
						acc_admin_id = corp.acct_admin_id,
						corp_name = corp.corp_name,
						corp_address1 = corp.corp_address1,
						corp_city = corp.corp_city,
						corp_state = corp.corp_state,
						corp_phone = corp.corp_phone,
						corp_email = corp.corp_email,
						corp_wif_name = "",
						corp_wifi_password = "",
						firsttimelogin_flag = corp.firsttimelogin_flag,
						corp_payment_status = corp.payment_status,
						referral_source=corp.referral_source,
						plan_id=corp.plan_id

					},
					store_sales_report_mst = data1,
					corp_sales_report_mst = data2,
					store_cash_report_mst = data3,
					corp_cash_diff_report_mst = data4,
					store_list = data8,
					plan_mst=data9
				});
			}
			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.OK, ex.Msg);
			}
			catch (Exception ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.InnerException.Message);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_stores_list([FromUri] int corp_id, [FromUri] String manager_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				//var data=  db.SqlQuery<store_profile_mst_col>("Select a.id,a.store_name,a.store_address1,a.store_city,a.store_state,a.managers_num from store_profile_mst a where id in ( select store_id from storelist_dtls where corp_id=" + corp_id.ToString() + " and managerID=" + manager_id.ToString() + ")").ToList();

				var data = db.SqlQuery<store_profile_mst_col>("exec usp_sr_get_stores_list @corp_id=" + corp_id.ToString() + " ,@managerID=" + manager_id.ToString()).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{

					store_profile_mst = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage save_corp_profile([FromBody] sve_corp_mst_req_param a)
		{
			try
			{
				// TODO: Add logic to update the modified_userid and date

				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("update managers_mst set manager_name='" + a.manager_name + "',manager_pw='" + a.manager_pw + "',manager_cell_phone='" + a.manager_cell_phone + "' where manager_id= " + a.manager_id);

				db.ExecuteSqlCommand("update corporate_mst set corp_name='" + a.corp_name + "',corp_address1='" + a.corp_address1 + "',corp_address2='" + a.corp_address2 + "',corp_city='" + a.corp_city + "',corp_state='" + a.corp_state + "',corp_phone='" + a.corp_phone + "',corp_fax='" + a.corp_fax + "',corp_email='" + a.corp_email + "',corp_zip='" + a.corp_zip + "' where id=" + a.corp_id);

				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_manager_next_id()
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				var m = db.SqlQuery<get_next_manger_id>("select id from managers_mst").ToList().LastOrDefault();
				int maxID = m.id + 1;
				db.ExecuteSqlCommand("insert into managers_mst(manager_id) values('" + maxID.ToString() + "')");
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					manager_mst = new
					{
						id = maxID
					}
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage add_manager([FromBody] add_manager_dm data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("exec usp_sr_addnewmanager @manger_id=" + data.manager_id + ",@manager_name='" + data.manager_name + "',@manager_pw='" + data.manager_password + "',@manager_cell_phone='" + data.manager_cell_phone + "',@manager_email='" + data.manager_email + "'");
				for (int i = 0; i < data.access_controle.Count; i++)
				{
					db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,store_type,access_level) VALUES(" + data.access_controle[i].store_id + "," + data.corp_id + "," + data.manager_id + ",'','" + data.access_controle[i].access_level + "')");
				}

				for (int i = 0; i < data.added_access_controle.Count; i++)
				{
					var mid = data.manager_id;
					db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,access_level) VALUES(" + data.added_access_controle[i].store_id + "," + data.corp_id + ",'" + mid + "','" + data.added_access_controle[i].access_level + "')");
				}
				db.ExecuteSqlCommand("update managers_mst set corp_id=" + data.corp_id + " where id=" + Convert.ToInt32(data.manager_id));



				var corpName = Context.corporate_mst2.Find(data.corp_id).corp_name;
 
				var xMan = Context.managers_mst2.Where(xm => xm.manager_id == data.manager_id).FirstOrDefault();

           
                
                // send multiple emails depending on stores access levels
                for (int i = 0; i < data.access_controle.Count; i++)
                {
                    if (data.access_controle[i].access_level != "N/A") { 
                    var store = Context.store_profile_mst2.Find(data.access_controle[i].store_id);
                    // Send VenueCash Email
                    
                    VenueCashEmails.newManagerBusiness(xMan.manager_email,xMan.manager_name ,corpName,xMan.Id.ToString(),xMan.manager_pw, store.store_name, data.access_controle[i].access_level);
                    }
                }


                   
 
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_current_managers([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int id = Convert.ToInt32(corp_id);
				var data = (from x in Context.managers_mst2
							where x.corp_id == id
							select new { manager_id = x.manager_id, manager_name = x.manager_name }).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					managers_mst = data

				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_manager([FromUri] String manager_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int id = Convert.ToInt32(manager_id);
				var db = Context.Database;
				var data = (from x in Context.managers_mst2
							where x.Id == id
							select new modify_manager_dm() { manager_id = x.Id.ToString(), manager_name = x.manager_name, manager_email = x.manager_email, manager_cellphone = x.manager_cell_phone, manager_password = x.manager_pw }).FirstOrDefault();
				data.access_controle = db.SqlQuery<mngr_accesslevel>("Select access_level,store_id,(select a.store_name from store_profile_mst a where a.id=store_id) as store_name from storelist_dtls where managerID=" + id.ToString()).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, data);
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage modify_manager([FromBody] add_manager_dm data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				String s = "exec usp_sr_addnewmanager @manger_id=" + data.manager_id + ",@manager_name='" + data.manager_name + "',@manager_pw='" + data.manager_password + "',@manager_cell_phone='" + data.manager_cell_phone + "',@manager_email='" + data.manager_email + "'";
				db.ExecuteSqlCommand(s);
				for (int i = 0; i < data.access_controle.Count; i++)
				{
					db.ExecuteSqlCommand("update storelist_dtls set access_level='" + data.access_controle[i].access_level + "' where managerID=" + data.manager_id + " and store_id=" + data.access_controle[i].store_id);

				}

				for (int i = 0; i < data.added_access_controle.Count; i++)
				{
					var mid = data.manager_id;
					db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,access_level) VALUES(" + data.added_access_controle[i].store_id + "," + data.corp_id + ",'" + mid + "','" + data.added_access_controle[i].access_level + "')");
				}

				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_corporate_dashboard([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var sr1 = db.SqlQuery<corp_sales_report_mst>("select * from corp_sales_report_mst where corp_id=" + corp_id).ToList(); // corp_sales_report

				var sr2 = db.SqlQuery<corp_cash_diff_report_mst>("select * from corp_cash_diff_report_mst where corp_id=" + corp_id).ToList(); // corp_cash_diff

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corp_id = corp_id,
					sales_report = sr1,
					cash_diff_report = sr2
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_store_dashboard([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				var sr1 = db.SqlQuery<store_sales_report_mst>("select * from store_sales_report_mst where store_id=" + store_id).ToList();

				var sr2 = db.SqlQuery<store_cash_diff_report_mst>("select * from store_cash_diff_report_mst where store_id=" + store_id).ToList();

				//var sum = db.SqlQuery<usp_sr_getshiftsummary_dm>("exec usp_sr_getshiftsummary @store_id=").ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					sales_report = sr1,
					cash_diff_report = sr2//,
										  //summary=sum
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_store_report_summary([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;



				//var sum = db.SqlQuery<usp_sr_getshiftsummary_dm>("select a.shift_id,a.cash_sales,a.shift_report_ttaf,a.shift_report_cash_counted as shift_report_cash_counted,a.shift_report_cash_difference as shift_report_cash_difference,c.cashier_name,s.shift_date  " +
				//  "from manager_report_mst a " +
				//  " inner join shift_details_mst s on a.shift_id=s.id and s.store_id=" + store_id +
				//  " inner join cashier_mst c on s.cashier_id=c.id" +
				//  " inner join store_profile_mst ss on s.store_id=ss.id").ToList();

				var sum = db.SqlQuery<usp_sr_getshiftsummary_dm>("exec usp_sr_getshiftsummary @store_id="+ store_id).ToList();
				var rd = (new DB_rackset_diff_mst()).Getrackset_diff_mstForStore(Convert.ToInt32(store_id));
				var rackset = (new DB_rackset_mst()).Getrackset_mstByStoreID(Convert.ToInt32(store_id));
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					//rack_set=rackset,
					summary = sum,
					rack_set_diff=rd
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		[HttpGet]
		public HttpResponseMessage get_corporate_cash_difference([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				var data = db.SqlQuery<corp_cash_diff_report_mst>("select * from corp_cash_diff_report_mst where corp_id=" + corp_id).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corp_id = corp_id,
					cash_difference = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_store_cash_difference([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				var data = db.SqlQuery<store_cash_diff_report_mst>("select * from store_cash_diff_report_mst where store_id=" + store_id).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					cash_difference = data


				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_coporate_mop_sales([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var data = db.SqlQuery<mop_sales_resp>("select daily_cash_sales,daily_credit_sales,daily_debit_sales,date_of_sales from corp_mop_sales_report_mst where corp_id=" + corp_id).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corp_id = corp_id,
					mop_sales = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_store_mop_sales([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var data = db.SqlQuery<shiftreportapp.data.AppModel.store_mop_sales_report_mst>("select * from store_mop_sales_report_mst where store_id=" + store_id).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					mop_sales = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}



		[HttpGet]
		public HttpResponseMessage get_corporate_total_sales([FromUri] String corp_id)
		{
			try
			{

				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				int sID = Int32.Parse(corp_id);
				var data = db.SqlQuery<corp_sales_report_mst>("select * from corp_sales_report_mst where corp_id=" + sID).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corp_id = corp_id,
					sales_reports = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		

		[HttpGet]
		public HttpResponseMessage get_store_total_sales([FromUri] String store_id)
		{
			try
			{


				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				int sID = Int32.Parse(store_id);
				var data = db.SqlQuery<shiftreportapp.data.store_sales_report_mst>("select total_day_sales,date_of_sales from store_sales_report_mst where store_id=" + sID);
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					sales_reports = data

				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_store_employee_hours([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var id = Convert.ToInt32(store_id);
				var db = Context.Database;

				var data = Context.store_employee_hours_report_mst2.Where(r => r.store_id == id).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_id = store_id,
					employee_hours_reports = data
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_corp_profile([FromUri] String manager_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int id = Convert.ToInt32(manager_id);
				var mngr = (from x1 in Context.managers_mst2
							where x1.Id == id
							select new { corp_id = x1.corp_id, manager_name = x1.manager_name, manager_pw = x1.manager_pw, manager_cell_phone = x1.manager_cell_phone }).FirstOrDefault();
				var corp = (from s in Context.corporate_mst2
							where s.Id == mngr.corp_id
							select new { corp_name = s.corp_name, corp_address1 = s.corp_address1, corp_address2 = s.corp_address2, corp_city = s.corp_city, corp_state = s.corp_state, corp_phone = s.corp_phone, corp_fax = s.corp_fax, corp_email = s.corp_email, corp_zip = s.corp_zip }).FirstOrDefault();

				var data = new List<sales_report_data>();
				//var pw = AES.DecryptData(mngr.manager_pw);
				//mngr.manager_pw = 

				return Request.CreateResponse(HttpStatusCode.OK, new
				{

					corporate_mst = new
					{
						corp_name = corp.corp_name,
						corp_address1 = corp.corp_address1,
						corp_address2 = corp.corp_address2,
						corp_city = corp.corp_city,
						corp_state = corp.corp_state,
						corp_phone = corp.corp_phone,
						corp_fax = corp.corp_fax,
						corp_email = corp.corp_email,
						corp_zip = corp.corp_zip,
						manager_name = mngr.manager_name,
						manager_pw = mngr.manager_pw,
						manager_cell_phone = mngr.manager_cell_phone
					}

				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_corporate_submitted_shifts([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int id = Convert.ToInt32(corp_id);
				var data = (from x in Context.shift_details_mst2
							where x.corp_id == id && x.shift_status.Equals("D")
							select new get_corporate_submitted_shifts_get_resp() { id =					x.id.ToString(),
						store_id = (int)x.store_id,
						store_name = Context.store_profile_mst2.Where(s => s.id ==	x.store_id).FirstOrDefault().store_name,
						cashier_name = x.cashier_name, cash_counted = x.cash_counted.ToString(),
						shift_session_started = x.shift_session_started,
					    shift_session_ended = x.shift_session_ended,
						shift_date = x.shift_date,
						shift_num = x.shift_num.ToString() }).ToList();

				
					for (int i = 0; i < data.Count; i++)
					{
						if (data[i].shift_session_ended == null || data[i].shift_session_started == null)
						{

						}
						else
						{
							TimeSpan span = TimeSpan.FromMinutes(Convert.ToInt32(data[i].shift_session_ended - data[i].shift_session_started));
							string label = span.ToString(@"hh\:mm");
							data[i].houres_loggedin = label;
						}

					}

				

				return Request.CreateResponse(HttpStatusCode.OK, new

				{
					shift_details_mst = data
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage getshiftdetails([FromUri] String store_id, [FromUri] String id, [FromUri] String shift_num)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int Id = Convert.ToInt32(id);
				int storeid = Convert.ToInt32(store_id);
				int shiftID = Convert.ToInt32(id);
				var db = Context.Database;
				var shift = Context.shift_details_mst2.Find(Id);
				var store = Context.store_profile_mst2.Find(storeid);
				var cashier = Context.cashier_mst2.Find(Convert.ToInt32(shift.cashier_id));
				var opend_drawer = Context.drawer_open_mst2.Where(r => r.shift_id == shiftID).ToList();
				var close_drawer = Context.drawer_close_mst2.Where(r => r.shift_id == shiftID).ToList();
				var safe_drop = db.SqlQuery<safe_drop_response>("Select safedrop_num,safedrop_amnt from safe_drops_mst where shift_id=" + Convert.ToInt32(id)).ToList();
				var question_cat_dtls = db.SqlQuery<question_cat_dtls_response>("select id,q_category_text from question_cat_dtls").ToList();
				var shift_checklist_store_mst = db.SqlQuery<shift_details_checklist_response>("select q_category_text from question_cat_dtls a inner join shift_checklist_mst b on a.id = b.category_id where store_id=" + store_id);
				//  var shift_checklist_mst = db.SqlQuery<shift_details_checklist_response>("select a.id,a.q_uuid,q_text,q_type,q_category,q_answer,a.shift_no from shift_checklist_mst a inner join shift_checklist_store_mst b on a.q_uuid = b.q_uuid where shift_id=" + shiftID);
				var shift_checklist_mst = db.SqlQuery<shift_details_checklist_response>("select distinct a.id,a.q_uuid,q_text,q_type,q_category,q_answer,a.shift_no from shift_checklist_mst a inner join shift_checklist_store_mst b on a.question_id = b.id where shift_id=" + shiftID);

                // VenueCash
                double cc_paid = 0;
                double vc_paid = 0;
                double ac_paid = 0;
                double of_paid = 0;
                if (Context.vc_CustomerTransactions2.Where(t=> t.shift_id == shiftID).Count() > 0)
                {
                 cc_paid = Context.vc_CustomerTransactions2.Where(t => t.shift_id == shiftID).Sum(t=> t.Paid_Credit_Amount ?? 0 );
                 vc_paid = Context.vc_CustomerTransactions2.Where(t => t.shift_id == shiftID).Sum(t => t.Paid_VenueCash_Amount ?? 0);
                 ac_paid = Context.vc_CustomerTransactions2.Where(t => t.shift_id == shiftID).Sum(t => t.Paid_AllCash_Amount ?? 0);
                 of_paid = Context.vc_CustomerTransactions2.Where(t => t.shift_id == shiftID).Sum(t => t.OtherFinancingAmount);
                }


                return Request.CreateResponse(HttpStatusCode.OK, new

                {
                    shift_details_mst = new
                    {
                        store_name = store.store_name,
                        cashier_name = cashier.cashier_name,
                        shift_date = shift.shift_date,
                        shift_open_time = shift.shift_open_time,
                        shift_close_time = shift.shift_close_time,
                        shift_session_started = shift.shift_session_started,
                        shif_session_ended = shift.shift_session_ended,
                        cash_counted = shift.cash_counted,
                        cash_sales_all_registers = shift.cash_sales_all_registers,
                        credit_sales_all_registers = shift.credit_sales_all_registers,
                        debit_sales_all_registers = shift.debit_sales_all_registers,
                        cash_sales_reg1 = shift.cash_sales_reg1,
                        credit_sales_reg1 = shift.credit_sales_reg1,
                        debit_sales_reg1 = shift.debit_sales_reg1,
                        cash_sales_reg2 = shift.cash_sales_reg2,
                        credit_sales_reg2 = shift.credit_sales_reg2,
                        debit_sales_reg2 = shift.debit_sales_reg2

                    },
                    drawer_open_mst = opend_drawer,
                    drawer_close_mst = close_drawer,
                    safe_drops_mst = safe_drop,
                    question_cat_dtls = question_cat_dtls,
                    shift_checklist_mst = shift_checklist_mst,

                    //VenueCash
                    cc_paid = cc_paid,
                    vc_paid = vc_paid,
                    ac_paid = ac_paid,
                    of_paid = of_paid

                });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_corporate_pending_shifts([FromUri] String corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int cid = Convert.ToInt32(corp_id);
				var data = (from x in Context.shift_details_mst2
							where x.corp_id == cid && x.shift_status.Equals("P")
							select new
							{
								store_name = Context.store_profile_mst2.Where(s => s.id == x.store_id).FirstOrDefault().store_name,
								id = x.id,
								shift_num = x.shift_num,
								shift_date = x.shift_date,
								cashier_name = x.cashier_name
							})
							.ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new

				{
					shift_details_mst = data
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage reset_pending_shifts([FromBody] reset_pending_shift_req_dm data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("update a set a.shift_status='" + data.shift_status + "' from shift_details_mst a where a.id=" + data.shift_id);

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					success = 1
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_store_customize_page([FromUri] int storeid)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int sid = Convert.ToInt32(storeid);
				var db = Context.Database;
				var store = Context.store_profile_mst2.Find(sid);
				var data1 = db.SqlQuery<shiftreportapp.data.AppModel.question_cat_dtls>("select id,q_category_text from question_cat_dtls").ToList();
				var data2 = db.SqlQuery<shift_checklist_store_mst_response>("Select id,q_uuid,q_type,q_category,q_text,shift_no from shift_checklist_store_mst where store_id=" + storeid).ToList();
				var data3 = db.SqlQuery<shiftreportapp.data.AppModel.shift_store_times>("select * from shift_store_times where store_id=" + storeid).ToList();

                // Get VenueCash data
                var vc_store_profile_mst = Context.vc_store_profile_mst2.Where(s => s.store_id == storeid).FirstOrDefault();
                // registers
                int registerCount = Context.vc_StoreRegisters2.Where(r => r.store_id == storeid && r.IsMobileRegister==0 && r.IsGasPump == 0).Count();
                // Pumps
                int pumpCount = Context.vc_StoreRegisters2.Where(r => r.store_id == storeid && r.IsMobileRegister == 0 && r.IsGasPump==1).Count();

                return Request.CreateResponse(HttpStatusCode.OK, new
				{
					store_profile_mst = new
					{
						store_name = store.store_name,
						store_address1 = store.store_address1,
						store_address2 = store.store_address2,
						store_city = store.store_city,
						store_zip = store.store_zip,
						store_state = store.store_state,
						store_phone = store.store_phone_no,
						store_email = store.store_email,
                        //VenueCash parameters
                        is_tips = vc_store_profile_mst.is_tips,
                        numberOfRegisters = registerCount,
                        cash_back_rate = vc_store_profile_mst.cash_back_rate,
                        gas_back_rate = vc_store_profile_mst.gas_back_rate,
                        is_gas_station = vc_store_profile_mst.is_gas_station,
                        is_mobile_register = vc_store_profile_mst.is_mobile_register,
                        numberOfGasPumps = pumpCount,
                        business_type = vc_store_profile_mst.business_type,
                        store_wifi_name = store.store_wifi_name,
						store_wifi_pw = store.store_wifi_pw
					},
					question_cat_dtls = data1,
					shift_checklist_store_mst = data2,
					shift_store_times = data3

				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpPost]
		public HttpResponseMessage update_store_customize_page([FromBody] update_store_req data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				//db.ExecuteSqlCommand("update a set a.store_name='" + data.update_store_customize_page_store_profile_mst_req.store_name + "',a.store_address1='" + data.update_store_customize_page_store_profile_mst_req.store_address1 + "',a.store_address2='" + data.update_store_customize_page_store_profile_mst_req.store_address2 + "',a.store_city='" + data.update_store_customize_page_store_profile_mst_req.store_city + "',a.store_zip=" + data.update_store_customize_page_store_profile_mst_req.store_zip + ",a.store_state='" + data.update_store_customize_page_store_profile_mst_req.store_state + "',a.store_phone_no='" + data.update_store_customize_page_store_profile_mst_req.store_phone + "',a.store_wifi_name='" + data.update_store_customize_page_store_profile_mst_req.store_wifi_name + "',a.store_wifi_pw='" + data.update_store_customize_page_store_profile_mst_req.store_wifi_pw + "',a.store_email='" + data.update_store_customize_page_store_profile_mst_req.store_email + "' from store_profile_mst a where a.id=" + data.update_store_customize_page_store_profile_mst_req.store_id);

                var storeProfileMst = Context.store_profile_mst2.Find(data.update_store_customize_page_store_profile_mst_req.store_id);
                storeProfileMst.store_name=data.update_store_customize_page_store_profile_mst_req.store_name;
                storeProfileMst.store_address1 = data.update_store_customize_page_store_profile_mst_req.store_address1;
                storeProfileMst.store_address2 = data.update_store_customize_page_store_profile_mst_req.store_address2;
                storeProfileMst.store_city = data.update_store_customize_page_store_profile_mst_req.store_city;
                storeProfileMst.store_zip = int.Parse( data.update_store_customize_page_store_profile_mst_req.store_zip);
                storeProfileMst.store_state = data.update_store_customize_page_store_profile_mst_req.store_state;
                storeProfileMst.store_phone_no = data.update_store_customize_page_store_profile_mst_req.store_phone;
                storeProfileMst.store_email = data.update_store_customize_page_store_profile_mst_req.store_email;
                storeProfileMst.store_wifi_name = data.update_store_customize_page_store_profile_mst_req.store_wifi_name;
                storeProfileMst.store_wifi_pw = data.update_store_customize_page_store_profile_mst_req.store_wifi_pw;



				for (int i = 0; i < data.shift_checklist_store_mst_response.Length; i++)
				{
					if (data.shift_checklist_store_mst_response[i].id == -1)
					{
						db.ExecuteSqlCommand("insert into shift_checklist_store_mst(q_uuid,q_type,q_category,q_text,store_id,shift_no) values('" + data.shift_checklist_store_mst_response[i].q_uuid + "','" + data.shift_checklist_store_mst_response[i].q_type + "'," + data.shift_checklist_store_mst_response[i].q_category + ",'" + data.shift_checklist_store_mst_response[i].q_text + "'," + data.update_store_customize_page_store_profile_mst_req.store_id + "," + data.shift_checklist_store_mst_response[i].shift_no + ")");
					}
					else
					{
						var d2 = data.shift_checklist_store_mst_response[i];
						db.ExecuteSqlCommand("Update shift_checklist_store_mst set q_uuid='" + d2.q_uuid + "', q_type='" + d2.q_type + "',q_category='" + d2.q_category + "',q_text='" + d2.q_text + "' where id=" + d2.id.ToString());
					}

				}
				for (int i = 0; i < data.deleted_checklist_items.Count; i++)
				{
					int id = data.deleted_checklist_items[i];
					db.ExecuteSqlCommand("delete from shift_checklist_store_mst where id=" + id);
				}



				for (int i = 0; i < data.shift_store_times.Length; i++)
				{
					var d3 = data.shift_store_times[i];
					db.ExecuteSqlCommand("update a set a.start_time='" + d3.start_time + "',a.end_time='" + d3.end_time + "' from shift_store_times a where id=" + d3.id.ToString());
				}

				for (int i = 0; i < data.new_shift_store_times.Length; i++)
				{
					var d3ins = data.new_shift_store_times[i];
					//db.ExecuteSqlCommand("update a set a.start_time='" + d3.start_time + "',a.end_time='" + d3.end_time + "' from shift_store_times a where id=" + d3.id.ToString());
					db.ExecuteSqlCommand("INSERT INTO shift_store_times(store_id,start_time,end_time,shift_no) VALUES(" + d3ins.store_id + ",'" + d3ins.start_time + "','" + d3ins.end_time + "'," + d3ins.shift_no + ")");

				}
				for (int i = 0; i < data.deleted_shift_time.Count; i++)
				{
					var d3id = data.deleted_shift_time[i];
					db.ExecuteSqlCommand("Delete from shift_store_times  where id=" + d3id.ToString());
				}



                // VenueCash Logic
                int store_id = data.update_store_customize_page_store_profile_mst_req.store_id;
                var vc_store_profile_mst = Context.vc_store_profile_mst2.Where(s => s.store_id == store_id).FirstOrDefault();

                if (vc_store_profile_mst != null)
                {
                    vc_store_profile_mst.is_tips = data.update_store_customize_page_store_profile_mst_req.is_tips;
                    vc_store_profile_mst.cash_back_rate = data.update_store_customize_page_store_profile_mst_req.cash_back_rate;
                    vc_store_profile_mst.gas_back_rate = data.update_store_customize_page_store_profile_mst_req.gas_back_rate;
                    vc_store_profile_mst.is_gas_station = data.update_store_customize_page_store_profile_mst_req.is_gas_station;
                    vc_store_profile_mst.is_mobile_register = data.update_store_customize_page_store_profile_mst_req.is_mobile_register;
                    vc_store_profile_mst.business_type = data.update_store_customize_page_store_profile_mst_req.business_type;

                    // Register logic  ############################################################################################################################
                    // get The register Count in DB
                    int registerCount = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id && r.IsMobileRegister == 0 && r.IsGasPump == 0).Count();

                    // check reg in db and reg entred by user
                    if (registerCount < data.update_store_customize_page_store_profile_mst_req.numberOfRegisters)
                    {

                        int RegisterToCreate = data.update_store_customize_page_store_profile_mst_req.numberOfRegisters - registerCount;

                        // creating Registers
                        for (int i = registerCount+1; i <= RegisterToCreate+ registerCount; i++)
                        {
                            vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                            storeRegister.create_dt = DateTime.Now;
                            storeRegister.created_userid = "System";
                            storeRegister.modify_dt = DateTime.Now;
                            storeRegister.Modify_usrid = "System";
                            storeRegister.RegisterId = -1;
                            storeRegister.store_id = store_id;
                            storeRegister.RegisterNumber = i.ToString();
                            storeRegister.IsGasPump = 0;
                            storeRegister.IsMobileRegister = 0;
                            storeRegister.Cashier_ID_on_duty = -1;
                            storeRegister.Cashier_ID_on_duty = -1;
                            storeRegister.Status = "NA";
                            storeRegister.shift_id = -1;
                            Context.vc_StoreRegisters2.Add(storeRegister);
                        }


                    }
                    else
                    {
                        int RegisterToRemove = registerCount - data.update_store_customize_page_store_profile_mst_req.numberOfRegisters ;


                        for (int i = 1; i <= RegisterToRemove; i++) { 
                            var register = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id && r.IsMobileRegister == 0 && r.IsGasPump == 0).OrderByDescending(r => r.RegisterNumber).FirstOrDefault();
                            Context.vc_StoreRegisters2.Remove(register);
                            Context.SaveChanges();
                        }
                    }
                    // End Register Logic

                    // Pumps logic ############################################################################################################################
                    // get The register Count in DB
                    int pumpCount = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id && r.IsMobileRegister == 0 && r.IsGasPump==1 ).Count();

                    // check reg in db and reg entred by user
                    if (pumpCount < data.update_store_customize_page_store_profile_mst_req.numberOfGasPumps)
                    {

                        int RegisterToCreate = data.update_store_customize_page_store_profile_mst_req.numberOfGasPumps - pumpCount;

                        // creating Registers
                        for (int i = pumpCount + 1; i <= RegisterToCreate + pumpCount; i++)
                        {
                            vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                            storeRegister.create_dt = DateTime.Now;
                            storeRegister.created_userid = "System";
                            storeRegister.modify_dt = DateTime.Now;
                            storeRegister.Modify_usrid = "System";
                            storeRegister.RegisterId = -1;
                            storeRegister.store_id = store_id;
                            storeRegister.RegisterNumber = i.ToString();
                            storeRegister.IsGasPump = 1;
                            storeRegister.IsMobileRegister = 0;
                            storeRegister.Cashier_ID_on_duty = -1;
                            storeRegister.Cashier_ID_on_duty = -1;
                            storeRegister.Status = "NA";
                            storeRegister.shift_id = -1;
                            Context.vc_StoreRegisters2.Add(storeRegister);
                        }


                    }
                    else
                    {
                        int PumpToRemove = pumpCount - data.update_store_customize_page_store_profile_mst_req.numberOfGasPumps;


                        for (int i = 1; i <= PumpToRemove; i++)
                        {
                            var register = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id && r.IsMobileRegister == 0 && r.IsGasPump==1).OrderByDescending(r => r.RegisterNumber).FirstOrDefault();
                            Context.vc_StoreRegisters2.Remove(register);
                            Context.SaveChanges();
                        }
                    }
                    // End Pump Logic




                    Context.SaveChanges();
                }







				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					success = 1
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_store_submitted_shifts([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var sid = Convert.ToInt32(store_id);

				var data = (from x in Context.shift_details_mst2
							where x.store_id == sid && x.shift_status.Equals("D")
							select new get_store_submitted_shifts_dm()
							{
								store_name = x.store_name,
								id = x.id,
								shift_num = x.shift_num,
								shift_date = x.shift_date,
								cashier_name = x.cashier_name,
								cash_counted = x.cash_counted,
								shift_session_started = x.shift_session_started,
								shift_session_ended = x.shift_session_ended,						
								date = x.shift_date,
                                total_sales = (Context.vc_CustomerTransactions2.Where(o => o.shift_id == x.id).Sum(o => (double?)o.TotalAmount) ?? 0),
                            })
							.ToList();
				//foreach(var item in data)
				//{
				//      db.ExecuteSqlCommand("exec usp_sr_total_day_sales_V2 @shift_id=" + item.id);
				//}

				//var data2 = db.SqlQuery<shiftreportapp.data.store_sales_report_mst>("select total_day_sales,date_of_sales from store_sales_report_mst where store_id=" + sid);

				for(int i = 0; i < data.Count; i++)
				{
					if (data[i].shift_session_ended == null || data[i].shift_session_started==null)
					{

					}
					else
					{
						TimeSpan span = TimeSpan.FromMinutes(Convert.ToInt32(data[i].shift_session_ended - data[i].shift_session_started));
						string label = span.ToString(@"hh\:mm");
						data[i].shift_hourloged = label;
					}
					
				}

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					shift_details_mst = data//,
											// sales_reports = data2
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		[HttpGet]
		public HttpResponseMessage get_store_pending_shifts([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var sid = Convert.ToInt32(store_id);
				var data = (from x in Context.shift_details_mst2
							where x.store_id == sid && x.shift_status.Equals("P")
							select new
							{
								store_name = x.store_name,
								id = x.id,
								shift_num = x.shift_num,
								shift_date = x.shift_date,
								cashier_name = x.cashier_name
							})
							.ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					shift_details_mst = data
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_cash_counted([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var sid = Convert.ToInt32(store_id);
				var data = (from x in Context.shift_details_mst2
							where x.store_id == sid && x.shift_status.Equals("D")
							select new
							{

								id = x.id,
								shift_num = x.shift_num,
								cash_counted = x.cash_counted,
								cashier_name = x.cashier_name,
								shift_date = x.shift_date
							})
							.ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					shift_details_mst = data
				});
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage update_cash_counted([FromBody] update_cash_counted_req_dm2 data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				for (int i = 0; i < data.shift_details_mst.Count; i++)
				{
					int id = Convert.ToInt32(data.shift_details_mst[i].id);
					db.ExecuteSqlCommand("UPDATE A SET A.cash_counted=" + data.shift_details_mst[i].cash_counted + " from shift_details_mst A where id=" + data.shift_details_mst[i].id.ToString());

					db.ExecuteSqlCommand("update A SET A.shift_report_cash_counted=" + data.shift_details_mst[i].cash_counted + "from manager_report_mst A where shift_id=" + data.shift_details_mst[i].id.ToString());


					db.ExecuteSqlCommand("update a set a.shift_report_cash_difference=shift_report_cash_counted-shift_report_ttaf  from manager_report_mst a where a.shift_id=" + id);


					db.ExecuteSqlCommand("exec usp_sr_get_store_cash_difference_V2 @shift_id=" + id);
					db.ExecuteSqlCommand("exec usp_sr_get_corp_cash_difference_V2 @shift_id=" + id);

				}
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_cashier_list([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var sid = Convert.ToInt32(store_id);
				var data = (from x in Context.cashier_mst2
							where x.store_id == sid
							select new get_cashier_list_resp_dm() { id = x.id.ToString(), cashier_name = x.cashier_name, cashier_num = x.cashier_num, cashier_pw = x.cashier_pw, title=x.title }).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					cashier_mst = data
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_cashier_next_id([FromUri] String store_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var nxtID = db.SqlQuery<shiftreportapp.data.add_cashier_dm>("exec usp_sr_addnewcashier @store_id=" + store_id.ToString()).FirstOrDefault();
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					cashier_mst = new
					{
						id = nxtID.id
					}
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage add_cashier([FromBody] shiftreportapp.data.AppModel.cashier_mst data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
                /* Disabled by Adil
				var db = Context.Database;
				db.ExecuteSqlCommand("exec usp_sr_addnewcashier @cashier_id=" + data.id + ",@cashier_name='" + data.cashier_name + "',@cashier_pw='" + data.cashier_pw + "',@cashier_cell_phone='" + data.cashier_cell_phone + "',@cashier_email='" + data.cashier_email + "',title='"+data.title+"'");
				db.ExecuteSqlCommand("update cashier_mst set cashier_num=" + data.cashier_num + ", store_id=" + data.store_id + ",cashier_name='" + data.cashier_name + "',cashier_pw='" + data.cashier_pw + "',cashier_cell_phone='" + data.cashier_cell_phone + "',cashier_email='" + data.cashier_email + "' where id=" + data.id);
                */

                // adding cashier by updating the exting record created by the get_next_id api
                var cashier_mst = Context.cashier_mst2.Find(data.id);

                cashier_mst.cashier_name = data.cashier_name;
                cashier_mst.title = data.title;
                cashier_mst.cashier_pw = data.cashier_pw;
                cashier_mst.cashier_email = data.cashier_email;
                cashier_mst.cashier_cell_phone = data.cashier_cell_phone;
                cashier_mst.is_mobile_register = data.is_mobile_register;

                Context.SaveChanges();


				var store = Context.store_profile_mst2.Find(cashier_mst.store_id);
                var corporate = Context.corporate_mst2.Find(store.corp_id);
 
                // Send VC email newCashierEmployee(string email, string username, string companyName, string userID, string password, string storeName)
                VenueCashEmails.newCashierEmployee(cashier_mst.cashier_email, cashier_mst.cashier_name, corporate.corp_name, cashier_mst.id.ToString(), cashier_mst.cashier_pw, store.store_name);

				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_cashier(int id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var data = Context.cashier_mst2.Find(id);
				return Request.CreateResponse(HttpStatusCode.OK, new { cashier_mst = data });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage modify_cashier([FromBody] shiftreportapp.data.AppModel.cashier_mst data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("exec usp_sr_addnewcashier @cashier_id=" + data.id + ",@cashier_name='" + data.cashier_name + "',@cashier_pw='" + data.cashier_pw + "',@cashier_cell_phone='" + data.cashier_cell_phone + "',@cashier_email='" + data.cashier_email + "',@title='"+data.title+"'");
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage delete_checklist([FromBody] shiftreportapp.data.AppModel.cashier_mst data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("exec usp_sr_addnewcashier @cashier_id=" + data.id + ",@cashier_name='" + data.cashier_name + "',@cashier_pw='" + data.cashier_pw + "',@cashier_cell_phone='" + data.cashier_cell_phone + "',@cashier_email='" + data.cashier_email + "'");
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpGet]
		public HttpResponseMessage get_wizard([FromUri] int corp_id, [FromUri] int id)
		{
			try
			{
				List<managers_get_wizard> data0 = new List<managers_get_wizard>();
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				var corp = db.SqlQuery<get_wiz_corp_mst>("Select plan_id,payment_status,available_stores,yelp_id from corporate_mst where id=" + corp_id).FirstOrDefault();

				var data1 = db.SqlQuery<shiftreportapp.data.AppModel.question_cat_dtls>("select id,q_category_text from question_cat_dtls").ToList();
				var data2 = db.SqlQuery<shift_checklist_store_mst_response>("Select id, q_uuid,q_type,q_category,q_text from shift_checklist_store_mst where shift_no=9999 and store_id=" + 1).ToList();
				var data3 = db.SqlQuery<managers_get_wizard>("select distinct manager_id from storelist_dtls a inner join managers_mst b on a.corp_id=b.corp_id where a.corp_id=" + corp_id).ToList();

				foreach (var item in data3)
				{
					managers_get_wizard m = new managers_get_wizard();
					var manager = db.SqlQuery<shiftreportapp.data.AppModel.managers_mst>("select * from managers_mst where manager_id=" + item.manager_id).ToList().FirstOrDefault();
					var access = db.SqlQuery<login_storelist_dtls_dm>("select access_level from storelist_dtls where managerID='" + item.manager_id + "'").FirstOrDefault();
					m.manager_id = manager.manager_id;
					m.manager_name = manager.manager_name;
					m.manager_password = manager.manager_pw;
					m.manager_cellphone = manager.manager_cell_phone;
					m.manager_email = manager.manager_email;
					m.access_controle = access.access_level;
					data0.Add(m);
				}

                //Get Yelp business info if yelp_id is not null
         
                if (!String.IsNullOrEmpty(corp.yelp_id ))
                {
                    var yelpBusiness = YelpV3.getBusiness(corp.yelp_id);
                    return Request.CreateResponse(HttpStatusCode.OK, new
                    {
                        question_cat_dtls = data1,
                        shift_checklist_store_mst = data2,
                        managers_mst = data0,
                        corporate_mst = corp,
                        yelpBusiness = yelpBusiness
                    });
                }
           

                return Request.CreateResponse(HttpStatusCode.OK, new
				{
					question_cat_dtls = data1,
					shift_checklist_store_mst = data2,
					managers_mst = data0,
					corporate_mst = corp,
                    yelpBusiness= 0
                });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		[HttpPost]
		public HttpResponseMessage save_wizard([FromBody] save_wizard_dm data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;

				shiftreportapp.data.AppModel.store_profile_mst D = new shiftreportapp.data.AppModel.store_profile_mst();
				D.corp_id = data.corp_id;
				D.store_name = data.store_profile_mst.store_name;
				D.store_address1 = data.store_profile_mst.store_address1;
				D.store_address2 = data.store_profile_mst.store_address2;
				D.store_city = data.store_profile_mst.store_city;
				D.store_email = data.store_profile_mst.store_email;
				D.store_state = data.store_profile_mst.store_state;
				D.store_zip = data.store_profile_mst.store_zip;
				D.store_phone_no = data.store_profile_mst.store_phone_no;
				D.store_fax_no = data.store_profile_mst.store_fax_no;
				D.store_wifi_name = data.store_profile_mst.store_wifi_name;
				D.store_wifi_pw = data.store_profile_mst.store_wifi_pw;
				D.added_paid_status = data.added_paid_status;

                // get corporate 
                var corporate = Context.corporate_mst2.Find(D.corp_id);
                if(corporate.plan_id== 46)
                {
                    D.is_venuecash = true;
                }
                else
                {
                    D.is_venuecash = false;
                }


				Context.store_profile_mst2.Add(D);
				Context.SaveChanges();

                // VenueCash Addition
                if (!String.IsNullOrEmpty(data.store_profile_mst.yelp_store_id))
                {
                    vc_store_profile_mst vcStoreProfileMst = new vc_store_profile_mst();

                    vcStoreProfileMst.create_dt = DateTime.Now;
                    vcStoreProfileMst.created_userid = "System";
                    vcStoreProfileMst.modify_dt = DateTime.Now;
                    vcStoreProfileMst.Modify_usrid = "System";
                    vcStoreProfileMst.store_id = D.id;
                    vcStoreProfileMst.is_gas_station = data.store_profile_mst.is_gas_station;
					vcStoreProfileMst.business_type = data.store_profile_mst.business_type;
                    vcStoreProfileMst.gas_back_rate = data.store_profile_mst.gas_back_rate;
                    vcStoreProfileMst.cash_back_rate = data.store_profile_mst.cash_back_rate;
                    vcStoreProfileMst.latitude = data.store_profile_mst.latitude;
                    vcStoreProfileMst.longitude = data.store_profile_mst.longitude;
                    vcStoreProfileMst.Rating = data.store_profile_mst.Rating;

                    vcStoreProfileMst.website = data.store_profile_mst.Website;
                    vcStoreProfileMst.SubCategorieId = 1;
                    vcStoreProfileMst.CategorieId = 1;
                    vcStoreProfileMst.yelp_store_id = data.store_profile_mst.yelp_store_id;
                    vcStoreProfileMst.Category_Name = data.store_profile_mst.Category_Name;
                    vcStoreProfileMst.price = data.store_profile_mst.price;
                    vcStoreProfileMst.review_count = data.store_profile_mst.review_count;
                    vcStoreProfileMst.is_closed = false;
                    vcStoreProfileMst.image_url = data.store_profile_mst.image_url;
                    vcStoreProfileMst.is_mobile_register = data.store_profile_mst.is_mobile_register;
                    vcStoreProfileMst.is_tips = data.store_profile_mst.is_tips;
                    // Adding to db
                    Context.vc_store_profile_mst2.Add(vcStoreProfileMst);

                    // creating Registers
                    for (int i = 1; i <= data.store_profile_mst.numberOfRegisters; i++)
                    {
                        vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                        storeRegister.create_dt = DateTime.Now;
                        storeRegister.created_userid = "System";
                        storeRegister.modify_dt = DateTime.Now;
                        storeRegister.Modify_usrid = "System";
                        storeRegister.RegisterId = -1;
                        storeRegister.store_id = vcStoreProfileMst.store_id;
                        storeRegister.RegisterNumber = i.ToString();
                        storeRegister.IsGasPump = 0;
                        storeRegister.IsMobileRegister = 0;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Status = "NA";
                        storeRegister.shift_id = -1;
                        Context.vc_StoreRegisters2.Add(storeRegister);
                    }
                    // creating Gas pumps
                    for (int i = 1; i <= data.store_profile_mst.numberOfGasPumps; i++)
                    {
                        vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                        storeRegister.create_dt = DateTime.Now;
                        storeRegister.created_userid = "System";
                        storeRegister.modify_dt = DateTime.Now;
                        storeRegister.Modify_usrid = "System";
                        storeRegister.RegisterId = -1;
                        storeRegister.store_id = vcStoreProfileMst.store_id;
                        storeRegister.RegisterNumber = i.ToString();
                        storeRegister.IsGasPump = 1;
                        storeRegister.IsMobileRegister = 0;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Status = "NA";
                        storeRegister.shift_id = -1;
                        Context.vc_StoreRegisters2.Add(storeRegister);
                    }


                }
                else // if not on yelp
                {
                    vc_store_profile_mst vcStoreProfileMst = new vc_store_profile_mst();

                    vcStoreProfileMst.create_dt = DateTime.Now;
                    vcStoreProfileMst.created_userid = "System";
                    vcStoreProfileMst.modify_dt = DateTime.Now;
                    vcStoreProfileMst.Modify_usrid = "System";
                    vcStoreProfileMst.store_id = D.id;
                    vcStoreProfileMst.is_gas_station = data.store_profile_mst.is_gas_station;
					vcStoreProfileMst.business_type = data.store_profile_mst.business_type;
                    vcStoreProfileMst.gas_back_rate = data.store_profile_mst.gas_back_rate;
                    vcStoreProfileMst.cash_back_rate = data.store_profile_mst.cash_back_rate;
                    vcStoreProfileMst.latitude = data.store_profile_mst.latitude;
                    vcStoreProfileMst.longitude = data.store_profile_mst.longitude;
                    vcStoreProfileMst.Rating = 0;

                    vcStoreProfileMst.website = "";
                    vcStoreProfileMst.SubCategorieId = 1;
                    vcStoreProfileMst.CategorieId = 1;
                    vcStoreProfileMst.yelp_store_id = "";
                    vcStoreProfileMst.Category_Name = ""; // need to add category to UI
                    vcStoreProfileMst.price = "";
                    vcStoreProfileMst.review_count = 0;
                    vcStoreProfileMst.is_closed = false;
                    vcStoreProfileMst.image_url = "";
                    vcStoreProfileMst.is_mobile_register = data.store_profile_mst.is_mobile_register;
                    vcStoreProfileMst.is_tips = data.store_profile_mst.is_tips;
                    // Adding to db
                    Context.vc_store_profile_mst2.Add(vcStoreProfileMst);

                    // creating Registers
                    for (int i = 1; i <= data.store_profile_mst.numberOfRegisters; i++)
                    {
                        vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                        storeRegister.create_dt = DateTime.Now;
                        storeRegister.created_userid = "System";
                        storeRegister.modify_dt = DateTime.Now;
                        storeRegister.Modify_usrid = "System";
                        storeRegister.RegisterId = -1;
                        storeRegister.store_id = vcStoreProfileMst.store_id;
                        storeRegister.RegisterNumber = i.ToString();
                        storeRegister.IsGasPump = 0;
                        storeRegister.IsMobileRegister = 0;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Status = "NA";
                        storeRegister.shift_id = -1;
                        Context.vc_StoreRegisters2.Add(storeRegister);
                    }
                    // creating Gas pumps
                    for (int i = 1; i <= data.store_profile_mst.numberOfGasPumps; i++)
                    {
                        vc_StoreRegisters storeRegister = new vc_StoreRegisters();

                        storeRegister.create_dt = DateTime.Now;
                        storeRegister.created_userid = "System";
                        storeRegister.modify_dt = DateTime.Now;
                        storeRegister.Modify_usrid = "System";
                        storeRegister.RegisterId = -1;
                        storeRegister.store_id = vcStoreProfileMst.store_id;
                        storeRegister.RegisterNumber = i.ToString();
                        storeRegister.IsGasPump = 1;
                        storeRegister.IsMobileRegister = 0;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Cashier_ID_on_duty = -1;
                        storeRegister.Status = "NA";
                        storeRegister.shift_id = -1;
                        Context.vc_StoreRegisters2.Add(storeRegister);
                    }
                }


                // Create Promoter record using store info on promoter_mst
                
                promoters_mst promoter = new promoters_mst();
                
                promoter.create_date = DateTime.Now;
                promoter.create_userid = "system";
                promoter.modified_userid = "system";
                promoter.modified_date = DateTime.Now;

                promoter.promoter_name = D.store_name;
                promoter.Promoter_Address1 = D.store_address1;
                promoter.Promoter_Address2 = D.store_address2;
                promoter.Promoter_City = D.store_city;
                promoter.Promoter_State = D.store_state;
                promoter.Promoter_Zip = D.store_zip.ToString();
                promoter.Promoter_Country = D.store_country;
                promoter.Store_ID = D.id;
                // promoter.Corp_ID = D.corp_id;

                var corpPayoutMethod = Context.vc_CorpPayoutMethods2.Where(p => p.corp_id == D.corp_id && p.IsDefault == true).FirstOrDefault();
                if (corpPayoutMethod != null)// will not work for first store and if no payout method is added
                {
                    promoter.PayoutMethodId = corpPayoutMethod.Id;
                }

                Context.promoters_mst2.Add(promoter);
               


                // END VenueCash Addition
                Context.SaveChanges();



                // check if Business want a welcome kit
                if (data.store_profile_mst.is_welcome_kit)
                {
                    string body = "Corporate " + corporate.corp_name + " with the ID: " + corporate.Id.ToString() + " asked for a welcome kit for store" + D.store_name +
                                 "Store ID: " + D.id.ToString();
                                 
                    MailLogService.SendMail("support@shiftreports.com", "john@shiftreports.com", "We want a welcome Kit", body);
                }





				int maxID = D.id;

				for (int i = 0; i < data.shift_checklist_store_mst.Count; i++)
				{
					var d2 = data.shift_checklist_store_mst[i]; // insert


					db.ExecuteSqlCommand("INSERT INTO shift_checklist_store_mst(q_uuid,q_type,q_category,q_text,store_id,shift_no) Values('" + d2.q_uuid + "','" + d2.q_type + "'," + d2.q_category + ",'" + d2.q_text + "'," + D.id + "," + d2.shift_no + ")");
				}
				for (int i = 0; i < data.shift_store_times.Count; i++)
				{
					var d3 = data.shift_store_times[i]; // insert

					db.ExecuteSqlCommand("INSERT INTO shift_store_times (store_id,shift_no,start_time,end_time) values (" + D.id + "," + d3.shift_no + ",'" + d3.start_time + "','" + d3.end_time + "');");
				}
				var corp = Context.corporate_mst2.Find(data.corp_id);

				int mngr_corp = Convert.ToInt32(corp.acct_admin_id);

				db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,access_level) VALUES(" + maxID + "," + corp.Id + ",'" + mngr_corp + "','AA')");



				//string body2 = "";

				//var webRequest2 = WebRequest.Create(S3_path + "/NewManager.htm");

				//using (var response = webRequest2.GetResponse())
				//using (var content = response.GetResponseStream())
				//using (var reader = new StreamReader(content))
				//{
				//	body2 = reader.ReadToEnd();
				//}

				//var xMan = Context.managers_mst2.Where(xm => xm.manager_id.Equals(mngr_corp.ToString())).FirstOrDefault();
				//var email = xMan.manager_email;
				//body2 = body2.Replace("*[user first name]*", xMan.manager_name);
				//body2 = body2.Replace("*[id]*", xMan.Id.ToString());
				//body2 = body2.Replace("*[pw]*", xMan.manager_pw);

				//body2 = body2.Replace("*[company]*", corp.corp_name);


				//MailLogService.SendMail("support@shiftreports.com", email, "Welcome to Shift Reports", body2);


				for (int i = 0; i < data.managers_mst.Count; i++)
				{
					db.ExecuteSqlCommand("update managers_mst set manager_id='" + data.managers_mst[i].manager_id + "',manager_name='" + data.managers_mst[i].manager_name + "', manager_pw='" + data.managers_mst[i].manager_password + "', manager_cell_phone='" + data.managers_mst[i].manager_cell_phone + "',manager_email='" + data.managers_mst[i].manager_email + "',corp_id=" + data.managers_mst[i].corp_id + " where manager_id='" + data.managers_mst[i].manager_id + "'");

 
					for (int j = 0; j < data.managers_mst[i].access_controle.Count; j++)
					{
						db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,access_level) VALUES(" + maxID + "," + data.managers_mst[i].corp_id + ",'" + data.managers_mst[i].manager_id + "','" + data.managers_mst[i].access_controle[j].access_level + "')");
					}

				 
					if (Convert.ToBoolean(data.managers_mst[i].new_manager_flag))
					{
                        // send venuecash email
                        VenueCashEmails.newManagerBusiness(data.managers_mst[i].manager_email, data.managers_mst[i].manager_name, corp.corp_name, data.managers_mst[i].manager_id.ToString(), data.managers_mst[i].manager_password, D.store_name, data.managers_mst[i].access_controle[0].access_level);

                    }
                 


				}

				////////////////////////////////////////////////////////////////

				//	for (int i = 0; i < data.new_managers.Count; i++)
				//	{
				//			var mid = maxID;
				//			db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,access_level) VALUES(" + data.new_managers [i].store_id + "," + D.corp_id + ",'" + mid + "','" + data.new_managers[i].access_level + "')");
				//	}
				///////////////////////////////////////////////////////////////////////
				for (int i = 0; i < data.cashier_mst.Count; i++)
				{
					db.ExecuteSqlCommand("update cashier_mst set store_id=" + D.id + ",cashier_num=" + data.cashier_mst[i].id + ",cashier_name='" + data.cashier_mst[i].cashier_name + "',cashier_pw='" + data.cashier_mst[i].cashier_pw + "',cashier_cell_phone='" + data.cashier_mst[i].cashier_cell_phone + "',cashier_email='" + data.cashier_mst[i].cashier_email + "' where id=" + data.cashier_mst[i].id);

                    // Send VC email newCashierEmployee(string email, string username, string companyName, string userID, string password, string storeName)
                    VenueCashEmails.newCashierEmployee(data.cashier_mst[i].cashier_email, data.cashier_mst[i].cashier_name, corporate.corp_name, data.cashier_mst[i].id.ToString(), data.cashier_mst[i].cashier_pw, D.store_name);

                }
                db.ExecuteSqlCommand("update corporate_mst set available_stores=" + data.available_stores + " where id=" + data.corp_id);

				Context.Database.ExecuteSqlCommand("Update a set a.firsttimelogin_flag=" + data.firsttimelogin_flag + " from corporate_mst a where id=" + data.corp_id);
 

                //get Manager info
                var manager = Context.managers_mst2.Find(corporate.acct_admin_id);

                // Send VC Email
                VenueCashEmails.newStoreBusiness(data.store_profile_mst.store_email, manager.manager_name, corporate.corp_name, manager.Id.ToString(),manager.manager_pw, data.store_profile_mst.store_name);

                return Request.CreateResponse(HttpStatusCode.OK, new { success = maxID });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("appsupport@shiftreports.com", "support@shiftreports.com", "Error While Executeing Save_Wizard", ex.Msg);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);

			}
		}


		[HttpPost]
		public HttpResponseMessage add_corporate([FromBody] add_corporate_post_req data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				int f = Convert.ToInt32(data.firsttimelogin_flag);
				corporate_mst_helper corp = new corporate_mst_helper();
				var db = Context.Database;

				int corpID = corp.Addcorporate_mst(new shiftreportapp.data.AppModel.corporate_mst()
				{
					corp_name = data.corp_name,
					corp_address1 = data.corp_address1,
					corp_address2 = data.corp_address2,
					corp_city = data.corp_city,
					corp_zip = data.corp_zip,
					corp_state = data.corp_state,
					corp_phone = data.corp_phone,
					corp_fax = data.corp_fax,
					corp_email = data.email,
					firsttimelogin_flag = f,
					acct_admin_id = data.acc_admin_id,
					plan_name = data.plan_name,
					plan_id = data.plan_id,
					coupon_code = data.Coupon_Code,
					sign_up_date = data.sign_up_date,
					free_trial_expire_date = data.free_trial_expire_date,
					payment_status = data.payment_status,
					referral_source=data.referral_source,
                    yelp_id =data.yelp_id
				});
				var m = db.SqlQuery<get_next_manger_id>("select id from managers_mst").ToList().LastOrDefault();

				//if(!data.Coupon_Code.Equals(""))
				//	if(data.Coupon_Code!="")
				//		{
				//			Context.Database.ExecuteSqlCommand("UPDATE A Set A.promoters_used=" + data.promoter_id + " FROM promoters_mst A WHERE A.coupon_code='" + data.Coupon_Code + "'");
				//		}
				var maxID = m.id + 1;

				managers_mst_helper mng = new managers_mst_helper();

				mng.Savemanagers_mst(corpID.ToString(), data.manager_id, data.name, data.password, data.email, data.manager_cell_phone);
                //    Context.Database.ExecuteSqlCommand("update managers_mst set corp_id=" + corpID + " where id=" + Convert.ToInt32(data.manager_id));



                // Sending email

                var xMan = Context.managers_mst2.Find(Convert.ToInt32(data.manager_id));

                VenueCashEmails.newAccountBusiness(data.email, xMan.manager_name, data.corp_name, data.manager_id, xMan.manager_pw);
                 


				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					success = 1
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.StackTrace.ToString());
			}
		}


		[HttpPost]
		public HttpResponseMessage add_store([FromBody] add_store_post_req data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("update a set a.store_name='" + data.store_profile_mst.store_name + "',a.store_address1='" + data.store_profile_mst.store_address1 + "',a.store_address2='" + data.store_profile_mst.store_address2 + "',a.store_city='" + data.store_profile_mst.store_city + "',a.store_zip=" + data.store_profile_mst.store_zip + ",a.store_state='" + data.store_profile_mst.store_state + "',a.store_phone='" + data.store_profile_mst.store_phone_no + "' from store_profile_mst a where a.id=" + data.store_id);

				for (int i = 0; i < data.question_cat_dtls.Count; i++)
				{
					var d = data.question_cat_dtls[i];
					db.ExecuteSqlCommand("Update a set a.q_category_text from question_cat_dtls a where id=" + d.Id.ToString());
				}
				for (int i = 0; i < data.shift_checklist_store_mst.Count; i++)
				{
					var d2 = data.shift_checklist_store_mst[i];
					db.ExecuteSqlCommand("Update a set a.q_uuid='' a.q_type='',a.q_category,a.q_text='' from question_cat_dtls a where id=" + d2.id.ToString());
				}
				for (int i = 0; i < data.shift_store_times.Count; i++)
				{
					var d3 = data.shift_store_times[i];
					db.ExecuteSqlCommand("update a set a.start_time='" + d3.start_time + "',a.end_time='" + d3.end_time + "' from shift_store_times a where id=" + d3.id.ToString());
				}
				for (int i = 0; i < data.managers_mst.Count; i++)
				{
					db.SqlQuery<int>("exec usp_sr_addnewmanager @manger_id=" + data.managers_mst[i].manager_id + ",@manager_name='" + data.managers_mst[i].manager_name + "',@manager_pw='" + data.managers_mst[i].manager_pw + "',@manager_cell_phone='" + data.managers_mst[i].manager_cell_phone + "',@manager_email='" + data.managers_mst[i].manager_email + "')").FirstOrDefault();
					for (int j = 0; i < data.managers_mst[i].access_controle.Count; i++)
					{
						db.ExecuteSqlCommand("Insert into storelist_dtls(store_id,corp_id,managerID,store_type,access_level) VALUES(" + data.store_id + "," + data.managers_mst[i].corp_id + "," + data.managers_mst[j].manager_id + ",'','" + data.managers_mst[i].access_controle[j].access_level + "')");
					}
				}
				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					success = 1
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}


		#region Plan

		[HttpGet]
		public HttpResponseMessage get_plan([FromUri] string corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var cid = Convert.ToInt32(corp_id);
				var corp = Context.corporate_mst2.Find(cid);
				var plan = Context.plan_mst2.ToList();

				if(corp==null)
				{
					return Request.CreateResponse(HttpStatusCode.OK, new
					{
						corporate_mst = new
						{
							corp_payment_status = "",
							plan_id = "-1"
						},
						plan_mst = plan


					});
				}
				else
				{
					return Request.CreateResponse(HttpStatusCode.OK, new
					{
						corporate_mst = new
						{
							corp_payment_status = corp.payment_status,
							plan_id = corp.plan_id
						},
						plan_mst = plan


					});
				}
				
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region Get Credit Card

		[HttpGet]
		public HttpResponseMessage get_credit_card_on_file([FromUri] string corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var cid = Convert.ToInt32(corp_id);
				var data = (from x in Context.customer_payment_mst2
							where x.corp_id == cid
							select new get_credit_card_on_file_req() { card_id = x.id.ToString(), card_type = x.card_type, card_last_4_digit = (x.card_number.Substring(x.card_number.Length - 4)).ToString(), default_payment = (int)x.default_payment, payment_type = x.payment_type.ToString() }).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, data);
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Add Credit Card

		[HttpPost]
		public HttpResponseMessage addcreditcard([FromBody] add_credit_card_req card)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("Update a set a.default_payment=0 from customer_payment_mst a where corp_id=" + card.corp_id);
				db.ExecuteSqlCommand("Insert into customer_payment_mst(corp_id,firstname,lastname,city,state,zip,card_number,card_exp,card_csc,payment_type,default_payment,card_type) VALUES(" + card.corp_id + ",'" + card.firstname + "','" + card.lastname + "','" + card.city + "','" + card.state + "','" + card.zip_code + "','" + card.card_number + "','" + card.expiration_month + "/" + card.expiration_year + "','" + card.security_code + "'," + card.payment_type + "," + card.default_payment + ",'" + card.card_type + "')");

				int cardID = db.SqlQuery<add_credit_card_req>("select top 1 id from customer_payment_mst order by id desc").LastOrDefault().id;

				return Request.CreateResponse(HttpStatusCode.OK, new
				{

					card_id = cardID

				});

			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Get Billing History
		public HttpResponseMessage get_billing_history([FromUri] string corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var history = new get_billing_history_req();
				history.payment_transactions = new List<CustomerTransaction>();
				var db = Context.Database;
				var cid = Convert.ToInt32(corp_id);

				var corp = Context.corporate_mst2.Find(cid);

				var data = db.SqlQuery<AppModel.payment_transactions_mst>("select id,card_id,corp_id,plan_id,amount_charged,txn_id,transaction_date_time,amount_charged,number_of_stores_charged from payment_transactions_mst where corp_id=" + corp_id).ToList();

				for (int i = 0; i < data.Count; i++)
				{
					CustomerTransaction t = new CustomerTransaction();
					t.id = data[i].id;
					t.transaction_date_time = data[i].transaction_date_time;
					t.amount_charged = data[i].amount_charged;
					var p = Context.plan_mst2.Find(data[i].plan_id);
					t.plan_name = p.plan_name;
					if (data[i].card_id != 0)
					{
						t.default_payment = Convert.ToInt32(Context.customer_payment_mst2.Find(data[i].card_id).default_payment);

						if (Context.customer_payment_mst2.Find(data[i].card_id).card_number != "")
						{
							t.card_last_4_digits = Context.customer_payment_mst2.Find(data[i].card_id).card_number.Substring(Context.customer_payment_mst2.Find(data[i].card_id).card_number.Length - 4);
						}
					}

					t.number_of_stores_charged = data[i].number_of_stores_charged;
					history.payment_transactions.Add(t);
				}
				int pid = Convert.ToInt32(corp.plan_id);
				var data2 = (from x in Context.plan_mst2
							 where x.Id == pid
							 select new { id = x.Id, plan_name = x.plan_name }).ToList();

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					payment_transactions = history,
					plan_mst = data2
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region Get Checkout Information
		[HttpGet]
		public HttpResponseMessage getcheckoutinformation([FromUri] string corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var cid = Convert.ToInt32(corp_id);

				var data = (from x in Context.corporate_mst2
							where x.Id == cid
							select new get_checkout_information_req() { corp_name = x.corp_name, corp_address1 = x.corp_address1, corp_address2 = x.corp_address2, corp_city = x.corp_city, corp_email = x.corp_email, corp_fax = x.corp_fax, corp_phone = x.corp_phone, corp_state = x.corp_state, corp_zip = x.corp_zip, number_of_stores_in_use = (int)x.num_of_stores_in_use, firsttimelogin_flag = (int)x.firsttimelogin_flag, plan_name = x.plan_name, plan_id = (int)x.plan_id }).ToList();
				//	var data2 = db.SqlQuery<customer_payment_mst_resp>("select id,card_number,payment_type from customer_payment_mst where corp_id=" + corp_id);
				var data3 =  db.SqlQuery<AppModel.plan_mst>("select * from plan_mst");

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corporate_mst2 = data,
					today = DateTime.Now.ToShortDateString(),
					plan_mst = data3
				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Submit Order

		[HttpPost]
		public HttpResponseMessage submit_order([FromBody] submit_order_req order)
		{
			try
			{
				try
				{
					AppModel Context = new shiftreportapp.data.AppModel();

					var db = Context.Database;
					Dictionary<string, string> sdkConfig = new Dictionary<string, string>();

					/* Live 
					sdkConfig.Add("mode", "live");
					string accessToken = new OAuthTokenCredential("AV8Hl_oI3XJoi3HswWJyPVUZcGZkV-Zh9vIubnRP80p_YPM_025-d24kBlVY16bKeqUUzXQB0NRAaXVI", "EIrE_0wOpxhquP7eQlEPlIwbvGrtMJhPmM7xBFb-Fqc36IHMLYVUAvvpXpoIae1VmfvqfWbojX_abWjT", sdkConfig).GetAccessToken();
					*/

					/* Sandbox */
					sdkConfig.Add("mode", "sandbox");
					string accessToken = new OAuthTokenCredential("Aa3fQicf6E747E5Snc9I2XgEmGbOIyKBTkmVHDfU6BfjD6mSn-7goLwYSSrOYuUrbD6Ifb5CzQa7R_BH", "EE4BJAi8KirBrsu11CDWhKzhIxmoYxwuw0ZL7StRcSrvxC524vA3-KJndFQgDJxoyuBu-Q7byo9YiLMW", sdkConfig).GetAccessToken();
					 

				


					APIContext apiContext = new APIContext(accessToken);

					apiContext.Config = sdkConfig;
					var cardDetails = db.SqlQuery<AppModel.customer_payment_mst>("select * from customer_payment_mst where ID=" + order.card_id).ToList().FirstOrDefault();
					CreditCard credtCard = new CreditCard();
					switch (cardDetails.payment_type)
					{
						case ("1"):
							credtCard.type = "mastercard";
							break;
						case ("2"):
							credtCard.type = "visa";
							break;
						case ("3"):
							credtCard.type = "amercan express";
							break;
						case ("4"):
							credtCard.type = "discover";
							break;
						default:
							credtCard.type = "visa";
							break;
					}
					// credtCard.type = cardDetails.card_type;
					//credtCard.type = "visa";
					credtCard.number = cardDetails.card_number;
					credtCard.expire_month = Convert.ToInt32(cardDetails.card_exp.Split('/')[0]);
					credtCard.expire_year = Convert.ToInt32(cardDetails.card_exp.Split('/')[1]);
					credtCard.first_name = cardDetails.firstname;
					credtCard.last_name = cardDetails.lastname;
					string last4digits = cardDetails.card_number.Substring(cardDetails.card_number.Length - 4);

					FundingInstrument fundInstrument = new FundingInstrument();
					fundInstrument.credit_card = credtCard;

					List<FundingInstrument> fundingInstrumentList = new List<FundingInstrument>();
					fundingInstrumentList.Add(fundInstrument);

					Payer payr = new Payer();
					payr.funding_instruments = fundingInstrumentList;
					payr.payment_method = "credit_card";

					Amount amnt = new Amount();
					amnt.currency = "USD";
					amnt.total = order.next_charge_amt.ToString();

					Transaction tran = new Transaction();
					tran.description = "creating a direct payment with credit card";
					tran.amount = amnt;
				  

					List<Transaction> transactions = new List<Transaction>();
					transactions.Add(tran);

					Payment pymnt = new Payment();
					pymnt.intent = "sale";
					pymnt.payer = payr;
					pymnt.transactions = transactions;
					

					Payment createdPayment = pymnt.Create(apiContext);
					if (createdPayment.state == "approved")
					//if (true)
						{
						// This is a temporary master

						db.ExecuteSqlCommand("update corporate_mst set firsttimelogin_flag=" + order.firsttimelogin_flag + " ,available_stores=" + order.available_stores + " where id=" + order.corp_id);

						db.ExecuteSqlCommand("insert into payment_transactions_mst(txn_id,amount_charged,transaction_date_time,number_of_stores_charged,card_id,corp_id,plan_id) values('" + createdPayment.id + "'," + order.amount_charged + ",'" + order.transaction_date_time + "'," + order.number_of_stores_charged + "," + order.card_id + "," + order.corp_id + "," + order.plan_id + ")");
						db.ExecuteSqlCommand("update corporate_mst set payment_status='" + order.corp_payment_status + "', num_of_stores_last_paid=" + order.num_of_stores_last_paid + ", last_payment_date='" + order.last_payment_date + "' , next_charge_amt=" + order.next_charge_amt + " , next_charge_date='" + order.next_charge_date + "', plan_id=" + order.plan_id + " where id=" + order.corp_id);
						string planName = db.SqlQuery<plan_item>("select plan_name from plan_mst where id=" + order.plan_id).FirstOrDefault().plan_name;

						DateTime d = new DateTime();
						d = DateTime.Now;
						DateTime nextChargeDate;

						if (planName.Contains("monthly"))
						{
							d = d.AddDays(30);
							nextChargeDate = d;
						}
						else
						{
							d = d.AddYears(1);
							nextChargeDate = d;
						}
					
						db.ExecuteSqlCommand("update corporate_mst set next_charge_date='" + d + "' where id=" + order.corp_id);

						var r = Context.corpoarte_billing_discount_mst2.Where(x => x.corp_id == order.corp_id).FirstOrDefault();
						if(r==null)
						{
							Context.corpoarte_billing_discount_mst2.Add(new		AppModel.corpoarte_billing_discount_mst() {
								corp_id=order.corp_id,
								isyearly=order.isyearly,
								discountVal=order.discountVal
							});
							
						}
						else
						{
							r.discountVal = order.discountVal;
							r.isyearly = order.isyearly;
							Context.Entry(r).State = EntityState.Modified;
						}
						Context.SaveChanges();


						return Request.CreateResponse(HttpStatusCode.OK, new
						{
							createdPayment.state,
							last4digits,
							createdPayment.id

						});
					}
					else
					{
						return Request.CreateResponse(HttpStatusCode.OK, new { success = 0 });

					}
				}
				catch (PayPal.PayPalException ex)
				{

					MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "Paypal  Error", ex.Message);

					throw new AppException(1, "The transaction could not be completed at this time. Please try again later or try a different payment method.");
				}

			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion



		#region Update FirstTimeLogin Flag
		[HttpPost]
		public HttpResponseMessage update_firsttimelogin_flag([FromBody] update_firsttimelogin_flag_req c)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("update corporate_mst set firsttimelogin_flag=" + c.firsttimelogin_flag + " where id=" + c.corp_id);
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Get Available Stores Flag

		// need to be worked on and clerify with miruna & john
		[HttpGet]
		public HttpResponseMessage get_available_stores([FromUri] string corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				var cid = Convert.ToInt32(corp_id);

				var data = (from x in Context.store_profile_mst2
							where x.corp_id == cid
							select x.id).ToList();
				int allStores = Convert.ToInt32(Context.corporate_mst2.Find(cid).available_stores);
				string paymentStatus = Context.corporate_mst2.Find(cid).payment_status;

				return Request.CreateResponse(HttpStatusCode.OK, new
				{
					corporate_mst2 = new
					{
						available_stores = allStores,
						payment_status = paymentStatus

					},
					store_profile_mst = data

				});
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Cancel Plan
		[HttpPost]
		public HttpResponseMessage cancel_plan([FromBody] cancel_plan_req c)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("update corporate_mst set payment_status='" + c.corp_payment_status + "' where id=" + c.corp_id);
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{
				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}
		#endregion

		#region Get Credit Card Details
		[HttpGet]
		public HttpResponseMessage get_card_details([FromUri] string card_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var cid = Convert.ToInt32(card_id);
				var data = (from x in Context.customer_payment_mst2
							where x.id == cid
							select new get_credit_card_req() { firstname = x.firstname, lastname = x.lastname, city = x.city, state = x.state, card_number = x.card_number, card_exp = x.card_exp, default_payment = x.default_payment, zip_code = x.zip, payment_type = x.payment_type, security_code = x.card_csc }).ToList();
				return Request.CreateResponse(HttpStatusCode.OK, data);
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region Modify Credit Card Details
		[HttpPost]
		public HttpResponseMessage modify_credit_card([FromBody] add_credit_card_req card)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				var db = Context.Database;
				db.ExecuteSqlCommand("Update a set a.default_payment=0 from customer_payment_mst a where corp_id=" + card.corp_id);
				string s = "update customer_payment_mst set firstname='" + card.firstname + "',lastname='" + card.lastname + "',city='" + card.city + "',state='" + card.state + "',zip='" + card.zip_code + "',card_number='" + card.card_number + "',card_exp='" + card.expiration_month + "/" + card.expiration_year + "',card_csc='" + card.security_code + "',payment_type='" + card.payment_type + "', default_payment=" + card.default_payment + " where id=" + card.id;
				db.ExecuteSqlCommand(s);

				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region delete cashier
		[HttpGet]
		public HttpResponseMessage delete_cashier([FromUri] int cashier_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				Context.Database.ExecuteSqlCommand("DELETE from cashier_mst where id=" + cashier_id);
				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region delete cashier
		[HttpPost]
		public HttpResponseMessage delete_managers([FromBody] List<delete_managers_dm> data)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();
				for (int i = 0; i < data.Count; i++)
				{
					Context.Database.ExecuteSqlCommand("DELETE from managers_mst where id=" + data[i].manager_id);
					for (int j = 0; j < data[i].store_id.Count; j++)
					{
						Context.Database.ExecuteSqlCommand("DELETE from storelist_dtls where managerID=" + data[i].manager_id + " and store_id=" + data[i].store_id[j]);
					}

				}

				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}
			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion

		#region send forget manager password
		

		[HttpGet]
		public HttpResponseMessage manager_forgot_password(string manager_email)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();

				var data = (from x in Context.managers_mst2
							where x.manager_email == manager_email
							select new
							{
								id = x.Id,
								manager_name = x.manager_name,
								manager_pw = x.manager_pw
							})
							.ToList().FirstOrDefault();
				//sendMail
				var webRequest = WebRequest.Create(S3_path + "/forgot_pw_employee.htm");
				string body = "";
				using (var response = webRequest.GetResponse())
				using (var content = response.GetResponseStream())
				using (var reader = new StreamReader(content))
				{
					body = reader.ReadToEnd();
				}
				body = body.Replace("*[user first name]*", data.manager_name);
				body = body.Replace("*[id]*", data.id.ToString());
				body = body.Replace("*[pw]*", data.manager_pw);

				MailLogService.SendMail("support@shiftReports.com", manager_email, "Shift Reports Forgot Password Request", body);
				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion


		#region send forget cashier password
		


		[HttpGet]
		public HttpResponseMessage cashier_forgot_password(string cashier_email)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();

				var data = (from x in Context.cashier_mst2
							where x.cashier_email == cashier_email
							select new
							{
								id = x.id,
								cashier_name = x.cashier_name,
								cashier_pw = x.cashier_pw
							})
							.ToList().FirstOrDefault();

				var webRequest = WebRequest.Create(S3_path + "/forgot_pw_manager.htm");
				string body = "";
				using (var response = webRequest.GetResponse())
				using (var content = response.GetResponseStream())
				using (var reader = new StreamReader(content))
				{
					body = reader.ReadToEnd();
				}
				body = body.Replace("*[user first name]*", data.cashier_name);
				body = body.Replace("*[id]*", data.id.ToString());
				body = body.Replace("*[pw]*", data.cashier_pw);

				MailLogService.SendMail("support@shiftReports.com", cashier_email, "Shift Reports Forgot Password Request", body);

				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}

		}
		#endregion

		#region Update basic plan

		[HttpPost]
		public HttpResponseMessage update_free_plan([FromBody] update_free_plan_req data)
		{
			try
			{

				AppModel Context = new shiftreportapp.data.AppModel();

				var corp = Context.corporate_mst2.Find(data.corp_id);

				var plan = (from x in Context.plan_mst2
							where x.plan_type == data.plan_type && x.frequency.Equals("trial")
							select x.Id).FirstOrDefault();

				corp.plan_id = plan;

				Context.Entry(corp).State = EntityState.Modified;
				Context.SaveChanges();


				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}





		#endregion


		#region update_shiftdetails

		[HttpPost]
		public HttpResponseMessage update_shiftdetails([FromBody] update_shiftdetails_req data)
		{
			try
			{

				AppModel Context = new shiftreportapp.data.AppModel();

				var shift = Context.shift_details_mst2.Find(data.shift_id);

				shift.shift_open_time = data.shift_details_mst.shift_open_time;
				shift.shift_close_time = data.shift_details_mst.shift_close_time;
				shift.cash_counted = data.shift_details_mst.cash_counted;
				shift.cash_sales_all_registers = data.shift_details_mst.cash_sales_all_registers;
				shift.credit_sales_all_registers = data.shift_details_mst.credit_sales_all_registers;
				shift.debit_sales_all_registers = data.shift_details_mst.debit_sales_all_registers;
				shift.cash_sales_reg1 = data.shift_details_mst.cash_sales_reg1;
				shift.credit_sales_reg1 = data.shift_details_mst.credit_sales_reg1;
				shift.debit_sales_reg1 = data.shift_details_mst.debit_sales_reg1;
				shift.cash_sales_reg2 = data.shift_details_mst.cash_sales_reg2;
				shift.credit_sales_reg2 = data.shift_details_mst.credit_sales_reg2;
				shift.debit_sales_reg2 = data.shift_details_mst.debit_sales_reg2;

				Context.Entry(shift).State = EntityState.Modified;
				Context.Entry(data.drawer_open_mst).State = EntityState.Modified;
				Context.Entry(data.drawer_close_mst).State = EntityState.Modified;
				Context.Entry(data.safe_drops_mst).State = EntityState.Modified;

				Context.SaveChanges();

				var racks = Context.rack_ans_mst2.Where(r => r.shift_id == data.shift_id);

				// Loop to update the rack ans table
				
				foreach(var R in racks)
				{
					var l = (from L in data.shift_racks
							where L.rackset_id==R.rackset_id && L.rack_no==R.rack_no && L.row_no==R.row_no && L.col_no==R.row_no
							select L).FirstOrDefault();
					R.rack_added_value = l.added_value.ToString();
					R.rack_started_value = l.started_value.ToString();
					R.rack_ended_value = l.ended_value.ToString();

					Context.Entry(R).State = EntityState.Modified;
					Context.SaveChanges();
				}

			
				


				return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}
		}

		#endregion


		#region get_corporate_employee_hours


		[HttpGet]
		public HttpResponseMessage get_corporate_employee_hours(int corp_id)
		{
			try
			{
				AppModel Context = new shiftreportapp.data.AppModel();

				

				return Request.CreateResponse(HttpStatusCode.OK, new { success = 1 });
			}

			catch (AppException ex)
			{

				MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex.Message);

				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
			}

		}



		#endregion
	}
}
