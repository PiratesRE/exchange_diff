using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Flags]
	[Serializable]
	public enum MigrationType
	{
		[LocDescription(ServerStrings.IDs.MigrationTypeNone)]
		None = 0,
		[LocDescription(ServerStrings.IDs.MigrationTypeIMAP)]
		IMAP = 1,
		[LocDescription(ServerStrings.IDs.MigrationTypeXO1)]
		XO1 = 2,
		[LocDescription(ServerStrings.IDs.MigrationTypeExchangeOutlookAnywhere)]
		ExchangeOutlookAnywhere = 4,
		[LocDescription(ServerStrings.IDs.MigrationTypeBulkProvisioning)]
		BulkProvisioning = 8,
		[LocDescription(ServerStrings.IDs.MigrationTypeExchangeRemoteMove)]
		ExchangeRemoteMove = 16,
		[LocDescription(ServerStrings.IDs.MigrationTypeExchangeLocalMove)]
		ExchangeLocalMove = 32,
		[LocDescription(ServerStrings.IDs.MigrationTypePSTImport)]
		PSTImport = 64,
		[LocDescription(ServerStrings.IDs.MigrationTypePublicFolder)]
		PublicFolder = 128
	}
}
