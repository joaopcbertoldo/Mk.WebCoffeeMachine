namespace Mkafeina.Server.Domain.Entities.Ingredients
{
	public class Coffee : Ingredient
	{
		public override string Name => GetType().Name;

		public override char Code => 'c';
	}
}