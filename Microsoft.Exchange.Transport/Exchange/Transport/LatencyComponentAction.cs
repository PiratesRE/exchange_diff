using System;

namespace Microsoft.Exchange.Transport
{
	internal enum LatencyComponentAction
	{
		Normal,
		SkipEndToEnd,
		ResetEndToEnd
	}
}
