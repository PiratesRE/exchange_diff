using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum DeleteMessagesFlags
	{
		None = 0,
		ForceHardDelete = 16
	}
}
