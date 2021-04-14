using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationStage
	{
		[LocDescription(ServerStrings.IDs.MigrationStageDiscovery)]
		Discovery = 16,
		[LocDescription(ServerStrings.IDs.MigrationStageValidation)]
		Validation = 21,
		[LocDescription(ServerStrings.IDs.MigrationStageInjection)]
		Injection = 26,
		[LocDescription(ServerStrings.IDs.MigrationStageProcessing)]
		Processing = 31
	}
}
