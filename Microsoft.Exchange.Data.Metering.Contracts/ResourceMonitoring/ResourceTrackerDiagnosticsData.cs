using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal class ResourceTrackerDiagnosticsData
	{
		public TimeSpan GetResourceMeterCallDuration(ResourceIdentifier resource)
		{
			if (this.resourceMeterCallDurations.ContainsKey(resource))
			{
				return this.resourceMeterCallDurations[resource];
			}
			return TimeSpan.Zero;
		}

		public void SetResourceMeterCallDuration(ResourceIdentifier resource, TimeSpan callDuration)
		{
			this.resourceMeterCallDurations[resource] = callDuration;
		}

		private readonly Dictionary<ResourceIdentifier, TimeSpan> resourceMeterCallDurations = new Dictionary<ResourceIdentifier, TimeSpan>();
	}
}
