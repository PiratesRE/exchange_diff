using System;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseCheckCopyStatusRpcSuccessful : ActiveOrPassiveDatabaseCopyValidationCheck
	{
		public DatabaseCheckCopyStatusRpcSuccessful() : base(DatabaseValidationCheck.ID.DatabaseCheckCopyStatusRpcSuccessful)
		{
		}

		protected override DatabaseValidationCheck.Result ValidateInternal(DatabaseValidationCheck.Arguments args, ref LocalizedString error)
		{
			if (args.Status.Result != CopyStatusRpcResult.Success)
			{
				error = ReplayStrings.DbValidationCopyStatusRpcFailed(args.DatabaseName, args.TargetServer.NetbiosName, args.Status.LastException.Message);
				return DatabaseValidationCheck.Result.Failed;
			}
			DiagCore.RetailAssert(args.Status.CopyStatus != null, "CopyStatus cannot be null if Result is Success!", new object[0]);
			return DatabaseValidationCheck.Result.Passed;
		}
	}
}
