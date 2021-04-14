using System;

namespace Microsoft.Exchange.Data
{
	[Flags]
	public enum TenantConnectorSource
	{
		[LocDescription(DataStrings.IDs.ConnectorSourceDefault)]
		Default = 0,
		[LocDescription(DataStrings.IDs.ConnectorSourceMigrated)]
		Migrated = 1,
		[LocDescription(DataStrings.IDs.ConnectorSourceHybridWizard)]
		HybridWizard = 2
	}
}
