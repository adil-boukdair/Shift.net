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


    public partial class vc_PayoutTransactions
    {
        [Key]
        public int id { get; set; }
        public Nullable<System.DateTime> create_dt { get; set; }
        public string created_userid { get; set; }
        public Nullable<System.DateTime> modify_dt { get; set; }
        public string modify_usrid { get; set; }
        public double Amount { get; set; }
        public bool PaidOut { get; set; }
        public string ProcessorConfirmationId { get; set; }
        public Nullable<System.DateTime> Date_1st_New_Customer_Transaction { get; set; }
        public Nullable<int> promoter_userid { get; set; }
        public Nullable<int> Corp_ID { get; set; }
        public int PayoutMethodID { get; set; }
        public DateTime DateToBePaid { get; set; }
        public double AmountSent { get; set; }
        public double PayoutAmount { get; set; }
        public Nullable<DateTime> DateSent { get; set; }

    }
}
