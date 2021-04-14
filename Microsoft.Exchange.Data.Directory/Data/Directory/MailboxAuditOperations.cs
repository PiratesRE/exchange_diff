using System;

namespace Microsoft.Exchange.Data.Directory
{
	[Flags]
	public enum MailboxAuditOperations
	{
		None = 0,
		Update = 1,
		Copy = 2,
		Move = 4,
		MoveToDeletedItems = 8,
		SoftDelete = 16,
		HardDelete = 32,
		FolderBind = 64,
		SendAs = 128,
		SendOnBehalf = 256,
		MessageBind = 512,
		Create = 1024,
		MailboxLogin = 2048
	}
}
