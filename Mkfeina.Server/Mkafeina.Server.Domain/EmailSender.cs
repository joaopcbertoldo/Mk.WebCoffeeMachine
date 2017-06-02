using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using System;
using System.Net;
using System.Net.Mail;

namespace Mkafeina.Server.Domain
{
	public class EmailSender
	{
		private string _user;
		private string _password;
		private string _host;
		private int _port;
		private bool _enableSsl;
		private bool _useDefaulCredentials;

		public EmailSender(string user, string password, string host, int port, bool enableSsl = true, bool useDefaultCredentials = false)
		{
			_user = user;
			_password = password;
			_host = host;
			_port = port;
			_enableSsl = enableSsl;
			_useDefaulCredentials = useDefaultCredentials;
		}

		public void SendMail(string subject, string message, string to, int timeoutMs = 3000)
		{
			SmtpClient client = new SmtpClient(_host, _port);
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.EnableSsl = _enableSsl;
			client.UseDefaultCredentials = _useDefaulCredentials;
			client.Timeout = timeoutMs;
			client.Credentials = new NetworkCredential(_user, _password);

			try
			{
				client.Send(_user, to, subject, message);
				AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>().LogAsync("Success! Please check your e-mail.");
			}
			catch (Exception ex)
			{
				AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>().LogAsync("Error: " + ex.ToString());
			}
		}
	}
}