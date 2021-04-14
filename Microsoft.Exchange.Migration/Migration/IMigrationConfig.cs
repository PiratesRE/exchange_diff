using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage.Management;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Migration
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IMigrationConfig
	{
		long Version { get; }

		long MinimumSupportedVersion { get; }

		long MaximumSupportedVersion { get; }

		long CurrentSupportedVersion { get; }

		long SupportedVersionUpgrade { get; }

		int MaxNumberOfBatches { get; }

		bool IsSupported(MigrationFeature features);

		bool IsDisplaySupported(MigrationFeature features);

		void CheckFeaturesAvailableToUpgrade(MigrationFeature features);

		bool EnableFeatures(IMigrationDataProvider dataProvider, MigrationFeature features);

		void DisableFeatures(IMigrationDataProvider dataProvider, MigrationFeature features, bool force);

		void CheckAndUpgradeToSupportedFeaturesAndVersion(IMigrationDataProvider dataProvider);

		bool CanCreateNewJobOfType(MigrationType migrationType, bool isStaged, out LocalizedException exception);
	}
}
