using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckPassiveCopyInspectorQueueLength : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckPassiveCopyInspectorQueueLength() : base(DatabaseValidationCheck.ID.DatabaseCheckPassiveCopyInspectorQueueLength)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = args.Status.CopyStatus;
			long num = Math.Max(0L, copyStatus.LastLogCopied - copyStatus.LastLogInspected);
			if (num > (long)RegistryParameters.DatabaseCheckInspectorQueueLengthFailedThreshold)
			{
				error = ReplayStrings.DbValidationInspectorQueueLengthTooHigh(args.DatabaseCopyName, num, (long)RegistryParameters.DatabaseCheckInspectorQueueLengthFailedThreshold);
				return DatabaseValidationCheck.Result.Failed;
			}
			if (num > (long)RegistryParameters.DatabaseCheckInspectorQueueLengthWarningThreshold)
			{
				error = ReplayStrings.DbValidationInspectorQueueLengthTooHigh(args.DatabaseCopyName, num, (long)RegistryParameters.DatabaseCheckInspectorQueueLengthWarningThreshold);
				return DatabaseValidationCheck.Result.Warning;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
