using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using shiftreportapp.data;

using ExpenseTrackerLib.Rest;

using ExpensTrackerAPI.App_Code;
using LinkedliveWebLib.Error;

namespace ExpensTrackerAPI.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class StoreController : ApiController
    {
        AppModel Context = new AppModel();
       
        [HttpPost]
        public HttpResponseMessage businesslogin(businessLoginDM data)
        {

            try
            {
                var corp = Context.corporate_mst2.Where(r => r.corp_un.Equals(data.store_un) && r.corp_pw.Equals(data.store_pw)).FirstOrDefault();
                if (corp == null)
                {
                    throw new LoginException("Wrong User name or password");

                }


                return Request.CreateResponse(HttpStatusCode.OK, new { store_id = 1, total_cig_racks = 9, total_cig_rows = 10, business_name = "Atlanta_BP_Test" });
            }
                
            catch(LoginException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }


        [HttpPost]
        public HttpResponseMessage Signup(CorporateSignupDM data)
        {

            try
            {
                corporate_mst_helper mngr = new corporate_mst_helper();
                mngr.Addcorporate_mst(new AppModel.corporate_mst() { 
                                                                    corp_name=data.corp_name,
                                                                    corp_address1=data.corp_address1,
                                                                    corp_address2 = data.corp_address2,
                                                                    corp_city = data.city,
                                                                    corp_zip=data.zip_code

                                                                     });
                return Request.CreateResponse(HttpStatusCode.OK, new { store_id = 1 });
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        /// <summary>
        /// Get Full Store basic profile
        /// </summary>
        /// <param name="store_id">store_id</param>
        /// <returns>{corp_name,corp_address1,corp_address2,corp_city,corp_zip,corp_phone,corp_fax,corp_email,managers_num}</returns>
        [HttpGet]
        public HttpResponseMessage GetCorpProfile([FromUri] int corp_id)
        {

            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { });
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        /// <summary>
        /// Update the Corp Profile
        /// </summary>
        /// <param name="data">{corp_name,corp_address1,corp_address2,corp_city,corp_zip,corp_phone,corp_fax,corp_email,managers_num,manager_id}</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage UpdateCorpProfile(shiftreportapp.data.AppModel.corporate_mst data)
        {

            try
            {
              
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        /// <summary>
        /// Get a List of all the managers of the Corporate
        /// </summary>
        /// <param name="corp_id">The Corpoarte Id</param>
        /// <returns>List of all the managers</returns>
        [HttpGet]
        public HttpResponseMessage GetCorpMamangers(int corp_id)
        {

            try
            {

                var mngrs = (from x in Context.managers_mst2
                             where x.corp_id == corp_id
                             select new GetCorpManagersDM(){ manager_name = x.manager_name, manager_id = x.manager_id, manager_pw = x.manager_pw }).ToList();
                mngrs.Add(new GetCorpManagersDM(){ manager_name = "", manager_id = "", manager_pw = ""  });
                return Request.CreateResponse(HttpStatusCode.OK, new { corp_id = corp_id, GetCorpManagersDM = mngrs });
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }


        [HttpPut]
        public HttpResponseMessage UpdateCorpMamangers(UpdateCorpManagersDM data)
        {

            try
            {
                for(int i=0;i<data.GetCorpManagersDM.Count-1;i++)
                {
                    if(data.GetCorpManagersDM[i].Id==-1)
                    {
                        shiftreportapp.data.AppModel.managers_mst m = new shiftreportapp.data.AppModel.managers_mst();
                        m.corp_id = data.corp_id;
                        m.manager_id = data.GetCorpManagersDM[i].manager_id;
                        m.manager_name = data.GetCorpManagersDM[i].manager_name;
                        m.manager_pw = data.GetCorpManagersDM[i].manager_pw;
                        Context.managers_mst2.Add(m);
                    }
                    else
                    {
                        Context.Entry(data.GetCorpManagersDM[i]).State = System.Data.Entity.EntityState.Modified;
                    }
                    

                }
                Context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        
        
        
        /// <summary>
        /// Add a new Manager
        /// </summary>
        /// <param name="data">{[manager_name,manager_id,manager_pw]}</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddManager(AddManagerDM data)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        /// <summary>
        /// Switch The User Plan
        /// </summary>
        /// <param name="data">{store_id,plan_num}</param>
        /// <returns></returns>
        [HttpPut]
        public HttpResponseMessage ChangePlan(ChangePlanDM data)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Add a new store if store id =-1 it is a new store else update a  one
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddStore(AddStoreBasicDM data)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Get Store if store id=-1 ignore
        /// </summary>
        /// <param name="store_id"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetStore(int store_id)
        {

            try
            {
                if (store_id != -1)
                {

                }
                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Add Store rack details
        /// </summary>
        /// <param name="data">store_id,[cigs_rack_name,cigs_rack_columns,cigs_rack_rows],[lotto_rack_name,lotto_rack_columns],[cashier_name,cashier_num,cashier_pw)]</param>
        /// <returns></returns>
        [HttpPost]
        public HttpResponseMessage AddStoreRacksDetails(StoreRackDetailsDM data)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        /// <summary>
        /// Search for store with the store name
        /// </summary>
        /// <param name="search_store_name">Store name</param>
        /// <param name="search_store_zip">Store Zim</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetStoreList([FromUri] string search_store_name, [FromUri] string search_store_zip)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Get all manager store
        /// </summary>
        /// <param name="search_store_name">store name</param>
        /// <param name="search_store_zip">zip</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetManagerrStores([FromUri] string search_store_name, [FromUri] string search_store_zip,[FromUri] int manager_id)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Get all submited report for a store
        /// </summary>
        /// <param name="store_id">store_id</param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetShiftReportForStore([FromUri] int store_id)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
        /// <summary>
        /// Update the casheir cunt in the report
        /// </summary>
        /// <param name="data">store_id,[shift_id,cash_count]</param>
        /// <returns></returns>
        [HttpPut]
        public HttpResponseMessage UpdateCashCount(Upadtecashercountdm data)
        {

            try
            {

                return Request.CreateResponse(HttpStatusCode.OK, "");
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }

        [HttpPost]
        public HttpResponseMessage Manager_login(ManagerloginDM data)
        {

            try
            {
                var corp = Context.managers_mst2.Where(r => r.manager_name.Equals(data.manager_user_id) && r.manager_pw.Equals(data.manager_pw)).FirstOrDefault();
                if (corp == null)
                {
                    throw new LoginException("Wrong User name or password");

                }


                return Request.CreateResponse(HttpStatusCode.OK, new { });
            }

            catch (LoginException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }


        [HttpGet]
        public HttpResponseMessage GetManagerList(int id)
        {

            try
            {
                List<corp_mgr_listview> ls = new List<corp_mgr_listview>();
                ls.Add(new corp_mgr_listview() { mngrid=1, managername="1", mngrno="test", mgrpassword="test" });
                ls.Add(new corp_mgr_listview() { mngrid = 2, managername = "1", mngrno = "test", mgrpassword = "test" });
                ls.Add(new corp_mgr_listview() { mngrid = 3, managername = "1", mngrno = "test2", mgrpassword = "test" });
                ls.Add(new corp_mgr_listview() { mngrid = -1, managername = "", mngrno = "", mgrpassword = "" });
               


                return Request.CreateResponse(HttpStatusCode.OK, ls);
            }

            catch (LoginException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
            catch (Exception err)
            {
                ErrorLog.WriteToErrorLog("Get cheked in venues error", err);
                //throw new HttpResponseException(HttpStatusCode.InternalServerError);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }



        }
    
    }

    public class corp_mgr_listview
    {
        public int mngrid { set; get; }
        public string managername { set; get; }
        public string mngrno { set; get; }
        public string mgrpassword { set; get; }
    }


}
