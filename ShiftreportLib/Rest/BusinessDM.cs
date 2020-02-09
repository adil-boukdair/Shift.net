using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerLib.Rest
{
    public class CorporateSignupDM
    {
        public string corp_name { set; get; }
        public int total_stores { set; get; }
        public string corp_address1 { set; get; }
        public string corp_address2 { set; get; }
        public string city { set; get; }
        public string zip_code { set; get; }
        public string manager_name { set; get; }
        public string manager_userid { set; get; }
        public string manager_password { set; get; }
    }
    public class businessLoginDM
    {

        public string store_un { get; set; }

        public string store_pw { get; set; }
    }
    public class GetCorpManagersDM
    {
        public int Id { set; get; }
        public string manager_name { set; get; }
        public string manager_id { set; get; }
        public string manager_pw { set; get; }
 
    }
    public class UpdateCorpManagersDM
    {
        public int corp_id { set; get; }
        public List<GetCorpManagersDM> GetCorpManagersDM { set; get; }
       

    }
    public class UpdateCorpProfileDM
    {

    }
    public class AddManagerDM
    {

    }
    public class ChangePlanDM
    {

    }
    public class AddStoreBasicDM
    {

    }
    public class StoreRackDetailsDM
    {

    }
    public class Upadtecashercountdm
    {

    }
    public class ManagerloginDM
    {

        public string manager_user_id { get; set; }

        public string manager_pw { get; set; }
    }
}
