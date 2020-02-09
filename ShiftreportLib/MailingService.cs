using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Net.Mime;
using System.Net;



namespace BiomedicaLib.Net
{
    public class MailLogService
    {


       
      

        /// <summary>
        /// Send a simple Mail to a recipiant
        /// </summary>
        /// <param name="subjectmessage">Subject of the message</param>
        /// <param name="bodymessage">The body of the message</param>
        public static void SendMail(string senderemail,string reciveremail,string subjectmessage, string bodymessage)
        {
			if (reciveremail.Equals(String.Empty) || senderemail.Equals(String.Empty))
				return;
              
			
			MailMessage theMail = new System.Net.Mail.MailMessage(senderemail, reciveremail);

			theMail.Subject = subjectmessage;
			theMail.Body = bodymessage;
			theMail.IsBodyHtml = true;
			SmtpClient client = new SmtpClient();

            // GMAIL CONFIGURATION #Temporary#
            client.Host = "smtp.gmail.com";//System.Configuration.ConfigurationSettings.AppSettings["emailhost"]; // "172.31.29.141";
            client.EnableSsl = true;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.Port = 587;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("support@venuecash.com", "shiftlive77");


            /*
            // AMAZON SES configuration
            client.Host = "email-smtp.us-west-2.amazonaws.com";//System.Configuration.ConfigurationSettings.AppSettings["emailhost"]; // "172.31.29.141";
            client.Credentials = new System.Net.NetworkCredential("AKIAIKS3DERL5IHOCG2A", "AshwQagO6ap4oI5GEpS3YKhMlAgXrR/ft/Qo23jEs+Uu"); 
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.Port = 587;
            client.EnableSsl = true;
            */
            client.Send(theMail);
		}

		public static void SendMail(string v1, string v2, string v3, Exception ex)
		{
			String msg = FlattenException(ex);
			SendMail(v1, v2, v3, msg);
		}


		public static string FlattenException(Exception exception)
		{
			var stringBuilder = new StringBuilder();

			while (exception != null)
			{
				stringBuilder.AppendLine(exception.Message);
				stringBuilder.AppendLine(exception.StackTrace);

				exception = exception.InnerException;
			}

			return stringBuilder.ToString();
		}

	}
}
