using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal abstract class ActiveOrPassiveDatabaseCopyValidationCheck : DatabaseValidationCheck
	{
		protected ActiveOrPassiveDatabaseCopyValidationCheck(DatabaseValidationCheck.ID checkId) : base(checkId)
		{
		}

		protected override bool IsPrerequisiteMetForCheck(DatabaseValidationCheck.Arguments args)
		{
			return true;
		}
	}
}
