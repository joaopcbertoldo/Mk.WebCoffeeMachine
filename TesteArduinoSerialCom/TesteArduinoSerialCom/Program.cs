using System;

namespace TesteArduinoSerialCom
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var arduino = new ArduinoControllerMain();
            arduino.SetComPort();
            while (true) {
                var key = Console.ReadKey().Key;
                switch (key) {
                    case ConsoleKey.UpArrow:
                        arduino.LightUp(255, 1000);
                        break;

                    case ConsoleKey.DownArrow:
                        arduino.LightUp(0, 1000);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}