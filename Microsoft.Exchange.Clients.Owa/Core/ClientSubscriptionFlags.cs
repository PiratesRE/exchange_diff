using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Flags]
	internal enum ClientSubscriptionFlags
	{
		None = 0,
		FolderCount = 1,
		FolderChange = 2,
		NewMail = 4,
		Reminders = 8,
		StaticSearch = 16
	}
}
