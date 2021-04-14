using System;

namespace Microsoft.Exchange.Server.Storage.PhysicalAccess
{
	[Flags]
	public enum TableAccessHints
	{
		None = 0,
		ForwardScan = 1
	}
}
