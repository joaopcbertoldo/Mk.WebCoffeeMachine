using Mkafeina.Domain.ServerArduinoComm;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mkafeina.ArduinoDriver.Serial
{
	internal class ArduinoSerialController
	{
		private string _serverApiUrl = @"http://192.168.0.103:80/api/coffeemachine";

		private const string
		REGISTRATION_ROUTE = "/registration",
		REPORT_ROUTE = "/report",
		ORDER_ROUTE = "/order"
		;

		private List<byte> buffer = new List<byte>();

		public ServerCaller ServerCaller = new ServerCaller();

		public SerialPort Port { get; set; }

		public void StartListening()
		{
			bool inMsg = false;
			bool completeMsg = false;
			string msgBuf = "";
			var startBytes = new byte[] { 1, 1, 1 };
			var endBytes = new byte[] { 2, 2, 2 };
			Port.Encoding = Encoding.ASCII;
			Port.Open();
			Task.Factory.StartNew(() =>
			{
				try
				{
					while (true)
					{
						byte b;
						try { b = (byte)Port.ReadByte(); }
						catch { continue; }

						buffer.Add(b);
						Console.Write((char)b);

						if (buffer.Count > 3)
						{
							var last3bytes = buffer.GetRange(buffer.Count - 3, 3);
							if (last3bytes.SequenceEqual(startBytes))
							{
								inMsg = true;
								msgBuf = "";
								Console.WriteLine();
								Console.WriteLine("----------------------------------------------------------------------");
								Console.WriteLine($"Receiving from arduino");
							}
							else if (inMsg)
							{
								if (b != (byte)1 && b != (byte)2)
									msgBuf += (char)b;

								if (last3bytes.SequenceEqual(endBytes))
								{
									inMsg = false;
									completeMsg = true;
									Console.WriteLine();
									Console.WriteLine($"ENDED");
									Console.WriteLine("----------------------------------------------------------------------");
									Console.WriteLine();
								}
							}
						}

						if (completeMsg)
						{
							completeMsg = false;
							var response = SendToServer(msgBuf);

							Thread.Sleep(1000);

							WriteResponse(response);
							msgBuf = "";
						}
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine();
					Console.WriteLine();
					Console.WriteLine(ex);
					Console.WriteLine();
					Console.WriteLine();
				}
			});
		}

		private void WriteResponse(string responseStr)
		{
			byte[] buffer = new byte[] { Convert.ToByte(3), Convert.ToByte(3), Convert.ToByte(3) };
			Port.Write(buffer, 0, 3);

			buffer = Encoding.ASCII.GetBytes(responseStr);
			for (var i = 0; i < buffer.Length; i++)
			{
				Port.Write(buffer, i, 1);
				Thread.Sleep(50);
				//Port.Write(buffer, 0, buffer.Length);
			}

			buffer = new byte[] { Convert.ToByte(4), Convert.ToByte(4), Convert.ToByte(4) };
			Port.Write(buffer, 0, 3);
		}

		private void WaitStartBytes()
		{
			int counter = 0;
			while (counter < 3)
			{
				int b;
				try { b = Port.ReadByte(); }
				catch { continue; }

				if (b == 1)
					counter++;
				else
				{
					Console.Write((char)b);
				}
			}
		}

		private string ReadUntillEndBytes()
		{
			string message = "";
			int counter = 0;
			var start = DateTime.UtcNow;
			while (counter < 3 && (DateTime.UtcNow - start) < new TimeSpan(0, 0, 5))
			{
				int b;
				try { b = Port.ReadByte(); }
				catch { continue; }

				if (b == 2)
					counter++;
				else
				{
					message += (char)b;
				}
			}
			return message;
		}

		public string SendToServer(string str)
		{
			RegistrationRequest regRequest;
			ReportRequest repRequest;
			OrderRequest ordRequest;

			RegistrationResponse regResponse;
			ReportResponse repResponse;
			OrderResponse ordResponse;

			string responseStr = "";

			List<Exception> exs = new List<Exception>();

			try
			{
				var request = JsonConvert.DeserializeObject<ArduinoRequest>(str);
				if (request.msg == MessageEnum.Registration || request.msg == MessageEnum.Offsets || request.msg == MessageEnum.Unregistration)
				{
					regRequest = JsonConvert.DeserializeObject<RegistrationRequest>(str);
					ServerCaller.Send(regRequest, out regResponse, _serverApiUrl + REGISTRATION_ROUTE);
					responseStr = JsonConvert.SerializeObject(regResponse);
				}
				else if (request.msg == MessageEnum.Signals || request.msg == MessageEnum.Disabling || request.msg == MessageEnum.Reenable)
				{
					repRequest = JsonConvert.DeserializeObject<ReportRequest>(str);
					ServerCaller.Send(repRequest, out repResponse, _serverApiUrl + REPORT_ROUTE);
					responseStr = JsonConvert.SerializeObject(repResponse);
				}
				else if (request.msg == MessageEnum.GiveMeAnOrder || request.msg == MessageEnum.Ready || request.msg == MessageEnum.CancelOrders)
				{
					ordRequest = JsonConvert.DeserializeObject<OrderRequest>(str);
					ServerCaller.Send(ordRequest, out ordResponse, _serverApiUrl + ORDER_ROUTE);
					responseStr = JsonConvert.SerializeObject(ordResponse);
				}
				else
				{
					throw new Exception("WTF");
				}

				Console.WriteLine("----------------------------------------------------------------------");
				Console.WriteLine($"Response");
				Console.WriteLine("----------------------------------------------------------------------");
				Console.WriteLine(responseStr);
				Console.WriteLine("----------------------------------------------------------------------");
				Console.WriteLine();
			}
			catch (JsonReaderException ex)
			{
			}
			catch (Exception ex)
			{
				exs.Add(ex);
			}

			return responseStr;
		}
	}
}