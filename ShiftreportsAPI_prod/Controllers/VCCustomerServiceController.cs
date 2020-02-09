using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShiftreportLib;
using static ShiftreportLib.internal_restful_dm;
using shiftreportapp.data;

namespace ShiftReportApi.Controllers
{


 
    public class VCCustomerServiceController : ApiController
    {

        public string AccessToken = "srrz$^4qHf5fvACWEU0CCMIU";
        AppModel Context = new AppModel();


        [HttpPost]
        public HttpResponseMessage login(LoginBindingModel data)
        {
            try
            {
                 if((data.UserName.ToLower()=="john" && data.Password == "shiftlive777") ||
                    (data.UserName.ToLower() == "ben" && data.Password == "shiftlive888")||
                    (data.UserName.ToLower() == "bogdan" && data.Password == "qwertu777") ||
                    (data.UserName.ToLower() == "daci" && data.Password == "qwertu888"))
                {


                    return Request.CreateResponse(HttpStatusCode.OK, new {AccessToken=AccessToken });
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);
                }


               

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }


        [HttpPost]
        public HttpResponseMessage getBusinessPayouts(businessPayoutsInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {
                    var businessPayouts = (from p in Context.vc_PayoutTransactions2
                                        from cp in Context.vc_CorpPayoutMethods2
                                        from c in Context.corporate_mst2
                                        where c.Id==p.Corp_ID
                                        where p.Corp_ID==cp.corp_id
                                        where cp.IsDefault==true
                                        where p.PaidOut==false
                                        select new
                                        {
                                            corp_id=c.Id,
                                            corp_name =c.corp_name,

                                            id=p.id,
                                            DateToBePaid=p.DateToBePaid,
                                            PayoutAmount=p.PayoutAmount,
                                            AmountSent = p.AmountSent,
                                            DateSent = p.DateSent,
                                            ProcessorConfirmationId=p.ProcessorConfirmationId,
                                            
                                            cp.RoutingNumber,
                                            cp.AccountNumber
                                        }).ToList();


                    //var businessPayouts = Context.vc_PayoutTransactions2.Where(p=>p.PaidOut==false).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, businessPayouts);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }
                


                

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }

        [HttpPost]
        public HttpResponseMessage processPayouts(processPayoutsInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    foreach( var payout in data.payouts)
                    {
                        if (payout.PaidOut)
                        {
                        var dbPayouts = Context.vc_PayoutTransactions2.Find(payout.id);
                        dbPayouts.modify_dt = DateTime.Now;
                        dbPayouts.modify_usrid = data.user;
                        dbPayouts.PaidOut = true;
                        dbPayouts.AmountSent = payout.AmountSent;
                        dbPayouts.ProcessorConfirmationId = payout.ProcessorConfirmationId;

                            // Date Sent
                            if (payout.DateSent==null)
                            {
                                dbPayouts.DateSent = dbPayouts.DateToBePaid;
                            }
                            else
                            {
                                dbPayouts.DateSent = payout.DateSent;
                            }
                            // Sent amount
                            if(payout.AmountSent==0)
                            {
                                dbPayouts.AmountSent = dbPayouts.PayoutAmount;
                            }
                            else
                            {
                                dbPayouts.AmountSent = payout.AmountSent;
                            }
                        //dbPayouts.DateSent = payout.DateSent;// formating 
                        Context.SaveChanges();

                        }

                    }


                    // Everything is good load the left payouts
                    var businessPayouts = (from p in Context.vc_PayoutTransactions2
                                           from cp in Context.vc_CorpPayoutMethods2
                                           from c in Context.corporate_mst2
                                           where c.Id == p.Corp_ID
                                           where p.Corp_ID == cp.corp_id
                                           where cp.IsDefault == true
                                           where p.PaidOut == false
                                           select new
                                           {
                                               corp_id = c.Id,
                                               corp_name = c.corp_name,

                                               id = p.id,
                                               DateToBePaid = p.DateToBePaid,
                                               PayoutAmount = p.PayoutAmount,
                                               AmountSent = p.AmountSent,
                                               DateSent = p.DateSent,
                                               ProcessorConfirmationId = p.ProcessorConfirmationId,

                                               cp.RoutingNumber,
                                               cp.AccountNumber
                                           }).ToList();

                    return Request.CreateResponse(HttpStatusCode.OK, businessPayouts);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }



        // Corporate
        [HttpPost]
        public HttpResponseMessage getCorporates(corporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    if (data.searchBy == -2) // Get only one instance
                    {
                        int value = int.Parse(data.value);
                        var corporate = Context.corporate_mst2.Find(value);
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == -1) // All
                    {
                        var corporate = Context.corporate_mst2.OrderByDescending(o=>o.Id).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 0) // by corp_id
                    {
                        int value = int.Parse(data.value);
                        var corporate = Context.corporate_mst2.Where(c=>c.Id== value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 1) // by acc_admin_id
                    {
                        int value = int.Parse(data.value);
                        var corporate = Context.corporate_mst2.Where(c => c.acct_admin_id == value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 2) // by Corp name
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_name.Contains(data.value) ).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 3) // by Address 1
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_address1.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 4) // by Address 2
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_address2.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 5) // by City
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_city.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 6) // by City
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_state.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 7) // by Phone
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_phone.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 8) // by Fax
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_fax.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 9) // by Email
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_email.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 10) // by zip
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.corp_zip == data.value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 11) // by first time flag
                    {
                        int value = int.Parse(data.value);
                        var corporate = Context.corporate_mst2.Where(c => c.firsttimelogin_flag == value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }
                    else if (data.searchBy == 12) // Yelp Id
                    {
                        var corporate = Context.corporate_mst2.Where(c => c.yelp_id == data.value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, corporate);
                    }



                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }

        [HttpPost]
        public HttpResponseMessage editCorporate(modifyCorporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    var corporate = Context.corporate_mst2.Find(data.corporate.Id);
                    if (corporate != null)
                    {
                        corporate.acct_admin_id = data.corporate.acct_admin_id;
                        corporate.corp_name = data.corporate.corp_name;
                        corporate.corp_address1 = data.corporate.corp_address1;
                        corporate.corp_address2 = data.corporate.corp_address2;
                        corporate.corp_city = data.corporate.corp_city;
                        corporate.corp_state = data.corporate.corp_state;
                        corporate.corp_phone = data.corporate.corp_phone;
                        corporate.corp_fax = data.corporate.corp_fax;
                        corporate.corp_email = data.corporate.corp_email;
                        corporate.corp_zip = data.corporate.corp_zip;
                        corporate.firsttimelogin_flag = data.corporate.firsttimelogin_flag;
                        corporate.yelp_id = data.corporate.yelp_id;

                        // Log
                        corporate.modified_date = DateTime.Now;
                        corporate.modified_userid = data.user;

                        Context.SaveChanges();
                    }

                  
                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }

        // End Corporate


        // Store
        [HttpPost]
        public HttpResponseMessage getStores(corporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    if (data.searchBy == -2) // Get only one instance
                    {
                        int value = int.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.id==value
                                      select new
                                      { s = s, vcs = vcs }).FirstOrDefault();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == -1) // All
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      select new
                                      {s = s,vcs=vcs  }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 0) // Corp ID
                    {
                        int value = int.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.corp_id== value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 1) // Store ID
                    {
                        int value = int.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.id == value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 2) // Store Name
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_name.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 3) // Store Address 1
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_address1.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 4) // Store Address 2
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_address2.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 5) // Store City
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_city.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 6) // Store State
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_state.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 7) // Store Phone
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_phone_no.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 8) // Store Fax
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_fax_no.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 9) // Store Email
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_email.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 10) // Store Zip
                    {
                        int value = int.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where s.store_zip== value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 11) // Store cash_back_rate
                    {
                        double value = double.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where vcs.cash_back_rate == value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 12) // Store Category Name
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where vcs.Category_Name.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 13) // Store Is Mobile Register
                    {
                        bool value = bool.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where vcs.is_mobile_register == value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 13) // Store Is tips
                    {
                        bool value = bool.Parse(data.value);
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where vcs.is_tips == value
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }
                    else if (data.searchBy == 14) // Store Yelp ID
                    {
                        var stores = (from s in Context.store_profile_mst2
                                      from vcs in Context.vc_store_profile_mst2
                                      where s.id == vcs.store_id
                                      where vcs.yelp_store_id.Contains(data.value)
                                      select new
                                      { s = s, vcs = vcs }).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, stores);
                    }

 

                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage editStore(modifyStoreInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    var store = Context.store_profile_mst2.Find(data.store.id);
                    var vc_store = Context.vc_store_profile_mst2.Find(data.vc_store.Id);
                    if (store != null && vc_store!=null)
                    {
                        // store 
                        store.store_name = data.store.store_name;
                        store.store_address1 = data.store.store_address1;
                        store.store_address2 = data.store.store_address2;
                        store.store_city = data.store.store_city;
                        store.store_state = data.store.store_state;
                        store.store_phone_no = data.store.store_phone_no;
                        store.store_fax_no = data.store.store_fax_no;
                        store.store_email = data.store.store_email;
                        store.store_zip = data.store.store_zip;

                        //vc store
                        vc_store.cash_back_rate = data.vc_store.cash_back_rate;
                        vc_store.Category_Name = data.vc_store.Category_Name;
                        vc_store.is_mobile_register = data.vc_store.is_mobile_register;
                        vc_store.is_tips = data.vc_store.is_tips;
                        vc_store.yelp_store_id = data.vc_store.yelp_store_id;

                        // log
                        store.modified_date = DateTime.Now;
                        store.modified_userid = data.user;
                        vc_store.modify_dt = DateTime.Now;
                        vc_store.Modify_usrid = data.user;

                        Context.SaveChanges();
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }


        // Managers

        // Store
        [HttpPost]
        public HttpResponseMessage getManagers(corporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    if (data.searchBy == -2) // Get only one instance
                    {
                        int value = int.Parse(data.value);
                        var manager = Context.managers_mst2.Find(value);
                  
                        return Request.CreateResponse(HttpStatusCode.OK, manager);
                    }
                    else if (data.searchBy == -1) // All
                    {

                        var managers = Context.managers_mst2.OrderByDescending(m => m.Id).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 0) // By Corp ID
                    {
                        int value = int.Parse(data.value);
                        var managers = Context.managers_mst2.Where(m => m.corp_id== value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 1) // By Manager ID
                    {
                        int value = int.Parse(data.value);
                        var managers = Context.managers_mst2.Where(m => m.Id == value).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 2) // By Manager name
                    {
                        var managers = Context.managers_mst2.Where(m => m.manager_name.Contains(data.value) ).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 3) // By Manager Email
                    {
                        var managers = Context.managers_mst2.Where(m => m.manager_email.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 4) // By Manager Phone
                    {
                        var managers = Context.managers_mst2.Where(m => m.manager_cell_phone.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 5) // By Manager Password
                    {
                        var managers = Context.managers_mst2.Where(m => m.manager_pw.Contains(data.value)).ToList();
                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }





                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }
        // modify manager

        [HttpPost]
        public HttpResponseMessage editManager(modifyManagerInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {
                    
                    var manager = Context.managers_mst2.Find(data.manager.Id);

                    if (manager != null)
                    {
                        manager.manager_name = data.manager.manager_name;
                        manager.manager_cell_phone = data.manager.manager_cell_phone;
                        manager.manager_pw = data.manager.manager_pw;

                        //Log
                        manager.modified_date = DateTime.Now;
                        manager.modified_userid = data.user;

                        Context.SaveChanges();
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }

        // Customer

        [HttpPost]
        public HttpResponseMessage getCustomers(corporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    if (data.searchBy == -2) // Get only one instance
                    {
                        int value = int.Parse(data.value);
                        var customer = Context.vc_customers2.Find(value);

                        return Request.CreateResponse(HttpStatusCode.OK, customer);
                    }
                    else if (data.searchBy == -1) // All
                    {

                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 0) // Customer ID
                    {
                        int value = int.Parse(data.value);
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c=>c.id== value).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 1) // First Name
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.FirstName.Contains(data.value)).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 2) // Last Name
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.LastName.Contains(data.value)  ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 3) // Username
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.UserName.Contains(data.value)   ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 4) // Email
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.Email.Contains(data.value)).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 5) // Password
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.PasswordHash.Contains(data.value)).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }
                    else if (data.searchBy == 6) // Pin
                    {
                        var managers = Context.vc_customers2.OrderByDescending(m => m.id).Where(c => c.Pin.Contains(data.value)).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, managers);
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }
        // modify Customer

        [HttpPost]
        public HttpResponseMessage editCustomer(modifyCustomerInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    var customer = Context.vc_customers2.Find(data.customer.id);

                    if (customer != null)
                    {

                        customer.FirstName = data.customer.FirstName;
                        customer.LastName = data.customer.LastName;
                        customer.PasswordHash = data.customer.PasswordHash;
                        customer.Email = data.customer.Email;
                        customer.Pin = data.customer.Pin;

                        //Log
                        customer.modify_dt = DateTime.Now;
                        customer.Modify_usrid = data.user;

                        Context.SaveChanges();
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }

        // Customer

        [HttpPost]
        public HttpResponseMessage getEmployees(corporateInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {

                    if (data.searchBy == -2) // Get only one instance
                    {
                        int value = int.Parse(data.value);
                        var employee = Context.cashier_mst2.Find(value);

                        return Request.CreateResponse(HttpStatusCode.OK, employee);
                    }
                    else if (data.searchBy == -1) // All
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 0) // Employee ID
                    {
                        int value = int.Parse(data.value);
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c=>c.id== value).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 1) // Store ID
                    {
                        int value = int.Parse(data.value);
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.store_id == value).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 2) // Name
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.cashier_name.Contains(data.value) ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 3) // Title
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.title.Contains(data.value)).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 4) // Password
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.cashier_pw.Contains(data.value) ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 5) // Phone
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.cashier_cell_phone.Contains(data.value) ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }
                    else if (data.searchBy == 6) // Email
                    {
                        var employees = Context.cashier_mst2.OrderByDescending(m => m.id).Where(c => c.cashier_email.Contains(data.value) ).ToList();

                        return Request.CreateResponse(HttpStatusCode.OK, employees);
                    }



                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }

            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }
        }
        // modify Customer

        [HttpPost]
        public HttpResponseMessage editEmployee(modifyEmployeeInput data)
        {
            try
            {
                if (data.AccessToken == AccessToken)
                {
                    var employee = Context.cashier_mst2.Find(data.employee.id); 

                    if (employee != null)
                    {
                        employee.cashier_name = data.employee.cashier_name;
                        employee.title = data.employee.title;
                        employee.cashier_pw = data.employee.cashier_pw;
                        employee.cashier_cell_phone = data.employee.cashier_cell_phone;
                        employee.cashier_email = data.employee.cashier_email;

                        //log
                        employee.modified_date = DateTime.Now;
                        employee.modified_userid = data.user;


                        Context.SaveChanges();
                    }


                    return Request.CreateResponse(HttpStatusCode.OK, 0);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// Access Denied
                }





            }
            catch (Exception err)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, err.Message);
            }


        }


    }//class VCCustomerServiceController
}// NameSpace
