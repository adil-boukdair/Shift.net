using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using YelpSharp;
using YelpSharp.Data;
using SimpleOAuth;
using static ShiftreportLib.internal_restful_dm;
using System.IO;
using YelpSharp.Data.Options;
using SRYelpAPI;
using Newtonsoft.Json;
using static SRYelpAPI.YelpSearchAgent;
using static SRYelpAPI.YelpSearchAgent.SearchResponse;

namespace ShiftreportLib
{
	public class YelpHelper
	{

		Yelp y;
		String CONSUMER_KEY
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["yelp_Consumer_Key"];
			}
		}

		String CONSUMER_SECRET
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["yelp_Consumer_Secret"];
			}
		}

		String TOKEN
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["yelp_Token"];
			}
		}

		String TOKEN_SECRET
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["yelp_Token_Secret"];
			}
		}


        /*
		public List<SearchBusinessBindingModel> SearchForBusinessNearme(float latitude, float longitude, string term, String location,int  radius, String categories, int sort_by)
		{
			List<SearchBusinessBindingModel> ls = new List<SearchBusinessBindingModel>();
			YelpContext cont = new YelpContext(TOKEN, TOKEN_SECRET, CONSUMER_KEY, CONSUMER_SECRET);
			SRYelpAPI.YelpSearchAgent search = new SRYelpAPI.YelpSearchAgent();
			cont.Term =(term!=null?term:"");

			if (latitude != null)
				 cont.setGelocation2(latitude,longitude);
			//else
				cont.Location = (location==null?"":location);

			cont.Limit = 10;
			cont.Sort = sort_by;
			//cont.radiusFilter = radius;
			if(categories!=null)
				cont.categoryFilter = categories;

			

			var searchResults =search.Send(cont);

			var x = JsonConvert.DeserializeObject<RootObject>(searchResults);


			if (Convert.ToInt32(x.total) > 0)
				foreach (var business in x.businesses)
				{
					ls.Add(new ShiftreportLib.internal_restful_dm.SearchBusinessBindingModel()
					{
						IsGasStation = false,
						OtherCashBackRate = "",
						GasCashBackRate = "",
						Rating = Convert.ToInt32(business.rating),
						yelp_store_id = business.id,
						store_name = business.name,
						image_url = business.image_url,
						store_address1 = business.location.display_address[0],
						store_city = business.location.city,
						store_zip = business.location.postal_code,
						store_state = business.location.state_code,
						distance = (double)(business.distance / 1000.0f) * 0.621371f,
						Website = business.url,
						IsVenueCashBusiness = false,
						SubCategorieId = null,
						CategorieId = null,
						latitude = (business.location != null ? 0.0 : business.location.coordinate.latitude),
						longitude = (business.location!=null?0.0: business.location.coordinate.longitude),
						categories = business.categories,
						price = business.price,
						is_closed = business.is_closed,
						review_count = business.review_count,
						store_country = business.country,
						VenueCash__Available_balance=null
					});
				}
	


			return ls;
		}

    */

		
		public Object GetYelpStore(string yelpid)
		{
			Object res = new object();
			var options = new Options()
			{
				AccessToken = TOKEN,
				AccessTokenSecret = TOKEN_SECRET,
				ConsumerKey = CONSUMER_KEY,
				ConsumerSecret = CONSUMER_SECRET
			};
			y = new Yelp(options);
			y.GetBusiness(yelpid);

			return res;
		}
	}

	
}