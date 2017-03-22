namespace WebCoffeeMachine.Domain
{
    public class RegistrationForms
    {
        public string UniqueName { get; set; }

        public string Ip { get; set; }

        public int Port { get; set; }

        public bool IsValid()
            => !string.IsNullOrEmpty(UniqueName) && !string.IsNullOrEmpty(Ip) && Port > 0;
    }
}