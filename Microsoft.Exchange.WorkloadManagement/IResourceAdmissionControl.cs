using System;
using Microsoft.Exchange.Data.Directory.ResourceHealth;

namespace Microsoft.Exchange.WorkloadManagement
{
	internal interface IResourceAdmissionControl
	{
		ResourceKey ResourceKey { get; }

		bool IsAcquired { get; }

		bool TryAcquire(WorkloadClassification classification, out double delayRatio);

		void Release(WorkloadClassification classification);
	}
}
