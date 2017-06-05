namespace Mkafeina.Server.Domain.Entities.Ingredients
{
	public class Sugar : Ingredient
	{
		public override string Name => GetType().Name;

		public override char Code => 's';
	}
}