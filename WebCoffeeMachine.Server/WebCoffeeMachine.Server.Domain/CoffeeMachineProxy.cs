using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebCoffeeMachine.Domain;

namespace WebCoffeeMachine.Server.Domain
{
    public class CoffeeMachineProxy : IObservable<CoffeeMachineProxy>
    {
        protected Dictionary<string, string> _recipes;

        protected List<IObserver<CoffeeMachineProxy>> _observers;

        private string _uniqueName;

        public string UniqueName {
            get { return _uniqueName; }
            set { _uniqueName = value; }
        }

        private bool _isMakingCoffee;

        public bool IsMakingCoffee {
            get { return _isMakingCoffee; }
            set { _isMakingCoffee = value; }
        }

        private float _coffeeLevel; // voltage

        public int CoffeeLevel { // %
            get {
                // convert voltage to %
                return (int)_coffeeLevel;
            }
            set {
                _coffeeLevel = value;
            }
        }

        private float _waterLevel; // voltage

        public int WaterLevel { // %
            get {
                // convert voltage to %
                return (int)_waterLevel;
            }
            set {
                _waterLevel = value;
            }
        }

        private bool _isConnected;

        public bool IsConnected {
            get { return _isConnected; }
            set { _isConnected = value; }
        }

        public string Ip { get; private set; }

        public int Port { get; private set; }

        protected int _communicationPin;

        public MakeCoffeeResponseEnum MakeCoffee(string recipe)
        {
            return MakeCoffeeResponseEnum.Undefined;
        }

        public CoffeeMachineProxy(string ip, int port, out int communicationPin)
        {
            Ip = ip;
            Port = port;
            _recipes = new Dictionary<string, string>();
            var random = new Random();
            communicationPin = random.Next();
            _communicationPin = communicationPin;
        }

        public List<string> ToPanel()
            => new List<string>() {
                $"Coffee Machine : {UniqueName}",
                $"ADDRESS : {Ip} | Port : {Port}",
                (IsConnected ? "Connection OK" : "Disconnected") + " | " + (IsMakingCoffee ? "Making some coffee" : "Waiting"),
                $"Coffee : {CoffeeLevel:00}%",
                $"Water : {WaterLevel:00}%"
            };

        public void RegisterObserver(IObserver<CoffeeMachineProxy> newObserver)
        {
            if (newObserver != null)
                _observers.Add(newObserver);
        }

        public void NotifyObservers()
        {
            foreach (var observer in _observers)
                observer.Notify(this);
        }

        public void TurnOnConnection()
        {
            Task.Factory.StartNew(() => {
                // communication routine
            });
        }
    }
}