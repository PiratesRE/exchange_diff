using System;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Assistants;

namespace Microsoft.Exchange.Assistants
{
	internal sealed class Starter : Base
	{
		public Starter(Init init)
		{
			this.init = init;
		}

		public void Stop()
		{
			this.serviceStoppingEvent.Set();
			if (!this.startupThread.Join(this.waitingThreadTerminationTime))
			{
				this.startupThread.Abort();
			}
		}

		public void Start()
		{
			this.serviceStoppingEvent.Reset();
			this.startupThread = new Thread(new ThreadStart(this.InitCallback));
			this.startupThread.Start();
		}

		private void InitCallback()
		{
			try
			{
				IL_00:
				this.init();
			}
			catch (TransientServerException arg)
			{
				ExTraceGlobals.DatabaseManagerTracer.TraceError<Starter, TransientServerException>((long)this.GetHashCode(), "{0}: Transient failure during startup. Will retry in a 10 seconds. Exception: {1}", this, arg);
				if (!this.serviceStoppingEvent.WaitOne(this.sleepStartingThread))
				{
					goto IL_00;
				}
			}
		}

		private Init init;

		private Thread startupThread;

		private ManualResetEvent serviceStoppingEvent = new ManualResetEvent(false);

		private readonly TimeSpan waitingThreadTerminationTime = TimeSpan.FromSeconds(30.0);

		private readonly TimeSpan sleepStartingThread = TimeSpan.FromSeconds(10.0);
	}
}
