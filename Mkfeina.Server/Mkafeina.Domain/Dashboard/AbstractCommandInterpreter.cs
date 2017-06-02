using System;

namespace Mkafeina.Domain.Dashboard
{
	public abstract class AbstractCommandInterpreter
	{
		public abstract void HandleCommand(ConsoleKeyInfo key);
	}
}