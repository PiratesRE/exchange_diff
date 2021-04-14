using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ImportMessageChangeFlags
	{
		None = 0,
		Associated = 16,
		FailOnConflict = 64,
		NewMessage = 2048
	}
}
