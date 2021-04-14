using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationStep
	{
		[LocDescription(ServerStrings.IDs.MigrationStepInitialization)]
		Initialization = 2,
		[LocDescription(ServerStrings.IDs.MigrationStepProvisioning)]
		Provisioning = 7,
		[LocDescription(ServerStrings.IDs.MigrationStepProvisioningUpdate)]
		ProvisioningUpdate = 12,
		[LocDescription(ServerStrings.IDs.MigrationStepDataMigration)]
		DataMigration = 17
	}
}
