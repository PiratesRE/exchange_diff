using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class HealthThread : ChangePoller
	{
		public HealthThread() : base(true)
		{
		}

		private static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ReplayServiceDiagnosticsTracer;
			}
		}

		public ReplayServiceDiagnostics GetResourceChecker()
		{
			return this.resourceChecker;
		}

		protected override void PollerThread()
		{
			while (!this.m_fShutdown)
			{
				TimeSpan timeout = TimeSpan.FromMilliseconds((double)RegistryParameters.ReplayServiceDiagnosticsIntervalMsec);
				if (this.m_shutdownEvent.WaitOne(timeout))
				{
					HealthThread.Tracer.TraceDebug(0L, "HealthThread noticed Shutdown");
					return;
				}
				this.resourceChecker.RunProcessDiagnostics();
			}
		}

		private ReplayServiceDiagnostics resourceChecker = new ReplayServiceDiagnostics();
	}
}
