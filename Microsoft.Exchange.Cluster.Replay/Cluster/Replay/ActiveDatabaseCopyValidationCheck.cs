using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ActiveDatabaseCopyValidationCheck : DatabaseValidationCheck
	{
		protected ActiveDatabaseCopyValidationCheck(DatabaseValidationCheck.ID checkId) : base(checkId)
		{
		}

		protected override bool IsPrerequisiteMetForCheck(DatabaseValidationCheck.Arguments args)
		{
			return args.Status.IsActive;
		}
	}
}
