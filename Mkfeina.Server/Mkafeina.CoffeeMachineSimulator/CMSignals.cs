using Mkafeina.Simulator;
using System;

namespace Mkafeina.CoffeeMachineSimulator
{
	internal class CMSignals
	{
		public const string

			REGISTERED = "simulator.registered",
			ENABLED = "simulator.enabled",
			MAKING_COFFEE = "simulator.makingCofee",
			COFFEE = "simulator.Coffee",
			WATER = "simulator.Water",
			SUGAR = "simulator.Sugar"
			;

#warning criar panel line e panel line builder + comandos de enable/disable, offsets

		private float _coffee;
		private float _sugar;
		private float _water;
		private bool _enabled;
		private bool _registered;
		private bool _makingCoffee;

		internal float CoffeeMin;
		internal float CoffeeMax;
		internal float SugarMin;
		internal float SugarMax;
		internal float WaterMin;
		internal float WaterMax;

		public CMSignals()
		{
			var rand = new Random((int)DateTime.Now.Ticks);

			WaterMin = rand.Next(0, 1);
			WaterMax = rand.Next(4, 5);

			CoffeeMin = rand.Next(0, 1);
			CoffeeMax = rand.Next(4, 5);

			SugarMin = rand.Next(0, 1);
			SugarMax = rand.Next(4, 5);

			Water = (float)rand.NextDouble() * 3 + 2;
			Coffee = (float)rand.NextDouble() * 3 + 2;
			Sugar = (float)rand.NextDouble() * 3 + 2;

			Enabled = true;
			Registered = false;
			MakingCoffee = false;
		}

		public event Action<string,object> ChangeEvent;

		private void OnChangeEvent(string lineName) => ChangeEvent?.Invoke(lineName, this);

		public bool Enabled {
			get => _enabled;
			set {
				_enabled = value;
				if (_enabled)
					Dashboard.Sgt.LogAsync("I am ENABLED.");
				else
					Dashboard.Sgt.LogAsync("I am  DISABLED : ( .");
				OnChangeEvent(ENABLED);
			}
		}

		public bool Registered {
			get => _registered;
			set {
				_registered = value;
				if (_registered)
					Dashboard.Sgt.LogAsync("I am REGISTERED.");
				else
					Dashboard.Sgt.LogAsync("I am UNREGISTERED : ( .");
				OnChangeEvent(REGISTERED);
			}
		}

		public bool MakingCoffee {
			get => _makingCoffee;
			set {
				_makingCoffee = value;
				if (_makingCoffee)
					Dashboard.Sgt.LogAsync("I am making some coffee.");
				else
					Dashboard.Sgt.LogAsync("I am doing nothing now.");
				OnChangeEvent(MAKING_COFFEE);
			}
		}

		public float Coffee {
			get { return _coffee > CoffeeMax ? CoffeeMax : (_coffee < CoffeeMin ? CoffeeMin : _coffee); }
			set { _coffee = value > CoffeeMax ? CoffeeMax : (value < CoffeeMin ? CoffeeMin : value); OnChangeEvent(COFFEE); }
		}

		public float Sugar {
			get { return _sugar > SugarMax ? SugarMax : (_sugar < SugarMin ? SugarMin : _sugar); }
			set { _sugar = value > SugarMax ? SugarMax : (value < SugarMin ? SugarMin : value); OnChangeEvent(SUGAR); }
		}

		public float Water {
			get { return _water > WaterMax ? WaterMax : (_water < WaterMin ? WaterMin : _water); }
			set { _water = value > WaterMax ? WaterMax : (value < WaterMin ? WaterMin : value); OnChangeEvent(WATER); }
		}
	}
}