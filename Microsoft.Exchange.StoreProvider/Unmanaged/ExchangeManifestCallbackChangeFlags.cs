using System;

namespace Microsoft.Mapi.Unmanaged
{
	[Flags]
	internal enum ExchangeManifestCallbackChangeFlags
	{
		None = 0,
		NewMessage = 2048
	}
}
