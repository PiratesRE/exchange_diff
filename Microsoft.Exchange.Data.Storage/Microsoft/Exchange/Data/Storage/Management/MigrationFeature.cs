using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum MigrationFeature
	{
		[LocDescription(ServerStrings.IDs.MigrationFeatureNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationFeatureMultiBatch)]
		MultiBatch = 1,
		[LocDescription(ServerStrings.IDs.MigrationFeatureEndpoints)]
		Endpoints = 2,
		[LocDescription(ServerStrings.IDs.MigrationFeatureUpgradeBlock)]
		UpgradeBlock = 4,
		[LocDescription(ServerStrings.IDs.MigrationFeaturePAW)]
		PAW = 8
	}
}
