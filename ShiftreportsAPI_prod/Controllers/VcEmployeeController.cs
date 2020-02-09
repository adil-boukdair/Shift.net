using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using BiomedicaLib.Net;
using ExpensTrackerAPI.App_Code;
using ExpensTrackerAPI.Controllers;
using shiftreportapp.data;
using ShiftreportLib;
using ShiftreportLib.Helpers;
using static ShiftreportLib.internal_restful_dm;

namespace ShiftReportApi.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class VcEmployeeController : ApiController
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

        cashier_mst_helper cH = new cashier_mst_helper();

        AppModel Context = new AppModel();

        [HttpGet]
        public HttpResponseMessage BusinessShiftsInfo([FromUri] int casheir_id)
        {
            try
            {
                var caheir = Context.cashier_mst2.Find(casheir_id);
                var store = Context.store_profile_mst2.Find(caheir.store_id);
                if (store == null)
                {
                    throw new AppException(casheir_id, "there is no store for this cashier");
                }

                int is_mobile_register = 0; // dealing with null values
                if (caheir.is_mobile_register == 1)
                {
                    is_mobile_register = 1;
                }

                return Request.CreateResponse(HttpStatusCode.OK, AppHelpers.VcCreateBuismesinfo(store.id, is_mobile_register));

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



        // ADD ADIL
        [HttpPost]
        public HttpResponseMessage SubmitPaymentRequest(SubmitPaymentRequest_post_req_dm data)
        {
            try
            {

                var transaction = Context.vc_CustomerTransactions2.Find(data.transaction_id);

                if (transaction.Status.ToUpper() == "CANCELLED")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1); // transaction has been cancelled
                }
                // Get CreditCard Info and customer info to send to paypal
                var vcCustomerCreditCard = Context.vc_CustomerCreditCards2.Find(transaction.CardId);

                // check if Gas pump
                var register = Context.vc_StoreRegisters2.Find(transaction.RegisterId);


                if (register.IsGasPump==1)
                {   // gas Pump transaction
                    // get VenueCash balance for that store
                    var venuecash = Context.vc_Customer_Rewards_Balances2.Where(r => r.store_id == transaction.StoreId && r.CustomerId == transaction.CustomerId).FirstOrDefault();
                    var customer = Context.vc_customers2.Find(transaction.CustomerId);

                    
 

                    bool paymentCovered = false;

                    double venuecashStoreBalance = 0;
                    if (venuecash != null)
                    {
                        venuecashStoreBalance = (double)venuecash.VenueCashBalance;
                    }
                 

                        // create paypal credit card
                        PayPal.Api.CreditCard creditCard = new PayPal.Api.CreditCard();
                        creditCard.type = vcCustomerCreditCard.CardType.ToLower();
                        creditCard.number = vcCustomerCreditCard.CardNumber;
                        creditCard.expire_month = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[0]);
                        creditCard.expire_year = Convert.ToInt32(vcCustomerCreditCard.ExpirationDate.Split('/')[1]) + 2000;
                        creditCard.first_name = vcCustomerCreditCard.NameOnCard.Split(' ')[0];
                        creditCard.last_name = vcCustomerCreditCard.NameOnCard.Split(' ')[1];

                        var paymentGateWayResponse = PayPalProcessor.Pay(creditCard, data.GasAmount);

                        if(paymentGateWayResponse.state== "approved") {
                        // Processor confirmation ID
                        transaction.ProcessorConfirmationId = paymentGateWayResponse.id;

                        transaction.Paid_VenueCash_Amount = 0;
                        transaction.Paid_AllCash_Amount = 0;
                        transaction.Paid_Credit_Amount = data.GasAmount;
                        paymentCovered = true;
                            // recalculate VC and AC blance
                       if (venuecash != null) { 
                        // venuecash.VenueCashBalance = 0;
                        //venuecash.VenueCash__Available_balance = 0;
                        }
                            // customer.TotalVenueCash = customer.TotalVenueCash - (double)transaction.Paid_VenueCash_Amount;
                        // customer.TotalAllCash = customer.TotalAllCash - (double)transaction.Paid_AllCash_Amount;
                        // Calculate earned VenueCash.
                        var storeProfile = Context.vc_store_profile_mst2.Where(s => s.store_id == transaction.StoreId).FirstOrDefault();
                        transaction.VenueCashEarnedGas= VCBalanceHelper.nearToZero((double)transaction.Paid_Credit_Amount * (double)storeProfile.gas_back_rate / 100);
                        transaction.VenueCashEarned_Total = transaction.VenueCashEarnedGas;
                        customer.TotalVenueCash = customer.TotalVenueCash + (double)transaction.VenueCashEarned_Total;
                        if (venuecash != null) { 
                       venuecash.VenueCashBalance = venuecash.VenueCashBalance + transaction.VenueCashEarnedGas;
                       venuecash.VenueCash__Available_balance = venuecash.VenueCashBalance;
                        }
                        else
                        {   // create balance for that store
                                vc_Customer_Rewards_Balances vcCustomerRewardsBalanceNew = new vc_Customer_Rewards_Balances();
                                vcCustomerRewardsBalanceNew.create_dt = DateTime.Now;
                                vcCustomerRewardsBalanceNew.modify_dt = DateTime.Now;
                                vcCustomerRewardsBalanceNew.store_id = transaction.StoreId;
                                vcCustomerRewardsBalanceNew.CustomerId = transaction.CustomerId;
                                vcCustomerRewardsBalanceNew.VenueCash_Gas_Awarded = transaction.VenueCashEarnedGas;
                                vcCustomerRewardsBalanceNew.VenueCash_Other_Awarded = transaction.VenueCashEarned_Other;
                                vcCustomerRewardsBalanceNew.VenueCash_Awarded = transaction.VenueCashEarned;
                                vcCustomerRewardsBalanceNew.VenueCashEarned_Total = transaction.VenueCashEarned_Total;
                                vcCustomerRewardsBalanceNew.VenueCashBalance = transaction.VenueCashEarned_Total;
                                vcCustomerRewardsBalanceNew.VenueCash__Available_balance = vcCustomerRewardsBalanceNew.VenueCashBalance;

                            }
                            


                        }
                        else
                        {
                            return Request.CreateResponse(HttpStatusCode.OK,-2);
                        }
                    

                    // Void $50 authorization
                    if (paymentCovered)
                    {
                        if (transaction.AuthorizationID != null)
                        {
                            PayPalProcessor.VoidAuthorize(transaction.AuthorizationID, 50);
                        }

                        transaction.OtherAmount = data.OtherAmount;
                        transaction.GasAmount = data.GasAmount;
                        transaction.Sub_TotalAmount = data.GasAmount;// data.Sub_TotalAmount;
                        transaction.Status = "approved";
                        transaction.cashier_id = data.cashier_id; //
                        transaction.TotalAmount = transaction.Sub_TotalAmount;
                        transaction.shift_id = data.shift_id;
                        transaction.OtherFinancingAmount = data.otherFinancingAmount;

                        // Calculating PayoutAmount for this transaction
                        double CCFees = 0;
                        if (transaction.Paid_Credit_Amount > 0)
                        {
                            CCFees = VCBalanceHelper.getCCFees((double)transaction.Paid_Credit_Amount, vcCustomerCreditCard.CardType);
                        }
                        transaction.PayoutAmount = (double)transaction.TotalAmount - (double)transaction.VenueCashEarned_Total - CCFees;
                        //


                    }

                    // Send email
                    // get customer info
                   //  var customer = Context.vc_customers2.Find(transaction.CustomerId);
                    var store = Context.store_profile_mst2.Find(transaction.StoreId);
                    VenueCashEmails.salesOrderConfirmationCustomer(customer.Email, customer.FirstName, store.store_name, store.store_address1, transaction, vcCustomerCreditCard);
                    // salesOrderConfirmationCustomer(string email, string firstName,string store_name,string store_address, vc_CustomerTransactions transaction, vc_CustomerCreditCards creditCard)


                }
                else
                {   // regular transaction
                    transaction.OtherAmount = data.OtherAmount;
                    transaction.GasAmount = data.GasAmount;
                    transaction.Sub_TotalAmount = data.Sub_TotalAmount;
                    transaction.Status = "PENDING_CUSTOMER_APPROVAL";
                    transaction.cashier_id = data.cashier_id; //
                    transaction.TotalAmount = transaction.Sub_TotalAmount;
                    transaction.shift_id = data.shift_id;
                    transaction.OtherFinancingAmount = data.otherFinancingAmount;
                }





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

        /* Disabled by ADIL
        [HttpPost]
		public HttpResponseMessage SubmitPaymentRequest(SubmitPaymentRequest_post_req_dm data)
		{
			try
			{
				var trans = Context.vc_CustomerTransactions2.Find(data.transaction_id);
				if(trans!=null)
				{
					//tarns.OtherAmount = data.OtherAmount;
					//trans.Amount = data.Amoun;
				}
				else
				{
					vc_CustomerTransactions vc = new vc_CustomerTransactions();
					vc.Amount = data.Amoun;
					vc.cashier_id = data.cashier_id;
					vc.created_userid = "system";
					vc.create_dt = DateTime.Now;
					vc.Modify_usrid = "system";
					vc.modify_dt = DateTime.Now;
					vc.GasAmount = (float)data.GasAmount;
					vc.TransactionType = data.TransactionType;
					vc.VenueCashEarned = 0;// data.VenueCashEarned;
					vc.VenueCashEarnedGas= 0;// = data.VenueCashEarnedGas;
					vc.Status = data.Status;
					vc.CustomerTransactionId = 0;
					vc.CustomerId = "0";
					vc.StoreId = 0;
					vc.Date = DateTime.Now;
					vc.PaidOut = false;
					vc.VenueCashEarned_Other = 0;
					vc.OtherAmount = 0;
					vc.TotalAmount = vc.Paid_VenueCash_Amount = vc.Paid_AllCash_Amount = vc.Paid_Credit_Amount = vc.other_cash_back_rate = vc.gas_back_rate = vc.cash_back_rate = 0;
					Context.vc_CustomerTransactions2.Add(vc);
				}

				
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

    */
        [HttpGet]
        public HttpResponseMessage ApprovedPayments([FromUri] int shift_id)
        {
            try
            {

                var transactions = (from x in Context.vc_CustomerTransactions2
                                    from z in Context.vc_customers2
                                    where x.shift_id == shift_id && x.CustomerId == z.id
                                    where x.Status == "approved"
                                    select new
                                    {
                                        date = x.Date,
                                        Status = x.Status,
                                        TotalAmount = x.TotalAmount,
                                        FirstName = z.FirstName,
                                        LastName = z.LastName,
                                        Customer_Profile_Pic = z.Customer_Profile_Pic
                                    }).ToList();





                return Request.CreateResponse(HttpStatusCode.OK, transactions);
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
        public HttpResponseMessage ProcessRefunds([FromBody] ProcessRefunds_post_req_dm data)
        {
            try
            {

                // Get the transaction by ID
                var transaction = Context.vc_CustomerTransactions2.Find(data.CustomerTransactionId);

                var previousRefunds = Context.vc_refunds2.Where(o => o.customer_transaction_id == data.CustomerTransactionId).ToList();

                double refunded_cashier_sum = 0;
                double refunded_cc_sum = 0;
                double refunded_ac_sum = 0;
                double refunded_vc_sum = 0;
                foreach (var refunded in previousRefunds)
                {
                    refunded_cashier_sum = refunded_cashier_sum + refunded.cashier_refunded_amount;
                    refunded_cc_sum = refunded_cc_sum + refunded.cc_refunded_amount - refunded.VenueCash_Deficient_Charge_Amount;
                    refunded_vc_sum = refunded_vc_sum + refunded.vc_refunded_amount;
                    refunded_ac_sum = refunded_ac_sum + refunded.ac_refunded_amount;
                }

                // Calculating max for every payment type
                double cc_max_Amount = 0;
                double vc_max_Amount = 0;
                double ac_max_Amount = 0;

                cc_max_Amount = VCBalanceHelper.nearToZero((double)transaction.Paid_Credit_Amount - refunded_cc_sum);
                vc_max_Amount = VCBalanceHelper.nearToZero((double)transaction.Paid_VenueCash_Amount - refunded_vc_sum);
                ac_max_Amount = VCBalanceHelper.nearToZero((double)transaction.Paid_AllCash_Amount - refunded_ac_sum);

                //## Removing TIP from MAX_ amout to refund from cc or vc or ac
                if (transaction.Tip_Amount <= cc_max_Amount)
                {
                    cc_max_Amount = VCBalanceHelper.nearToZero(cc_max_Amount - (double)transaction.Tip_Amount);
                }
                else if (transaction.Tip_Amount <= vc_max_Amount)
                {
                    vc_max_Amount = VCBalanceHelper.nearToZero(vc_max_Amount - (double)transaction.Tip_Amount);
                }
                else if (transaction.Tip_Amount <= ac_max_Amount)
                {
                    ac_max_Amount = VCBalanceHelper.nearToZero(ac_max_Amount - (double)transaction.Tip_Amount);
                }




                // Calculate the max amount to refund
                // Max Total amount to refund
                double maxAmountToRefund = VCBalanceHelper.nearToZero((double)transaction.Sub_TotalAmount - (double)refunded_cashier_sum);




                if (transaction == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -2);// no transaction found
                }


                // can't refund the transaction already refunded
                if (transaction.Status.ToUpper() == "REFUNDED" || maxAmountToRefund == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// can't refund the transaction already refunded
                }
                // max amount to refund
                if (data.Amount > maxAmountToRefund)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -3);// The amount is greater than what is left to refund
                }

                // find customer
                var customer = Context.vc_customers2.Find(transaction.CustomerId);

                // ############################################## REFUND LOGIC AND CALCULATIONS ##########################################################################
                //## 1) Calculate cc_refund_amount
                double cc_refund_amount = 0;
                //CASE 1 // replaced: data.Amount <= transaction.Paid_Credit_Amount || data.Amount <= transaction.Sub_TotalAmount 
                if (data.Amount <= maxAmountToRefund)//Note use this condition to set the Status partial or full refund
                {
                    cc_refund_amount = data.Amount;
                }
                //CASE 2
                else
                {
                    cc_refund_amount = maxAmountToRefund; // replaced: (double)transaction.Paid_Credit_Amount
                }

                //## 2) Calculate VenueCash_Deficient_Charge_Amount
                double VenueCash_Deficient_Charge_Amount = 0;
                //get user venueCashBalance from that store
                var venueCashBalance = Context.vc_Customer_Rewards_Balances2.Where(o => o.CustomerId == transaction.CustomerId && o.store_id == transaction.StoreId).FirstOrDefault();

                if (transaction.VenueCashEarned_Total > 0) //if the the customer didnt earned venueCash for this transaction the VenueCash_Deficient_Charge_Amount will be zero 
                {
                    // check for Balance if it exist for that store
                    if (venueCashBalance != null)
                    {
                        VenueCash_Deficient_Charge_Amount = Math.Round((double)venueCashBalance.VenueCashBalance - (double)transaction.VenueCashEarned_Total, 2);

                        if (VenueCash_Deficient_Charge_Amount > 0)//VenueCash_Deficient_Charge_Amount greater than zero
                        {
                            VenueCash_Deficient_Charge_Amount = 0;
                        }

                    }
                    else// no balance found for that store this mean the venueCashblance is equal to zero
                    {
                        VenueCash_Deficient_Charge_Amount = 0 - (double)transaction.VenueCashEarned_Total;
                    }


                }


                //## 3) Calculate Net_Refund_Amount (CC)
                double Net_Refund_Amount = 0;
                // new formula
                Net_Refund_Amount = VCBalanceHelper.nearToZero((cc_max_Amount / maxAmountToRefund) * cc_refund_amount);

                //## 4) Calculate AllCash_Refund_Amount
                double AllCash_Refund_Amount = 0;
                AllCash_Refund_Amount = VCBalanceHelper.nearToZero((ac_max_Amount / maxAmountToRefund) * data.Amount);

                //## 5) Calculate VenueCash_Refund_Amount
                double VenueCash_Refund_Amount = 0;
                VenueCash_Refund_Amount = VCBalanceHelper.nearToZero((vc_max_Amount / maxAmountToRefund) * data.Amount);


                // apply VenueCash_Deficient_Charge_Amount to refunded VC or CC or both
                double VenueCash_Deficient_Charge_Amount_vc_cc = VenueCash_Deficient_Charge_Amount;
                if (VenueCash_Refund_Amount > 0)
                {
                    VenueCash_Refund_Amount = VenueCash_Refund_Amount + VenueCash_Deficient_Charge_Amount_vc_cc;
                    if (VenueCash_Refund_Amount < 0)
                    {
                        VenueCash_Deficient_Charge_Amount_vc_cc = VenueCash_Refund_Amount;
                        VenueCash_Refund_Amount = 0;

                    }
                    else
                    {
                        VenueCash_Deficient_Charge_Amount_vc_cc = 0;

                    }

                }
                if (Net_Refund_Amount > 0)
                {
                    Net_Refund_Amount = Net_Refund_Amount + VenueCash_Deficient_Charge_Amount_vc_cc;
                }






                bool processRefund = false;


                if (Net_Refund_Amount > 0) {  // check if we will refund to CC

                    // Start refuning to CCC
                    double amountToRefund = Math.Round(Net_Refund_Amount, 2);//paypal require two fractions
                                                                             // Paypal Refund API
                    var refund = PayPalProcessor.refund(transaction.sale_id, amountToRefund.ToString("F"));

                    if (refund.state == "completed")
                    {
                        processRefund = true;
                    }

                }

                if (AllCash_Refund_Amount > 0)// if only VC and AC refunded No CC 
                {
                    processRefund = true;
                }


                if (VenueCash_Refund_Amount > 0)
                {
                    processRefund = true;
                }



                if (processRefund) // do this if at least one of the 3 payment source was refunded
                {

                    //## Balance Operations 
                    //# remove earned VC from balance
                    double earnedReturnedVC = (double)transaction.VenueCashEarned_Total;
                    venueCashBalance.VenueCashBalance = VCBalanceHelper.nearToZero((double)venueCashBalance.VenueCashBalance - (double)transaction.VenueCashEarned_Total);
                    // remove earned VC from customer global balance
                    customer.TotalVenueCash = VCBalanceHelper.nearToZero(customer.TotalVenueCash - (double)transaction.VenueCashEarned_Total);
                    //# Recalculate new Earned venueCash
                    transaction.VenueCashEarned_Total = VCBalanceHelper.nearToZero(((cc_max_Amount - cc_refund_amount) * (double)transaction.other_cash_back_rate) / 100);
                    transaction.VenueCashEarned_Other = transaction.VenueCashEarned_Total;
                    // Add Recalculated new Earned VenueCash to store and global balance
                    venueCashBalance.VenueCashBalance = VCBalanceHelper.nearToZero((double)venueCashBalance.VenueCashBalance + (double)transaction.VenueCashEarned_Total);
                    customer.TotalVenueCash = VCBalanceHelper.nearToZero(customer.TotalVenueCash + (double)transaction.VenueCashEarned_Total);
                    earnedReturnedVC = VCBalanceHelper.nearToZero(earnedReturnedVC - (double)transaction.VenueCashEarned_Total);
                    //########
                    //# Add refunded VenueCash and allCash to Balance
                    // add vc_refunded to balance
                    venueCashBalance.VenueCashBalance = venueCashBalance.VenueCashBalance + VenueCash_Refund_Amount;
                    venueCashBalance.VenueCash__Available_balance = venueCashBalance.VenueCashBalance;
                    // add vc_refunded to global balance
                    customer.TotalVenueCash = customer.TotalVenueCash + VenueCash_Refund_Amount;
                    // add AC refunded to global Balance
                    customer.TotalAllCash = customer.TotalAllCash + AllCash_Refund_Amount;
                    //## End Balance Operations


                    double totalRefunded = Net_Refund_Amount + VenueCash_Refund_Amount + AllCash_Refund_Amount;

                    transaction.Gross_Refund_Amount = Convert.ToDouble(totalRefunded);
                    transaction.modify_dt = DateTime.Now;
                    transaction.Modify_usrid = "system";
                    // set refund Status
                    transaction.Status = "PARTIAL REFUND";
                    if (totalRefunded == (maxAmountToRefund + VenueCash_Deficient_Charge_Amount))
                    {
                        transaction.Status = "REFUNDED";
                    }



                    // Adding A Refund record to vc_refunds table
                    vc_refunds vc_refund = new vc_refunds();
                    vc_refund.is_mobile_register = (int)Context.vc_StoreRegisters2.Find(transaction.RegisterId).IsMobileRegister;
                    vc_refund.customer_transaction_id = transaction.Id;
                    vc_refund.status = transaction.Status;
                    vc_refund.cashier_id = (int)transaction.cashier_id;
                    vc_refund.date_of_refund = DateTime.Now;
                    vc_refund.cc_refunded_amount = Net_Refund_Amount;
                    vc_refund.vc_refunded_amount = VenueCash_Refund_Amount;
                    vc_refund.ac_refunded_amount = AllCash_Refund_Amount;
                    vc_refund.vc_earned_returned_amount = earnedReturnedVC;
                    vc_refund.VenueCash_Deficient_Charge_Amount = VenueCash_Deficient_Charge_Amount;
                    vc_refund.cashier_refunded_amount = data.Amount;
                    vc_refund.shift_id = data.shift_id;
                    vc_refund.store_id = transaction.StoreId;

                    // Calculate PayoutAmount
                    /* Old formula
                    double CC_Net_Refund = VCBalanceHelper.nearToZero(vc_refund.cc_refunded_amount + (vc_refund.cc_refunded_amount * 0.00299));
                    vc_refund.PayoutAmount = (CC_Net_Refund + vc_refund.vc_refunded_amount + vc_refund.ac_refunded_amount) * -1;
                    */
                    /**/
                    double CC_Net_Refund = 0;
                    var creditCard = Context.vc_CustomerCreditCards2.Find(transaction.CardId);
                    if (creditCard != null)
                    {
                        if (creditCard.CardType.ToLower() == "amex") // for american express credit card
                        {
                            if (transaction.Status == "REFUNDED")
                            {
                                CC_Net_Refund = vc_refund.cc_refunded_amount - ((vc_refund.cc_refunded_amount * 0.04)+0.3);
                            }
                            else
                            {
                                CC_Net_Refund = vc_refund.cc_refunded_amount - (vc_refund.cc_refunded_amount * 0.04);
                            }
                        }
                        else // for other cards
                        { 
                            if (transaction.Status== "REFUNDED")
                            {
                                CC_Net_Refund = vc_refund.cc_refunded_amount - ((vc_refund.cc_refunded_amount * 0.035)+ 0.3);
                            }
                            else
                            {
                                CC_Net_Refund = vc_refund.cc_refunded_amount - (vc_refund.cc_refunded_amount * 0.035);
                            }
                                
                        }
                    }

                    vc_refund.PayoutAmount = ((CC_Net_Refund + vc_refund.vc_refunded_amount + vc_refund.ac_refunded_amount) - vc_refund.vc_earned_returned_amount) * -1;





                    vc_refund.PaidOut = false;

                    //need to implement use case for venuecash and allcash and if it's a full refund
                    /*
                     * yes but only the credit card paid will be debited the cc fees
                     * if its a full refund then you - .30
                     * if its not a full CC refund then its just - .0299%
                    */

                    Context.vc_refunds2.Add(vc_refund);


                    // update transaction modified date
                    transaction.modify_dt = DateTime.Now;

                    Context.SaveChanges();


                    // Send VenueCash email
                    // get store info
                    var store = Context.store_profile_mst2.Find(transaction.StoreId);
                    var refunds = Context.vc_refunds2.Where(r => r.customer_transaction_id == transaction.Id).ToList();

                    VenueCashEmails.refundOrderConfirmationCustomer(customer.Email, customer.FirstName, store.store_name, store.store_address1, transaction, creditCard, refunds);



                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = "refunded", Gross_Refund_Amount = totalRefunded });

                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Status = "denied", Gross_Refund_Amount = 0 });
                }



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
        public HttpResponseMessage PendingGasSales([FromUri] int cashier_id)
        {
            try
            {
                List<PendingGasSales_get_resp_dm> data = new List<PendingGasSales_get_resp_dm>();


                var d1 = Context.vc_CustomerTransactions2.Where(r => r.cashier_id == cashier_id && r.TransactionType == 2);
                foreach (var x in d1)
                {
                    PendingGasSales_get_resp_dm d = new PendingGasSales_get_resp_dm();

                    d.GasAmount = Convert.ToInt32(x.GasAmount);
                    d.TransactionType = Convert.ToInt32(x.TransactionType).ToString();

                    string cid = Convert.ToInt32(x.CustomerId).ToString();
                    using (AppModel Context2 = new AppModel())
                    {
                        var u = Context2.vc_customers2.Where(r => r.id.ToString() == cid).FirstOrDefault();
                        if (u != null)
                        {
                            d.FirstName = u.FirstName;
                            d.LastName = u.LastName;
                        }

                        Context2.Dispose();
                    }
                    data.Add(d);
                }



                return Request.CreateResponse(HttpStatusCode.OK, data);
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
        public HttpResponseMessage CompleteGasSale(CompleteGasSale_post_req_dm data)
        {
            try
            {
                var x = Context.vc_CustomerTransactions2.Where(r => r.Id == data.CustomerTransactionId).FirstOrDefault();
                if (x == null)
                    throw new AppException(data.CustomerTransactionId, "No Rrecord Found in table");
                x.GasAmount = data.GasAmount;
                x.Status = "";
                x.Modify_usrid = "system";
                x.modify_dt = DateTime.Now;
                Context.Entry(x).State = System.Data.Entity.EntityState.Modified;
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
        public HttpResponseMessage PendingTransactions([FromUri] int shift_id)
        {
            try
            {

                var pendingTransactions = (from t in Context.vc_CustomerTransactions2
                                           from c in Context.vc_customers2
                                           where t.shift_id == shift_id &&
                                                 t.Status != "Pending_Cashier_Request" &&
                                                 t.Status != "Customer_Submitted" &&
                                                 t.CustomerId == c.id
                                           select new
                                           {
                                               CustomerTransactionId = t.Id,
                                               Customer_id = c.id,
                                               FirstName = c.FirstName,
                                               LastName = c.LastName,
                                               Customer_Profile_Pic = c.Customer_Profile_Pic,
                                               RegisterId = t.RegisterId,
                                               Cashier_ID_on_duty = t.cashier_id,
                                               Status = t.Status,
                                               Sub_TotalAmount = t.Sub_TotalAmount,
                                               Tip_Amount = t.Tip_Amount,
                                               Date = t.Date
                                           }
                             ).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, pendingTransactions);
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


        /* Disabled by ADIL
		[HttpGet]
		public HttpResponseMessage PendingTransactions([FromUri] int cashier_id)
		{
			try
			{
				var l = (from x in Context.vc_CustomerTransactions2
						where x.Status.Equals("Pending_Cashier_Request")
						//select new { }).ToList();
						select x).ToList();
				return Request.CreateResponse(HttpStatusCode.OK,l);
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
        */
        [HttpGet]
        public HttpResponseMessage Check_Payment_Approval([FromUri] int shift_id)
        {
            try
            {
                var transactions = (from x in Context.vc_CustomerTransactions2
                                    from z in Context.vc_customers2
                                    where x.shift_id == shift_id && x.CustomerId == z.id
                                    where x.Status == "approved" || x.Status == "denied"
                                    select new
                                    {
                                        Status = x.Status,
                                        TotalAmount = x.TotalAmount,
                                        FirstName = z.FirstName,
                                        LastName = z.LastName,
                                        Customer_Profile_Pic = z.Customer_Profile_Pic
                                    }).ToList();


                return Request.CreateResponse(HttpStatusCode.OK, transactions);
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
        public HttpResponseMessage QRCode_CompletedPayment([FromUri] int CustomerTransactionId)
        {
            try
            {

                // Check if there is previous partial refunds
                double? previousPartialRefundSum = Context.vc_refunds2.Where(o => o.customer_transaction_id == CustomerTransactionId).Sum(o => (double?)o.cashier_refunded_amount) ?? 0;


                var completedPayment = (from a in Context.vc_CustomerTransactions2
                                            //from b in Context.vc_Customer_Rewards_Balances2 // No need to return the venueCash for now
                                        from c in Context.vc_customers2
                                        where a.CustomerId == c.id
                                             //   && a.CustomerId == b.CustomerId 
                                             //   && a.StoreId==b.store_id
                                             && a.Id == CustomerTransactionId
                                        select new
                                        {
                                            Date = a.Date,
                                            GasAmount = a.GasAmount,
                                            OtherAmount = a.OtherAmount,
                                            Sub_TotalAmount = a.Sub_TotalAmount,
                                            Status = a.Status,
                                            Paid_Credit_Amount = a.Paid_Credit_Amount,
                                            VenueCashEarned_Total = a.VenueCashEarned_Total,
                                            FirstName = c.FirstName,
                                            LastName = c.LastName,
                                            Customer_Profile_Pic = c.Customer_Profile_Pic,
                                            Tip_Amount = a.Tip_Amount,
                                            Amout_To_Refund = a.Sub_TotalAmount - previousPartialRefundSum,// minus the refunds transaction that already have been processed
                                            // VenueCash_Available_balance=b.VenueCashBalance,
                                            transactionID = a.Id
                                        }).FirstOrDefault();



                // Check if transaction exist
                if (completedPayment == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -1);// No transaction found
                }
                else if (completedPayment.Status.ToUpper() != "APPROVED" && completedPayment.Status.ToUpper() != "PARTIAL REFUND")
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -2);// Only approved and partial refund transaction can be refunded
                }
                else if (completedPayment.Status.ToUpper() == "REFUNDED" || completedPayment.Amout_To_Refund == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, -3);// This Transaction has been already Fully refunded
                }

                return Request.CreateResponse(HttpStatusCode.OK, completedPayment);
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


        /* Disabled by ADIL
		[HttpGet]
		public HttpResponseMessage QRCode_CompletedPayment([FromUri] int CustomerTransactionId)
		{
			try
			{
				var t = (from x in Context.vc_CustomerTransactions2
						 from y in Context.vc_customers2
						 from z in Context.vc_Customer_Rewards_Balances2

						 where x.CustomerTransactionId == CustomerTransactionId && 
								y.id.ToString() == x.CustomerId && 
								z.CustomerId == x.Id
						 select new
						 {
							 Date = x.Date,
							 GasAmount = x.GasAmount,
							 OtherAmount =x.OtherAmount,
							 Status =x.Status,
							 Paid_Credit_Amount =x.Paid_Credit_Amount,
							 VenueCashEarned_Total = x.VenueCashEarned_Total,
							 LastName = y.LastName,
							 FirstName = y.FirstName,
							 Customer_Profile_Pic = y.Customer_Profile_Pic,
							 VenueCash__Available_balance=z.VenueCash__Available_balance
						 }).ToArray();

				

				return Request.CreateResponse(HttpStatusCode.OK, t);
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
        */

        //[HttpPost]
        //public HttpResponseMessage Clear_Gas_Pending_Transaction(Clear_Gas_Pending_Transaction_req_post  data)
        //{
        //	try
        //	{
        //		var t = Context.vc_CustomerTransactions2.Find(data.CustomerTransactionId);

        //		if (t == null)
        //			throw new AppException(data.CustomerTransactionId, "Record not found");
        //		t.Status = data.Status;

        //		Context.Entry(t).State = System.Data.Entity.EntityState.Modified;
        //		Context.SaveChanges();
        //		return Request.CreateResponse(HttpStatusCode.OK, new { success = "1" });
        //	}
        //	catch (AppException ex)
        //	{
        //		MailLogService.SendMail("ShiftreortError@shiftReports.com", "support@shiftreports.com", "App Error", ex);
        //		return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Msg);
        //	}
        //	catch (Exception ex)
        //	{
        //		return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
        //	}
        //}



        [HttpGet]
        public HttpResponseMessage Employees_Tip_Payouts([FromUri] int cashier_id)
        {
            try
            {

                var tips = (from s in Context.shift_details_mst2
                            from c in Context.cashier_mst2
                            where s.cashier_id == c.id
                            where s.cashier_id == cashier_id
                            where s.shift_status != "TIPS_IN_PROGRESS" && Context.vc_CustomerTransactions2.Where(o => o.shift_id == s.id).Sum(o => o.Tip_Amount) > 0


                            select new
                            {
                                EmployeeName = c.cashier_name,
                                ShiftDate = s.shift_date,
                                Shift_id = s.id,
                                Tip_Payout_Amount = Context.vc_CustomerTransactions2.Where(o => o.shift_id == s.id).Sum(o => o.Tip_Amount),
                                Tip_Payout_Status = s.Tip_Payout_Status,
                                shift_num = s.shift_num
                            }
                            ).ToList();

                /*
                foreach(var tip in tips.ToList())
                {

                    if(  tip.Tip_Payout_Amount==0)
                    {
                         tips.Remove(tip);
                    }

                }*/



                // remote the tip_payout_amount if it's zero if !=shift_in_progress status

                return Request.CreateResponse(HttpStatusCode.OK, tips);
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
        public HttpResponseMessage CustomerSubmittedPaymentRequets([FromUri] int shift_id,[FromUri] int store_id,[FromUri] int cachier_id)
        {
            try
            {

                // pending transaction by store
                var transactions = (from t in Context.vc_CustomerTransactions2
                                    from c in Context.vc_customers2
                                    from r in Context.vc_StoreRegisters2
                                    where t.StoreId == store_id &&
                                          t.CustomerId == c.id
                                    where t.RegisterId==r.Id
                                    where t.Status == "Pending_Cashier_Request" 
                                    select new
                                    {
                                        customerTransactionID = t.Id,
                                        FirstName = c.FirstName,
                                        LastName = c.LastName,
                                        Customer_Profile_Pic = c.Customer_Profile_Pic,
                                        RegisterId = t.RegisterId,
                                        RegisterNumber = Context.vc_StoreRegisters2.Where(o => o.Id == t.RegisterId).FirstOrDefault().RegisterNumber,
                                        Date = t.Date,
                                        Sub_TotalAmount = t.Sub_TotalAmount,
                                        Tip_Amount = t.Tip_Amount,
                                        Status = t.Status,
                                        Gross_Refund_Amount = t.Gross_Refund_Amount,
                                        vc_refunds = Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Select(o => new { o.cashier_refunded_amount, o.cc_refunded_amount, o.vc_refunded_amount, o.ac_refunded_amount }).ToList(),
                                        modify_dt = t.modify_dt,
                                        OtherFinancingAmount = t.OtherFinancingAmount,
                                        IsGasPump=r.IsGasPump,
                                        order_number=t.order_number
                                    }
                                     ).ToList();

                // Transactions by employee
                var transactionsByEmployee = (from t in Context.vc_CustomerTransactions2
                                    from c in Context.vc_customers2
                                    from r in Context.vc_StoreRegisters2
                                    where t.cashier_id == cachier_id &&
                                          t.CustomerId == c.id
                                    where t.RegisterId == r.Id
                                    where t.Status == "DENIED" || t.Status == "approved" || t.Status == "PENDING_CUSTOMER_APPROVAL" || t.Status == "CANCELLED" || t.Status == "CUSTOMER_SUBMITTED"
                                              select new
                                    {
                                        customerTransactionID = t.Id,
                                        FirstName = c.FirstName,
                                        LastName = c.LastName,
                                        Customer_Profile_Pic = c.Customer_Profile_Pic,
                                        RegisterId = t.RegisterId,
                                        RegisterNumber = Context.vc_StoreRegisters2.Where(o => o.Id == t.RegisterId).FirstOrDefault().RegisterNumber,
                                        Date = t.Date,
                                        Sub_TotalAmount = t.Sub_TotalAmount,
                                        Tip_Amount = t.Tip_Amount,
                                        Status = t.Status,
                                        Gross_Refund_Amount = t.Gross_Refund_Amount,
                                        vc_refunds = Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Select(o => new { o.cashier_refunded_amount, o.cc_refunded_amount, o.vc_refunded_amount, o.ac_refunded_amount }).ToList(),
                                        modify_dt = t.modify_dt,
                                        OtherFinancingAmount = t.OtherFinancingAmount,
                                        IsGasPump = r.IsGasPump,
                                        order_number = t.order_number
                                    }
                                     ).ToList();

                transactions = transactions.Concat(transactionsByEmployee).ToList();

                // get Refund transaction of other Shifts
                var refundTransactions = (
                                    from r in Context.vc_refunds2
                                    from t in Context.vc_CustomerTransactions2
                                    from c in Context.vc_customers2
                                    from reg in Context.vc_StoreRegisters2
                                    where r.shift_id == shift_id && t.shift_id != shift_id
                                    where r.customer_transaction_id == t.Id
                                    where c.id == t.CustomerId
                                    where t.RegisterId == reg.Id
                                    select new
                                    {
                                        customerTransactionID = t.Id,
                                        FirstName = c.FirstName,
                                        LastName = c.LastName,
                                        Customer_Profile_Pic = c.Customer_Profile_Pic,
                                        RegisterId = t.RegisterId,
                                        RegisterNumber = Context.vc_StoreRegisters2.Where(o => o.Id == t.RegisterId).FirstOrDefault().RegisterNumber,
                                        Date = t.Date,
                                        Sub_TotalAmount = t.Sub_TotalAmount,
                                        Tip_Amount = t.Tip_Amount,
                                        Status = t.Status,
                                        Gross_Refund_Amount = t.Gross_Refund_Amount,
                                        vc_refunds = Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Select(o => new { o.cashier_refunded_amount, o.cc_refunded_amount, o.vc_refunded_amount, o.ac_refunded_amount }).ToList(),
                                        modify_dt = t.modify_dt,
                                        OtherFinancingAmount = t.OtherFinancingAmount,
                                        IsGasPump=reg.IsGasPump,
                                        order_number = t.order_number

                                    }
                                     ).ToList();


                // check if this store is a restaurant , business type 2
                var store = Context.vc_store_profile_mst2.Where(s => s.store_id ==store_id).FirstOrDefault();
                if (store.business_type == 2)
                {
                    var pendingTransactions = (from t in Context.vc_CustomerTransactions2
                                        from c in Context.vc_customers2
                                        from r in Context.vc_StoreRegisters2
                                        where t.StoreId == store_id &&
                                              t.CustomerId == c.id
                                        where t.RegisterId == r.Id
                                        where t.Status == "Pending_Cashier_Request"
                                        select new
                                        {
                                            customerTransactionID = t.Id,
                                            FirstName = c.FirstName,
                                            LastName = c.LastName,
                                            Customer_Profile_Pic = c.Customer_Profile_Pic,
                                            RegisterId = t.RegisterId,
                                            RegisterNumber = Context.vc_StoreRegisters2.Where(o => o.Id == t.RegisterId).FirstOrDefault().RegisterNumber,
                                            Date = t.Date,
                                            Sub_TotalAmount = t.Sub_TotalAmount,
                                            Tip_Amount = t.Tip_Amount,
                                            Status = t.Status,
                                            Gross_Refund_Amount = t.Gross_Refund_Amount,
                                            vc_refunds = Context.vc_refunds2.Where(o => o.customer_transaction_id == t.Id).Select(o => new { o.cashier_refunded_amount, o.cc_refunded_amount, o.vc_refunded_amount, o.ac_refunded_amount }).ToList(),
                                            modify_dt = t.modify_dt,
                                            OtherFinancingAmount = t.OtherFinancingAmount,
                                            IsGasPump = r.IsGasPump,
                                            order_number = t.order_number
                                        }
                     ).ToList();

                    // remove pending from the first transactions
                    transactions = transactions.Where(o => o.Status != "Pending_Cashier_Request").ToList();

                    transactions = transactions.Concat(pendingTransactions).ToList();

        
                }



                return Request.CreateResponse(HttpStatusCode.OK, transactions.Concat(refundTransactions));
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
        public HttpResponseMessage cashierTipsByShift([FromUri] int shift_id)
        {
            try
            {


                var tips = (from t in Context.vc_CustomerTransactions2
                            from c in Context.vc_customers2
                            where t.shift_id == shift_id &&
                                  t.CustomerId == c.id &&
                                  t.Tip_Amount > 0
                            select new
                            {
                                customerTransactionID = t.Id,
                                FirstName = c.FirstName,
                                LastName = c.LastName,
                                Register_id = t.RegisterId,
                                Tip_Amount = t.Tip_Amount,
                                Date = t.Date
                            }
                                     ).ToList();



                return Request.CreateResponse(HttpStatusCode.OK, tips);
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
        public HttpResponseMessage testSendEmail([FromUri] string email)
        {

            try
            {
                MailLogService.SendMail("ShiftreortError@shiftReports.com", "adil@shiftreports.com", "Test Amazon SES email server", "Test Amazon SES email server");

                return Request.CreateResponse(HttpStatusCode.OK, "Email sent");
            }
            catch (AppException ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Email not sent cause: " + ex.Message);
            }




        }

        [HttpPost]
        public HttpResponseMessage VC_shift_submit([FromBody] VC_shift_submitBindingInput inputData)
        {

            var shiftDetailsMSt = Context.shift_details_mst2.Find(inputData.shift_id);

            // Calculate Tips : sum all tips earned by the employee and save them to shift_details_mst table
            //already callculated    double EarnedTipAmount = Context.vc_CustomerTransactions2.Where(o => o.shift_id == inputData.shift_id && o.Status == "APPROVED" && o.Status == "REFUNDED" && o.Status == "PARTIAL REFUND").Sum(o => o.Tip_Amount) ?? 0;


            shiftDetailsMSt.Tip_Payout_Status = "PAYOUT_PENDING";//Payout_Pending
            shiftDetailsMSt.shift_session_ended = inputData.shift_session_ended;
            shiftDetailsMSt.shift_status = "D";
            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, 0); // code Zero OK
        }


        [HttpGet]
        public HttpResponseMessage getEmployeeProfile([FromUri] int cashier_id)
        {

            var employeeProfile = (from c in Context.cashier_mst2
                                   from s in Context.store_profile_mst2
                                   where s.id == c.store_id
                                   where c.id == cashier_id
                                   select new
                                   {
                                       store_name = s.store_name,
                                       store_id = s.id,
                                       store_address1 = s.store_address1,
                                       store_city = s.store_city,
                                       store_state = s.store_state,
                                       store_zip = s.store_zip,

                                       employee_id = c.id,
                                       employee_name = c.cashier_name,
                                       employee_title = c.title,
                                       employee_pw = c.cashier_pw,
                                       employee_email = c.cashier_email,
                                       employee_phone = c.cashier_cell_phone,
                                       is_mobile_register = c.is_mobile_register == 1 ? true : false
                                   }
                     ).FirstOrDefault();


            return Request.CreateResponse(HttpStatusCode.OK, employeeProfile); // code Zero OK
        }

        [HttpPost]
        public HttpResponseMessage putEmployeeProfile([FromBody] putEmployeeProfileBindingInput inputData)
        {

            var employeeProfile = Context.cashier_mst2.Find(inputData.employee_id);

            employeeProfile.cashier_name = inputData.employee_name;
            employeeProfile.title = inputData.employee_title;
            employeeProfile.cashier_pw = inputData.employee_pw;
            employeeProfile.cashier_email = inputData.employee_email;
            employeeProfile.cashier_cell_phone = inputData.employee_phone;

            if (inputData.is_mobile_register)
            {
                employeeProfile.is_mobile_register = 1;

                var storeRegister = Context.vc_StoreRegisters2.Where(o => o.Cashier_ID_on_duty == employeeProfile.id && o.IsMobileRegister == 1 && o.store_id == employeeProfile.store_id).FirstOrDefault();
                if (storeRegister == null)
                {
                    vc_StoreRegisters newMobileRegister = new vc_StoreRegisters();
                    newMobileRegister.create_dt = DateTime.Now;
                    newMobileRegister.created_userid = DateTime.Now.ToString();
                    newMobileRegister.modify_dt = DateTime.Now;
                    newMobileRegister.Modify_usrid = "System";

                    newMobileRegister.RegisterId = 0;// this paramter is not used
                    newMobileRegister.store_id = employeeProfile.store_id;
                    newMobileRegister.RegisterNumber = "Mobile Register";
                    newMobileRegister.IsMobileRegister = 1;
                    newMobileRegister.Cashier_ID_on_duty = employeeProfile.id;
                    newMobileRegister.Cashier_IDs_on_duty_Gas_Pump = -1;
                    newMobileRegister.Status = "";
                    newMobileRegister.shift_id = -1;

                    Context.vc_StoreRegisters2.Add(newMobileRegister);
                }

            }
            else
            {
                employeeProfile.is_mobile_register = 0;

            }



            Context.SaveChanges();

            // getStore info
            var store = Context.store_profile_mst2.Find(employeeProfile.store_id);

            // send email // newCashierEmployee(string email, string username, string companyName, string userID, string password, string storeName)

            VenueCashEmails.newCashierEmployee(employeeProfile.cashier_email, employeeProfile.cashier_name, "", employeeProfile.id.ToString(), employeeProfile.cashier_pw, store.store_name);


            return Request.CreateResponse(HttpStatusCode.OK, 0); // code Zero OK
        }

        [HttpPost]
        public HttpResponseMessage changeStore([FromBody] changeStoreProfileBindingInput inputData)
        {

            var storeProfileMst = Context.store_profile_mst2.Find(inputData.store_id);
            if (storeProfileMst == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1); // code -1 Store Not found
            }

            var cashierMst = Context.cashier_mst2.Find(inputData.cashier_id);

            cashierMst.store_id = inputData.store_id;

            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, new {
                store_name = storeProfileMst.store_name,
                store_id = storeProfileMst.id,
                store_address1 = storeProfileMst.store_address1,
                store_city = storeProfileMst.store_city,
                store_state = storeProfileMst.store_state,
                store_zip = storeProfileMst.store_zip
            });
        }

        public HttpResponseMessage getStoreInformation([FromUri] int store_id)
        {
            var storeProfile = Context.store_profile_mst2.Find(store_id);

            if (storeProfile != null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new {
                    store_id = storeProfile.id,
                    store_name = storeProfile.store_name,
                    store_city = storeProfile.store_city,
                    store_address1 = storeProfile.store_address1,
                    store_state = storeProfile.store_state,
                    store_zip = storeProfile.store_zip,
                }); // code -1 Store Not found
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1); // code -1 Store Not found
            }

        }

        public HttpResponseMessage getLastTransactionID([FromUri] int shift_id)
        {

            var transaction = (from t in Context.vc_CustomerTransactions2
                               from c in Context.vc_customers2
                               where t.CustomerId == c.id
                               where t.shift_id == shift_id && t.Status == "PENDING_CASHIER_REQUEST"
                               select new {
                                   customerTransactionID = t.Id,
                                   FirstName = c.FirstName,
                                   LastName = c.LastName
                               }
                              ).FirstOrDefault();



            if (transaction == null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);// no transaction found for that shift
            }


            return Request.CreateResponse(HttpStatusCode.OK, transaction);

        }


        [HttpGet]
        public HttpResponseMessage forgotPassword([FromUri] string email)
        {

            var employee = Context.cashier_mst2.Where(c => c.cashier_email == email).FirstOrDefault();

            if (employee != null)
            { 

                VenueCashEmails.forgotPassword(email, employee.cashier_name, employee.cashier_pw);

            return Request.CreateResponse(HttpStatusCode.OK, 0);
             }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, -1);
            }
 


        }


        //Pump API

        [HttpGet]
        public HttpResponseMessage getPumps([FromUri] int store_id)
        {
            var pumps = Context.vc_StoreRegisters2.Where(s => s.store_id == store_id && s.IsGasPump==1).ToList()
                .Select(s=> new {id=s.Id, Status = "idle", transactionID = -1, RegisterNumber=s.RegisterNumber });// zero for idle



            return Request.CreateResponse(HttpStatusCode.OK, pumps);

        }

        [HttpGet]
        public HttpResponseMessage gasPumpsBackgroundWorker([FromUri] int store_id)
        {
            var usedPumps = (from t in Context.vc_CustomerTransactions2
                                from s in Context.vc_StoreRegisters2
                                where t.StoreId== store_id
                                where t.RegisterId== s.Id
                                where s.IsGasPump==1 // only gas pumps
                                where t.Status== "Pending_Cashier_Request" || t.Status == "READY_TO_PUMP"
                             select new
                                    {
                                        transactionID =t.Id,
                                        id=s.Id,
                                        Status=t.Status
                                    }
                                 ).ToList();

            return Request.CreateResponse(HttpStatusCode.OK, usedPumps);
            
        }

        [HttpGet]
        public HttpResponseMessage clearPump([FromUri] int transactionID)
        {

            var transaction = Context.vc_CustomerTransactions2.Find(transactionID);

            transaction.Status = "READY_TO_PUMP";

            Context.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, 0);

        }








    }
}