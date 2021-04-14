using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckPassiveCopyStatusIsOkForRedundancy : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckPassiveCopyStatusIsOkForRedundancy() : base(DatabaseValidationCheck.ID.DatabaseCheckPassiveCopyStatusIsOkForRedundancy)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			RpcDatabaseCopyStatus2 copyStatus = args.Status.CopyStatus;
			CopyStatusEnum copyStatus2 = copyStatus.CopyStatus;
			switch (copyStatus2)
			{
			case CopyStatusEnum.DisconnectedAndHealthy:
			case CopyStatusEnum.Healthy:
				break;
			default:
				if (copyStatus2 != CopyStatusEnum.SeedingSource)
				{
					DatabaseValidationCheck.Tracer.TraceError((long)this.GetHashCode(), "{0}: Passive database copy '{1}' is in state '{2}'. Returning Failed! StatusFetched at {3}", new object[]
					{
						base.CheckName,
						args.DatabaseCopyName,
						copyStatus.CopyStatus,
						copyStatus.StatusRetrievedTime
					});
					error = ReplayStrings.DbValidationPassiveCopyUnhealthyState(args.DatabaseCopyName, copyStatus.CopyStatus.ToString(), string.IsNullOrEmpty(copyStatus.ErrorMessage) ? ReplayStrings.AmBcsNoneSpecified : copyStatus.ErrorMessage, string.IsNullOrEmpty(copyStatus.SuspendComment) ? ReplayStrings.AmBcsNoneSpecified : copyStatus.SuspendComment);
					return DatabaseValidationCheck.Result.Failed;
				}
				break;
			}
			DatabaseValidationCheck.Tracer.TraceDebug((long)this.GetHashCode(), "{0}: Passive database copy '{1}' is in state '{2}'. Returning Passed. StatusFetched at {3}", new object[]
			{
				base.CheckName,
				args.DatabaseCopyName,
				copyStatus.CopyStatus,
				copyStatus.StatusRetrievedTime
			});
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
