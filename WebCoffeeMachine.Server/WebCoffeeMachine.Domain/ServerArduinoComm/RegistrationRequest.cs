namespace WebCoffeeMachine.Domain.ServerArduinoComm
{
    public class RegistrationRequest
    {
        /// <summary>
        /// Unique Name
        /// </summary>
        public string un { get; set; }

        /// <summary>
        /// Ip
        /// </summary>
        public string i { get; set; }

        /// <summary>
        /// Port
        /// </summary>
        public int p { get; set; }
    }
}