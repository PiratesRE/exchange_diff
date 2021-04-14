using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum ImportDeletionFlags
	{
		None = 0,
		HardDelete = 4
	}
}
