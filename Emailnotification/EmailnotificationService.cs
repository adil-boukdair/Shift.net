using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BiomedicaLib.Net;

namespace Emailnotification
{
	class EmailnotificationService
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

		public static void Start()
		{
			String corpEmail = "0";
			Console.WriteLine("Email notifcation service started");
			bool isSent = false;
			String connString = System.Configuration.ConfigurationSettings.AppSettings["shiftreport"];
			using (SqlConnection con = new SqlConnection(connString))
			{
				if(con.State==System.Data.ConnectionState.Closed)
						con.Open();
				SqlCommand command = con.CreateCommand(); // initializing command
				using (command = con.CreateCommand()){
						command.CommandText = "usb_sr_get_email_sending_batch"; 
						using (var reader = command.ExecuteReader()) 
						{
							while (reader.Read()) // reading from the DB
							{
								try
								{
									var id = reader.GetInt32(0);
									var corpName = reader.GetString(1);
									var cEmail = corpEmail = reader.GetString(2);
									var admin = reader.GetInt32(3);
									var html_page = reader.GetString(4);
									var manager_name = reader.GetString(5);
								    var email_subject = reader.GetString(6);

								String path = S3_path + "/" + html_page;
									var webRequest = WebRequest.Create(path);
									string body = "";
									using (var response = webRequest.GetResponse())
									using (var content = response.GetResponseStream())
									using (var r = new StreamReader(content))
									{
										body = r.ReadToEnd();
									}

									body = body.Replace("*[user first name]*", manager_name);
									body = body.Replace("[user first name]", manager_name);
									MailLogService.SendMail("support@shiftreports.com", corpEmail, email_subject, body);
								    Console.WriteLine("Email Sent to recepiant "+ corpEmail);
								}
								catch (Exception ex)
								{
									System.Console.WriteLine(ex.Message);
									return;
									
								}
							}
						}
						con.Close();
						con.Dispose();
					}
					
				
			}
		}
	}	
	
}
