namespace Mkafeina.Server.Domain.Entities.Ingredients
{
	public class Water : Ingredient
	{
		public override string Name => GetType().Name;

		public override char Code => 'w';
	}
}