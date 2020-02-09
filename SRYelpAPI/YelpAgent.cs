using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SRYelpAPI
{
	/// <summary>
	/// The main Yelp gant that will Perform the Yelp Operation
	/// </summary>
	public abstract class YelpAgent
	{
		/// <summary>
		/// The Childe class must implment this proparty to get result back
		/// </summary>
		public abstract String YelpAction { get ; }

		public abstract String Send(YelpContext contex);

		
	}
}
