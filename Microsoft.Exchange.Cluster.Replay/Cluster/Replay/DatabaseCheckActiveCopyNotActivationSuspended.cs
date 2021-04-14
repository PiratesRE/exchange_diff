using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckActiveCopyNotActivationSuspended : ActiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckActiveCopyNotActivationSuspended() : base(DatabaseValidationCheck.ID.DatabaseCheckActiveCopyNotActivationSuspended)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			AmServerName targetServer = args.TargetServer;
			RpcDatabaseCopyStatus2 copyStatus = args.Status.CopyStatus;
			if (copyStatus.ActivationSuspended)
			{
				error = ReplayStrings.AmBcsDatabaseCopyActivationSuspended(args.DatabaseName, targetServer.NetbiosName, string.IsNullOrEmpty(copyStatus.SuspendComment) ? ReplayStrings.AmBcsNoneSpecified : copyStatus.SuspendComment);
				return DatabaseValidationCheck.Result.Failed;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
