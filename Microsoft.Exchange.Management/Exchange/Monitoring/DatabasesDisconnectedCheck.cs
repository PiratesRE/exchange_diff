using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class DatabasesDisconnectedCheck : DatabaseCheck
	{
		public DatabasesDisconnectedCheck(string serverName, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses, uint ignoreTransientErrorsThreshold) : base("DBDisconnected", CheckId.DatabasesDisconnected, Strings.DatabaseCopyStateCheckDesc(CopyStatus.DisconnectedAndHealthy), eventManager, momEventSource, replayConfigs, copyStatuses, serverName, new uint?(ignoreTransientErrorsThreshold))
		{
		}

		protected override bool ShouldCheckConfig(ReplayConfiguration replayconfig)
		{
			bool flag = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesSuspendedCheck)));
			bool flag2 = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesFailedCheck)));
			if (base.ShouldCheckConfig(replayconfig))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ShouldCheckConfig(): Config '{0}': Dependent checks SuspendCheck  and FailedCheck passed: {1}.", replayconfig.DisplayName, (flag && flag2).ToString());
				return flag && flag2;
			}
			return false;
		}

		protected override bool RunIndividualCheck(ReplayConfiguration configToCheck, RpcDatabaseCopyStatus2 copyStatus)
		{
			if (copyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndHealthy)
			{
				base.FailContinue(Strings.DatabaseCopyDisconnectedCheck(new LocalizedReplayConfigType(configToCheck.Type).ToString(), configToCheck.DisplayName, CopyStatus.DisconnectedAndHealthy.ToString(), copyStatus.MailboxServer));
				return false;
			}
			if (copyStatus.CopyStatus == CopyStatusEnum.DisconnectedAndResynchronizing)
			{
				base.FailContinue(Strings.DatabaseCopyDisconnectedCheck(new LocalizedReplayConfigType(configToCheck.Type).ToString(), configToCheck.DisplayName, CopyStatus.DisconnectedAndResynchronizing.ToString(), copyStatus.MailboxServer));
				return false;
			}
			return true;
		}
	}
}
