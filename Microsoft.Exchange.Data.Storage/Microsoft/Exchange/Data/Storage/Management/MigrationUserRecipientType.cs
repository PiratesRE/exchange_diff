using System;

namespace Microsoft.Exchange.Data.Storage.Management
{
	[Serializable]
	public enum MigrationUserRecipientType
	{
		Mailbox,
		Contact,
		Group,
		PublicFolder,
		Unsupported,
		Mailuser,
		MailboxOrMailuser
	}
}
