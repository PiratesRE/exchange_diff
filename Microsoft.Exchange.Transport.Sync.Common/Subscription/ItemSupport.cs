using System;

namespace Microsoft.Exchange.Transport.Sync.Common.Subscription
{
	[Flags]
	internal enum ItemSupport
	{
		None = 0,
		Email = 1,
		Contacts = 2,
		Generic = 32
	}
}
