using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class DatabaseConnectedPassiveChecks : DatabaseValidationMultiChecks
	{
		protected override void DefineChecks()
		{
			base.DefineChecks();
			base.AddCheck(new DatabaseCheckPassiveConnected());
		}
	}
}
