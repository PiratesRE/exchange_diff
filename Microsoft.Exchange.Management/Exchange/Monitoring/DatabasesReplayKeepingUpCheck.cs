using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class DatabasesReplayKeepingUpCheck : DatabaseCheck
	{
		public DatabasesReplayKeepingUpCheck(string serverName, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, uint ignoreTransientErrorsThreshold) : base("DBLogReplayKeepingUp", CheckId.DatabasesReplayKeepingUp, Strings.DatabasesReplayKeepingUpCheckDesc, eventManager, momEventSource, replayConfigs, copyStatuses, serverName, new uint?(ignoreTransientErrorsThreshold))
		{
		}

		protected override bool ShouldCheckConfig(ReplayConfiguration replayconfig)
		{
			bool flag = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesSuspendedCheck)));
			bool flag2 = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesFailedCheck)));
			bool flag3 = flag && flag2;
			if (this.UseReplayRpc())
			{
				bool flag4 = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesInitializingCheck)));
				flag3 = (flag3 && flag4);
			}
			if (base.ShouldCheckConfig(replayconfig))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string, string>((long)this.GetHashCode(), "ShouldCheckConfig(): Config '{0}': Dependent checks SuspendCheck  and FailedCheck {2} passed: {1}.", replayconfig.DisplayName, flag3.ToString(), this.UseReplayRpc() ? "and InitializingCheck" : string.Empty);
				return flag3;
			}
			return false;
		}

		protected override bool RunIndividualCheck(ReplayConfiguration configToCheck, RpcDatabaseCopyStatus2 copyStatus)
		{
			long replayQueueLength = copyStatus.GetReplayQueueLength();
			if (copyStatus.ReplayQueueNotKeepingUp)
			{
				base.FailContinue(Strings.DatabaseReplayQueueNotKeepingUp(configToCheck.DisplayName, base.ServerName, replayQueueLength));
				return false;
			}
			return true;
		}
	}
}
