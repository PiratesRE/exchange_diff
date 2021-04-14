using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Shared
{
	internal class ReplDiagCoreImpl : IDiagCoreImpl
	{
		public ReplDiagCoreImpl()
		{
			this.m_defaultEventSuppressionInterval = TimeSpan.FromSeconds((double)RegistryParameters.CrimsonPeriodicLoggingIntervalInSec);
			this.m_eventLog = new ExEventLog(ExTraceGlobals.ReplayApiTracer.Category, "MSExchangeRepl");
		}

		public TimeSpan DefaultEventSuppressionInterval
		{
			get
			{
				return this.m_defaultEventSuppressionInterval;
			}
		}

		public ExEventLog EventLog
		{
			get
			{
				return this.m_eventLog;
			}
		}

		private readonly TimeSpan m_defaultEventSuppressionInterval;

		private readonly ExEventLog m_eventLog;
	}
}
