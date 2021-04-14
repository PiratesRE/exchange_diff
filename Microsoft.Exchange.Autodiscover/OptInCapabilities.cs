using System;

namespace Microsoft.Exchange.Autodiscover
{
	[Flags]
	internal enum OptInCapabilities
	{
		None = 0,
		Negotiate = 1,
		ExHttpInfo = 2
	}
}
