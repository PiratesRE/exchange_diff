using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data.Metering.ResourceMonitoring;

namespace Microsoft.Exchange.Transport.ResourceThrottling
{
	internal class ResourceHelper
	{
		public static UseLevel TryGetCurrentUseLevel(IEnumerable<ResourceUse> resourceUses, ResourceIdentifier resource, UseLevel defaultUseLevel = UseLevel.Low)
		{
			if (resourceUses != null)
			{
				ResourceUse resourceUse = ResourceHelper.TryGetResourceUse(resourceUses, resource);
				if (resourceUse != null)
				{
					return resourceUse.CurrentUseLevel;
				}
			}
			return defaultUseLevel;
		}

		public static ResourceUse TryGetResourceUse(IEnumerable<ResourceUse> resourceUses, ResourceIdentifier resource)
		{
			if (resourceUses != null)
			{
				return resourceUses.SingleOrDefault((ResourceUse item) => item.Resource == resource);
			}
			return null;
		}
	}
}
