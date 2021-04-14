using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckPassiveCopyTotalQueueLength : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckPassiveCopyTotalQueueLength() : base(DatabaseValidationCheck.ID.DatabaseCheckPassiveCopyTotalQueueLength)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			error = LocalizedString.Empty;
			if (!AmBcsCopyValidation.IsTotalQueueLengthLessThanMaxThreshold(args.DatabaseName, args.Status.CopyStatus, args.TargetServer, ref error))
			{
				return DatabaseValidationCheck.Result.Failed;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
