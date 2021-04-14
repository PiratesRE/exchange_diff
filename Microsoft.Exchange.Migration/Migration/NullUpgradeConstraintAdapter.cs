using System;

namespace Microsoft.Exchange.Migration
{
	internal class NullUpgradeConstraintAdapter : IUpgradeConstraintAdapter
	{
		public void AddUpgradeConstraint(IMigrationDataProvider dataProvider, MigrationSession session)
		{
		}

		public void AddUpgradeConstraintIfNeeded(IMigrationDataProvider dataProvider, MigrationSession session)
		{
		}

		public void MarkUpgradeConstraintForExpiry(IMigrationDataProvider dataProvider, DateTime? expirationDate)
		{
		}
	}
}
