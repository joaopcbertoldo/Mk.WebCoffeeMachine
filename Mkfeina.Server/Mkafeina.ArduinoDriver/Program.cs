using Mkafeina.ArduinoDriver.Serial;
using System.IO.Ports;
using System.Linq;

namespace Mkafeina.ArduinoDriver
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string[] portNames;
			do
			{
				portNames = SerialPort.GetPortNames(); 
			} while (!portNames.Contains("COM4"));
			var port = new SerialPort("COM4", 9600);

			var ctrlr = new ArduinoSerialController()
			{
				Port = port
			};

			ctrlr.StartListening();

			while (true) { }
		}
	}
}