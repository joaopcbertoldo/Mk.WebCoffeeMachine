using System;
using System.IO.Ports;
using System.Threading;

namespace TesteArduinoSerialCom
{
    public class ArduinoControllerMain
    {
        private SerialPort currentPort;
        private string arduinoPort;
        private bool portFound;

        public void SetComPort()
        {
            try {
                string[] ports = SerialPort.GetPortNames();
                Console.WriteLine(ports.Length);
                foreach (string port in ports) {
                    Console.WriteLine(port);
                    currentPort = new SerialPort(port, 9600);
                    if (DetectArduino()) {
                        portFound = true;
                        arduinoPort = port;
                        break;
                    } else {
                        portFound = false;
                    }
                    Console.WriteLine(portFound);
                }

            } catch (Exception e) {
            }
        }

        private bool DetectArduino()
        {
            try {
                //The below setting are for the Hello handshake
                byte[] buffer = new byte[5];
                buffer[0] = Convert.ToByte(16);
                buffer[1] = Convert.ToByte(128);
                buffer[2] = Convert.ToByte(0);
                buffer[3] = Convert.ToByte(0);
                buffer[4] = Convert.ToByte(4);
                Console.WriteLine($"Buffer: {buffer[0]}, {buffer[1]}, {buffer[2]}, {buffer[3]}, {buffer[4]}");
                int intReturnASCII = 0;
                char charReturnValue = (Char)intReturnASCII;
                currentPort.Open();
                currentPort.Write(buffer, 0, 5);
                Thread.Sleep(1000);
                int count = currentPort.BytesToRead;
                Console.WriteLine(count);
                string returnMessage = "";
                while (count > 0) {
                    intReturnASCII = currentPort.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                //ComPort.name = returnMessage;
                currentPort.Close();
                if (returnMessage.Contains("HELLO FROM ARDUINO")) {

                    return true;
                } else {
                    return false;
                }
            } catch (Exception e) {
                return false;
            }
        }

        public void LightUp(int value, int milisec)
        {
            var arduino = new SerialPort(arduinoPort, 9600);
            //var arduino = new SerialPort("COM3", 9600);
            try {
                byte[] buffer = new byte[5];
                buffer[0] = Convert.ToByte(16);
                buffer[1] = Convert.ToByte(127);
                buffer[2] = Convert.ToByte(4);
                buffer[3] = Convert.ToByte(value);
                buffer[4] = Convert.ToByte(4);
                arduino.Open();
                arduino.Write(buffer, 0, buffer.Length);
                Console.WriteLine(milisec);
                Thread.Sleep(milisec);
                int count = arduino.BytesToRead;
                string returnMessage = "";
                int intReturnASCII;
                while (count > 0) {
                    intReturnASCII = arduino.ReadByte();
                    returnMessage = returnMessage + Convert.ToChar(intReturnASCII);
                    count--;
                }
                arduino.Close();
                Console.WriteLine(returnMessage);
                Console.WriteLine();
            } catch (Exception ex) {
                arduino.Close();
                Console.WriteLine(ex);
            }
        }
    }
}