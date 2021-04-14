using System;

namespace Microsoft.Exchange.Transport
{
	internal sealed class NullResourceMonitor : ResourceMonitor
	{
		public NullResourceMonitor(string displayName) : base(displayName, new ResourceManagerConfiguration.ResourceMonitorConfiguration(10, 8, 6))
		{
		}

		protected override bool GetCurrentReading(out int currentReading)
		{
			currentReading = 0;
			return true;
		}

		private const int HighThreshold = 10;

		private const int MediumThreshold = 8;

		private const int NormalThreshold = 6;

		private const int SafeValue = 0;
	}
}
