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
			SUGAR = "simulator.Sugar",
			MIN_COFFEE = "simulator.minCoffee",
			MAX_COFFEE = "simulator.maxCoffee",
			MIN_SUGAR = "simulator.minSugar",
			MAX_SUGAR = "simulator.maxSugar",
			MIN_WATER = "simulator.minWater",
			MAX_WATER = "simulator.maxWater"
			;

		private float _coffee;
		private float _sugar;
		private float _water;
		private bool _enabled;
		private bool _registered;
		private bool _makingCoffee;
		private float _coffeeMin;
		private float _coffeeMax;
		private float _sugarMin;
		private float _sugarMax;
		private float _waterMin;
		private float _waterMax;

		public CMSignals()
		{
			var rand = new Random((int)DateTime.Now.Millisecond);

			WaterMin  = (float)rand.NextDouble();
			WaterMax  = (float)rand.NextDouble() + 4;
			CoffeeMin = (float)rand.NextDouble();
			CoffeeMax = (float)rand.NextDouble() + 4;
			SugarMin  = (float)rand.NextDouble();
			SugarMax  = (float)rand.NextDouble() + 4;

			Water = (float)rand.NextDouble() * 3 + 2;
			Coffee = (float)rand.NextDouble() * 3 + 2;
			Sugar = (float)rand.NextDouble() * 3 + 2;

			Enabled = true;
			Registered = false;
			MakingCoffee = false;
		}

		public event Action<string, object> ChangeEvent;

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

		internal float CoffeeMin { get => _coffeeMin; set { _coffeeMin = value; OnChangeEvent(MIN_COFFEE); } }

		internal float CoffeeMax { get => _coffeeMax; set { _coffeeMax = value; OnChangeEvent(MAX_COFFEE); } }

		internal float SugarMin { get => _sugarMin; set { _sugarMin = value; OnChangeEvent(MIN_SUGAR); } }

		internal float SugarMax { get => _sugarMax; set { _sugarMax = value; OnChangeEvent(MAX_SUGAR); } }

		internal float WaterMin { get => _waterMin; set { _waterMin = value; OnChangeEvent(MIN_WATER); } }

		internal float WaterMax { get => _waterMax; set { _waterMax = value; OnChangeEvent(MAX_WATER); } }
	}
}