using System;

namespace Microsoft.Mapi.Unmanaged
{
	[Flags]
	internal enum ExchangeManifestCallbackDeleteFlags
	{
		SoftDelete = 1,
		Expiry = 2
	}
}
