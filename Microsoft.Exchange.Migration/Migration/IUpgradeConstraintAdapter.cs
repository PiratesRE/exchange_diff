using System;

namespace Microsoft.Exchange.Migration
{
	internal interface IUpgradeConstraintAdapter
	{
		void AddUpgradeConstraint(IMigrationDataProvider dataProvider, MigrationSession session);

		void AddUpgradeConstraintIfNeeded(IMigrationDataProvider dataProvider, MigrationSession session);

		void MarkUpgradeConstraintForExpiry(IMigrationDataProvider dataProvider, DateTime? expirationDate);
	}
}
