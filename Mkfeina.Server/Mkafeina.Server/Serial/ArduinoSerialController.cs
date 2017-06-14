using System;
using System.IO.Ports;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mkafeina.Server.Serial
{
	internal class ArduinoSerialController
	{
		public static void StartSearchingForArduinos()
		{
			Task.Factory.StartNew(() =>
			  {
				  while (true)
				  {
					  try
					  {
						  //The below setting are for the Hello handshake
						  byte[] buffer = new byte[] { Convert.ToByte(16), Convert.ToByte(128), Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(4) };

						  string[] portNames = SerialPort.GetPortNames();

						  foreach (string portName in portNames)
						  {
							  var port = new SerialPort(portName, 9600);
							  port.Open();
							  port.Write(buffer, 0, 5);

							  Thread.Sleep(1000);

							  int count = port.BytesToRead;
							  string returnMessage = "";
							  while (count > 0)
							  {
								  returnMessage = returnMessage + Convert.ToChar(port.ReadByte());
								  count--;
							  }

							  port.Close();

							  if (returnMessage.Contains("HELLO FROM ARDUINO"))
							  {
								  var serialController = new ArduinoSerialController()
								  {
									  Port = port
								  };
								  serialController.StartListening();
							  }
						  }
					  }
					  catch (Exception e)
					  {
					  }
				  }
			  });
		}

		public SerialPort Port { get; private set; }

		private void StartListening()
		{
			Port.Open();
			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					try
					{
						var request = Port.ReadTo("\n");
						request = request.Remove(request.Length - 1);
						Thread.Sleep(500);
						// converter em json
						// jogar no controller
						var response = "ok";
						byte[] buffer = Encoding.Default.GetBytes(response + "\n");
						Port.Write(buffer, 0, buffer.Length);
						Thread.Sleep(500);
						var ack = Port.ReadLine();
					}
					catch (Exception ex)
					{
					}
				}
			});
		}

		//public void LightUp(int value, int milisec)
		//{
		//	var arduino = new SerialPort(arduinoPort, 9600);
		//	//var arduino = new SerialPort("COM3", 9600);
		//	try
		//	{
		//		byte[] buffer = new byte[5];
		//		buffer[0] = Convert.ToByte(16);
		//		buffer[1] = Convert.ToByte(127);
		//		buffer[2] = Convert.ToByte(4);
		//		buffer[3] = Convert.ToByte(value);
		//		buffer[4] = Convert.ToByte(4);
		//		arduino.Open();
		//		arduino.Write(buffer, 0, buffer.Length);
		//		Console.WriteLine(milisec);
		//		Thread.Sleep(milisec);
		//		int count = arduino.BytesToRead;
		//		string returnMessage = "";
		//		int intReturnASCII;
		//		while (count > 0)
		//		{
		//			intReturnASCII = arduino.ReadByte();
		//			returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
		//			count--;
		//		}
		//		arduino.Close();
		//		Console.WriteLine(returnMessage);
		//		Console.WriteLine();
		//	}
		//	catch (Exception ex)
		//	{
		//		arduino.Close();
		//		Console.WriteLine(ex);
		//	}
		//}
	}
}