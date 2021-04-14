using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.QueueQuota
{
	internal class UsageData
	{
		internal UsageData(TimeSpan historyInterval, TimeSpan historyBucketLength) : this(historyInterval, historyBucketLength, () => DateTime.UtcNow)
		{
		}

		internal UsageData(TimeSpan historyInterval, TimeSpan historyBucketLength, Func<DateTime> currentTimeProvider)
		{
			this.currentTimeProvider = currentTimeProvider;
			this.pastRejectedMessageSubmissionQueueCount = new SlidingTotalCounter(historyInterval, historyBucketLength, currentTimeProvider);
			this.pastRejectedMessageTotalQueueCount = new SlidingTotalCounter(historyInterval, historyBucketLength, currentTimeProvider);
		}

		public DateTime LastUpdateTime { get; private set; }

		public DateTime ThrottlingStartTime
		{
			get
			{
				if (DateTime.Compare(this.submissionThrottlingStartTime, this.totalQueueThrottlingStartTime) < 0)
				{
					return this.submissionThrottlingStartTime;
				}
				return this.totalQueueThrottlingStartTime;
			}
		}

		public bool IsEmpty
		{
			get
			{
				return this.submissionQueueUsage == 0 && this.totalQueueUsage == 0 && this.messagesRejectedDueToSubmissionQueue == 0 && this.pastRejectedMessageSubmissionQueueCount.Sum == 0L && this.messagesRejectedDueToTotalQueue == 0 && this.pastRejectedMessageTotalQueueCount.Sum == 0L;
			}
		}

		public void SetOverQuotaFlags(QueueQuotaResources resources, bool overThreshold, bool overWarning)
		{
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				this.isSubmissionQueueOverQuota = overThreshold;
				this.isSubmissionQueueOverWarning = overWarning;
				if (overThreshold)
				{
					this.submissionThrottlingStartTime = this.currentTimeProvider();
				}
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				this.isTotalQueueSizeOverQuota = overThreshold;
				this.isTotalQueueOverWarning = overWarning;
				if (overThreshold)
				{
					this.totalQueueThrottlingStartTime = this.currentTimeProvider();
				}
			}
		}

		public bool GetIsOverQuota(QueueQuotaResources resources)
		{
			return ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0 && this.isSubmissionQueueOverQuota) || ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0 && this.isTotalQueueSizeOverQuota);
		}

		public bool GetOverWarningFlag(QueueQuotaResources resources)
		{
			return ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0 && this.isSubmissionQueueOverWarning) || ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0 && this.isTotalQueueOverWarning);
		}

		public int GetUsage(QueueQuotaResources resources)
		{
			int num = 0;
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				num += this.submissionQueueUsage;
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				num += this.totalQueueUsage;
			}
			return num;
		}

		public void IncrementUsage(QueueQuotaResources resources)
		{
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				Interlocked.Increment(ref this.submissionQueueUsage);
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				Interlocked.Increment(ref this.totalQueueUsage);
			}
			this.LastUpdateTime = DateTime.UtcNow;
		}

		public void DecrementUsage(QueueQuotaResources resources)
		{
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				Interlocked.Decrement(ref this.submissionQueueUsage);
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				Interlocked.Decrement(ref this.totalQueueUsage);
			}
			this.LastUpdateTime = DateTime.UtcNow;
		}

		public void IncrementRejected(QueueQuotaResources resources)
		{
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				Interlocked.Increment(ref this.messagesRejectedDueToSubmissionQueue);
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				Interlocked.Increment(ref this.messagesRejectedDueToTotalQueue);
			}
		}

		public int GetRejectedCount(QueueQuotaResources resources)
		{
			int num = 0;
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				num += this.messagesRejectedDueToSubmissionQueue + (int)this.pastRejectedMessageSubmissionQueueCount.Sum;
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				num += this.messagesRejectedDueToTotalQueue + (int)this.pastRejectedMessageTotalQueueCount.Sum;
			}
			return num;
		}

		public void ResetThrottledData(QueueQuotaResources resources, out int rejectedCount, out DateTime throttlingStartTime)
		{
			throttlingStartTime = DateTime.MaxValue;
			int num = 0;
			if ((byte)(resources & QueueQuotaResources.SubmissionQueueSize) != 0)
			{
				int num2 = Interlocked.Exchange(ref this.messagesRejectedDueToSubmissionQueue, 0);
				this.pastRejectedMessageSubmissionQueueCount.AddValue((long)num2);
				num += num2;
				throttlingStartTime = this.submissionThrottlingStartTime;
				this.submissionThrottlingStartTime = DateTime.MaxValue;
			}
			if ((byte)(resources & QueueQuotaResources.TotalQueueSize) != 0)
			{
				int num3 = Interlocked.Exchange(ref this.messagesRejectedDueToTotalQueue, 0);
				this.pastRejectedMessageTotalQueueCount.AddValue((long)num3);
				num += num3;
				if (throttlingStartTime != DateTime.MaxValue && DateTime.Compare(this.totalQueueThrottlingStartTime, throttlingStartTime) < 0)
				{
					throttlingStartTime = this.totalQueueThrottlingStartTime;
				}
				this.totalQueueThrottlingStartTime = DateTime.MaxValue;
			}
			rejectedCount = num;
		}

		public virtual void Merge(UsageData source)
		{
			this.MergeValue(ref this.submissionQueueUsage, ref source.submissionQueueUsage);
			this.MergeValue(ref this.totalQueueUsage, ref source.totalQueueUsage);
			this.MergeValue(ref this.messagesRejectedDueToSubmissionQueue, ref source.messagesRejectedDueToSubmissionQueue);
			this.MergeValue(ref this.messagesRejectedDueToTotalQueue, ref source.messagesRejectedDueToTotalQueue);
			this.LastUpdateTime = DateTime.UtcNow;
			this.submissionThrottlingStartTime = new DateTime(Math.Min(this.submissionThrottlingStartTime.Ticks, source.submissionThrottlingStartTime.Ticks));
			this.totalQueueThrottlingStartTime = new DateTime(Math.Min(this.totalQueueThrottlingStartTime.Ticks, source.totalQueueThrottlingStartTime.Ticks));
		}

		private void MergeValue(ref int value, ref int newValue)
		{
			int num;
			do
			{
				num = value;
			}
			while (Interlocked.CompareExchange(ref value, num + newValue, num) != num);
		}

		internal static void Cleanup<KT, VT>(ConcurrentDictionary<KT, VT> dictionary, TimeSpan cleanupInterval) where VT : UsageData
		{
			foreach (KeyValuePair<KT, VT> keyValuePair in dictionary)
			{
				DateTime utcNow = DateTime.UtcNow;
				VT value = keyValuePair.Value;
				if (utcNow - value.LastUpdateTime >= cleanupInterval)
				{
					VT value2 = keyValuePair.Value;
					if (value2.IsEmpty)
					{
						UsageData.SafeRemove<KT, VT>(dictionary, keyValuePair.Key);
						continue;
					}
				}
				OrganizationUsageData organizationUsageData = keyValuePair.Value as OrganizationUsageData;
				if (organizationUsageData != null)
				{
					UsageData.Cleanup<string, UsageData>(organizationUsageData.SenderQuotaDictionary, cleanupInterval);
				}
			}
		}

		private static void SafeRemove<KT, VT>(ConcurrentDictionary<KT, VT> dictionary, KT key) where VT : UsageData
		{
			VT data;
			if (dictionary.TryRemove(key, out data) && !data.IsEmpty)
			{
				UsageData.AddOrMerge<KT, VT>(dictionary, key, data);
			}
		}

		internal static void AddOrMerge<KT, VT>(ConcurrentDictionary<KT, VT> dictionary, KT key, VT data) where VT : UsageData
		{
			VT orAdd = dictionary.GetOrAdd(key, data);
			if (orAdd != data)
			{
				orAdd.Merge(data);
			}
		}

		internal XElement GetUsageElement(string elementName, QueueQuotaResources resource, string id)
		{
			XElement xelement = new XElement(elementName);
			xelement.SetAttributeValue("Id", id);
			xelement.SetAttributeValue("QueueUsage", this.GetUsage(resource));
			xelement.SetAttributeValue("IsOverQuota", this.GetIsOverQuota(resource));
			xelement.SetAttributeValue("RejectedCount", this.GetRejectedCount(resource));
			if ((byte)(resource & QueueQuotaResources.SubmissionQueueSize) != 0 && this.submissionThrottlingStartTime != DateTime.MaxValue)
			{
				xelement.SetAttributeValue("ThrottlingDuration", this.currentTimeProvider().Subtract(this.submissionThrottlingStartTime));
			}
			else if ((byte)(resource & QueueQuotaResources.TotalQueueSize) != 0 && this.totalQueueThrottlingStartTime != DateTime.MaxValue)
			{
				xelement.SetAttributeValue("ThrottlingDuration", this.currentTimeProvider().Subtract(this.totalQueueThrottlingStartTime));
			}
			return xelement;
		}

		private readonly Func<DateTime> currentTimeProvider;

		private int submissionQueueUsage;

		private int totalQueueUsage;

		private bool isSubmissionQueueOverQuota;

		private bool isTotalQueueSizeOverQuota;

		private bool isSubmissionQueueOverWarning;

		private bool isTotalQueueOverWarning;

		private int messagesRejectedDueToSubmissionQueue;

		private int messagesRejectedDueToTotalQueue;

		private SlidingTotalCounter pastRejectedMessageSubmissionQueueCount;

		private SlidingTotalCounter pastRejectedMessageTotalQueueCount;

		private DateTime submissionThrottlingStartTime = DateTime.MaxValue;

		private DateTime totalQueueThrottlingStartTime = DateTime.MaxValue;
	}
}
