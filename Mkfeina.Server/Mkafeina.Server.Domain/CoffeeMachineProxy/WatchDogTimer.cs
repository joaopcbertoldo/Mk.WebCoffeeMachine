using System;
using System.Threading;
using System.Threading.Tasks;

namespace Mkafeina.Server.Domain.CoffeeMachineProxy
{
	internal class WatchDogTimer
	{
		internal DateTime _lastReset;
		private Task _watcher;
		private Action _callback;
		private CancellationTokenSource _tokenSource;

		internal TimeSpan LimitTime { get; set; }

		public WatchDogTimer(Action callback)
		{
			_callback = callback;
		}

		internal void Start()
		{
			if (_watcher != null && _watcher.Status == TaskStatus.Running)
				_tokenSource.Cancel();

			_tokenSource = new CancellationTokenSource();

			_watcher = Task.Factory.StartNew(() =>
			{
				_lastReset = DateTime.UtcNow;
				while (DateTime.UtcNow - _lastReset < LimitTime)
				{
					Thread.Sleep(1000);
					if (_tokenSource.Token.IsCancellationRequested)
						return;
				}
				_callback.Invoke();
			}, _tokenSource.Token);
		}

		internal void Reset()
		{
			_lastReset = DateTime.UtcNow;
		}

		internal void Stop()
		{
			_tokenSource?.Cancel();
			_watcher = null;
		}
	}
}