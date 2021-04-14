using System;

namespace Microsoft.Exchange.Provisioning.LoadBalancing
{
	internal sealed class LoadBalancingReport
	{
		public override string ToString()
		{
			return string.Format("Load balancing report: enabled databases with local copy - {0}, local databases count - {1}, remote databases count - {2}, databases selected for load balancing - {3}, databases excluded from initial provisioning - {4}.", new object[]
			{
				this.enabledDatabasesWithLocalCopyCount,
				this.firstPreferenceDatabasesCount,
				this.secondPreferenceDatabasesCount,
				this.databasesAndLocationCount,
				this.databasesExcludedFromInitialProvisioning
			});
		}

		public int enabledDatabasesWithLocalCopyCount;

		public int firstPreferenceDatabasesCount;

		public int secondPreferenceDatabasesCount;

		public int databasesAndLocationCount;

		public int databasesExcludedFromInitialProvisioning;
	}
}
