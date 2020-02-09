using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BiomedicaLib.Net;
using ActiveUp.Net.Mail;

namespace Campain_emails
{
	class Program
	{


		static String html_root_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_root_path"];
			}
		}


		static String html_folder_path
		{
			get
			{
				return System.Configuration.ConfigurationSettings.AppSettings["html_folder_path"];
			}
		}

		static String S3_path
		{
			get
			{
				return html_root_path + html_folder_path;
			}
		}

		static void Main(string[] args)
		{

			DB_email_campain_status_mst h = new DB_email_campain_status_mst();
			bool con = true;
			while (con)
			{
				var d = h.GetWaitingQ();
				if (d == null)
					break;
				try
				{
					Console.WriteLine("Verifying Email " + d.email_address);
					if (checkDNS(d.email_address))
					{
						d.status = "D";
						Console.Write("Sent");

					}
					else
					{
						d.status = "F";
						Console.Write("Fail to sent");
					}
				}
				catch(Exception err)
				{
					d.status = "F"; Console.Write("Fail to sent with error " + err.Message);
				}
				Console.WriteLine("");
				h.Saveemail_campain_status_mst(d.Id, d);
			}
			
			  
		}

		protected static bool checkDNS(string host, string recType = "MX")
		{
			bool result = false;
			result=SmtpValidator.Validate(host);
			return result;
		}

	}
}
