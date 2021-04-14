using System;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.ActiveManagerServer
{
	internal class AmSystemSwitchover : AmSystemMoveBase
	{
		internal AmSystemSwitchover(AmServerName nodeName, AmDbActionReason reasonCode) : base(nodeName)
		{
			this.m_reasonCode = reasonCode;
		}

		protected override void LogStartupInternal()
		{
			AmTrace.Debug("Starting {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.InitiatingServerSwitchover.Log<AmServerName>(this.m_nodeName);
			ExTraceGlobals.FaultInjectionTracer.TraceTest(4050005309U);
		}

		protected override void LogCompletionInternal()
		{
			AmTrace.Debug("Finished {0} for {1}", new object[]
			{
				base.GetType().Name,
				this.m_nodeName
			});
			ReplayCrimsonEvents.CompletedSwitchover.Log<AmServerName, int>(this.m_nodeName, this.m_moveRequests);
		}

		protected override void RunInternal()
		{
			AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Automatic, this.m_reasonCode, AmDbActionCategory.Move);
			base.MoveDatabases(actionCode);
		}

		protected override void PrepareMoveArguments(ref AmDbMoveArguments moveArgs)
		{
			moveArgs.MountDialOverride = DatabaseMountDialOverride.Lossless;
		}

		private AmDbActionReason m_reasonCode;
	}
}
