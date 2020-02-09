using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SRYelpAPI;
using static shiftreportapp.data.AppModel;

namespace ShiftreportLib
{

	public class vc_CustomerRewardsBalance
	{
		[Key]
		public int Id { set; get; }

		public Nullable<DateTime> create_dt { set; get; }

		public string created_userid { set; get; }
		public Nullable<DateTime> modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public int customerId { set; get; }

		public int store_id { set; get; }

	}

	public class vc_customers
	{



		[Key]
		public int id { set; get; }

		public string UserName { set; get; }

		public bool TwoFactorEnabled { set; get; }

		public string SecurityStamp { set; get; }

		public bool PhoneNumberConfirmed { set; get; }

		public string PhoneNumber { set; get; }

		public string PasswordHash { set; get; }

		public Nullable<DateTime> LockoutEndDateUtc { set; get; }

		public bool LockoutEnabled { set; get; }

		public bool EmailConfirmed { set; get; }

		public string Email { set; get; }

		

		public string FirstName { set; get; }

		public string LastName { set; get; }

		public string Pin { set; get; }

		

		public int AccessFailedCount { set; get; }

		public string Discriminator { set; get; }
		

		public double TotalVenueCash { set; get; }

		public double TotalAllCash { set; get; }
		public Nullable<DateTime> create_dt { set; get; }

		public string created_userid { set; get; }
		public Nullable<DateTime> modify_dt { set; get; }

		public String Modify_usrid { set; get; }
		public int Account_Deactivated { get; set; }

		public String Customer_Profile_Pic { set; get; }

	}

	public class vc_vc_VenueCash_Conversions
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
	}

	public class vc_store_profile_mst
	{
		[Key]
		public int Id { set; get; }

		public int store_id { set; get; }
		public Nullable<bool> is_gas_station { set; get; }
		public Nullable<int> business_type { set; get; }
		public Nullable<Double> gas_back_rate { set; get; }
		public Nullable<Double> cash_back_rate { set; get; }
		public Nullable<Double> latitude { set; get; }
		public Nullable<Double> longitude { set; get; }
		public double Rating { set; get; }
		public string website { set; get; }
		public Nullable<int> SubCategorieId { set; get; }
		public Nullable<int> CategorieId { set; get; }

		public string yelp_store_id { set; get; }

		public String Category_Name { set; get; }

		public Nullable<bool> is_closed { set; get; }

		public Nullable<int> review_count { set; get; }
		public string price { set; get; }
		public Nullable<DateTime> create_dt { set; get; }
		public string created_userid { set; get; }
		public Nullable<DateTime> modify_dt { set; get; }
		public String Modify_usrid { set; get; }
        
        public String image_url { set; get; }		
        public bool is_mobile_register { get; set; }
        public bool is_tips { get; set; }


    }
	public class vc_StoreAvailableVC
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public Nullable<int> StoreAvailableVCId { set; get; }

		public String CustomerId { set; get; }

		public Nullable<int> store_id{ set; get; }

		public Nullable<double> VenueCash { set; get; }
	}
    /* Replaced by New model in Model directory
	public class vc_PayoutTransactions
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public Nullable<int> PayoutTransactionId { set; get; }


		public Nullable<int> storeId { set; get; }

		public DateTime Date { set; get; }

		public double Amount { set; get; }

	
		public Nullable<int> PayoutMethodId { set; get; }

	}
    */
	public class vc_StoreGasPump
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public Nullable<int> GasPumpId { set; get; }

		public Nullable<int> store_id { set; get; }

		public string PumpName { set; get; }

		public Nullable<int> PumpNumber { set; get; }

	}
	public class vc_CustomerCreditCards
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

	

		public int CustomerId { set; get; }

		public String NameOnCard{ set; get; }

		public string BillingAddress1 { set; get; }

		public string BillingAddress2 { set; get; }

		public string City { set; get; }

		public string State { set; get; }


		public string CardType { set; get; }


		public string CardNumber { set; get; }

		public string SecurityCode { set; get; }

		public string ExpirationDate { set; get; }

		public bool IsDefault { set; get; }






	}
	public class vc_CustomerTransactions
	{
		[Key]
		public int Id { set; get; }

		public Nullable<DateTime> create_dt { set; get; }

		public string created_userid { set; get; }
		public Nullable<DateTime> modify_dt { set; get; }

		public String Modify_usrid { set; get; }


 

		public int CustomerId { set; get; }

		public Nullable<int> StoreId { set; get; }

		public Nullable<double> Amount { set; get; }

		public Nullable<int> TransactionType { set; get; }

		public Nullable<DateTime> Date { set; get; }

		public Nullable<Double> GasAmount { set; get; }

		public Nullable<Double> VenueCashEarned { set; get; }

		public Nullable<Double> VenueCashEarnedGas { set; get; }

		public Nullable<bool> PaidOut { set; get; }

		public string ProcessorConfirmationId { set; get; }

		public Nullable<int> CardId { set; get; }

		public string Status { set; get; }

		public Nullable<int> RegisterId { set; get; }
		public Nullable<int> cashier_id { set; get; }
		public Nullable<Double> VenueCashEarned_Other { set; get; }
		public Nullable<Double> OtherAmount { set; get; }
		public Nullable<Double> TotalAmount { set; get; }
		public Nullable<Double> Paid_VenueCash_Amount { set; get; }
		public Nullable<Double> Paid_AllCash_Amount { set; get; }
		public Nullable<Double> Paid_Credit_Amount { set; get; }
		public Nullable<Double> other_cash_back_rate { set; get; }
		public Nullable<Double> gas_back_rate { set; get; }
		public Nullable<Double> cash_back_rate { set; get; }

		public Nullable<Double> VenueCashEarned_Total { set; get; }

		public Nullable<Double> Gross_Refund_Amount { set; get; }

		public Nullable<Double> Sub_TotalAmount { set; get; }

		public Nullable<Double> Tip_Amount  {set;get;}

        public Nullable<int> shift_id { set; get; }

        public string sale_id { set; get; }

        public double OtherFinancingAmount { get; set; }
        public double PayoutAmount { get; set; }
        public string AuthorizationID { set; get; }

        public Nullable<int> order_number { set; get; }

    }
	public class vc_PayoutMethods
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public string Name { set; get; }

		public string Decription { set; get; }
		public int PayoutMethodId { set; get; }

		public string fees { set; get; }

		public string Currency { set; get; }

		public string details { set; get; }

	}
	public class vc_CustomerClaims
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String modify_usrid { set; get; }

		public string UserId { set; get; }

		public string IdentityUser_Id { set; get; }

		public int CustomerClaimId { set; get; }

		public string ClaimValue { set; get; }

		public string ClaimType { set; get; }

	}
	public class vc_CustomerLogins
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public string UserId { set; get; }

		public string ProviderKey { set; get; }
		public string LoginProvider { set; get; }

		public string IdentityUser_Id{ set; get;}
	}
	public class vc_CustomerRoles
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public string UserId { set; get; }

		public string RoleId { set; get; }

		public string IdentityUser_Id { set; get; }

	}
	public class vc_Roles
	{
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public string RoleId { set; get; }

		public string Name { set; get; }

	}
	public class vc_BusinessCategories
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public int store_id { set; get; }

		public string Name { set; get; }

		public string CategorieId { set; get; }
	}
	public class vc_CorpPayoutMethods
    {
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

	//	public Nullable<int> StorePayoutMethodsId { set; get; }

		public int PayoutMethodId { set; get; }

		public int corp_id { set; get; }

		public bool IsDefault { set; get; }

		public string AccountName { set; get; }

		public string AccountType { set; get; }

		public string RoutingNumber { set; get; }

		public string AccountNumber { set; get; }

		public string AccountDetails { set; get; }

		public string Status { set; get; }



	}
	public class vc_AllCashTransactions
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }
		public Nullable<int> AllCashTransactionId { set; get; }

		public string  CustomerId { set; get; }

		public Nullable<double> VenueCashAmountConverted { set; get; }

		public Nullable<double> allCashAmount { set; get; }


	}
	public class vc_Customer_Rewards_Balances
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public Nullable<int> store_id { set; get; }

		public Nullable<int> CustomerId { set; get; }

		//public Nullable<Double> VenueCashAmountConverted { set; get; }

		public Nullable<Double> VenueCash__Available_balance { set; get; }

		public Nullable<Double> AllCash__Available_balance { set; get; }

		public Nullable<Double> VenueCash_Gas_Awarded { set; get; }

		public Nullable<Double> VenueCash_Other_Awarded { set; get; }

		public Nullable<Double> VenueCash_Awarded { set; get; }

		public Nullable<Double> VenueCashEarned_Total { set; get; }
		public Nullable<Double> VenueCashBalance { get; set; }
	}

	public class vc_shift_register_progress
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }

		public int shift_id { set; get; }
		public int store_id { set; get; }
		public int register_id { set; get; }
		public int cashier_id_on_duty { set; get; }

	}

	public class vc_StoreRegisters
	{
		[Key]
		public int Id { set; get; }

		public DateTime create_dt { set; get; }

		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }

		public String Modify_usrid { set; get; }
		public int RegisterId { set; get; }

		public int store_id { set; get; }
		public string RegisterNumber { set; get; }
		public Nullable<int> IsGasPump { set; get; }
		public Nullable<int> IsMobileRegister { set; get; }
		public Nullable<int> Cashier_ID_on_duty { set; get; }
		public Nullable<int> Cashier_IDs_on_duty_Gas_Pump { set; get; }

		public string Status { set; get; }

		public int shift_id { set; get; }
	}
	public class vc_Business_Insights_Store
	{
		[Key]
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
		public int  store_id {set;get;}
		public float Paid_Out { set; get; }
		public float VenueCash_Awarded_Total { set; get; }
		public float VenueCash_Outstanding { set; get; }
		public float Total_Sales { set; get; }
	    public float AllCash_Sales { set; get; }
		public float VenueCash_Sales { set; get; }
		public float Return_on_Rewards { set; get; }
		public float Returning_Customers_Sales { set; get; }
		public float New_Customers_Sales { set; get; }
		public float New_Customers { set; get; }
		public float Returning_Customers { set; get; }

	}
	public class vc_VenueCash_Conversions
	{
		[Key]
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
		public int CustomerId { set; get; }
		public int store_id { set; get; }
		public double VenueCashAmountConverted { set; get; }
		//public int conversion_transaction_id { set; get; }
	}
	public class business_details_mst
	{
		[Key]
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
		public int store_id { set; get; }
		public string yelp_id { set; get; }
		public Nullable<Double> latitude { set; get; }
		public Nullable<Double> longitude { set; get; }
		public Nullable<Double> price { set; get; }
		public Nullable<Boolean> open_now { set; get; }
		public string open_at { set; get; }
		public Nullable<int> rate { set; get; }
	}
	public class vc_business_term_detail
	{
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }

		public int store_id { set; get; }

		public string term_text { set; get; }

	}
	public class vc_business_catagory_detail
	{
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }

		public int store_id { set; get; }
		public int CategorieId { set; get; }
		public string alias { set; get; }
		public string title { set; get; }

	}
	public class vc_business_hourse_mst
	{
		[Key]
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
		public int store_id { set; get; }
		public Nullable<bool> is_open_now { set; get; }
		public string open_time { set; get; }

		public string close_time { set; get; }

		public string day { set; get; }

	}

	public class vc_business_review_mst
	{
		[Key]
		public int Id { set; get; }
		public DateTime create_dt { set; get; }
		public string created_userid { set; get; }
		public DateTime modify_dt { set; get; }
		public String Modify_usrid { set; get; }
		public int store_id { set; get; }

		public string review_text { set; get; }
	}

	public class internal_restful_dm
	{
		public class add_payment_method_post_req_dm
		{
            public int id { get; set; }
            public int PayoutMethodId { get; set; }
            public String AccountName { set; get; }
			public String AccountType { set; get; }
			public String RoutingNumber { set; get; }
			public String AccountNumber { set; get; }
            public String AccountDetails { set; get; }
			public int corp_id { get; set; }
			public bool IsDefault { get; set; }
		}

		public class TransactionHistory_get_resp_dm
		{
			public double Amount { set; get; }
			public DateTime Date { set; get; }
			public Double GasAmount { set; get; }

			public Double VenueCashEarned { set; get; }
			public Double enueCashEarnedGas { set; get; }
			public string PaidOut { set; get; }
			public String FirstName { set; get; }
			public String LastName { set; get; }
		}

		public class RewardSettingsList_get_resp_dm
		{
			public int store_id { set; get; }
			public string store_name { set; get; }
			public bool is_gas_station { set; get; }
			public int gas_back_rate { set; get; }
			public double cash_back_rate { set; get; }
		}
		public class GetRewardSettings_get_resp_dm
		{

			public int store_id { set; get; }
			public bool is_gas_station { set; get; }
			public int gas_back_rate { set; get; }
			public double cash_back_rate { set; get; }
		

		}
		public class EditRewardSettings_post_req_dm{
			
			public int store_id { set; get; }
			public double gas_back_rate { set; get; }
			public double cash_back_rate { set; get; }
            public int businessType { get; set; }

		}

		public class store_registers_GetRegisterList_get_resp
		{
			public string RegisterId { set; get; }
			public int store_id { set; get; }
			public string RegisterName { set; get; }

			public String RegisterNumber { set; get; }
		}

		public class GetQrCode_get_req
		{
			public int store_id { set; get; }
			public int RegisterNumber { set; get; }
			public string RegisterName { set; get; }


		
		}
		public class SubmitPaymentRequest_post_req_dm
		{
			public float OtherAmount { set; get; }
			public float GasAmount { set; get; }
			public int cashier_id { set; get; }
			public int transaction_id { set; get; }
            public float Sub_TotalAmount { set; get; }
            public int shift_id { set; get; }
            public float otherFinancingAmount { get; set; }
        }

		public class ProcessRefunds_post_req_dm
		{
			 public int CustomerTransactionId { set; get; }

             public double Amount { set; get; }

            public int shift_id { get; set; }
		}

		public class PendingGasSales_get_resp_dm
		{

				public string CustomerTransactionId {set;get;}
				public string TransactionType { set; get; }
				public DateTime Date { set; get; }
				public int GasAmount {set;get;}
				public string LastName { set; get; }
				public string FirstName { set; get; }



		}

		public class CompleteGasSale_post_req_dm
		{
			public int CustomerTransactionId { set; get; }
			public float GasAmount { set; get; }
 
		}

		public class approve_payments_get_resp_dm
		{

			public DateTime date { set; get; }
			public String FirstName { set; get; }
			public String LastName { set; get; }
			public Double Amount { set; get; }
            public float TotalAmount { set; get; }
            public double GasAmount { set; get; }
            public string Customer_Profile_Pic { set; get; }
            public int CustomerTransactionId { set; get; }

        }
        // Added by ADIL
        public class PendingTransactions_get_resp_dm
        {

            public int shif_id { set; get; }
            public int CustomerTransactionId { set; get; }
            public int Customer_id { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Customer_Profile_Pic { get; set; }
            public string RegisterNumber { get; set; }
            public int RegisterId { get; set; }
            public int? Cashier_ID_on_duty {get;set;}

        }


    public class ScanQRCodeBindingModel_post_req_dm
		{
			public int store_id { set; get; }

			public int register_id { set; get; }

			public int CustomerId { set; get;  }

            public DateTime Date { set; get; }
		}

		public class CAccount_Deactivated_post_req
		{
			public int CustomerId { set; get; }
		}

        public class ConvertAllVenueCashToAllCash_post_req
        {
            public int CustomerId { set; get; }

            public int store_id { set; get; }

            public double VenueCashToConvert { set;get;}
		}
		public class ConvertAllVenueCashToAllCash_post_req_2
		{
			public int CustomerId { set; get; }

			
		}



		public class SearchBusinessBindingModel
		{

			public double rating { get; set; }
			//public Nullable<int> CategorieId { get; set; }
			//public Object categories { get;  set; }
			public double distance { get; set; }
			public string image_url { get; set; }
			public bool is_closed { get;  set; }
			public double latitude { get; set; }
			public double longitude { get; set; }
			//public string price { get;  set; }
			public int review_count { get; set; }
			public string store_address1 { get; set; }
			public string store_city { get; set; }
			public string store_country { get;  set; }
			public string store_name { get; set; }
			public string store_phone_no { get; set; }
			public string store_state { get; set; }
			public string store_zip { get; set; }
			//public string SubCategorieId { get; set; }
			public string Website { get; set; }
			public string yelp_store_id { get; set; }

            // related to VC business
            public int id { get; set; }
            public double GasCashBackRate { get; set; }
            public bool IsGasStation { get; set; }
            public bool IsVenueCashBusiness { get; set; }
            public double OtherCashBackRate { get; set; }
            public int store_id { get; set; }
            public double VenueCash__Available_balance { get; set; }

            public string price { get; set; }

            public string categories { get; set; }



        }
		public class OrderHistoryResponseBModel
		{
			public string store_name { get; set; }
			public string store_address1 { get; set; }
			public string store_address2 { get; set; }
			public string store_city { get; set; }
			public Nullable<int> store_zip { get; set; }
			public string store_state { get; set; }

			public Nullable<DateTime> Date { get; set; }
			public Nullable<float> TotalAmount { get; set; }
			public Nullable<float> VenueCashEarned_Total { get; set; }

			public Nullable<float> VenueCashEarned { set; get; }
			public Nullable<float> GasAmount { get; set; }
			public Nullable<float> OtherAmount { get; set; }
			public Nullable<float> VenueCashEarnedGas { get; set; }
			public Nullable<float> VenueCashEarned_Other { get; set; }
			public string Cardnumber { get; set; }
			public int CustomerTransactionId { get; set; }
			public double? Paid_Credit_Amount { get; set; }
			public double? Paid_VenueCash_Amount { get; set; }

            public double? Paid_AllCash_Amount { get; set; }
            public string CardType { get; set; }

            public string Status { get; set; }

            public double Tip_Amount { get; set; }

		}

		public class ReceivePaymentRequestResponseBindingModel
		{
			
			public DateTime? Date { get; set; }
			public float GasAmount { get; set; }
			public string Status { get; set; }
			public float OtherAmount{ get; set; }

			public float OtherCashBackRate { set; get; }
			public float GasCashBackRate { set; get; }

			public float TotalAllCash { set; get; }
			public float TotalVenueCash { set; get; }


            public int CardId { set; get; }
			public double TotalAmount { get; set; }

            public String card4digits { get; set; }

            public String CardType { get; set; }

            public String Customer_Profile_Pic { get; set; }

            public double OtherFinancingAmount { get; set; }

            public bool isTips { get; set; }
            public bool canPay { get; set; }


        }

		public class SendPaymentRequestBModel
		{
			public int CustomerId { set; get; }
			public int CustomerTransactionId { get; set; }
			public float Paid_AllCash_Amount { get; set; }
			public float Paid_VenueCash_Amount { get; set; }

			//public float OtherAmount { set; get; }
			//public float TotalAmount { set; get; }
			//public float Sub_TotalAmount { set; get; } // Sub_TotalAmount is set by the employee
			public float Tip_Amount { set; get; }

			public float Paid_Credit_Amount { set; get; }

		}

		public class vc_CustomerCreditCards_post_req
		{
			public string BillingAddress1 { get; set; }
			public string BillingAddress2 { get; set; }
			public string CardNumber { get; set; }
			public string CardType { get; set; }
			public string City { get; set; }
			public int CustomerId { get; set; }
			public string ExpirationDate { get; set; }
			public bool IsDefault { get; set; }
			public string NameOnCard { get; set; }
			public string SecurityCode { get; set; }
			public string State { get; set; }
		}

		public class vc_CustomerCreditCards_put_req
		{
			public int id { set; get; }
			public string BillingAddress1 { get; set; }
			public string BillingAddress2 { get; set; }
			public string CardNumber { get; set; }
			public string CardType { get; set; }
			public string City { get; set; }
			public int CustomerId { get; set; }
			public string ExpirationDate { get; set; }
			public bool IsDefault { get; set; }
			public string NameOnCard { get; set; }
			public string SecurityCode { get; set; }
			public string State { get; set; }
		}

		public class my_walletObj
		{
			public double TotalVenueCash { set; get; }

			public double TotalAllCash { set; get; }

			public Object vc_Customer_Rewards_Balances { set; get; }

		}

		public class UserInfoViewModel
		{
			public String Email { get; set; }
			public string FirstName { get; set; }
			public bool HasRegistered { get; set; }
			public string LastName { get; set; }
			public object LoginProvider { get; set; }
			public double? TotalAllCash { get; set; }
			public double? TotalVenueCash { get; set; }
            public double totalEarned { get; set; }
            public string Customer_Profile_Pic { get; set; }
            public int customerTransactionsCount { get; set; }

        }

		public class RegisterBindingModel
		{
			public string Email { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }
			public string Password { get; set; }
			public string Pin { get; set; }
            public string Customer_Profile_Pic { get; set; }
		}

		public class LoginBindingModel
		{
			public string Password { get; set; }
			public string UserName { get; set; }
		}
		public class ChangeFirstNameLastNameModel
		{
			public String Customer_id { set; get; }

			 public String FirstName { set; get; }
			 public String LastName { set; get; }
		}

		public class ChangeEmailModel
		{
			public int Customer_id { get; set; }
			public string Email { get; set; }

		
		}

		public class ChangePasswordModel
		{
			public int Customer_id { get; set; }
			public string Password { get; set; }
            public string currentPassword { get; set; }

        }

		public class ChangePinModel
		{
			public int Customer_id { get; set; }
			public string Pin{ get; set; }
            public string OldPin { get; set; }

        }

		public class Business_Tip_Paid_Out_dm
		{
			public int id { get; set; }
			public string Tip_Payout_Status { get; set; }
		}

		public class Clear_Gas_Pending_Transaction_req_post
		{
			public int CustomerTransactionId { set; get; }
			public string Status { set; get; }

		}

		public class ConvertAllVenueCashToAllCashConvertAll_post_resp
		{
			public double? TotalAllCash { get; set; }
			public double? TotalVenueCash { get; set; }
		}

		public class ConvertStoreVenueCashToAllCash_post_resp
		{
			public int TotalVenueCash { set; get; }

			public int TotalAllCash { set; get; }

			public int store_id { set; get; }
			public string store_name { set; get; }
			public string store_address1 { set; get; }
			public string store_city { set; get; }
			public string store_state { set; get; }
			public string Store_images {set;get;}



		}

		public class customer_update_transaction_post_req
		{

			public int CustomerTransactionId { set; get; }
			public String Status { set; get; }

		}
		public class customer_cancel_transaction_post_req
		{

			public int CustomerTransactionId { set; get; }
			

		}
		public class Send_payment_post_resp
		{
			public string ProcessorConfirmationId { set; get; }
			public float VenueCashEarned_Total { set; get; }
		    public int CustomerTransactionId { set; get; }
			public string Status { set; get; }

            public double Sub_TotalAmount { get; set; }
            public double TotalAmount { get; set; }
            public double Tip_Amount { get; set; }

            public double TotalVenueCash { get; set; }
            public double TotalAllCash { get; set; }
            public double totalEarned { get; set; }

            public string store_name { get; set; }
            public string store_address1 { get; set; }
            public string image_url { get; set; }
            public double OtherFinancingAmount { get; set; }

        }


        public class VC_shift_submitBindingInput
        { 
            public int shift_id;
            public int shift_session_ended;
        }

        public class putEmployeeProfileBindingInput
        {

            public int employee_id { get; set; }
            public string employee_name { get; set; }
            public string employee_title { get; set; }
            public string employee_pw { get; set; }
            public string employee_email { get; set; }
            public string employee_phone { get; set; }
            public bool is_mobile_register { get; set; }
 
        }

        public class changeStoreProfileBindingInput
        {
            public int cashier_id { get; set; }
            public int store_id { get; set; }
        }



    }

    public class vc_refunds
    {
        [Key]
        public int id { set; get; }
        public int is_mobile_register { get; set; }
        public int customer_transaction_id { get; set; }
        public string status { get; set; }
        public int cashier_id { get; set; }
        public DateTime date_of_refund { get; set; }
        public double cc_refunded_amount { get; set; }
        public double vc_refunded_amount { get; set; }
        public double ac_refunded_amount { get; set; }
        public double vc_earned_returned_amount { get; set; }
        public double VenueCash_Deficient_Charge_Amount { get; set; }
        public double cashier_refunded_amount { get; set; }
        public int shift_id { get; set; }
        public double PayoutAmount { get; set; }
        public bool PaidOut { get; set; }
        public Nullable<int> store_id { get; set; }

    }

    public class payment
    {
        public string id { get; set; }
        public string state { get; set; }
        public string sale_id { get; set; }
    }

    //VCBusiness biding models
    //TransactionHistory API
    public class TransactionHistoryInput
    {
        public int store_id { get; set; }
        public int filters { get; set; }//0:ALL,1:ORDERS,2:Refunds,3:Payouts
        public int sort { get; set; }//0:Newest Transactions,1:Last Transactions,2:Highest To lowest payout,3:Lowest to Highest Payout
        public string startDate { get; set; }
        public string endDate { get; set; }
        public int page { get; set; }// page number
        public int pageSize { get; set; }// number of rows on a page
        public bool csv { get; set; }
    }
    public class TransactionHistoryOutPut
    {

        public int id { get; set; }
        public DateTime? date { get; set; }
        public string type { get; set; }
        public string details { get; set; }
        public double totalSale { get; set; }
        public double VenueCashEarned_Total { get; set; }
        public bool paidOut { get; set; }
        public string Status { get; set; }
        public double ccFees { get; set; }
        public double netPayoutAmount { get; set; }
    }

    public class ConfirmPaidOutInput
    {

        public int shift_id;
    }


    public class setPinInput
    {
        public int customer_id { get; set; }
        public String pin { get; set; }

    }

    public class addCouponInput
    {
        public int Coupon_Code_ID { get; set; }
        public int Code_Type { get; set; }
        public string Coupon_Promotion_Name { get; set; }
        public DateTime Coupon_Code_Start_Date { get; set; }
        public DateTime Coupon_Code_Expire_Date { get; set; }
        public double VenueCashAmount { get; set; }

        public int store_id { get; set; }
        public int corp_id { get; set; }
        public int manager_id { get; set; }

    }
    
    public class orderHistoryOutput
    {

        public int CustomerTransactionId;
        public DateTime Date;
        public double TotalAmount;
        public double VenueCashEarned;
        public double VenueCashEarned_Total;
        public double VenueCashEarnedGas;
        public string Status;
        public double Paid_VenueCash_Amount;
        public double Paid_AllCash_Amount;
        public double Paid_Credit_Amount;
        public string Cardnumber;
        public string store_name;
        public string store_address1;
        public string store_address2;
        public string store_city;
        public string store_state;
        public int store_zip;
        public double cash_back_rate;
        public double gas_back_rate;
        public DateTime modify_dt;
        public double newTotal;
        public double OtherFinancingAmount;

    }
    public class redeemCouponInput
    {
        public int customer_id;
        public int Coupon_Code_ID;
        public DateTime Date_Redeemed_Customer;

    }
    // VC Customer Service

    public  class businessPayoutsInput
    {
        public string AccessToken;
        
    }
    //
    public class processPayoutsInput
    {
        public string AccessToken;
        public string user;
        public List<vc_PayoutTransactions> payouts;

    }
    public class corporateInput
    {
        public string AccessToken;
        public int searchBy;
        public string value;
    }
    public class modifyCorporateInput
    {
        public string AccessToken;
        public corporate_mst corporate;
        public string user;

    }
    public class modifyStoreInput
    {
        public string AccessToken;
        public store_profile_mst store;
        public vc_store_profile_mst vc_store;
        public string user;
    }
    public class modifyManagerInput
    {
        public string AccessToken;
        public string user;
        public managers_mst manager;
    }
    public class modifyCustomerInput
    {
        public string AccessToken;
        public string user;
        public vc_customers customer;
    }
    public class modifyEmployeeInput
    {
        public string AccessToken;
        public string user;
        public cashier_mst employee;
    }

}
