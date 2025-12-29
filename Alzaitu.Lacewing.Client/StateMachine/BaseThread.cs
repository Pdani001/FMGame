using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Alzaitu.Lacewing.Client.StateMachine
{
	abstract class BaseThread
	{
		private Thread _thread;
		protected BaseThread()
		{
			_thread = new Thread(new ThreadStart(RunThread));
		}
		public void Start() => _thread.Start();
		public void Join() => _thread.Join();
		public bool IsAlive => _thread.IsAlive;

		protected abstract void RunThread();
	}
}
