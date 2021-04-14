using System;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckPassiveCopyRealCopyQueueLength : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckPassiveCopyRealCopyQueueLength() : base(DatabaseValidationCheck.ID.DatabaseCheckPassiveCopyRealCopyQueueLength)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			error = LocalizedString.Empty;
			if (!AmBcsCopyValidation.IsRealCopyQueueLengthAcceptable(args.DatabaseName, args.Status.CopyStatus, RegistryParameters.MaxAutoDatabaseMountDial, args.TargetServer, ref error))
			{
				return DatabaseValidationCheck.Result.Failed;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
