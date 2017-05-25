using System;
using System.Threading.Tasks;

namespace Mkafeina.Domain
{
	public abstract class CommandInterpreter
	{
		private Task _keyListenerTask;

		public Task KeyListenerTask {
			get {
				if (_keyListenerTask == null)
					_keyListenerTask = Task.Factory.StartNew(() =>
					{
						while (true)
							HandleCommand(Console.ReadKey(intercept: true));
					});
				return _keyListenerTask;
			}
		}

		public abstract void HandleCommand(ConsoleKeyInfo key);
	}
}