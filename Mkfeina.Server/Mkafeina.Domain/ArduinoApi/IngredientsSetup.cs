namespace Mkafeina.Domain.ArduinoApi
{
	public class IngredientsSetup
	{
		public bool CoffeeAvailable { get; set; }

		public float CoffeeEmptyOffset { get; set; }

		public float CoffeeFullOffset { get; set; }

		public bool WaterAvailable { get; set; }

		public float WaterEmptyOffset { get; set; }

		public float WaterFullOffset { get; set; }

		public bool MilkAvailable { get; set; }

		public float MilkEmptyOffset { get; set; }

		public float MilkFullOffset { get; set; }

		public bool SugarAvailable { get; set; }

		public float SugarEmptyOffset { get; set; }

		public float SugarFullOffset { get; set; }
	}
}