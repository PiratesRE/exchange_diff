using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ResourceLevelMediatorDiagnosticsData
	{
		public TimeSpan GetResourceObserverCallDuration(string resource)
		{
			if (this.resourceObserverCallDurations.ContainsKey(resource))
			{
				return this.resourceObserverCallDurations[resource];
			}
			return TimeSpan.Zero;
		}

		public void SetResourceObserverCallDuration(string resource, TimeSpan callDuration)
		{
			this.resourceObserverCallDurations[resource] = callDuration;
		}

		private readonly Dictionary<string, TimeSpan> resourceObserverCallDurations = new Dictionary<string, TimeSpan>();
	}
}
