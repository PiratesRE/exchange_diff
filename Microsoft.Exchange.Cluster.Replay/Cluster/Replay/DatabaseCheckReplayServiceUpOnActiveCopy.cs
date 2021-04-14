using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckReplayServiceUpOnActiveCopy : PassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckReplayServiceUpOnActiveCopy() : base(DatabaseValidationCheck.ID.DatabaseCheckReplayServiceUpOnActiveCopy)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			if (args.IsCopyRemoval)
			{
				DatabaseValidationCheck.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: Validation is being run in copy-removal mode, so skipping DatabaseCheckReplayServiceUpOnActiveCopy check for database copy '{1}'!", base.CheckName, args.DatabaseCopyName);
				return DatabaseValidationCheck.Result.Passed;
			}
			if (args.ActiveStatus == null)
			{
				DatabaseValidationCheck.Tracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: Could not find active copy status result for database copy '{1}'.", base.CheckName, args.DatabaseCopyName);
				error = ReplayStrings.DbValidationActiveCopyStatusUnknown(args.DatabaseName);
				return DatabaseValidationCheck.Result.Warning;
			}
			CopyStatusClientCachedEntry activeStatus = args.ActiveStatus;
			if (!activeStatus.IsActive)
			{
				return DatabaseValidationCheck.Result.Passed;
			}
			if (activeStatus.Result != CopyStatusRpcResult.Success)
			{
				error = ReplayStrings.DbValidationActiveCopyStatusRpcFailed(args.DatabaseName, activeStatus.ServerContacted.NetbiosName, activeStatus.LastException.Message);
				return DatabaseValidationCheck.Result.Warning;
			}
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
