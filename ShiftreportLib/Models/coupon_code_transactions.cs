//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShiftreportLib
{


    public partial class coupon_code_transactions
    {

        [Key]
        public int coupon_code_transaction_ID { get; set; }
        public Nullable<int> promoter_userid { get; set; }
        public Nullable<int> Coupon_Code_ID { get; set; }
        public double SignUP_VenueCash_Credit_Amt { get; set; }
        public double VenueCash_Credit_Amt { get; set; }
        public double SignUP_VC_Credit_Issued_by_VenueCash_Amt { get; set; }
        public double SignUP_AC_Credit_Issued_by_VenueCash_Amt { get; set; }
        public double VC_Credit_Issued_by_VenueCash_Amt { get; set; }
        public double AC_Credit_Issued_by_VenueCash_Amt { get; set; }
        public double Promoter_business_Commission_Sign_Up_Amt { get; set; }
        public double Promoter_customer_Commission_Sign_Up_Amt { get; set; }
        public int customer_ID { get; set; }
        public DateTime Date_Redeemed_Server { get; set; }
        public DateTime Date_Redeemed_Customer { get; set; }
        public bool PaidOut { get; set; }
    }
}
