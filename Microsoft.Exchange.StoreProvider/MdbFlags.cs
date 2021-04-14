using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum MdbFlags
	{
		Private = 1,
		Public = 2,
		System = 4,
		User = 8
	}
}
