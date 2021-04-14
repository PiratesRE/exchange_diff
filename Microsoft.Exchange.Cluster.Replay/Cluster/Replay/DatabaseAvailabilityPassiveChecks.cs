using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAvailabilityPassiveChecks : DatabaseValidationMultiChecks
	{
		protected override void DefineChecks()
		{
			base.DefineChecks();
			base.AddCheck(new DatabaseCheckClusterNodeUp());
			base.AddCheck(new DatabaseCheckServerAllowedForActivation());
			base.AddCheck(new DatabaseCheckPassiveCopyStatusIsOkForAvailability());
			base.AddCheck(new DatabaseCheckPassiveCopyTotalQueueLength());
			base.AddCheck(new DatabaseCheckPassiveCopyRealCopyQueueLength());
		}
	}
}
