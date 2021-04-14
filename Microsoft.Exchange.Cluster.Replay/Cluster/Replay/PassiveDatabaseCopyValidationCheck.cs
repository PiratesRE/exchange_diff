using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class PassiveDatabaseCopyValidationCheck : DatabaseValidationCheck
	{
		protected PassiveDatabaseCopyValidationCheck(DatabaseValidationCheck.ID checkId) : base(checkId)
		{
		}

		protected override bool IsPrerequisiteMetForCheck(DatabaseValidationCheck.Arguments args)
		{
			return !args.Status.IsActive;
		}
	}
}
