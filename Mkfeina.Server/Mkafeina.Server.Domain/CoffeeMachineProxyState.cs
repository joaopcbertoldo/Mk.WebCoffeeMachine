namespace Mkfeina.Server.Domain
{
	internal class CoffeeMachineProxyState
	{
		internal string UniqueName;

		internal bool IsMakingCoffee;

		internal int CoffeeLevel; // voltage

		internal int WaterLevel; // voltage

		internal int MilkLevel; // voltage

		internal int SugarLevel; // voltage

		internal bool RegistrationIsAccepted;

		internal bool IsEnabled;

		internal string Mac;
	}
}