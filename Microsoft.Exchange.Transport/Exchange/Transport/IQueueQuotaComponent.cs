using System;
using Microsoft.Exchange.Data.Metering;
using Microsoft.Exchange.Data.Metering.Throttling;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaComponent : ITransportComponent
	{
		void SetRunTimeDependencies(IQueueQuotaConfig config, IFlowControlLog log, IQueueQuotaComponentPerformanceCounters perfCounters, IProcessingQuotaComponent processingQuotaComponent, IQueueQuotaObservableComponent submissionQueue, IQueueQuotaObservableComponent deliveryQueue, ICountTracker<MeteredEntity, MeteredCount> metering);

		void TrackEnteringQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resource);

		void TrackExitingQueue(IQueueQuotaMailItem mailItem, QueueQuotaResources resource);

		bool IsOrganizationOverQuota(string accountForest, Guid externalOrganizationId, string sender, out string reason);

		bool IsOrganizationOverWarning(string accountForest, Guid externalOrganizationId, string sender, QueueQuotaResources resource);

		void TimedUpdate();
	}
}
