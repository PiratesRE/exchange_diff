using System;

namespace Microsoft.Exchange.Management.RecipientTasks
{
	[Flags]
	public enum MailboxRights
	{
		DeleteItem = 65536,
		ReadPermission = 131072,
		ChangePermission = 262144,
		ChangeOwner = 524288,
		FullAccess = 1,
		ExternalAccount = 4,
		SendAs = 2
	}
}
