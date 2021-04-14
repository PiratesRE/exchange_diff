using System;

namespace Microsoft.Mapi
{
	[Flags]
	internal enum GetTableFlags
	{
		None = 0,
		FreeBusy = 2,
		DeferredErrors = 8
	}
}
