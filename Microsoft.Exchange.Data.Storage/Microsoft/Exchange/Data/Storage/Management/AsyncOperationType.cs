using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	public enum AsyncOperationType
	{
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeUnknown)]
		Unknown,
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeImportPST)]
		ImportPST = 3,
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeExportPST)]
		ExportPST,
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeMigration)]
		Migration = 6,
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeMailboxRestore)]
		MailboxRestore = 8,
		[LocDescription(ServerStrings.IDs.AsyncOperationTypeCertExpiry)]
		CertExpiry
	}
}
