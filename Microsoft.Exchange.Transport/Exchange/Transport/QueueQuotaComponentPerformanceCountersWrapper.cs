using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport
{
	internal class QueueQuotaComponentPerformanceCountersWrapper : IQueueQuotaComponentPerformanceCounters
	{
		public QueueQuotaComponentPerformanceCountersWrapper(TimeSpan rejectedWindow, TimeSpan rejectBucketSize)
		{
			this.rejectedWindow = rejectedWindow;
			this.rejectBucketSize = rejectBucketSize;
			this.rejectedRecently = new SlidingTotalCounter[Enum.GetNames(typeof(QueueQuotaEntity)).Length + 3];
			this.rejectedTotalIndex = this.rejectedRecently.Length - 1;
			this.rejectedSafeTenantIndex = this.rejectedRecently.Length - 2;
			this.rejectedOutlookTenantIndex = this.rejectedRecently.Length - 3;
		}

		public long IncrementThrottledEntities(QueueQuotaEntity entity, Guid organizationId)
		{
			return this.GetInstance(new QueueQuotaEntity?(entity), new Guid?(organizationId)).EntitiesInThrottledState.Increment();
		}

		public long DecrementThrottledEntities(QueueQuotaEntity entity, Guid organizationId)
		{
			return this.GetInstance(new QueueQuotaEntity?(entity), new Guid?(organizationId)).EntitiesInThrottledState.Decrement();
		}

		public long IncrementMessagesRejected(QueueQuotaEntity? entity = null, Guid? organizationId = null)
		{
			int num;
			QueueQuotaComponentPerfCountersInstance instance = this.GetInstance(entity, organizationId, out num);
			if (this.rejectedRecently[num] == null)
			{
				lock (this.syncObject)
				{
					if (this.rejectedRecently[num] == null)
					{
						this.rejectedRecently[num] = new SlidingTotalCounter(this.rejectedWindow, this.rejectBucketSize);
					}
				}
			}
			instance.MessagesTempRejectedRecently.RawValue = this.rejectedRecently[num].AddValue(1L);
			return instance.MessagesTempRejectedTotal.Increment();
		}

		public void UpdateOldestThrottledEntity(QueueQuotaEntity entity, TimeSpan throttledInterval, Guid organizationId)
		{
			this.GetInstance(new QueueQuotaEntity?(entity), new Guid?(organizationId)).OldestThrottledEntityIntervalInSeconds.RawValue = (long)throttledInterval.TotalSeconds;
		}

		public void Refresh(QueueQuotaEntity? entity = null)
		{
			int num;
			QueueQuotaComponentPerfCountersInstance instance = this.GetInstance(entity, new Guid?(Guid.Empty), out num);
			SlidingTotalCounter slidingTotalCounter = this.rejectedRecently[num];
			if (slidingTotalCounter != null)
			{
				instance.MessagesTempRejectedRecently.RawValue = slidingTotalCounter.Sum;
			}
			if (entity == QueueQuotaEntity.Organization)
			{
				instance = this.GetInstance(entity, new Guid?(MultiTenantTransport.SafeTenantId), out num);
				slidingTotalCounter = this.rejectedRecently[num];
				if (slidingTotalCounter != null)
				{
					instance.MessagesTempRejectedRecently.RawValue = slidingTotalCounter.Sum;
				}
			}
		}

		private QueueQuotaComponentPerfCountersInstance GetInstance(QueueQuotaEntity? entity, Guid? organizationId)
		{
			int num;
			return this.GetInstance(entity, organizationId, out num);
		}

		private QueueQuotaComponentPerfCountersInstance GetInstance(QueueQuotaEntity? entity, Guid? organizationId, out int entityIndex)
		{
			QueueQuotaComponentPerfCountersInstance result;
			if (entity == null)
			{
				result = QueueQuotaComponentPerfCounters.TotalInstance;
				entityIndex = this.rejectedTotalIndex;
			}
			else if (entity == QueueQuotaEntity.Organization && organizationId == MultiTenantTransport.SafeTenantId)
			{
				result = QueueQuotaComponentPerfCounters.GetInstance("SafeTenantOrg");
				entityIndex = this.rejectedSafeTenantIndex;
			}
			else if (entity == QueueQuotaEntity.Organization && organizationId == TemplateTenantConfiguration.TemplateTenantExternalDirectoryOrganizationIdGuid)
			{
				result = QueueQuotaComponentPerfCounters.GetInstance("OutlookTenantOrg");
				entityIndex = this.rejectedOutlookTenantIndex;
			}
			else
			{
				result = QueueQuotaComponentPerfCounters.GetInstance(entity.ToString());
				entityIndex = (int)entity.Value;
			}
			return result;
		}

		private const string SafeTenantOrgInstance = "SafeTenantOrg";

		private const string OutlookTenantOrgInstance = "OutlookTenantOrg";

		private readonly TimeSpan rejectedWindow;

		private readonly TimeSpan rejectBucketSize;

		private readonly int rejectedTotalIndex;

		private readonly int rejectedSafeTenantIndex;

		private readonly int rejectedOutlookTenantIndex;

		private SlidingTotalCounter[] rejectedRecently;

		private object syncObject = new object();
	}
}
