using System;

namespace Microsoft.Exchange.Connections.Common
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
