using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SRYelpAPI
{
	public class YelpSearchAgent : YelpAgent
	{
		public override string YelpAction
		{
			get
			{
				return "/v3/businesses/search?";
			}
		}

		public override String Send(YelpContext contex)
		{
			return contex.Get(YelpAction);
			
		}

		public class SearchResponse
		{


			public class Span
			{
				public double latitude_delta { get; set; }
				public double longitude_delta { get; set; }
			}

			public class Center
			{
				public double latitude { get; set; }
				public double longitude { get; set; }
			}

			public class Region
			{
				public Span span { get; set; }
				public Center center { get; set; }
			}

			public class Coordinate
			{
				public double? latitude { get; set; }
				public double? longitude { get; set; }
			}

			public class Location
			{
				public string city { get; set; }
				public List<string> display_address { get; set; }
				public double geo_accuracy { get; set; }
				public string postal_code { get; set; }
				public string country_code { get; set; }
				public List<string> address { get; set; }
				public Coordinate coordinate { get; set; }
				public string state_code { get; set; }
				public List<string> neighborhoods { get; set; }

                //ADDED For V3
                public string country { get; set; }
                public string state { get; set; }
                public string zip_code { get; set; }
                public string address1 { get; set; }
                public string address2 { get; set; }



            }

			public class Option
			{
				public int original_price { get; set; }
				public string title { get; set; }
				public int price { get; set; }
				public string purchase_url { get; set; }
				public string formatted_original_price { get; set; }
				public string formatted_price { get; set; }
				public bool is_quantity_limited { get; set; }
			}

			public class Deal
			{
				public bool is_popular { get; set; }
				public string what_you_get { get; set; }
				public int time_start { get; set; }
				public string title { get; set; }
				public string url { get; set; }
				public string additional_restrictions { get; set; }
				public List<Option> options { get; set; }
				public string image_url { get; set; }
				public string id { get; set; }
				public string currency_code { get; set; }
			}

            public class Business
            {
                public Coordinate coordinates { get; set; }
                public string price { set; get; }
				public bool is_claimed { get; set; }
				public double rating { get; set; }
				public string mobile_url { get; set; }
				public string rating_img_url { get; set; }
				public int review_count { get; set; }
				public string name { get; set; }
				public string snippet_image_url { get; set; }
				public string rating_img_url_small { get; set; }
				public string url { get; set; }
				public List<categories> categories { get; set; }
				public int menu_date_updated { get; set; }
				public string phone { get; set; }
				public string snippet_text { get; set; }
				public string image_url { get; set; }
				public Location location { get; set; }
				public string display_phone { get; set; }
				public string rating_img_url_large { get; set; }
				public string menu_provider { get; set; }
				public string id { get; set; }
				public bool is_closed { get; set; }
				public double distance { get; set; }
				public List<Deal> deals { get; set; }
				
				public string country { set; get; }
			}

			public class categories
			{
				public String alias { set; get; }

				public String title { set; get; }
			}

			public class RootObject
			{
				public Region region { get; set; }
				public int total { get; set; }
				public List<Business> businesses { get; set; }
			}



		}
	}
}
