using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	[Flags]
	public enum TraceContextFlags : uint
	{
		None = 0U,
		Passive = 1U
	}
}
