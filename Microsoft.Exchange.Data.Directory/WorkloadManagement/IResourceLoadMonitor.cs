using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface IResourceLoadMonitor
	{
		ResourceKey Key { get; }

		DateTime LastUpdateUtc { get; }

		ResourceLoad GetResourceLoad(WorkloadType type, bool raw = false, object optionalData = null);

		ResourceLoad GetResourceLoad(WorkloadClassification classification, bool raw = false, object optionalData = null);
	}
}
