using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckClusterNodeUp : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckClusterNodeUp() : base(DatabaseValidationCheck.ID.DatabaseCheckClusterNodeUp)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = args.Status.CopyStatus;
			if (copyStatus.NodeStatus == NodeUpStatusEnum.Down)
			{
				error = ReplayStrings.AmBcsTargetNodeDownError(args.TargetServer.NetbiosName);
				return DatabaseValidationCheck.Result.Failed;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
