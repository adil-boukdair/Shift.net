using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRYelpAPI
{
	public class YelpBusinessAgent : YelpAgent
	{
		public override string YelpAction
		{
			get
			{
				return "/v3/businesses/";
			}
		}

		public override string Send(YelpContext contex)
		{
			return contex.Get(YelpAction);
		}
        public  string getBusiness(YelpContext contex,string id)
        {
            return contex.Get(YelpAction+id);
        }
	}
}
