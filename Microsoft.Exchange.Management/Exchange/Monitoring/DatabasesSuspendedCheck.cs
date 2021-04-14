using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Management.SystemConfigurationTasks;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Monitoring
{
	internal class DatabasesSuspendedCheck : DatabaseCheck
	{
		public DatabasesSuspendedCheck(string serverName, IEventManager eventManager, string momEventSource, List<ReplayConfiguration> replayConfigs, Dictionary<Guid, RpcDatabaseCopyStatus2> copyStatuses) : base("DBCopySuspended", CheckId.DatabasesSuspended, Strings.DatabaseCopyStateCheckDesc(CopyStatus.Suspended), eventManager, momEventSource, replayConfigs, copyStatuses, serverName, new uint?(0U))
		{
		}

		protected override bool RunIndividualCheck(ReplayConfiguration configToCheck, RpcDatabaseCopyStatus2 copyStatus)
		{
			bool flag = false;
			string text = null;
			CopyStatus copyStatus2 = CopyStatus.Unknown;
			if (copyStatus.CopyStatus == CopyStatusEnum.Suspended)
			{
				copyStatus2 = CopyStatus.Suspended;
				text = copyStatus.SuspendComment;
				flag = true;
			}
			else if (copyStatus.CopyStatus == CopyStatusEnum.FailedAndSuspended)
			{
				copyStatus2 = CopyStatus.FailedAndSuspended;
				text = copyStatus.ErrorMessage;
				flag = true;
			}
			if (flag)
			{
				text = ((!string.IsNullOrEmpty(text)) ? text : Strings.ReplicationCheckBlankMessage);
				base.FailContinue(Strings.DatabaseCopySuspendedCheck(new LocalizedReplayConfigType(configToCheck.Type).ToString(), configToCheck.DisplayName, copyStatus2.ToString(), base.ServerName, text));
				return false;
			}
			return true;
		}
	}
}
