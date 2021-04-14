using System;

namespace Microsoft.Exchange.Transport
{
	internal interface IQueueQuotaComponentPerformanceCounters
	{
		long IncrementThrottledEntities(QueueQuotaEntity entity, Guid organizationId);

		long DecrementThrottledEntities(QueueQuotaEntity entity, Guid organizationId);

		long IncrementMessagesRejected(QueueQuotaEntity? entity = null, Guid? organizationId = null);

		void UpdateOldestThrottledEntity(QueueQuotaEntity entity, TimeSpan throttledInterval, Guid organizationId);

		void Refresh(QueueQuotaEntity? entity);
	}
}
