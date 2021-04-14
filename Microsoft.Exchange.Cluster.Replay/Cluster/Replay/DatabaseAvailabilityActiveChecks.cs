using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseAvailabilityActiveChecks : DatabaseValidationMultiChecks
	{
		protected override void DefineChecks()
		{
			base.DefineChecks();
			base.AddCheck(new DatabaseCheckClusterNodeUp());
			base.AddCheck(new DatabaseCheckServerAllowedForActivation());
			base.AddCheck(new DatabaseCheckActiveMountState());
			base.AddCheck(new DatabaseCheckActiveCopyNotActivationSuspended());
		}
	}
}
