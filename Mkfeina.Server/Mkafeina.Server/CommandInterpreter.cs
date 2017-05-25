using Mkafeina.Domain;
using System;
using System.Threading.Tasks;

namespace Mkafeina.Server
{
	public class CommandInterpreter : Mkafeina.Domain.CommandInterpreter
	{
		public override void HandleCommand(ConsoleKeyInfo key)
		{
			Task.Factory.StartNew(() =>
			{
				switch (key.Key)
				{
					case ConsoleKey.F5:
						AppConfig.Sgt.ReloadConfigs();
						Dashboard.Sgt.ReloadAllPanelsAsync(AppConfig.Sgt.PanelsConfigs);
						CookBook.Sgt.LoadRecipes();
						break;

					case ConsoleKey.F4:
						CookBook.Sgt.LoadRecipes();
						break;

					default:
						return;
				}
			});
		}
	}
}