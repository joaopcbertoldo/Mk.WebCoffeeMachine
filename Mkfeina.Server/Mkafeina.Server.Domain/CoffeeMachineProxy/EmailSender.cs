using Microsoft.Practices.Unity;
using Mkafeina.Domain;
using Mkafeina.Server.Domain.Entities;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	public enum TemplateEmailEnum
	{
		Undef = 0,
		OrderTaken = 1,
		OrderCanceled = 2
	}

	public class EmailSender
	{
		private static string DefaultSignature(string uniqueName)
			=> $"\r\n\r\nMKafeína - {uniqueName}\r\n" +
			   "mkafeina@gmail.com\r\n" +
			   "São Carlos, SP, Brazil\r\n" +
			   "Avenida Trabalhador São-Carlense, 400, Pq Arnold Schimidt\r\n" +
			   "Prédio da Engenharia Mecatrônica, USP\r\n" +
			   "\r\n" +
			   "© 2017 - MKafeína - Engenharia Mecatrônica EESC USP";

		#region Internal Stuff

		private string _user;
		private string _password;
		private string _host;
		private int _port;
		private bool _enableSsl;
		private bool _useDefaulCredentials;
		private int _timeoutMs;
		private CMProxy _owner;
		private string _signature;

		#endregion Internal Stuff

		internal EmailSender(CMProxy owner)
		{
			_owner = owner;
			_signature = DefaultSignature(_owner.Info.UniqueName);
			var appconfig = (AppConfig)AppDomain.CurrentDomain.UnityContainer().Resolve<AbstractAppConfig>();
			_user = appconfig.EmailSenderUser;
			_password = appconfig.EmailSenderPassword;
			_host = appconfig.EmailSenderHost;
			_port = appconfig.EmailSenderPort;
			_enableSsl = appconfig.EmailSenderEnableSsl;
			_useDefaulCredentials = appconfig.EmailSenderUseDefaultCredentials;
			_timeoutMs = appconfig.EmailSenderTimeoutMs;
		}

		private void SendMailAsync(string to, string subject, string message)
			=> Task.Factory.StartNew(() =>
			   {
				   SmtpClient client = new SmtpClient(_host, _port);
				   client.DeliveryMethod = SmtpDeliveryMethod.Network;
				   client.EnableSsl = _enableSsl;
				   client.UseDefaultCredentials = _useDefaulCredentials;
				   client.Timeout = _timeoutMs;
				   client.Credentials = new NetworkCredential(_user, _password);
#warning logar exception
				   try { client.Send(_user, to, subject, message); } catch (Exception ex) { }
			   });

		internal void SendMailOrderTakenAsync(Order orderUnderProcessing)
		{
			var subject = $"MKafeína - Pedido ref #{orderUnderProcessing.Reference} sendo processado!";
			var message = $"O seu pedido de {orderUnderProcessing.RecipeName} (ref #{orderUnderProcessing.Reference}) está sendo processado na MKafeína {_owner.Info.UniqueName}. \r\nSeu pedido estará pronto dentro de alguns instantes." + _signature;
			SendMailAsync(orderUnderProcessing.CustomerEmail, subject, message);
		}

		internal void SendMailQueuePositionHasChangedAsync(Order order, int position)
		{
			var subject = $"MKafeína - Pedido ref #{order.Reference} - A fila andou...";
			var message = $"A fila de cafés andou! O seu pedido de {order.RecipeName} (ref #{order.Reference}) está na posição {position} da fila." + _signature;
			SendMailAsync(order.CustomerEmail, subject, message);
		}

		internal void SendMailOrderReadyAsync(Order orderUnderProcessing)
		{
			var subject = $"MKafeína - Pedido ref #{orderUnderProcessing.Reference} pronto!";
			var message = $"O seu pedido de {orderUnderProcessing.RecipeName} (ref #{orderUnderProcessing.Reference}) já pode ser retirado na MKafeína {_owner.Info.UniqueName}." + _signature;
			SendMailAsync(orderUnderProcessing.CustomerEmail, subject, message);
		}

		internal void SendMailOrderCanceledAsync(Order orderUnderProcessing)
		{
			var subject = $"MKafeína - Pedido ref #{orderUnderProcessing.Reference} CANCELADO!";
			var message = $"O seu pedido de {orderUnderProcessing.RecipeName} (ref #{orderUnderProcessing.Reference}) não pode ser processado na MKafeína {_owner.Info.UniqueName}. \r\nPedimos desculpas pelo inconveniente e agradecemos a compreensão." + _signature;
			SendMailAsync(orderUnderProcessing.CustomerEmail, subject, message);
		}

	}
}