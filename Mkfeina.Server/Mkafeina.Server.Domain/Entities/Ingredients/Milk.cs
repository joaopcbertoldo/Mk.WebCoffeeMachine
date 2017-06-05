namespace Mkafeina.Server.Domain.Entities.Ingredients
{
	public class Milk : Ingredient
	{
		public override string Name => GetType().Name;

		public override char Code => 'm';
	}
}