using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseRedundancyDatabaseLevelPassiveChecks : DatabaseValidationMultiChecks
	{
		protected override void DefineChecks()
		{
			base.DefineChecks();
			base.AddCheck(new DatabaseCheckPassiveCopyStatusIsOkForRedundancy());
			base.AddCheck(new DatabaseCheckPassiveCopyRealCopyQueueLength());
			base.AddCheck(new DatabaseCheckPassiveCopyInspectorQueueLength());
			base.AddCheck(new DatabaseCheckReplayServiceUpOnActiveCopy());
			base.AddCheck(new DatabaseCheckServerHasTooManyActives());
		}
	}
}
