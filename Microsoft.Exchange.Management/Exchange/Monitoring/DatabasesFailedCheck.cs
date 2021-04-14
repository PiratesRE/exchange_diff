using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class DatabasesFailedCheck : DatabaseCheck
	{
		public DatabasesFailedCheck(string serverName, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses) : base("DBCopyFailed", CheckId.DatabasesFailed, Strings.DatabaseCopyStateCheckDesc(CopyStatus.Failed), eventManager, momEventSource, replayConfigs, copyStatuses, serverName, new uint?(0U))
		{
		}

		protected override bool ShouldCheckConfig(ReplayConfiguration replayconfig)
		{
			bool result = IgnoreTransientErrors.HasPassed(base.GetDefaultErrorKey(typeof(DatabasesSuspendedCheck)));
			if (base.ShouldCheckConfig(replayconfig))
			{
				ExTraceGlobals.HealthChecksTracer.TraceDebug<string, string>((long)this.GetHashCode(), "ShouldCheckConfig(): Config '{0}': Dependent check SuspendCheck passed: {1}.", replayconfig.DisplayName, result.ToString());
				return result;
			}
			return false;
		}

		protected override bool RunIndividualCheck(ReplayConfiguration configToCheck, RpcDatabaseCopyStatus2 copyStatus)
		{
			if ((this.UseReplayRpc() && copyStatus.CopyStatus == CopyStatusEnum.Failed) || (!this.UseReplayRpc() && configToCheck.ReplayState.ConfigBroken))
			{
				string text = this.UseReplayRpc() ? copyStatus.ErrorMessage : configToCheck.ReplayState.ConfigBrokenMessage;
				text = ((!string.IsNullOrEmpty(text)) ? text : Strings.ReplicationCheckBlankMessage);
				base.FailContinue(Strings.DatabaseCopyFailedCheck(new LocalizedReplayConfigType(configToCheck.Type).ToString(), configToCheck.DisplayName, CopyStatus.Failed.ToString(), base.ServerName, text), this.UseReplayRpc() ? copyStatus.ErrorEventId : 0U);
				return false;
			}
			return true;
		}
	}
}
