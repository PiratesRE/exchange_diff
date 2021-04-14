using System;

namespace Microsoft.Exchange.Connections.Imap
{
	internal enum ImapCommandType
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
