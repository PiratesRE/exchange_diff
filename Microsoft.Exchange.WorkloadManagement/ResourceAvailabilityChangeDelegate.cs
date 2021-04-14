using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal delegate void ResourceAvailabilityChangeDelegate(ResourceKey resourceKey, WorkloadClassification classification, bool available);
}
