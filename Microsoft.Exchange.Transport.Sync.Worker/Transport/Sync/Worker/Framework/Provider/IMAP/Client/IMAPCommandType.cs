using System;

namespace Microsoft.Exchange.Transport.Sync.Worker.Framework.Provider.IMAP.Client
{
	internal enum IMAPCommandType
	{
		None,
		Login,
		Logout,
		Select,
		Fetch,
		Append,
		Noop,
		Search,
		List,
		CreateMailbox,
		DeleteMailbox,
		RenameMailbox,
		Capability,
		Store,
		Expunge,
		Id,
		Starttls,
		Authenticate,
		Status
	}
}
