using System;
using System.Net;
using System.Net.Mail;

namespace Mkafeina.Server.Domain
{
	public class EmailSender
	{
		private string _from;

		public EmailSender(string from)
		{
			_from = from;
		}

		public string SendMail(string message, string to)
		{
			string returnString = "";

			SmtpClient client = new SmtpClient("smtp.gmail.com");
			client.Port = 465;
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.Credentials = new NetworkCredential(_from, "3uqVer0c4f33!");
			client.EnableSsl = true;
			client.UseDefaultCredentials = false;
			client.Timeout = 20000;
			
			try
			{
				client.Send(_from, to, "MKafeína", message);
				returnString = "Success! Please check your e-mail.";
			}
			catch (Exception ex)
			{
				returnString = "Error: " + ex.ToString();
			}

			return returnString;
		}
	}
}