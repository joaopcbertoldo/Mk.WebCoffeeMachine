namespace Mkafeina.Server.Domain
{
	public class CMProxyOffsets
	{
		public int AdjustSignal(float signal, string name)
		{
			var empty = (float)GetType().GetField(name + "EmptyOffset").GetValue(this);
			var full = (float)GetType().GetField(name + "FullOffset").GetValue(this);
			return (int)((signal - empty) / (full - empty) * 100);
		}

		public float CoffeeEmptyOffset;

		public float CoffeeFullOffset;

		public float WaterEmptyOffset;

		public float WaterFullOffset;

		public float MilkEmptyOffset;

		public float MilkFullOffset;

		public float SugarEmptyOffset;

		public float SugarFullOffset;
	}
}