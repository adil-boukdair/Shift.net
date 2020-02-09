
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftreportLib.Rest
{
	public class emplyees_apps_DM
	{
		public class search_chriteria_datamodel
		{
			public string corp_name { set; get; }

			public string number_of_stores_in_use { set; get; }

			public string next_charge_date { set; get; }

			public string sign_up_date { set; get; }
			public string plan_name { get; set; }
			public string payment_status { get; set; }
			public string from_sign_up_date { get; set; }
			public string to_sign_up_date { get; set; }
		}
	}
}
