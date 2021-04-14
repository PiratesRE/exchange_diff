using System;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal sealed class StageInfo
	{
		public StageInfo(StageHandler handler, LatencyComponent latencyComponent)
		{
			this.Handler = handler;
			this.LatencyComponent = latencyComponent;
		}

		public readonly StageHandler Handler;

		public readonly LatencyComponent LatencyComponent;
	}
}
