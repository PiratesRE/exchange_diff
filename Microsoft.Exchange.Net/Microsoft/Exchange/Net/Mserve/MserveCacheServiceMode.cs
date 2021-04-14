using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal enum MserveCacheServiceMode
	{
		NotEnabled,
		EnabledWithFallback,
		AlwaysEnabled,
		EnabledForReadOnly
	}
}
