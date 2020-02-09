using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleOAuth;

namespace SRYelpAPI
{
	/// <summary>
	/// The main Yelp Context
	/// </summary>
    public class YelpContext
    {

		Dictionary<String, String> ReqParams = new Dictionary<string, string>();

		

		private const string yelpendpoint = "https://api.yelp.com";
		string AccessToken = System.Configuration.ConfigurationSettings.AppSettings["AccessToken"];
		string AccessTokenSecret = System.Configuration.ConfigurationSettings.AppSettings["AccessTokenSecret"];
		string ConsumerKey = System.Configuration.ConfigurationSettings.AppSettings["ConsumerKey"];
		string ConsumerSecret = System.Configuration.ConfigurationSettings.AppSettings["ConsumerSecret"];
		private string tOKEN;
		private string tOKEN_SECRET;
		private string cONSUMER_KEY;
		private string cONSUMER_SECRET;
        private const string APIKey = "iLsKk1MpetjmTwV7oOMQXOWMZ4S_PgH_9Y785t0_s_dgFENh4aKrJdtymoacLFbofClzLVglsvjIp0pW-9QrLXFIG5x7YAmg6vV-fBaZjo-SdOQHtA5K5YcKNc0pWHYx";


		/// <summary>
		/// The Main Constrcutor that will read the yelp configuration from the app.conf/web.conf file
		/// </summary>
		public YelpContext()
		{
			ReqParams.Clear();
		}

		public YelpContext(string tOKEN, string tOKEN_SECRET, string cONSUMER_KEY, string cONSUMER_SECRET)
		{
			this.AccessToken = tOKEN;
			this.AccessTokenSecret = tOKEN_SECRET;
			this.ConsumerKey = cONSUMER_KEY;
			this.ConsumerSecret = cONSUMER_SECRET;
		}

		public  String Get(string path)
		{
			String R = "";

			var query = System.Web.HttpUtility.ParseQueryString(String.Empty);
	
			foreach (var queryParam in ReqParams)
			{
				
					query[queryParam.Key] = queryParam.Value;
			}

			var uriBuilder = new UriBuilder(yelpendpoint + path);
			uriBuilder.Query = query.ToString().Replace("%2c",",");

			var request = WebRequest.Create(uriBuilder.ToString());
			request.Method = "GET";
			request.ContentType = "application/json";
            //request.Headers.Add("Content-Type", "application/json");

            //string json = getAccessToken();
            //TokenObject d = JsonConvert.DeserializeObject< TokenObject>(json);
            TokenObject auth = (TokenObject)getAPIKey();


            request.Headers.Add("Authorization", auth.token_type +" "+ auth.access_token);
	

			var httpResponse = (HttpWebResponse)request.GetResponse();
			using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
			{
				var result = streamReader.ReadToEnd();
				R = result.ToString();
			}
			return R;
		}


		public string getAccessToken()
		{
            
			var t = new YelpContext();
			var d = new Dictionary<string, string>();
			d.Add("grant_type", "client_credentials");
			d.Add("client_id", "G6CbmlMY8GktHvc6nc_ZkA");
			d.Add("client_secret", "710RAlqE43vv7MuZLftS2BNzZcjMYsusd9yoVyqyccvpf8ZoIIgqCYYb3LyOpU73");
			return t.Post("/oauth2/token?", d);
 
		}

        public Object getAPIKey()
        {
            var auth = new TokenObject {
                expires_in = "",
                token_type = "Bearer",
                access_token = APIKey
            };
        
            return auth;
        }

        public String Post(String path,Dictionary<string,string> p)
		{
			String R = "";

			var query = System.Web.HttpUtility.ParseQueryString(String.Empty);

			foreach (var queryParam in p)
			{

				query[queryParam.Key] = queryParam.Value;
			}

			var uriBuilder = new UriBuilder(yelpendpoint + path);
			uriBuilder.Query = query.ToString();

			var request = WebRequest.Create(uriBuilder.ToString());
			request.ContentType = "application/json";
		

			var t = new Tokens()
			{
				ConsumerKey = ConsumerKey,
				ConsumerSecret = ConsumerSecret,
				AccessToken = AccessToken,  
				AccessTokenSecret = AccessTokenSecret
			};
			


			var httpResponse = (HttpWebResponse)request.GetResponse();
			using (var streamReader = new System.IO.StreamReader(httpResponse.GetResponseStream(), Encoding.UTF8))
			{
				var result = streamReader.ReadToEnd();
				R = result.ToString();
			}
			return R;
		}
	

		#region Yelp Paramerters

		/// <summary>
		/// Number of search results braught back
		/// </summary>
		public int Limit
		{
			set
			{
				if (ReqParams.ContainsKey("limit"))
				{
					ReqParams.Add("limit", value.ToString());
				}
				else
				{
					ReqParams["limit"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Sort the returned result as follow
		/// 0=Best matched (default), 1=Distance, 2=Highest Rated.
		/// For details see yelp documentation
		/// </summary>
		public string Sort
		{
			set
			{
				if (ReqParams.ContainsKey("sort_by"))
				{
					ReqParams.Add("sort_by", value.ToString());
				}
				else
				{
					ReqParams["sort_by"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// The search tearm
		/// </summary>
		public String Term
		{
			set
			{
				if (ReqParams.ContainsKey("term"))
				{
					ReqParams.Add("term", value.ToString());
				}
				else
				{
					ReqParams["term"] = value.ToString();
				}
			}
		}
		/// <summary>
		/// Offset the list of returned business results by this amount
		/// For details see yelp documentation
		/// </summary>
		public int Offset
		{
			set
			{
				if (ReqParams.ContainsKey("offset"))
				{
					ReqParams.Add("offset", value.ToString());
				}
				else
				{
					ReqParams["offset"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Category to filter search results with
		/// </summary>
		public string categoryFilter
		{
			set
			{
				if (ReqParams.ContainsKey("categories"))
				{
					ReqParams.Add("categories", value.ToString());
				}
				else
				{
					ReqParams["categories"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Search radius in meters.
		/// </summary>
		public int radiusFilter
		{
			set
			{
				if (ReqParams.ContainsKey("radius"))
				{
					ReqParams.Add("radius", value.ToString());
				}
				else
				{
					ReqParams["radius"] = value.ToString();
				}
			}
		}
		/// <summary>
		/// Whether to exclusively search for businesses with deals
		/// </summary>
		public int dealsFilter
		{
			set
			{
				if (ReqParams.ContainsKey("deals_filter"))
				{
					ReqParams.Add("deals_filter", value.ToString());
				}
				else
				{
					ReqParams["deals_filter"] = value.ToString();
				}
			}
		}
		/// <summary>
		/// The location 
		/// Specifies the combination of "address, neighborhood, city, state or zip, optional country" to be used when searching for businesses.
		/// </summary>
		public string Location
		{
			set
			{
				if (ReqParams.ContainsKey("location"))
				{
					ReqParams.Add("location", value.ToString());
				}
				else
				{
					ReqParams["location"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Set the geolocation searching criteria
		/// </summary>
		/// <param name="latitude">The Latitude to searchj for</param>
		/// <param name="longitude"> The Longtitude to search for</param>
		public void setGelocation(float latitude,float longitude)
		{
			string p = latitude + "," + longitude;
			if (ReqParams.ContainsKey("cll"))
			{
				ReqParams.Add("cll", p);
			}
			else
			{
				ReqParams["cll"] = p;
			}

		}

		public void setGelocation2(float latitude, float longitude)
		{

			if (!ReqParams.ContainsKey("latitude"))
			{
				ReqParams.Add("latitude", latitude.ToString());
			}
			if (!ReqParams.ContainsKey("longitude"))
			{
				ReqParams.Add("longitude", longitude.ToString());
			}


		}
        public void setID(string id)
        {

            if (!ReqParams.ContainsKey("id"))
            {
                ReqParams.Add("id", id.ToString());
            }

        }




        /// <summary>
        /// Set teh Searching Bounding Box
        /// </summary>
        /// <param name="sw_latitude">SW Latitude</param>
        /// <param name="sw_longitude">SW Longtitude</param>
        /// <param name="ne_latitude">NE Latitude</param>
        /// <param name="ne_longitude">NE Longtitude</param>
        public void setGeolocationBound(float sw_latitude, float sw_longitude, float ne_latitude, float ne_longitude)
		{
			string p = sw_latitude + "," + sw_longitude + "|"+ ne_latitude + "," + ne_longitude;
			if (ReqParams.ContainsKey("bounds"))
			{
				ReqParams.Add("bounds", p);
			}
			else
			{
				ReqParams["bounds"] = p;
			}
		}

		/// <summary>
		/// Specify Location by Geographic Coordinate
		/// </summary>
		/// <param name="latitude"></param>
		/// <param name="longitude"></param>
		/// <param name="accuracy"></param>
		/// <param name="altitude"></param>
		/// <param name="altitude_accuracy"></param>
		public void setGeolocationCoordinate(float latitude, float longitude, float accuracy, float altitude, float altitude_accuracy)
		{
			string p = latitude + "," + longitude + "," + accuracy + "," + altitude+","+ altitude_accuracy;
			if (ReqParams.ContainsKey("ll"))
			{
				ReqParams.Add("ll", p);
			}
			else
			{
				ReqParams["ll"] = p;
			}
		}

		/// <summary>
		/// Set the Country code local
		/// </summary>
		public string countryCode
		{
			set
			{
				if (ReqParams.ContainsKey("cc"))
				{
					ReqParams.Add("cc", value.ToString());
				}
				else
				{
					ReqParams["c"] = value.ToString();
				}
			}
		}
		
		
		/// <summary>
		/// The Language Code
		/// </summary>
		public string Language
		{
			set
			{
				if (ReqParams.ContainsKey("lang"))
				{
					ReqParams.Add("lang", value.ToString());
				}
				else
				{
					ReqParams["lang"] = value.ToString();
				}
			}
		}

		/// <summary>
		/// Whether to include links to actionable content if available
		/// </summary>
		public bool actionLinks
		{
			set
			{
				if (ReqParams.ContainsKey("actionlinks"))
				{
					ReqParams.Add("actionlinks", value.ToString());
				}
				else
				{
					ReqParams["actionlinks"] = value.ToString();
				}
			}
		}
		#endregion

	}

	internal class TokenObject
	{
		public String token_type { set; get; }
		public String access_token { set; get; }
		public String expires_in { set; get; }
	}
}
