using System;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	[Flags]
	internal enum MailboxServerRoleFlags
	{
		None = 0,
		MAPIEncryptionRequired = 1,
		ExpirationAuditLogEnabled = 2,
		AutocopyAuditLogEnabled = 4,
		FolderAuditLogEnabled = 8,
		ElcSubjectLoggingEnabled = 16
	}
}
