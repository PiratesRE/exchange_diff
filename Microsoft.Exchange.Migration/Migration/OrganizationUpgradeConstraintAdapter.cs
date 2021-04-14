using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Migration.Logging;

namespace Microsoft.Exchange.Migration
{
	internal class OrganizationUpgradeConstraintAdapter : IUpgradeConstraintAdapter
	{
		public void AddUpgradeConstraintIfNeeded(IMigrationDataProvider dataProvider, MigrationSession session)
		{
			TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MigrationUpgradeConstraintEnforcementPeriod");
			if (ExDateTime.UtcNow - session.LastUpgradeConstraintEnforcedTimestamp < config)
			{
				MigrationLogger.Log(MigrationEventType.Verbose, "Constraint was checked and enforced on {0}, which means it's below the enforcement period of {1}, no need to do anything else.", new object[]
				{
					session.LastUpgradeConstraintEnforcedTimestamp,
					config
				});
				return;
			}
			this.AddUpgradeConstraint(dataProvider, session);
		}

		public void AddUpgradeConstraint(IMigrationDataProvider dataProvider, MigrationSession session)
		{
			UpgradeConstraint constraint = new UpgradeConstraint("MigrationService", "Organization has migration batches and can't be upgraded.");
			dataProvider.ADProvider.UpdateMigrationUpgradeConstraint(constraint);
			session.SetLastUpdateConstraintEnforcedTimestamp(dataProvider, ExDateTime.UtcNow);
		}

		public void MarkUpgradeConstraintForExpiry(IMigrationDataProvider dataProvider, DateTime? expirationDate)
		{
			if (expirationDate == null)
			{
				TimeSpan config = ConfigBase<MigrationServiceConfigSchema>.GetConfig<TimeSpan>("MigrationUpgradeConstraintExpirationPeriod");
				expirationDate = new DateTime?(DateTime.UtcNow.Date.Add(config));
			}
			UpgradeConstraint constraint = new UpgradeConstraint("MigrationService", "Organization has migration batches and can't be upgraded.", expirationDate.Value);
			dataProvider.ADProvider.UpdateMigrationUpgradeConstraint(constraint);
		}

		internal const string MigrationConstraintName = "MigrationService";

		internal const string OrganizationHasActiveBatches = "Organization has migration batches and can't be upgraded.";
	}
}
