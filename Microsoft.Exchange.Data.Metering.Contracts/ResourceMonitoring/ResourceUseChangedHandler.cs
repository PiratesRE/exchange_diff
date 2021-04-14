using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.Exchange.Data.Metering.ResourceMonitoring
{
	internal delegate Task ResourceUseChangedHandler(IEnumerable<ResourceUse> allResourceUses, IEnumerable<ResourceUse> changedResources, IEnumerable<ResourceUse> rawResources);
}
