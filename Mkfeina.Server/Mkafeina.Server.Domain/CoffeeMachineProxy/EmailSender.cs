using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Domain.Dashboard;
using System;
using System.Net;
using System.Net.Mail;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum TemplateEmailEnum
	{
		Undef = 0
	}

	public class EmailSender
	{
		#region Internal Stuff

		private string _user;
		private string _password;
		private string _host;
		private int _port;
		private bool _enableSsl;
		private bool _useDefaulCredentials;
		private int _timeoutMs;
		private EmailMessageFactory _emailMessageFactory;

		#endregion Internal Stuff

		public static EmailSender CreateEmailSender(string signature)
		{
			var appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			var sender = new EmailSender()
			{
				_user = appconfig.EmailSenderUser,
				_password = appconfig.EmailSenderPassword,
				_host = appconfig.EmailSenderHost,
				_port = appconfig.EmailSenderPort,
				_enableSsl = appconfig.EmailSenderEnableSsl,
				_useDefaulCredentials = appconfig.EmailSenderUseDefaultCredentials,
				_timeoutMs = appconfig.EmailSenderTimeoutMs,
				_emailMessageFactory = new EmailMessageFactory(signature)
			};
			return sender;
		}

		private EmailSender()
		{
		}

		public void SendMail(string to, TemplateEmailEnum message)
		{
			SmtpClient client = new SmtpClient(_host, _port);
			client.DeliveryMethod = SmtpDeliveryMethod.Network;
			client.EnableSsl = _enableSsl;
			client.UseDefaultCredentials = _useDefaulCredentials;
			client.Timeout = _timeoutMs;
			client.Credentials = new NetworkCredential(_user, _password);

			var msgStr = _emailMessageFactory.MakeMessage(message);
			var sbjStr = _emailMessageFactory.MakeSubject(message);

			try
			{
				client.Send(_user, to, sbjStr, msgStr);
				AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>().LogAsync("Success! Please check your e-mail.");
			}
			catch (Exception ex)
			{
				AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractDashboard>().LogAsync("Error: " + ex.ToString());
			}
		}
	}
}