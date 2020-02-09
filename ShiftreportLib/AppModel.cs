using ExpenseTrackerLib;
using System;

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;

using System.Data.Entity.Core;
using System.Data.Entity.Core.Objects;
using System.ComponentModel.DataAnnotations.Schema;
using ShiftreportLib;
using static shiftreportapp.data.AppModel;

namespace shiftreportapp.data
{
	public class AppModel : DbContext
	{

		public AppModel() : base("name=shiftreport")
		{
			Database.SetInitializer<AppModel>(null);
		}

		#region Data Sets

		public virtual DbSet<vc_shift_register_progress> vc_shift_register_progress2 { set; get; }
		public virtual DbSet<vc_business_review_mst> vc_business_review_mst2 { set; get; }
		public virtual DbSet<bin_ticktcount_mst> bin_ticktcount_mst2 { set; get; }

		public virtual DbSet<vc_business_term_detail> vc_business_term_detail2 { set; get; }

		public virtual DbSet<vc_business_hourse_mst> vc_business_hourse_mst2 { set; get; }

		public virtual DbSet<business_details_mst> business_details_mst2 { set; get; }

		public virtual DbSet<vc_Customer_Rewards_Balances> vc_Customer_Rewards_Balances2 { set; get; }

		public virtual DbSet<vc_VenueCash_Conversions> vc_VenueCash_Conversions2 { set; get; }
		public virtual DbSet<shift_report_employee_mst> shift_report_employee_mst2 { set; get; }
		public virtual DbSet<shift_cigarit_dtl> shift_cigarit_dtl2 { set; get; }
		public virtual DbSet<lotto_bin_dtls> lotto_bin_dtls2 { set; get; }
		public virtual DbSet<program_error_log> program_error_log2 { set; get; }
		public virtual DbSet<drawer_open_mst> drawer_open_mst2 { set; get; }
		public virtual DbSet<phone_cards> phone_cards2 { set; get; }
		public virtual DbSet<cartons_of_cigarrets> cartons_of_cigarrets2 { set; get; }
		public virtual DbSet<shift_checklist_mst> shift_checklist_mst2 { set; get; }
		public virtual DbSet<cigs_sold_mst> cigs_sold_mst2 { set; get; }
		public virtual DbSet<drawer_close_mst> drawer_close_mst2 { set; get; }
		public virtual DbSet<store_profile_mst> store_profile_mst2 { set; get; }
		public virtual DbSet<manager_report_mst> manager_report_mst2 { set; get; }
		public virtual DbSet<corporate_mst> corporate_mst2 { set; get; }
		public virtual DbSet<managers_mst> managers_mst2 { set; get; }
		public virtual DbSet<cashier_mst> cashier_mst2 { set; get; }
		public virtual DbSet<shift_details_mst> shift_details_mst2 { set; get; } 
		public virtual DbSet<cashier_shift_pending_mst> cashier_shift_pending_mst2 { set; get; }
		public virtual DbSet<cig_rackrowtray_open_mst> cig_rackrowtray_open_mst2 { set; get; }
		public virtual DbSet<cig_rackrowtray_close_mst> cig_rackrowtray_close_mst2 { set; get; }
		public virtual DbSet<store_cig_rack_dtl> store_cig_rack_dtl2 { set; get; }
		public virtual DbSet<lotto_rack_dtls> lotto_rack_dtls2 { set; get; }
		public virtual DbSet<store_lotto_rack_dtl> store_lotto_rack_dtl2 { set; get; }
		public virtual DbSet<plan_mst> plan_mst2 { set; get; }
		public virtual DbSet<customer_payment_mst> customer_payment_mst2 { set; get; }
		public virtual DbSet<payment_transactions_mst> payment_transactions { set; get; }
		public virtual DbSet<payment_status_mst> payment_status_mst2 { set; get; }
		public virtual DbSet<store_cash_diff_report_mst> store_cash_diff_report_mst { set; get; }
		public virtual DbSet<store_sales_report_mst> store_sales_report_mst2 { set; get; }
		public virtual DbSet<error_code_mst> error_code_mst2 { set; get; }
		public virtual DbSet<store_employee_hours_report_mst> store_employee_hours_report_mst2 { set; get; }
		public virtual DbSet<store_mop_sales_report_mst> store_mop_sales_report_mst2 { set; get; }
		public virtual DbSet<storelist_dtls> storelist_dtls2 { set; get; }
		public virtual DbSet<scheduler_mst> scheduler_mst2 { set; get; }
		public virtual DbSet<scheduler_decline_reasons_dtls> scheduler_decline_reasons_dtls2 { set; get; }
		public virtual DbSet<rackset_mst> rackset_mst2 { set; get; }
		public virtual DbSet<section_mst> section_mst2 { set; get; }
		public virtual DbSet<section_elemnts_dtls> section_elemnts_dtls2 { set; get; }
		public virtual DbSet<rack_mst> rack_mst2 { set; get; }
		public virtual DbSet<section_elem_ans_mst> section_elem_ans_mst2 { set; get; }
		public virtual DbSet<rack_names_mst> rack_names_mst2 { set; get; }
		public virtual DbSet<rack_ans_mst> rack_ans_mst2 { set; get; }
		public virtual DbSet<rack_template_mst> rack_template_mst2 { set; get; }
		public virtual DbSet<rack_inventory_level_mst> rack_inventory_level_mst2 { set; get; }
		public virtual DbSet<rackset_diff_mst> rackset_diff_mst2 { set; get; }
		public virtual DbSet<plan_major_minor_mst> plan_major_minor_mst2 { set; get; }
		public virtual DbSet<corpoarte_billing_discount_mst> corpoarte_billing_discount_mst2 { set; get; }
		public virtual DbSet<modify_schediualer_post_dm> modify_schediualer_post_dm2 { get; set; }

		public virtual DbSet<safe_drops_mst> safe_drops_mst2 { set; get; }

		//  Venue cash Models
		public virtual DbSet<vc_customers> vc_customers2 { set; get; }
		public virtual DbSet<vc_store_profile_mst> vc_store_profile_mst2 { set; get; }

		public virtual DbSet<vc_StoreAvailableVC> vc_StoreAvailableVC2 { set; get; }

		public virtual DbSet<vc_StoreRegisters> vc_StoreRegisters2 { set; get; }

		public virtual DbSet<vc_StoreGasPump> vc_StoreGasPump2 { set; get; }

		public virtual DbSet<vc_CustomerCreditCards> vc_CustomerCreditCards2 { set; get; }

		public virtual DbSet<vc_PayoutMethods> vc_PayoutMethods2 { set; get; }

		public virtual DbSet<vc_CustomerClaims> vc_CustomerClaims2 { set; get; }

		public virtual DbSet<vc_CustomerLogins> vc_CustomerLogins { set; get; }
		public virtual DbSet<vc_CustomerRoles> vc_CustomerRoles { set; get; }

		public virtual DbSet<vc_Roles> vc_Roles2 { set; get; }

		public virtual DbSet<vc_BusinessCategories> vc_BusinessCategories2 { set; get; }
		public virtual DbSet<vc_CorpPayoutMethods> vc_CorpPayoutMethods2 { set; get; }

		public virtual DbSet<vc_AllCashTransactions> vc_AllCashTransactions2 { set; get; }

		public virtual DbSet<vc_CustomerTransactions> vc_CustomerTransactions2 { set; get; }

        public virtual DbSet<vc_refunds> vc_refunds2 { set; get; }


        public virtual DbSet<vc_vc_VenueCash_Conversions> vc_vc_VenueCash_Conversions { set; get; }
		public virtual DbSet<corporate_employee_hours_report_mst> corporate_employee_hours_report_mst2 { set; get; }

		public virtual DbSet<vc_business_catagory_detail> vc_business_catagory_detail2 { set; get; }

        // Models Added By Adil
        public virtual DbSet<coupon_code_transactions> coupon_code_transactions2 { set; get; }
        public virtual DbSet<promoters_mst> promoters_mst2 { set; get; }
        public virtual DbSet<vc_coupon_code_mst> vc_coupon_code_mst2 { set; get; }
        public virtual DbSet<vc_PayoutTransactions> vc_PayoutTransactions2 { set; get; }


        //public virtual DbSet<vc_CustomerRewardsBalance> vc_CustomerRewardsBalance2 { set; get; }
        #endregion

        public class corporate_employee_hours_report_mst
		{
			public int id { set; get; }
			public int corp_id { set; get; }

			public int store_id { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

			public string plan_name { set; get; }

			public string corporate_name { set; get; }

			public Nullable<double> hours_logged_in { set; get; }

			public Nullable<double> hours_on_register { set; get; }

			public Nullable<Double> hours_difference { set; get; }

			public Nullable<DateTime> date_of_hours { set; get; }



		}
		public class corpoarte_billing_discount_mst
		{
			[Key]
			public int id { set; get; }
			public int corp_id { set; get; }
			public bool isyearly { set; get; }
			public Nullable<Double> discountVal { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rackset_diff_mst
		{
			[Key]
			public int Id { set; get; }
			public Nullable<int> rackset_id { set; get; }
			public Nullable<float> rackset_diffrance { set; get; }
			public Nullable<int> store_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rack_inventory_level_mst
		{
			[Key]
			public int Id { set; get; }
			public string inventory_level_amt { set; get; }
			public int row_no { set; get; }
			public int col_no { set; get; }
			public int rack_no { set; get; }
			public string product_name { set; get; }
			public int store_id { set; get; }
			public int rack_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class scheduler_decline_reasons_dtls
		{
			public int id { set; get; }
			public string reason { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_mop_sales_report_mst
		{
			[Key]
			public int id { set; get; }
			public int store_id { set; get; }
			public Nullable<Double> daily_cash_sales { set; get; }
			public Nullable<Double> daily_credit_sales { set; get; }
			public Nullable<Double> daily_debit_sales { set; get; }
			public Nullable<DateTime> date_of_sales { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_lotto_rack_dtl
		{

			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public int lotto_num_racks { set; get; }
			public int lotto_rack_columns { set; get; }
			public int lotto_rack_rows { set; get; }
			public string lotto_rack_name { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rack_names_mst
		{
			[Key]
			public int Id { set; get; }
			public int rack_id { set; get; }
			public int rack_no { set; get; }
			public int col_no { set; get; }
			public int row_no { set; get; }
			public string product_name { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_rack_img_path
		{

			[Key]
			public int Id { set; get; }
			public int rack_id { set; get; }
			public int store_id { set; get; }
			public string rack_path { set; get; }
			public DateTime upload_time { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_cig_rack_dtl
		{

			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public int cigs_num_rack { set; get; }
			public int cigs_rack_columns { set; get; }
			public int cigs_rack_rows { set; get; }
			public string rack_name { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_cigarit_dtl
		{

			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int rack_name { set; get; }
			public int rack_no { set; get; }
			public float rack_amnt { set; get; }
			public string rack_openclose { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class lotto_bin_dtls
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public string open_close_flag { set; get; }
			public string lotto_name_open { set; get; }
			public int cell_amt_open { set; get; }
			public int cell_amt_added { set; get; }
			public int cell_amt_close { set; get; }
			public string cell_symbol { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class program_error_log
		{
			[Key]
			public int Id { set; get; }
			public int err_message { set; get; }
			public int error_stacktrace { set; get; }
			public int datetime { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class drawer_open_mst
		{
			[Key]
			public int Id { set; get; }
			public Nullable<int> shift_id { set; get; }
			public int fifties_open { set; get; }
			public int twenties_open { set; get; }
			public int tens_open { set; get; }
			public int fives_open { set; get; }
			public int singles_open { set; get; }
			public int dollars_open { set; get; }
			public int quarters_open { set; get; }
			public int dimes_open { set; get; }
			public int nickels_open { set; get; }
			public int rolled_quarters_open { set; get; }
			public int rolled_dimes_open { set; get; }
			public int rolled_nickels_open { set; get; }
			public int rolled_pennies_open { set; get; }
			public int pennies_open { set; get; }
			public Nullable<int> cash_drawer_open { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class phone_cards
		{

			[Key]
			public int Id { set; get; }
			public int phone_begin_1 { set; get; }
			public int phone_begin_2 { set; get; }
			public int phone_begin_3 { set; get; }
			public int phone_begin_4 { set; get; }
			public int phone_added_5 { set; get; }
			public int phone_added_6 { set; get; }
			public int phone_added_7 { set; get; }
			public int phone_added_8 { set; get; }
			public int hone_ended_9 { set; get; }
			public int phone_ended_10 { set; get; }
			public int phone_ended_11 { set; get; }
			public int phone_ended_12 { set; get; }
			public int shift_id { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cartons_of_cigarrets
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int carton_begin { set; get; }
			public int carton_added { set; get; }
			public int carton_ended { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_checklist_mst
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public string q_uuid { set; get; }
			public string q_answer { set; get; }
			public int shift_no { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cigs_sold_mst
		{

			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int cigs_sold_open { set; get; }
			public int cigs_sold_close { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class drawer_close_mst
		{

			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int tens_close { set; get; }
			public int fives_close { set; get; }
			public int singles_close { set; get; }
			public int dollars_close { set; get; }
			public int quarters_close { set; get; }
			public int dimes_close { set; get; }
			public int nickels_close { set; get; }
			public int pennies_close { set; get; }
			public int rolled_quarters_close { set; get; }
			public int rolled_dimes_close { set; get; }
			public int rolled_nickels_close { set; get; }
			public int rolled_pennies_close { set; get; }
			public Nullable<int> cash_drawer_close { set; get; }
			public int fifties_close { set; get; }
			public int twenties_close { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class manager_report_mst
		{
			[Key]
			public int id { set; get; }
			public int shift_id { set; get; }
			public float shift_report_ttaf { set; get; }
			public float shift_report_cash_counted { set; get; }
			public int shift_report_cigs_difference { set; get; }
			public float shift_report_lotto_difference { set; get; }
			public float shift_report_instant_difference { set; get; }
			public float shift_report_cash_difference { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class corporate_mst
		{


			[Key]
			public int Id { set; get; }
			[Display(Name ="Account Adminstrator ID")]
			public Nullable<int> acct_admin_id { set; get; }
			[Display(Name = "Corporate Name")]
			public string corp_name { set; get; }
			[Display(Name = "Address 1")]
			public string corp_address1 { set; get; }
			[Display(Name = "Address 2")]
			public string corp_address2 { set; get; }
			[Display(Name = "City")]
			public string corp_city { set; get; }
			[Display(Name = "State")]
			public string corp_state { set; get; }
			[Display(Name = "Phone")]
			public string corp_phone { set; get; }
			[Display(Name = "AFax")]
			public string corp_fax { set; get; }
			[Display(Name = "Email")]
			public string corp_email { set; get; }
			[Display(Name = "Zip")]
			public string corp_zip { set; get; }
			[Display(Name = "First time login flag")]
			public Nullable<int> firsttimelogin_flag { set; get; }
			[Display(Name = "Number of Store in Use")]
			public Nullable<int> num_of_stores_in_use { set; get; }
			[Display(Name = "Next Chargeing date")]
			public Nullable<DateTime> next_charge_date { set; get; }
			[Display(Name = "NNext Chargeing Ammount")]
			public Nullable<double> next_charge_amt { set; get; }
			[Display(Name = "Last Payment Date")]
			public Nullable<DateTime> last_payment_date { set; get; }
			[Display(Name = "Number of Store Paid")]
			public Nullable<int> num_of_stores_last_paid { set; get; }
			[Display(Name = "Free Trail Signupdate")]
			public Nullable<DateTime> free_trial_sign_up_date { set; get; }
			[Display(Name = "Plan ID")]
			public Nullable<int> plan_id { set; get; }
			[Display(Name = "Available Stores")]
			public Nullable<int> available_stores { get; set; }
			[Display(Name = "Plan Name")]
			public String plan_name { set; get; }
			[Display(Name = "Paymnet Status")]
			public String payment_status { set; get; }
			[Display(Name = "Billable Number of Stores")]
			public Nullable<int> billable_number_of_stores { set; get; }
			[Display(Name = "Coupon Code")]
			public String coupon_code { set; get; }
			[Display(Name = "Sign up date")]
			public Nullable<DateTime> sign_up_date { get; set; }
			[Display(Name = "Free trial expire date")]
			public Nullable<DateTime> free_trial_expire_date { set; get; }
			[Display(Name = "Refferal Source Code")]
			public String referral_source { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

            public string yelp_id { get; set; }
		}
		public class scheduler_mst
		{
			[Key]
			public int id { set; get; }
			public Nullable<int> cashier_id { set; get; }
			public Nullable<int> shift_no { set; get; }
			public Nullable<int> store_id { set; get; }
			public Nullable<int> status { set; get; }
			public Nullable<int> decline_reason { set; get; }
			public Nullable<DateTime> assignment_date { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class modify_schediualer_post_dm
		{
			public int id { set; get; }
			public int status { set; get; }
			public int decline_reason { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class get_referral_checkout_information_post_dm
		{
			public string receipt { set; get; }
			public string manager_id { set; get; }
			public string manager_pw { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class assign_shift_post_dm
		{
			public int store_id { set; get; }
			public int cashier_id { set; get; }
			public int shift_no { set; get; }
			public int status { set; get; }
			public DateTime assignment_date { set; get; }
			public int decline_reason { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

		}
		public class get_wizard_managers_mst_
		{
			public string manager_id { set; get; }
			public String manager_email { set; get; }
			public string manager_name { set; get; }
			public string manager_pw { set; get; }

			public String manager_cell_phone { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class managers_mst
		{
			[Key]
			public int Id { set; get; }
			public String manager_email { set; get; }
			public String manager_cell_phone { set; get; }
			public Nullable<int> corp_id { set; get; }
			public string manager_name { set; get; }
			public string manager_id { set; get; }
			public string manager_pw { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_sales_report_mst
		{
			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public float total_day_sales { set; get; }
			public DateTime date_of_sales { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_employee_hours_report_mst
		{
			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public string plan_name { set; get; }
			public string cashier_name { set; get; }
			public int cashier_id { set; get; }
			public Nullable<Double> hours_logged_in { set; get; }
			public Nullable<Double> hours_on_register { set; get; }
			public Nullable<Double> hours_difference { set; get; }
			public DateTime date_of_hours { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cashier_mst
		{

			[Key]
			public int id { set; get; }
			public int store_id { set; get; }
			public string cashier_name { set; get; }
			public string cashier_pw { set; get; }
			public string cashier_num { set; get; }
			public string cashier_cell_phone { set; get; }
			public string cashier_email { set; get; }
			public string title { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

			public Nullable<int> is_mobile_register { set; get; }
			public Nullable<int> RegisterId_on_duty { set; get; }

		}
		public class safe_drops_mst
		{

			[Key]
			public int id { set; get; }
			public int shift_id { set; get; }
			public float safedrop_num { set; get; }

			public Nullable<float> safedrop_amnt { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class error_code_mst
		{
			[Key]
			public int Id { set; get; }
			public String err_code { set; get; }
			public String err_html { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_details_mst
		{

			[Key]
			public int id { set; get; }
			public Nullable<int> shift_session_started { set; get; }
			public Nullable<int> shift_session_ended { set; get; }
			public Nullable<int> shift_open_time { set; get; }
			public Nullable<int> shift_close_time { set; get; }
			public Nullable<int> shift_scheduled_open { set; get; }
			public Nullable<int> shift_scheduled_close { set; get; }
			public Nullable<Double> credit_sales_reg1 { set; get; }
			public Nullable<Double> credit_sales_reg2 { set; get; }
			public Nullable<Double> credit_sales_all_registers { set; get; }
			public Nullable<Double> debit_sales_reg1 { set; get; }
			public Nullable<Double> debit_sales_reg2 { set; get; }
			public Nullable<Double> debit_sales_all_registers { set; get; }
			public Nullable<Double> cash_sales_reg1 { set; get; }
			public Nullable<Double> cash_sales_reg2 { set; get; }
			public Nullable<Double> cash_sales_all_registers { set; get; }
			public Nullable<int> cig_paks_sold { set; get; }
			public Nullable<Double> safedrop_total { set; get; }
			public Nullable<int> shift_num { set; get; }
			public Nullable<int> cashier_id { set; get; }
			public String cashier_name { set; get; }

			public string shift_status { set; get; }
			public Nullable<int> store_id { set; get; }
			public string store_name { set; get; }
			public Nullable<int> corp_id { set; get; }
			public string shift_date { set; get; }
			public Nullable<Double> cashsales { set; get; }
			public Nullable<Double> cash_counted { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

			public Nullable<int> RegisterId_on_duty { set; get; }
			public Nullable<Double> Tip_Payout_Amount { get; set; }
			public string Tip_Payout_Status { get; set; }
		}
		public class cashier_shift_pending_mst
		{
			[Key]
			public int Id { set; get; }
			public int cashier_id { set; get; }
			public string shift_status { set; get; }
			public int shift_id { set; get; }
			public int shift_num { set; get; }
			public string shift_date { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_websql_dtls
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int cigs_num_rack { set; get; }
			public int cigs_rack_columns { set; get; }
			public int cigs_rack_rows { set; get; }
			public int lotto_num_racks { set; get; }
			public int lotto_rack_columns { set; get; }
			public int lotto_rack_rows { set; get; }
			public int shift_no { set; get; }
			public int shift_opened { set; get; }
			public int shift_closed { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cig_rack_dtl
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int rack_no { set; get; }
			public int row_no { set; get; }
			public int tray_no { set; get; }
			public float open_value { set; get; }
			public float close_value { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class lotto_rack_dtls
		{

			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public int rack_no { set; get; }
			public int rack_row { set; get; }
			public int rack_col { set; get; }
			public string open_amnt { set; get; }
			public string added_amnt { set; get; }
			public string close_amnt { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class storelist_dtls
		{
			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public int managerID { set; get; }
			public string store_type { set; get; }
			public string access_level { set; get; }
			public int corp_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class store_profile_mst
		{
			[Key]
			public int id { set; get; }
			public Nullable<int> corp_id { set; get; }
			public Nullable<int> cigs_num_racks { set; get; }
			public Nullable<int> lotto_num_racks { set; get; }
			public Nullable<int> cashiers_num { set; get; }
			public Nullable<int> shift_1_time_begins { set; get; }
			public Nullable<int> shift_1_time_ends { set; get; }
			public Nullable<int> shift_2_time_begins { set; get; }
			public Nullable<int> shift_2_time_ends { set; get; }
			public Nullable<int> shift_3_time_begins { set; get; }
			public Nullable<int> shift_3_time_ends { set; get; }
			public string store_name { set; get; }
			public string store_address1 { set; get; }
			public string store_address2 { set; get; }
			public string store_city { set; get; }
			public string store_state { set; get; }
			public Nullable<int> store_zip { set; get; }
			public Nullable<int> Store_Catagory { set; get; }
			public string store_email { set; get; }
			public string store_phone_no { set; get; }
			public Nullable<int> managers_num { set; get; }
			public String payment_status { set; get; }
			public Nullable<int> added_paid_status { set; get; }
			public Nullable<int> plan_id { set; get; }
			public String store_wifi_name { set; get; }
			public String store_wifi_pw { set; get; }
			public String store_fax_no { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

			 
			public string Store_images { set; get; }
			public string store_country { get; set; }

			public Nullable<int> review_count { set; get; }

			public Nullable<bool> is_closed { set; get; }
            public Nullable <bool> is_venuecash { get; set; }
		}
		public class bin_ticktcount_mst
		{

			[Key]
			public int Id { set; get; }
			public int store_id { set; get; }
			public int bin_num { set; get; }
			public int row_no { set; get; }
			public int col_num { set; get; }
			public int current_book_value { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class question_cat_dtls
		{

			[Key]
			public int Id { set; get; }
			public string q_category_text { set; get; }
			public string q_caption { set; get; }
			public string q_answer_caption { set; get; }
			public char q_leg_flag { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cig_rackrowtray_open_mst
		{

			[Key]
			public int Id { set; get; }
			public int rack_no { set; get; }
			public string uuid { set; get; }
			public int shift_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rackset_mst
		{
			[Key]
			public int id { set; get; }
			public int store_id { set; get; }
			public string rack_name { set; get; }
			public string rack_title { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class section_mst
		{
			[Key]
			public int Id { set; get; }
			public string section_name { set; get; }
			public string section_title { set; get; }
			public bool show_boarder { set; get; }
			public int rackset_id { set; get; }
			public string section_type { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

		}
		public class section_elemnts_dtls
		{
			[Key]
			public int Id { set; get; }
			public int section_id { set; get; }
			public string elem_lable { set; get; }
			public string elem_name { set; get; }
			public string elem_uuid { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rack_mst
		{
			[Key]
			public int Id { set; get; }
			public string rack_name { set; get; }
			public int rack_no { set; get; }
			public int rack_row { set; get; }
			public int rock_col { set; get; }
			public bool show_started { set; get; }
			public bool show_added { set; get; }
			public bool show_ended { set; get; }
			public int rackset_id { set; get; }
			public bool show_product_label { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class section_elem_ans_mst
		{
			[Key]
			public int Id { set; get; }
			public int elem_id { set; get; }
			public string elem_uuid { set; get; }
			public string elem_val { set; get; }
			public int shift_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class cig_rackrowtray_close_mst
		{

			[Key]
			public int Id { set; get; }
			public int rack_no { set; get; }
			public string uuid { set; get; }
			public int shift_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class ShiftResult
		{
			public int shiftkey { set; get; }
			public int shiftstatus { set; get; }
			public string shift_time { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
			public int store_id { get; set; }

            public int mobile_register_id { get; set; }
		}
		public class shift_store_times
		{
			[Key]
			public int id { set; get; }
			public int store_id { set; get; }
			public string start_time { set; get; }
			public string end_time { set; get; }
			public int shift_no { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_checklist_store_mst
		{
			[Key]
			public int id { set; get; }
			public int store_id { set; get; }
			public string q_uuid { set; get; }
			public string q_type { set; get; }
			public int shift_no { set; get; }
			public string q_text { set; get; }
			public string q_category { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		} 
		public class rack_ans_mst
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public string rack_started_value { set; get; }
			public string rack_added_value { set; get; }
			public string rack_ended_value { set; get; }
			public int rackset_id { set; get; }
			public int rack_no { set; get; }
			public int row_no { set; get; }
			public int col_no { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class rack_ans_mst_resp
		{
			[Key]
			public int Id { set; get; }
			public int shift_id { set; get; }
			public string started_value { set; get; }
			public string added_value { set; get; }
			public string ended_value { set; get; }		
			public int row_no { set; get; }
			public int col_no { set; get; }
			public String product_label { get; set; }


			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class plan_major_minor_mst
		{
			[Key]
			public int Id { set; get; }
			public int major_plan_id { set; get; }
			public int minor_plan_id { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class empolyee_app_details
		{
			[Key]
			public int Id { set; get; }
			public int app_id { set; get; }
			public string user_id { set; get; }
			//public string create_userid { set; get; }

			//public Nullable<DateTime> create_date { set; get; }

			//public string modified_userid { set; get; }

			//public Nullable<DateTime> modified_date { set; get; }
		}
		public class webapps_mst
		{
			[Key]
			public int Id { set; get; }
			public string application_name { set; get; }
			public string application_hint { set; get; }
			public string application_link_name { set; get; }
			public string application_path { set; get; }
			public string application_icon_path { set; get; }

			//public string create_userid { set; get; }

			//public Nullable<DateTime> create_date { set; get; }

			//public string modified_userid { set; get; }

			//public Nullable<DateTime> modified_date { set; get; }
		}
		public class shift_report_employee_mst
		{
			[Key]
			public int Id { set; get; }
			public string empolyee_un { set; get; }
			public string empolyee_pw { set; get; }
			public string emplyeee_first_name { set; get; }
			public string empolyee_last_name { set; get; }
			public string emplyeee_email_address { set; get; }
			public string emplyee_phone { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class plan_mst
		{
			[Key]
			public int Id { set; get; }
			public string plan_name { set; get; }
			public Nullable<Double> price_per_store { set; get; }

			public String payment_transactions { set; get; }

			public Nullable<int> plan_type { set; get; }

			public String plan_label { set; get; }

			public Nullable<int> discount { set; get; }

			public String frequency { set; get; }


			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }

		}
		public class customer_payment_mst
		{
			[Key]
			public int id { set; get; }
			//public int customer_id { set; get; }
			public int corp_id { set; get; }
			public string card_type { set; get; }
			public string firstname { set; get; }
			public string lastname { set; get; }
			public string city { set; get; }
			//public string email { set; get; }
			public string state { set; get; }
			public string zip { set; get; }
			public string card_number { set; get; }
			public string card_exp { set; get; }
			public string card_csc { set; get; }
			//public string customer { set; get; }
			public string payment_type { set; get; }
			public Nullable<int> default_payment { set; get; }
			//public float payment_amount { set; get; }
			//public string txn_id { set; get; }
			//public DateTime transaction_time { set; get; }
			//public string customer_ip { set; get; }
			//public string transaction_ip { set; get; }
			public Nullable<int> order_id { set; get; }
			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class payment_transactions_mst
		{
			[Key]
			public int id { set; get; }
			public int card_id { set; get; }
			public int corp_id { set; get; }
			public int plan_id { set; get; }
			public double? amount_charged { set; get; }
			public int number_of_stores_charged { set; get; }
			public DateTime transaction_date_time { set; get; }
			public string txn_id { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}
		public class payment_status_mst
		{
			[Key]
			public int id { set; get; }
			public string payment_status { set; get; }

			public string create_userid { set; get; }

			public Nullable<DateTime> create_date { set; get; }

			public string modified_userid { set; get; }

			public Nullable<DateTime> modified_date { set; get; }
		}

	}



	[Serializable()]
	public class ShiftreportDtls
	{
		public List<shiftreportapp.data.AppModel.lotto_rack_dtls> lotto_rack_dtls { get; set; }
		public List<shiftreportapp.data.AppModel.cig_rack_dtl> cig_rack_dtl { get; set; }
		public List<shiftreportapp.data.AppModel.shift_checklist_mst> shift_checklist_mst { get; set; }
		public List<shiftreportapp.data.AppModel.phone_cards> phone_cards { get; set; }
		public List<shiftreportapp.data.AppModel.lotto_bin_dtls> lotto_bin_dtls { get; set; }
		public List<shiftreportapp.data.AppModel.drawer_open_mst> drawer_open_mst { get; set; }
		public List<shiftreportapp.data.AppModel.drawer_close_mst> drawer_close_mst { get; set; }
		public List<shiftreportapp.data.AppModel.cig_rackrowtray_open_mst> cig_rackrowtray_open_mst { get; set; }
		public List<shiftreportapp.data.AppModel.cig_rackrowtray_close_mst> cig_rackrowtray_close_mst { get; set; }
		public List<shiftreportapp.data.AppModel.cartons_of_cigarrets> cartons_of_cigarrets { get; set; }

	}
	public class store_profile_mst_nxt_store_col
	{
		public String store_name { set; get; }
		public String store_id { set; get; }
	}
	public class checklist_resulst
	{
		public Nullable<int> q_id { set; get; }
		public String q_uuid { set; get; }
		public Nullable<int> cat_id { set; get; }
		public Nullable<int> shift_no { set; get; }
		public String question_text { set; get; }
		public String q_type { set; get; }
		public String cat_text { set; get; }
	}
	public class shift_checklist_mst_login_resp
	{
		public int id { set; get; }
		public string q_type { set; get; }
		public string q_uuid { set; get; }
		public string q_text { set; get; }
	}
	public class store_times
	{
		public String start_time { set; get; }
		public String end_time { set; get; }
	}
	public class store_sales_report_mst_col
	{
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class corp_sales_report_mst_col
	{
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class store_cash_diff_report_mst_col
	{
		public int id { get; set; }
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class sales_report_dm
	{
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class corop_cash_diff_report_mst_col
	{
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class corp_cash_diff_report_mst_col
	{
		public String total_day_sales { set; get; }
		public String date_of_sales { set; get; }
	}
	public class cash_diff_report_dm
	{
		public String daily_store_cash_difference { set; get; }
		public String date_of_cash_diff { set; get; }
	}
	public class shift_checklist_mst_col
	{
		public String q_id { set; get; }
		public String q_type { set; get; }
		public String q_uuid { set; get; }
		public String q_answer { set; get; }
	}
	public class store_profile_mst_col
	{ 
		public int id { set; get; }
		public String store_name { set; get; }
		public String store_address1 { set; get; }
        public String store_address2 { set; get; }
        public String store_city { set; get; }
		public String store_state { set; get; }
        public int store_zip { set; get; }
        public Nullable<int> managers_num { set; get; }
		public string access_level { set; get; }
        public bool is_tips { get; set; }

	}
	public class cash_difference_dm
	{
		public String cash_difference { set; get; }
		public String date { set; get; }
	}
	public class mop_sales_dm
	{
		public String mop_sales { set; get; }
		public String date { set; get; }
	}
	public class sales_report_data
	{
		public String total_sales { set; get; }
		public String date { set; get; }
	}
	public class employee_hours_reports_dm
	{
		public String id_employee { set; get; }
		public String employee_name { set; get; }
		public List<total_employee_hours_reports_dm> reports { set; get; }

	}
	public class sve_corp_mst_req_param
	{
		public String corp_id { set; get; }
		public String corp_name { set; get; }
		public String corp_address1 { set; get; }
		public String corp_address2 { set; get; }
		public String corp_city { set; get; }
		public String corp_state { set; get; }
		public String corp_phone { set; get; }
		public String corp_fax { set; get; }
		public String corp_email { set; get; }
		public String corp_zip { set; get; }
		public int manager_id { set; get; }
		public String manager_name { set; get; }
		public String manager_pw { set; get; }
		public String manager_cell_phone { set; get; }
	}
	public class total_employee_hours_reports_dm
	{
		public String time { set; get; }
		public String date { set; get; }
	}
	public class wizard_manager_dm
	{
		public int corp_id { set; get; }
		public int id { set; get; }
		public String manager_id { set; get; }
		public String manager_name { set; get; }
		public String manager_pw { set; get; }
		public String manager_cell_phone { set; get; }
		public String manager_email { set; get; }
		public String manager_password { set; get; } //used with modify_manager
		public String access_level { set; get; }
	}
	public class add_manager_dm
	{
		public int corp_id { set; get; }
		public int id { set; get; }
		public String manager_id { set; get; }
		public String manager_name { set; get; }
		public String manager_pw { set; get; }
		public String manager_cell_phone { set; get; }
		public String manager_email { set; get; }
		public String manager_password { set; get; } //used with modify_manager
		public List<mngr_accesslevel> access_controle { set; get; }
		public List<nu_mngr_accesslevel> added_access_controle { set; get; } // new managers
		public String new_manager_flag { get; set; }
	}
	public class add_new_manager_dm
	{

	}
	public class mngr_accesslevel
	{
		public String access_level { set; get; }
		public int store_id { set; get; }
		public String store_name { set; get; }
	}
	public class nu_mngr_accesslevel
	{
		public String access_level { set; get; }
		public int store_id { set; get; }
		public int corp_id { set; get; }
		public int manager_id { set; get; }

	}
	public class add_cashier_dm
	{
		public int store_id { set; get; }
		public int id { set; get; }
		public String cashier_name { set; get; }
		public String cashier_pw { set; get; }
		public String cashier_num { set; get; }
		public String cashier_cell_phone { set; get; }
		public String cashier_email { set; get; }
	}
	public class modify_manager_dm
	{

		public String manager_id { set; get; }
		public String manager_name { set; get; }
		public String manager_email { set; get; }
		public String manager_cellphone { set; get; }
		public String manager_password { set; get; }
		public List<mngr_accesslevel> access_controle { set; get; }

	}
	public class managers_get_wizard
	{

		public String manager_id { set; get; }
		public String manager_name { set; get; }
		public String manager_email { set; get; }
		public String manager_cellphone { set; get; }
		public String manager_password { set; get; }
		public String access_controle { set; get; }

	}
	public class shift_details
	{
		public int id { set; get; }

		public int store_id { set; get; }
		public String store_name { set; get; }
		public String cashier_name { set; get; }
		public String cash_counted { set; get; }
		public String shift_session_started { set; get; }
		public String shift_session_ended { set; get; }
		public String shift_date { set; get; }
		public String shift_num { set; get; }
	}
	public class shiftDetails
	{
		public in_shift_details_mst Inshift_details_mst { set; get; }
		public List<shiftreportapp.data.AppModel.drawer_open_mst> drawer_open_mst { set; get; }
		public List<shiftreportapp.data.AppModel.drawer_close_mst> drawer_close_mst { set; get; }
		public List<shiftreportapp.data.AppModel.safe_drops_mst> safe_drops_mst { set; get; }
		public List<shiftreportapp.data.AppModel.question_cat_dtls> question_cat_dtls { set; get; }
		public List<shiftreportapp.data.AppModel.shift_checklist_store_mst> shift_checklist_store_mst { set; get; }
		public List<shiftreportapp.data.AppModel.shift_checklist_mst> shift_checklist_mst { set; get; }
		public class in_shift_details_mst
		{
			public String store_name { set; get; }
			public String cashier_name { set; get; }
			public String shift_date { set; get; }
			public String shift_open_time { set; get; }
			public String shift_close_time { set; get; }
			public String shift_session_started { set; get; }
			public String shif_session_ended { set; get; }
			public String cash_counted { set; get; }
			public String cash_sales_all_registers { set; get; }
			public String credit_sales_all_registers { set; get; }
			public String debit_sales_all_registers { set; get; }
			public String cash_sales_reg1 { set; get; }
			public String credit_sales_reg1 { set; get; }
			public String debit_sales_reg1 { set; get; }
			public String cash_sales_reg2 { set; get; }
			public String credit_sales_reg2 { set; get; }
			public String debit_sales_reg2 { set; get; }
		}
	}
	public class shift_details_mst3
	{
		public int id { set; get; }
		public int store_id { set; get; }
		public String store_name { set; get; }
		public int shift_num { set; get; }

		public String shift_date { set; get; }
		public String cashier_name { set; get; }
	}
	public class post_pending_shift
	{
		//shift_id=12232,shift_status="E"
		public String shift_id { set; get; }
		public String shift_status { set; get; }

	}
	public class update_store_customize_page_dm
	{
		public int corp_id { get; set; }
		public int store_id { set; get; }
		public store_profile_mst_rq store_profile_mst { set; get; }
		public List<shiftreportapp.data.AppModel.question_cat_dtls> question_cat_dtls { set; get; }
		public List<shiftreportapp.data.AppModel.shift_checklist_store_mst> shift_checklist_store_mst { set; get; }
		public List<shiftreportapp.data.AppModel.shift_store_times> shift_store_times { set; get; }
		public List<add_manager_dm> managers_mst { set; get; }

		public List<add_cashier_dm> cashier_mst { get; set; }
	}
	public class save_wizard_dm
	{
		public int added_paid_status { set; get; } // store_profile_mst
		public int firsttimelogin_flag { set; get; } // store_profile_mst
		public int corp_id { get; set; }
		//public int store_id { set; get; }
		public int available_stores { get; set; }
		public store_profile_mst_rq store_profile_mst { set; get; }
		//public List<shiftreportapp.data.AppModel.question_cat_dtls> question_cat_dtls { set; get; }
		public List<shiftreportapp.data.AppModel.shift_checklist_store_mst> shift_checklist_store_mst { set; get; }
		public List<shiftreportapp.data.AppModel.shift_store_times> shift_store_times { set; get; }
		public List<add_manager_dm> managers_mst { set; get; }
		public List<add_manager_dm> add_mngrs { set; get; }
		public Boolean new_manager_flag { set; get; }
		//public List<nu_mngr_accesslevel> new_managers { set; get; }
		public List<add_cashier_dm> cashier_mst { get; set; }
	}
	public class get_store_submitted_shifts_rep_dm
	{
		public String id { set; get; }
		public String cashier_name { set; get; }
		public String cash_counted { set; get; }
		public String shift_session_started { set; get; }
		public String shift_session_ended { set; get; }
		public String shift_date { set; get; }
		public String shift_num { set; get; }
	}
	public class get_store_pending_shifts_resp_dm
	{
		public String id { set; get; }
		public String shift_num { set; get; }
		public String shift_date { set; get; }
		public String cashier_name { set; get; }
	}
	public class get_cash_counted_resp_dm
	{
		public String id { set; get; }
		public String shift_num { set; get; }
		public String shift_date { set; get; }
		public String cashier_name { set; get; }
		public String cash_counted { set; get; }
	}
	public class update_cash_counted_req_dm
	{
		public String id { set; get; }
		public String cash_counted { set; get; }
	}
	public class get_cashier_list_resp_dm
	{
		public String id { set; get; }
		public String cashier_name { set; get; }
		public String cashier_num { set; get; }
		public String cashier_pw { set; get; }

		public String title { set; get; }
	}
	public class store_cash_diff_report_mst_dm
	{
		public string daily_store_cash_difference { get; set; }

		public string date_of_cash_diff { get; set; }
	}
	public class shift_times
	{
		public String _id { set; get; }
		public String begin_time { set; get; }
		public String end_time { set; get; }
	}
	public class reset_pending_shift_req_dm
	{
		public String shift_id { set; get; }
		public String shift_status { set; get; }
	}
	public class store_sales_report_mst
	{
		public Nullable<double> total_day_sales { set; get; }
		public Nullable<DateTime> date_of_sales { set; get; }
	}
	public class corp_sales_report_mst
	{
		[Key]
		public int id { set; get; }
		public int corp_id { set; get; }
		public Nullable<Double> total_day_sales { set; get; }
		public Nullable<DateTime> date_of_sales { set; get; }
	}
	public class store_cash_diff_report_mst
	{
		[Key]
		public int id { set; get; }
		public int store_id { set; get; }
		public Nullable<Double> daily_store_cash_difference { set; get; }
		public Nullable<DateTime> date_of_cash_diff { set; get; }
	}
	public class corp_cash_diff_report_mst
	{
		[Key]
		public int id { set; get; }
		public int corp_id { set; get; }
		public Nullable<Double> daily_corp_cash_difference { set; get; }
		public Nullable<DateTime> date_of_cash_diff { set; get; }
	}
	public class store_profile_mst_rq
	{
		public int id { set; get; }
		public string store_name { set; get; }
		public string store_address1 { set; get; }
		public string store_address2 { set; get; }
		public string store_city { set; get; }
		public string store_state { set; get; }
		public Nullable<int> store_zip { set; get; }
		public string store_email { set; get; }
		public string store_phone_no { set; get; }
		public String store_fax_no { set; get; }
		public String store_phone { set; get; }
		public String store_wifi_name { set; get; }
		public String store_wifi_pw { set; get; }
        //VenueCash Parameters
        public int numberOfRegisters { set; get; }
        public int numberOfGasPumps { set; get; }
        public double cash_back_rate { get; set; }
        public double gas_back_rate { get; set; }
        public bool is_gas_station { get; set; }
        public bool is_mobile_register { get; set; }
        public bool is_tips { get; set; }
        public bool is_venuecash { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string image_url { get; set; }
        public string Category_Name { get; set; }
        public string yelp_store_id { get; set; }
        public string Website { get; set; }
        public string price { get; set; }
        public int review_count { get; set; }
        public double Rating { get; set; }
        public bool is_welcome_kit { get; set; }
		public int business_type {get;set;}
    }
	public class get_next_manger_id
	{
		public int id { set; get; }
		public String manager_id { set; get; }
	}
	public class add_corporate_post_req
	{
		public DateTime sign_up_date;

		public int corp_id { get; set; }
		public int promoter_id { set; get; }
		public String corp_name { set; get; }
		public String corp_address1 { set; get; }
		public String corp_address2 { set; get; }
		public String corp_city { set; get; }
		public String corp_zip { set; get; }
		public String corp_state { set; get; }
		public String corp_phone { set; get; }
		public String corp_fax { set; get; }
		public String corp_email { set; get; }
		public String Coupon_Code { set; get; }
		public String firsttimelogin_flag { set; get; }
		public String manager_id { set; get; }
		public String name { set; get; }
		public String password { set; get; }
		public String email { set; get; }
		public int acc_admin_id { set; get; }
		public String plan_name { set; get; }
		public int plan_id { set; get; }
		public String manager_cell_phone { set; get; }
		public DateTime free_trial_expire_date { get; set; }
		public String payment_status { get; set; }
		public String referral_source { set; get; }
        public String yelp_id { get; set; }
	}
	public class add_store_post_req
	{
		public String store_id { set; get; }
		public shiftreportapp.data.AppModel.store_profile_mst store_profile_mst { set; get; }
		public List<shiftreportapp.data.AppModel.question_cat_dtls> question_cat_dtls { set; get; }
		public List<shiftreportapp.data.AppModel.shift_checklist_store_mst> shift_checklist_store_mst { set; get; }
		public List<shiftreportapp.data.AppModel.shift_store_times> shift_store_times { set; get; }
		public List<shiftreportapp.data.AppModel.cashier_mst> cashier_mst { set; get; }

		public List<add_manager_dm> managers_mst { set; get; }

	}
	public class get_corporate_submitted_shifts_get_resp
	{
		public String id { set; get; }
		public String cashier_name { set; get; }
		public String cash_counted { set; get; }
		public Nullable<int> shift_session_started { set; get; }
		public Nullable<int> shift_session_ended { set; get; }
		public String shift_date { set; get; }
		public String shift_num { set; get; }

		public int store_id { get; set; }

		public String store_name { set; get; }

		public String houres_loggedin { set; get; }
	}
	public class safe_drop_response
	{
		public int safedrop_num { set; get; }
		public double? safedrop_amnt { set; get; }
	}
	public class question_cat_dtls_response
	{
		public String q_category_text { set; get; }
		public Nullable<int> id { set; get; }
	}
	public class shift_checklist_store_mst_response
	{
		public int id { set; get; } // expected from adil
		public String q_uuid { set; get; }
		public String q_type { set; get; }
		public Nullable<int> q_category { set; get; }
		public String q_text { set; get; }

		public int shift_no { set; get; }
	}
	public class shift_checklist_mst_response
	{
		public String q_uuid { set; get; }
		public String q_answer { set; get; }
	}
	public class shift_details_checklist_response
	{
		[Key]
		public int id { get; set; }
		public string q_uuid { get; set; }
		public string q_text { set; get; }
		public string q_type { set; get; }
		public int q_category { get; set; }
		public string q_answer { get; set; }
		public Nullable<int> shift_no { set; get; }
	}
	public class update_store_req
	{
		public update_store_customize_page_store_profile_mst_req update_store_customize_page_store_profile_mst_req { set; get; }
		//public question_cat_dtls_response[] question_cat_dtls_response { set; get; }
		public shift_checklist_store_mst_response[] shift_checklist_store_mst_response { set; get; }
		public shiftreportapp.data.AppModel.shift_store_times[] shift_store_times { set; get; }
		public shiftreportapp.data.AppModel.shift_store_times[] new_shift_store_times { set; get; }
		//public List<modify_manager_dm> managers_mst { set; get; }
		public List<int> deleted_checklist_items { get; set; }
		public List<int> deleted_shift_time { set; get; }
	}
	public class update_store_customize_page_store_profile_mst_req
	{
		public int store_id { set; get; }
		public String store_name { set; get; }
		public String store_address1 { set; get; }
		public String store_address2 { set; get; }
		public String store_city { set; get; }
		public String store_zip { set; get; }
		public String store_state { set; get; }
		public String store_phone { set; get; }
		public String store_email { set; get; }
		public String store_wifi_name { set; get; }
		public String store_wifi_pw { set; get; }

        //VenueCash Parameters
        public int numberOfRegisters { set; get; }
        public int numberOfGasPumps { set; get; }
        public double cash_back_rate { get; set; }
        public double gas_back_rate { get; set; }
        public bool is_gas_station { get; set; }
        public bool is_mobile_register { get; set; }
        public bool is_tips { get; set; }
        public bool is_venuecash { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string image_url { get; set; }
        public string Category_Name { get; set; }
        public string yelp_store_id { get; set; }
        public string Website { get; set; }
        public string price { get; set; }
        public int review_count { get; set; }
        public double Rating { get; set; }
        public int business_type { get; set; }

    }
	public class report_item
	{
		public Nullable<Double> Value { set; get; }
		public String Date { set; get; }
	}

	#region Get Plan
	public class get_plan_req
	{
		public string payment_status { set; get; }
		public List<AppModel.plan_mst> plans { set; get; }

	}


	public class plan_item
	{
		public int id { set; get; }
		public string plan_name { set; get; }
		public double? price_per_store { set; get; }

	}
	#endregion

	#region Get Credit Card From File
	public class get_credit_card_on_file_req
	{
		public Nullable<int> default_payment { get; set; }
		public String card_id { set; get; }
		public String card_type { set; get; }
		public String card_last_4_digit { set; get; }
		public String payment_type { set; get; }
	}
	#endregion

	#region Add Credit Card
	public class add_credit_card_req
	{
		public int id { get; set; }
		public int corp_id { set; get; }
		public string card_type { set; get; }
		public int payment_type { set; get; }
		public string firstname { set; get; }
		public string lastname { set; get; }
		public string email { set; get; }
		public string state { set; get; }
		public string city { set; get; }
		public string zip_code { set; get; }
		public string card_number { set; get; }
		public string expiration_month { set; get; }
		public string expiration_year { set; get; }
		public string security_code { set; get; }
		public int default_payment { get; set; }

	}
	#endregion

	#region Get Billing History
	public class get_billing_history_req
	{

		public int plan_id { set; get; }
		public string plan_name { set; get; }
		public List<CustomerTransaction> payment_transactions { set; get; }

	}

	public class CustomerTransaction
	{
		public int id { set; get; }
		public int default_payment { get; set; }
		public string card_last_4_digits { get; set; }
		public string plan_name { get; set; }
		public DateTime transaction_date_time { set; get; }
		public double? amount_charged { set; get; }
		public int number_of_stores_charged { set; get; }
	}
	#endregion

	#region Get Checkout Information
	public class get_checkout_information_req
	{
		public string corp_name { set; get; }
		public string corp_address1 { set; get; }
		public string corp_address2 { set; get; }
		public string corp_city { set; get; }
		public string corp_state { set; get; }
		public string corp_phone { set; get; }
		public string corp_fax { set; get; }
		public string corp_email { set; get; }
		public string corp_zip { set; get; }
		public Nullable<int> number_of_stores_in_use { set; get; }
		public string card_number { set; get; }
		public string payment_type { set; get; }
		public Nullable<Double> price_per_store { set; get; }

		public int firsttimelogin_flag { set; get; }

		public string plan_name { set; get; }

		public Nullable<int> plan_id { set; get; }

	}
	#endregion

	#region Submit Order
	public class submit_order_req
	{
		public int corp_id { set; get; }
		public int plan_id { set; get; }
		public string corp_payment_status { set; get; }
		public int num_of_stores_last_paid { set; get; }
		public DateTime last_payment_date { set; get; }
		public float next_charge_amt { set; get; }
		public DateTime next_charge_date { set; get; }
		public float amount_charged { set; get; }
		public DateTime transaction_date_time { set; get; }
		//public string txn_id { set; get; }
		public int number_of_stores_charged { set; get; }
		public int card_id { set; get; }

		public int firsttimelogin_flag { get; set; }
		public int available_stores { set; get; }
		public Double discountVal { set; get; }
		public Boolean isyearly { set; get; }
	}
	#endregion



	#region Get Rack Flag
	//public class get_rack_flag_req
	//{
	//    public int corp_id { set; get; }
	//    public int plan_id { set; get; }
	//    public int rack_flag { set; get; }

	//}
	#endregion

	#region Update FirstTimeLogin Flag
	public class update_firsttimelogin_flag_req
	{
		public int corp_id { set; get; }
		public int firsttimelogin_flag { set; get; }

	}
	#endregion

	#region Get Available Stores
	public class get_available_stores_req
	{
		public int corp_id { set; get; }

		public int available_stores { set; get; }

		public string payment_status { set; get; }

		public int store_id { set; get; }

	}
	#endregion

	#region Cancel Plan
	public class cancel_plan_req
	{
		public int corp_id { set; get; }
		public string corp_payment_status { set; get; }

	}
	#endregion

	#region Get Credit Card
	public class get_credit_card_req
	{

		public string firstname { set; get; }
		public string lastname { set; get; }
		public string city { set; get; }
		public string state { set; get; }
		public string card_number { set; get; }
		public string card_exp { set; get; }
		public Nullable<int> default_payment { set; get; }
		public string zip_code { set; get; }
		public string payment_type { set; get; }
		public string security_code { set; get; }
	}
	#endregion


	public class get_available_stores_resp
	{
		public shiftreportapp.data.AppModel.corporate_mst corporate_mst { set; get; }
		public List<int> store_ids { set; get; }
	}
	public class customer_payment_mst_resp
	{
		public int id { set; get; }
		public string card_number { set; get; }
		public string payment_type { set; get; }

	}
	public class mop_sales_resp
	{
		public double? daily_cash_sales { set; get; }
		public double? daily_credit_sales { set; get; }
		public double? daily_debit_sales { set; get; }
		public DateTime date_of_sales { set; get; }
	}
	public class usp_sr_getshiftsummary_dm
	{
		public int shift_id { set; get; }
		public Nullable<Double> cash_sales { set; get; }
		public Nullable<Double> shift_report_ttaf { set; get; }
		public Nullable<Double> shift_report_cash_counted { set; get; }
		public Nullable<Double> shift_report_cash_difference { set; get; }
		public String cashier_name { set; get; }
		//public float store_name { set; get; }
		//public float cash_sales_all_registers { set; get; }
		public String shift_date { set; get; }
		public String shift_starttime { set; get; }
		public String shift_endtime { set; get; }
		public String shift_hourloged { set; get; }
	}
	public class delete_managers_dm
	{
		public int manager_id { set; get; }
		public List<int> store_id { set; get; }
	}
	public class update_cash_counted_req_dm2
	{
		public List<update_cash_counted_req_dm> shift_details_mst { set; get; }
	}
	public class cash_counted_req
	{
		public List<update_cash_counted_req_dm> shift_details_mst { set; get; }
	}
	public class login_storelist_dtls_dm
	{

		public int store_id { set; get; }


		public string access_level { set; get; }
		public int corp_id { set; get; }
	}
	public class get_wiz_corp_mst
	{
		public int plan_id { set; get; }
		public string payment_status { set; get; }
		public Nullable<int> available_stores { set; get; }
        public string yelp_id { get; set; }
	}
	public class cashier_mst_get_sheduler
	{
		public int cashier_id { set; get; }
		public string cashier_name { set; get; }
	}
	public class shift_store_times_get_scheduler
	{
		public int shift_no { set; get; }
		public DateTime start_time { set; get; }
		public DateTime end_time { set; get; }


	}
	public class scheduler_ms_get_scheduler
	{
		public int id { set; get; }
		public int cashier_id { set; get; }
		public string cashier_name { set; get; }
		public Nullable<int> status { set; get; }
		public Nullable<int> shift_no { set; get; }
		public DateTime assignment_date { set; get; }
		public String decline_reson { set; get; }

	}
	public class assigne_shift_dmt_list
	{
		public List<assigne_shift_dmt> scheduler_mst { set; get; }
	}
	public class assigne_shift_dmt
	{
		public int id { set; get; }
		public int store_id { set; get; }
		public int cashier_id { set; get; }
		public int shift_no { set; get; }
		public int status { set; get; }
		public DateTime assignment_date { set; get; }
		public int decline_reason { set; get; }

	}
	public class assign_shift_response_dm
	{
		public int id { set; get; }
		public int cashier_id { set; get; }
	}
	public class delete_assigned_shift_req_dm
	{
		public List<int> ids { set; get; }
	}
	public class clickbank_notify_req_dm
	{
		public String notification { set; get; }
		public String iv { set; get; }
	}
	public class update_rack_inventory_levels_dm
	{
		public List<update_rack_inventory_levels_dm_cell> racks { set; get; }
	}
	public class update_rack_inventory_levels_dm_cell
	{
		public List<update_rack_inventory_levels_dm_items> cell { set; get; }
	}
	public class update_rack_inventory_levels_dm_items
	{
		public int rack_id { set; get; }
		public int inventory_level_amt { set; get; }
		public int row_no { set; get; }
		public int col_no { set; get; }
		public int rack_no { set; get; }
		public string product_name { set; get; }
	}
	public class store_racks_array
	{

		public List<store_racks> store_racks { set; get; }

	}
	public class store_racks_array_add
	{
		public int store_id { set; get; }
		public List<store_racks> insert_store_racks { set; get; }

	}
	public class store_racks
	{
		public int id { set; get; }
		public string title { set; get; }
		public header header { set; get; }
		public List<racks> rack { set; get; }
		public footer footer { get;  set; }
		public bool show_boarder { get;  set; }
	}
	public class header
	{
		public List<Sections> Sections { set; get; }
	}
	public class footer
	{
		public List<Sections> Sections { set; get; }
	}
	public class racks
	{
		public String rack_name { set; get; }
		public int rack_no { set; get; }
		public int rack_row { set; get; }
		public int rack_col { set; get; }
		public bool show_started { set; get; }
		public bool show_added { set; get; }
		public bool show_ended { set; get; }
		public int id { get; set; }
		public Boolean show_product_label { set; get; }
		public string added_val { get; set; }
		public string started_val { get; set; }
		public string ended_val { get; set; }
		public product_label_dm[] product_label { get; set; }
		public List<AppModel.rack_ans_mst_resp> cell { set; get; }
	}
	public class product_label_cell_dm
	{
		public int rack_no { set; get; }
		public int row_no { set; get; }
		public int col_no { set; get; }
		public string product_name { set; get; }
		public string added { set; get; }
		public string ended { set; get; }
		public string started { set; get; }
	}
	public class Sections
	{
		

		public int id { set; get; }
		public String Title { set; get; }
		public List<elements> elemnts { set; get; }
		public Boolean show_border { set; get; }
	}
	public class elements
	{
		public int id { set; get; }
		public string title { set; get; }
		public string def { set; get; }
		public string val { set; get; }
		public string uuid { set; get; }
	}
	public class BusinessInfoDM
	{
		public List<vc_StoreRegisters> vc_StoreRegisters;

		public string store_plan { set; get; }
		public string shift_1_time_begins { set; get; }
		public string shift_1_time_ends { set; get; }
		public string shift_2_time_begins { set; get; }
		public string shift_2_time_ends { set; get; }
		public string shift_3_time_begins { set; get; }
		public string shift_3_time_ends { set; get; }
		public string cigs_num_rack { set; get; }
		public string cigs_rack_columns { set; get; }
		public string cigs_rack_rows { set; get; }
		public string lotto_num_racks { set; get; }
		public string lotto_rack_columns { set; get; }
		public string lotto_rack_rows { set; get; }
		public string cig_rack_layout { set; get; }
		public string lotto_bin_layout { set; get; }
		public Object shifts { set; get; }
		public Object check { set; get; }
		public int store_cat { get; set; }
		public store_racks_array store_racks_array { set; get; }

        public int is_mobile_register { get; set; }


    }
	public class product_label_dm
	{
		public int row_no { set; get; }
		public int col_no { set; get; }

		public int rack_no { set; get; }
		public string product_name { set; get; }
	}
	public class rack_template_mst
	{
		[Key]
		public int Id { set; get; }
		public int storerack_id { set; get; }
		public string template_name { set; get; }
	}
	public class store_racks_array_update
	{
		public int store_id { set; get; }
		public List<store_racks> insert_store_racks { set; get; }
		public List<store_racks> update_store_racks { set; get; }
		public List<store_racks> delete_store_racks { set; get; }
	}
	public class get_rack_inventory_levels_get__resp
	{
		public List<get_rack_inventory_levels_get_resp_items_racks> store_racks { set; get; }
	}
	public class get_rack_inventory_levels_get_resp_items_racks
	{
		public String title { set; get; }
		public int id { set; get; }
		public List<get_rack_inventory_levels_get_resp_items_racks_item> rack { set; get; }
		
	}
	public class get_rack_inventory_levels_get_resp_items_racks_item
	{
		public string rack_name { set; get; }
		public int rack_id { set; get; }
		public int rack_no { set; get; }
		public int rack_rows { set; get; }
		public int rack_col { set; get; }
		public bool show_product_label { set; get; }
		public List<get_rack_inventory_levels_get_resp_items_rack_cell> cell { set; get; }
	}
	public class get_rack_inventory_levels_get_resp_items_rack_cell
	{
		public string inventory_level_amt { set; get; }
		public int rack_no { set; get; }
		public int row_no { set; get; }
		public int col_no { set; get; }
		public string product_name { set; get; }
	}
	public class add_new_rack_store_collection_dm
	{
		public List<add_new_rack_store_collection_dm_items> store_racks { set; get; }
		
	}
	public class add_new_rack_store_collection_dm_items
	{
		public string inventory_level_amt { set; get; }
		public int row_no { set; get; }
		public int col_no { set; get; }
		public int rack_id { set; get; }
		public int rack_no { set; get; }
		public int store_Id { set; get; }
		public string product_name { get;  set; }
	}

	public class get_store_submitted_shifts_dm
	{
		
		public string cashier_name { get;  set; }
		public Nullable<double> cash_counted { get;  set; }
		public string date { get;  set; }
		public int id { set; get; }
		public string shift_date { get;  set; }
		public String  shift_hourloged { get;  set; }
		public Nullable<int> shift_num { get;  set; }
		public Nullable<int> shift_session_ended { get;  set; }
		public Nullable<int> shift_session_started { get;  set; }
		public string store_name { get;  set; }
        public double total_sales { get; set; }
	}


	public class update_free_plan_req
	{
		public int plan_type { set; get; }

		public int corp_id { set; get; }
	}


	public class update_shiftdetails_req
	{

		public int shift_id { set; get; }

		public update_shiftdetails_req_shift_details_mst shift_details_mst { set; get; }
		public AppModel.drawer_open_mst drawer_open_mst { set; get; }
		public AppModel.drawer_close_mst drawer_close_mst { set; get; }

		public AppModel.safe_drops_mst[] safe_drops_mst { set; get; }

		public shift_racks[] shift_racks { set; get; }

	}
	public class update_shiftdetails_req_shift_details_mst
	{

		public int shift_open_time { set; get; }

		public int shift_close_time { set; get; }

		public int cash_counted { set; get; }

		public int cash_sales_all_registers { set; get; }

		public int credit_sales_all_registers { set; get; }

		public int debit_sales_all_registers { set; get; }

		public float cash_sales_reg1 { set; get; }

		public float credit_sales_reg1 { set; get; }

		public float debit_sales_reg1 { set; get; }

		public float cash_sales_reg2 { set; get; }

		public float credit_sales_reg2 { set; get; }

		public float debit_sales_reg2 {set;get;}
	}

	public class shift_racks
	{
		public int shift_id { set; get; }
		public int Id { set; get; }

		public int rackset_id { set; get; }

		public float started_value { set; get; }

		public float added_value { set; get; }

		public float ended_value { set; get; }

		public int row_no { set; get; }

		public int col_no { set; get; }

		public int rack_no { set; get; }

	}

	public class update_shift_details_post_req
	{
		public int shift_id { set; get; }

		public update_shift_shift_details_mst_req update_shift_shift { set; get; }

		public drawer_open_mst drawer_open_mst { set; get; }

		public drawer_close_mst drawer_close_mst { set; get; }

		public List<safe_drops_mst> safe_drops_mst { set; get; }

		public List<shift_racks> shift_racks { set; get; }

		public List<section_elem_ans_mst> section_elem_ans_mst { set; get; }



	}

	public class update_shift_shift_details_mst_req
	{
		 public int shift_open_time { set; get; }
		public int shift_close_time { set; get; }
		public int cash_counted { set; get; }
		public double cash_sales_all_registers { set; get; }
		public int credit_sales_all_registers { set; get; }
		public double debit_sales_all_registers { set; get; }
		public double cash_sales_reg1 { set; get; }
		public double credit_sales_reg1 { set; get; }
		public double debit_sales_reg1 { set; get; }
		public double cash_sales_reg2 { set; get; }
		public double credit_sales_reg2 { set; get; }
		public double debit_sales_reg2 { set; get; }
	}


	public class update_free_plan_post_req
	{
		public int corp_id { set; get; }
		public int plan_type { set; get;  }
	}
}
