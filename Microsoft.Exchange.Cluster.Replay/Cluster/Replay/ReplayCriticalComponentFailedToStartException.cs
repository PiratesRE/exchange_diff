using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class ReplayCriticalComponentFailedToStartException : Exception
	{
		public ReplayCriticalComponentFailedToStartException(string componentName)
		{
			this.ComponentName = componentName;
		}

		public string ComponentName { get; private set; }
	}
}
