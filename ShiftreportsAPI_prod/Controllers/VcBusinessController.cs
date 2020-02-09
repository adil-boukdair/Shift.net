using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BiomedicaLib.Net;
using ExpensTrackerAPI.Controllers;
using shiftreportapp.data;
using ShiftreportLib;
using static shiftreportapp.data.AppModel;
using static ShiftreportLib.internal_restful_dm;
using System.Globalization;
using System.IO;
using System.Configuration;

namespace ShiftReportApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VcBusinessController : ApiController
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


        AppModel Context = new AppModel();
        /// <summary>
        /// Register the user
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage BusinessInsight([FromUri] int store_id)
        {
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage CorpPayoutPreferences([FromUri] int corp_id)
        {
            try
            {
                var payoutPreferences = (from cpm in Context.vc_CorpPayoutMethods2
                                         from pm in Context.vc_PayoutMethods2
                                         where cpm.corp_id == corp_id
                                         where cpm.PayoutMethodId == pm.PayoutMethodId
                                         select new
                                         {
                                             Name = pm.Name,
                                             Decription = pm.Decription,
                                             fees = pm.fees,
                                             Currency = pm.Currency,
                                             details = pm.details,
                                             Status = cpm.Status,
                                             IsDefault=cpm.IsDefault,
                                             AccountDetails=cpm.AccountDetails,

                                             cpm.PayoutMethodId,
                                             cpm.Id
                                         }
                                         ).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, payoutPreferences);
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage ListPayoutPreferences()
        {
            try
            {
                var payoutMethods = Context.vc_PayoutMethods2.ToList();


                return Request.CreateResponse(HttpStatusCode.OK, payoutMethods);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage getCorpPayoutDetails([FromUri] int corp_id, [FromUri] int payoutMethodID)
        {
            try
            {

                var payoutDetails = Context.vc_CorpPayoutMethods2.Where(o => o.corp_id == corp_id && o.Id == payoutMethodID).FirstOrDefault();


                return Request.CreateResponse(HttpStatusCode.OK, payoutDetails);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage ModifyPayoutMethod(add_payment_method_post_req_dm data)
        {
            try
            {
                if (data.IsDefault) // turn all other method isDefault to false
                {
                    var corpPayoutMethods = Context.vc_CorpPayoutMethods2.Where(o => o.corp_id == data.corp_id).ToList();

                    foreach (var item in corpPayoutMethods)
                    {
                        item.IsDefault = false;

                    }

                }

                var corpPayoutMethod = Context.vc_CorpPayoutMethods2.Find(data.id);
                corpPayoutMethod.AccountDetails = data.AccountDetails;// bank name
                corpPayoutMethod.AccountName = data.AccountName;
                corpPayoutMethod.AccountType = data.AccountType;
                corpPayoutMethod.RoutingNumber = data.RoutingNumber;
                corpPayoutMethod.AccountNumber = data.AccountNumber;
                corpPayoutMethod.IsDefault = data.IsDefault;
                //log
                corpPayoutMethod.modify_dt = DateTime.Now;




                Context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        public HttpResponseMessage AddPayoutMethod(add_payment_method_post_req_dm data)
        { // ADD and Modify API
            try
            {
                if (data.IsDefault) // turn all other method isDefault to false
                {
                    var corpPayoutMethods = Context.vc_CorpPayoutMethods2.Where(o => o.corp_id == data.corp_id).ToList();

                    foreach (var item in corpPayoutMethods)
                    {
                        item.IsDefault = false;

                    }

                }



                vc_CorpPayoutMethods corpPayoutMethod = new vc_CorpPayoutMethods();

                corpPayoutMethod.AccountDetails = data.AccountDetails;// bank name
                corpPayoutMethod.AccountName = data.AccountName;
                corpPayoutMethod.AccountType = data.AccountType;
                corpPayoutMethod.RoutingNumber = data.RoutingNumber;
                corpPayoutMethod.AccountNumber = data.AccountNumber;
                corpPayoutMethod.IsDefault = data.IsDefault;
                corpPayoutMethod.PayoutMethodId = data.PayoutMethodId;
                corpPayoutMethod.Status = "Ready to Use";
                corpPayoutMethod.corp_id = data.corp_id;

                    //log data
                    corpPayoutMethod.create_dt = DateTime.Now;
                    corpPayoutMethod.created_userid = "system";
                    corpPayoutMethod.modify_dt = DateTime.Now;
                    corpPayoutMethod.Modify_usrid = "system";

                Context.vc_CorpPayoutMethods2.Add(corpPayoutMethod);


                // send VenueCash Email
                var corporate = Context.corporate_mst2.Find(data.corp_id);
                var manager = Context.managers_mst2.Find(corporate.acct_admin_id);
                VenueCashEmails.bankAccountAddedBusiness(manager.manager_email, manager.manager_name);


                Context.SaveChanges();

                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage TransactionHistory(TransactionHistoryInput data)
        {

            try {
                // var customerTransactions = Context.vc_CustomerTransactions2.Where(o=>o.StoreId==data.store_id);
                var customerTransactions = (from t in Context.vc_CustomerTransactions2
                                            where t.StoreId == data.store_id
                                            select new {
                                                Id = t.Id,
                                                modify_dt = t.modify_dt,
                                                TotalAmount = t.TotalAmount,
                                                VenueCashEarned_Total = t.VenueCashEarned_Total,
                                                PaidOut = t.PaidOut,
                                                Status = t.Status,
                                                Paid_Credit_Amount = t.Paid_Credit_Amount,
                                                totalRefundedCC = (Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Sum(o => (double?)o.cc_refunded_amount) ?? 0),
                                                totalEmployeeRefunded = (Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Sum(o => (double?)o.cashier_refunded_amount) ?? 0),
                                                OtherFinancingAmount = t.OtherFinancingAmount,
                                                PayoutAmount=t.PayoutAmount
                                            }
                                            );

 





                //Filters //0:ALL,1:ORDERS,2:Refunds,3:Payouts
                switch (data.filters)
                {
                    case 0://ALL
                        customerTransactions = customerTransactions.Where(o => o.Status == "APPROVED");
                        break;
                    case 1://ORDERS
                        customerTransactions = customerTransactions.Where(o => o.PaidOut == false && o.Status == "APPROVED");
                        break;
                    case 2://REFUNDS
                        customerTransactions = customerTransactions.Where(o => o.Status.ToUpper() == "REFUNDED" || o.Status.ToUpper() == "PARTIAL REFUND");
                        break;
                    case 3://PAYOUTS
                        customerTransactions = customerTransactions.Where(o => o.PaidOut == true);
                        break;
                }

                //Sort //0:Newest Transactions,1:Last Transactions,2:Highest To lowest payout,3:Lowest to Highest Payout
                switch (data.sort)
                {
                    case 0: //0:Newest Transactions
                        customerTransactions = customerTransactions.OrderByDescending(o => o.modify_dt);
                        break;
                    case 1: //1:Last Transactions ?? NOT YET IMPLEMENTED ASK JOHN
                        customerTransactions = customerTransactions.OrderByDescending(o => o.modify_dt);
                        break;
                    case 2: //2:Highest To lowest payout
                        customerTransactions = customerTransactions.OrderByDescending(o => o.TotalAmount);
                        break;
                    case 3: //2:Highest To lowest payout
                        customerTransactions = customerTransactions.OrderByDescending(o => o.TotalAmount);
                        break;

                }

                //Date Range
                DateTime startDate = new DateTime();
                DateTime endtDate = new DateTime();
                if (data.startDate != null && DateTime.TryParse(data.startDate, out startDate) && data.endDate != null && DateTime.TryParse(data.endDate, out endtDate)) {
                    customerTransactions = customerTransactions.Where(o => o.modify_dt >= startDate && o.modify_dt <= endtDate);
                }



                if (data.csv == false)
                {
                    // pagination Code must be last
                    customerTransactions = customerTransactions.Skip(data.pageSize * (data.page - 1)).Take(data.pageSize);
                }




                List<TransactionHistoryOutPut> TransactionHistory = new List<TransactionHistoryOutPut>();

                //
                double pendingPayouts = 0;
                double paidOut = 0;

                //Loop over customerTransaction results
                foreach (var customerTransaction in customerTransactions)
                {
                    TransactionHistoryOutPut transactionHistory = new TransactionHistoryOutPut();

                    transactionHistory.id = customerTransaction.Id;
                    transactionHistory.date = (DateTime?)customerTransaction.modify_dt;
                    transactionHistory.details = "";//???
                    transactionHistory.totalSale = (double)customerTransaction.TotalAmount;
                    transactionHistory.VenueCashEarned_Total = (double)customerTransaction.VenueCashEarned_Total;
                    transactionHistory.paidOut = (bool)customerTransaction.PaidOut;
                    transactionHistory.Status = customerTransaction.Status;
                     

                    transactionHistory.netPayoutAmount = (double)customerTransaction.PayoutAmount;

 


                    TransactionHistory.Add(transactionHistory);
                }


                /*############################################ Adding coupon Transactions #########################################################*/
                // get promoter id
                var promoter = Context.promoters_mst2.Where(p => p.Store_ID == data.store_id).FirstOrDefault();
                if (promoter != null)
                {
                    var couponTransactions = Context.coupon_code_transactions2.Where(c => c.promoter_userid==promoter.promoters_userid).OrderByDescending(c=>c.Date_Redeemed_Customer).Skip(data.pageSize * (data.page - 1)).Take(data.pageSize);

                    foreach ( var couponTransaction in couponTransactions)
                    {
                        TransactionHistoryOutPut transactionHistory = new TransactionHistoryOutPut();

                        transactionHistory.id = couponTransaction.coupon_code_transaction_ID;
                        transactionHistory.date = couponTransaction.Date_Redeemed_Customer;
                        transactionHistory.totalSale = couponTransaction.VenueCash_Credit_Amt;
                        transactionHistory.VenueCashEarned_Total = couponTransaction.VenueCash_Credit_Amt;
                        transactionHistory.paidOut = false;
                        transactionHistory.Status = "Redeemed";
                        transactionHistory.ccFees =  0;//to be calculated
                        transactionHistory.netPayoutAmount= couponTransaction.VenueCash_Credit_Amt * -1;

                        TransactionHistory.Add(transactionHistory);
                    }

                }
                /*#########################################################################################################################################*/

                 
                pendingPayouts = Context.vc_CustomerTransactions2.Where(t => t.PaidOut == false && t.StoreId == data.store_id && t.Status == "approved").Sum(t => (double?) t.PayoutAmount) ?? 0;

                paidOut = Context.vc_CustomerTransactions2.Where(t => t.PaidOut == true && t.StoreId == data.store_id && t.Status == "approved").Sum(t => (double?)t.PayoutAmount) ?? 0;


                // deduce coupon used from pendingPayout
                // get sum coupon
                if (promoter != null)
                {
                    double totalCoupon = (Context.coupon_code_transactions2.Where(c => c.promoter_userid == promoter.promoters_userid && c.PaidOut==false).Sum(o => (double?)o.VenueCash_Credit_Amt) ?? 0);

                    pendingPayouts = pendingPayouts - totalCoupon;
                }

                // deduce refunds
                double totalRefunds = Context.vc_refunds2.Where(r => r.store_id == data.store_id && r.PaidOut == false).Sum(r => (double?) r.PayoutAmount) ??0;
                pendingPayouts = pendingPayouts - totalRefunds;


                // Reorder TransactionHistory list by modify date
                TransactionHistory =TransactionHistory.OrderByDescending(t => t.date).ToList();


                    return Request.CreateResponse(HttpStatusCode.OK, new { pendingPayout = pendingPayouts, paidOut = paidOut, Transactions = TransactionHistory });
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage RewardSettingsList([FromUri] int store_id)
        {
            try
            {
                var resp = (from n in Context.store_profile_mst2
                            from m in Context.vc_store_profile_mst2
                            where n.id == m.store_id
                            select new {
                                store_id = n.id,
                                store_name = n.store_name,
                                is_gas_station = m.is_gas_station,
                                gas_back_rate = m.gas_back_rate,
                                cash_back_rate = m.cash_back_rate
                            }).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, resp);
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage GetRewardSettings([FromUri] int store_id)
        {
            try
            {
                var rewardSettings = (from s in Context.vc_store_profile_mst2
                                      where s.store_id == store_id
                                      select new
                                      {
                                          id = s.Id,
                                          store_id = s.store_id,
                                          is_gas_station=s.is_gas_station,
                                          cash_back_rate = s.cash_back_rate,
                                          gas_back_rate = s.gas_back_rate,
                                      }
                                  ).FirstOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, rewardSettings);
            }

            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage EditRewardSettings(EditRewardSettings_post_req_dm data)
        {
            try
            {
                var rewardSettings = Context.vc_store_profile_mst2.Where(o => o.store_id == data.store_id).FirstOrDefault();

                if (data.businessType == 1) { 
                rewardSettings.cash_back_rate = data.cash_back_rate;
                }
                else if (data.businessType == 2)
                {
                    rewardSettings.gas_back_rate = data.gas_back_rate;
                }
                

                Context.SaveChanges();


                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage add_cashier(cashier_mst data)
        {
            try
            {
                data.create_date = DateTime.Now;
                data.modified_date = DateTime.Now;
                data.create_userid = "system";
                data.modified_userid = "system";

                Context.cashier_mst2.Add(data);
                Context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        public HttpResponseMessage modify_cashier(cashier_mst data)
        {
            try
            {
                var c = Context.cashier_mst2.Find(data.id);
                data.modified_date = DateTime.Now;
                data.create_userid = "system";
                Context.Entry(c).State = System.Data.Entity.EntityState.Modified;
                Context.SaveChanges();
                return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetRegisterList([FromUri] int store_id)
        {
            try
            {


                var storeRegister = Context.vc_StoreRegisters2.Where(r => r.store_id == store_id).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, storeRegister);
            }
            catch (AppException ex)
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpGet]
        public HttpResponseMessage Employeetips([FromUri] int store_id, [FromUri] int page, [FromUri] int pageSize)
		{
			try
			{
				var tips = (from s in Context.shift_details_mst2
                            from c in Context.cashier_mst2
						    where s.store_id == store_id
                            where s.cashier_id==c.id
						 select new {
                             employeeName=c.cashier_name,
                             shift_date=s.shift_date,
                             shift_id=s.id,
                             shift_num=s.shift_num,
                             Tip_Payout_Amount=s.Tip_Payout_Amount,
                             Tip_Payout_Status=s.Tip_Payout_Status
						 });

                //orderby shift Date
                tips = tips.OrderByDescending(o => o.shift_id);

                // pagination Code must be last
                tips = tips.Skip(pageSize * (page - 1)).Take(pageSize);

                return Request.CreateResponse(HttpStatusCode.OK, tips.ToList());
			}
			catch (Exception ex)
			{
				return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
			}
		}

        [HttpGet]
        public HttpResponseMessage EmployeeTipsByShift([FromUri] int shift_id)
        {
            try
            {
                var transactions = (from t in Context.vc_CustomerTransactions2
                            from c in Context.cashier_mst2
                            from cu in Context.vc_customers2
                            where t.shift_id== shift_id
                            where t.cashier_id == c.id
                            where t.CustomerId ==cu.id
                            where t.Tip_Amount>0
                            where t.Status.ToUpper()== "APPROVED" || t.Status.ToUpper() == "REFUNDED" || t.Status.ToUpper() == "PARTIAL REFUND"
                            select new
                            {
                                employeeName = c.cashier_name,
                                Date = t.Date,
                                Status = t.Status,
                                FirstName = cu.FirstName,
                                LastName = cu.LastName,
                                PaidOut = t.PaidOut,
                                Tip_Amount=t.Tip_Amount,
                                customerTransactionID=t.Id
                            }).OrderByDescending(o => o.Date).ToList();

                return Request.CreateResponse(HttpStatusCode.OK, transactions);
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        //Yelp API's
        [HttpGet]
        public HttpResponseMessage searchYelpBusinesses([FromUri] string term, [FromUri] string location,[FromUri] int offset )
        {


            var yelpBusinesses = YelpV3.Search(-1, -1, -1, term, location, null, 1, offset);



            return Request.CreateResponse(HttpStatusCode.OK, yelpBusinesses);

        }
        [HttpGet]
        public HttpResponseMessage getYelpBusiness([FromUri] string yelp_id)
        {
            try
            {

                var yelpBusinesses = YelpV3.getBusiness(yelp_id);


                return Request.CreateResponse(HttpStatusCode.OK, yelpBusinesses);
            }



          catch (AppException ex)
            {

             

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
            }

        }



        [HttpPost]
        public HttpResponseMessage confirmPaidOut([FromBody] ConfirmPaidOutInput data)
        {


            var shiftDetailsMst = Context.shift_details_mst2.Find(data.shift_id);



            shiftDetailsMst.Tip_Payout_Status = "PAYOUT_PAID";

            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, 0);

        }


        /*############### Coupon System API's ######################*/
        [HttpGet]
        public HttpResponseMessage getCouponCodeListByStore([FromUri] int store_id)
        {

            // get promoter user Id
            var promoter = Context.promoters_mst2.Where(p => p.Store_ID == store_id).FirstOrDefault();
            if (promoter == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);// promoter doesnt exist
            }

            var coupon = Context.vc_coupon_code_mst2.Where(c => c.promoter_userid == promoter.promoters_userid)
                        .Select(c => new {
                            Coupon_Code_ID = c.Coupon_Code_ID,
                            Coupon_Promotion_Name = c.Coupon_Promotion_Name,
                            VenueCash_Credit_Amt = c.VenueCash_Credit_Amt,
                            Code_Type = c.Code_Type,
                        }).ToList();
 



            return Request.CreateResponse(HttpStatusCode.OK, coupon);

        }


        [HttpGet]
        public HttpResponseMessage getCouponCode()
        {

            var coupon = new vc_coupon_code_mst();

            coupon.Promoter_business_Sales_Commision_Rate = 0;
            coupon.Promoter_business_Commission_Sign_Up_Amt = 0;
            coupon.Promoter_customer_Sales_Commision_Rate = 0;
            coupon.Promoter_customer_Commission_Sign_Up_Amt = 0;
            coupon.SignUP_VenueCash_Credit_Amt = 0;
            coupon.VenueCash_Credit_Amt = 0;
            coupon.SignUP_VC_Credit_Issued_by_VenueCash_Amt = 0;
            coupon.SignUP_AC_Credit_Issued_by_VenueCash_Amt = 0;
            coupon.VC_Credit_Issued_by_VenueCash_Amt = 0;
            coupon.AC_Credit_Issued_by_VenueCash_Amt = 0;
            coupon.Code_Redeemed = false;


            Context.vc_coupon_code_mst2.Add(coupon);

            Context.SaveChanges();


            return Request.CreateResponse(HttpStatusCode.OK, new { Coupon_Code_ID = coupon.Coupon_Code_ID });

        }

        [HttpPost]
        public HttpResponseMessage addCoupon([FromBody] addCouponInput data)
        {
            var coupon = Context.vc_coupon_code_mst2.Find(data.Coupon_Code_ID);

            coupon.Code_Type = data.Code_Type;
            coupon.Coupon_Promotion_Name = data.Coupon_Promotion_Name;
            coupon.Coupon_Code_Start_Date = data.Coupon_Code_Start_Date;
            coupon.Coupon_Code_Expire_Date = data.Coupon_Code_Expire_Date;
            coupon.VenueCash_Credit_Amt = data.VenueCashAmount;

            coupon.create_date = DateTime.Now;
            coupon.create_dt = DateTime.Now;
            coupon.create_userid = "System";
            coupon.modified_date = DateTime.Now;
            coupon.modified_userid = "System";
            
            // get promoter user Id
            var promoter = Context.promoters_mst2.Where(p=>p.Store_ID==data.store_id).FirstOrDefault();
            if (promoter == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);// promoter doesnt exist
            }

            coupon.promoter_userid = promoter.promoters_userid;


            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, 0);

        }
        [HttpPost]
        public HttpResponseMessage editCoupon([FromBody] addCouponInput data)
        {
            var coupon = Context.vc_coupon_code_mst2.Find(data.Coupon_Code_ID);

            coupon.Code_Type = data.Code_Type;
            coupon.Coupon_Promotion_Name = data.Coupon_Promotion_Name;
            coupon.Coupon_Code_Start_Date = data.Coupon_Code_Start_Date;
            coupon.Coupon_Code_Expire_Date = data.Coupon_Code_Expire_Date;
            coupon.VenueCash_Credit_Amt = data.VenueCashAmount;

 
            coupon.modified_date = DateTime.Now;
            coupon.modified_userid = "System";

         

            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, 0);

        }


        [HttpGet]
        public HttpResponseMessage getCoupon([FromUri] int Coupon_Code_ID)
        {


            var coupon = Context.vc_coupon_code_mst2.Find(Coupon_Code_ID);



            return Request.CreateResponse(HttpStatusCode.OK, new {
                Coupon_Code_ID = coupon.Coupon_Code_ID,
                Code_Type = coupon.Code_Type,
                Coupon_Promotion_Name = coupon.Coupon_Promotion_Name,
                Coupon_Code_Start_Date = coupon.Coupon_Code_Start_Date,
                Coupon_Code_Expire_Date = coupon.Coupon_Code_Expire_Date,
                VenueCashAmount = 0,
            });

        }


        [HttpGet]
        public HttpResponseMessage forgotPassword([FromUri] string email)
        {

            var manager = Context.managers_mst2.Where(m => m.manager_email== email).FirstOrDefault();

            if (manager != null)
            {

                VenueCashEmails.forgotPassword(manager.manager_email, manager.manager_name, manager.manager_pw);

                return Request.CreateResponse(HttpStatusCode.OK, 0);
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);
            }



        }

        [HttpGet]
        public HttpResponseMessage payoutHistory([FromUri] int corp_id)
        {

            // Create Response
            var payouts =      (from pt in Context.vc_PayoutTransactions2
                                from pm in Context.vc_CorpPayoutMethods2
                                where pt.Corp_ID == corp_id
                                where pt.PayoutMethodID==pm.Id
                                where pt.PaidOut==true
                                select new
                                {
                                    date=pt.create_dt,
                                    Amount =pt.AmountSent,
                                    AccountNumber = pm.AccountNumber.Substring(pm.AccountNumber.Length - 4, 4)
                                }
                                 ).OrderByDescending(p=>p.date).ToList();


             
            


                return Request.CreateResponse(HttpStatusCode.OK, payouts);
            
        }



        [HttpGet]
        public HttpResponseMessage testAPI()
        {

            /* Authorization API
             // Get CreditCard Info and customer info to send to paypal
             var vcCustomerCreditCard = Context.vc_CustomerCreditCards2.Where(o => o.CustomerId == 1 && o.IsDefault == true).FirstOrDefault();


             / create paypal credit card
                 PayPal.Api.CreditCard creditCard = new PayPal.Api.CreditCard();
                 creditCard.type = vcCustomerCreditCard.CardType.ToLower();
                 creditCard.number = vcCustomerCreditCard.CardNumber;
                 creditCard.expire_month = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[0]);
                 creditCard.expire_year = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[1]) + 2000;
                 creditCard.first_name = vcCustomerCreditCard.NameOnCard.Split(' ')[0];
                 creditCard.last_name = vcCustomerCreditCard.NameOnCard.Split(' ')[1];

                 var paymentGateWayResponse = PayPalProcessor.Authorize(creditCard, 50);

                  return Request.CreateResponse(HttpStatusCode.OK, paymentGateWayResponse);
                 */


            // void authorization api
            //var paymentGateWayResponse = PayPalProcessor.VoidAuthorize("", 50);

            // MailLogService.SendMail("support@shiftreports.com", "adil@shiftrepots.com,adil.boukdair@gmail.com", "We want a welcome Kit", "Hello");
            //var vcCustomerCreditCard = Context.vc_CustomerCreditCards2.Where(o => o.CustomerId == 1 && o.IsDefault == true).FirstOrDefault();


            var order_number=usefullFunctions.getNextOrderNumber(161,2);


            return Request.CreateResponse(HttpStatusCode.OK, order_number);
        }



    }



}