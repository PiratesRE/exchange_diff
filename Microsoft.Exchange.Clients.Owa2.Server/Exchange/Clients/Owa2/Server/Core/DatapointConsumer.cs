using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Flags]
	public enum DatapointConsumer
	{
		None = 0,
		Analytics = 1,
		Inference = 2,
		Diagnostics = 4,
		Watson = 8
	}
}
