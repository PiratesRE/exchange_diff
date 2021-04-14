using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseRedundancyDatabaseLevelActiveChecks : DatabaseValidationMultiChecks
	{
		protected override void DefineChecks()
		{
			base.DefineChecks();
			base.AddCheck(new DatabaseCheckActiveMountState());
			base.AddCheck(new DatabaseCheckServerHasTooManyActives());
		}
	}
}
