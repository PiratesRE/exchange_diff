using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Microsoft.Exchange.Transport.QueueQuota
{
	internal class OrganizationUsageData : UsageData, IEnumerable<KeyValuePair<string, UsageData>>, IEnumerable
	{
		public OrganizationUsageData(TimeSpan historyInterval, TimeSpan historyBucketLength) : base(historyInterval, historyBucketLength)
		{
		}

		public OrganizationUsageData(Guid key, TimeSpan historyInterval, TimeSpan historyBucketLength, Func<DateTime> currentTimeProvider) : base(historyInterval, historyBucketLength, currentTimeProvider)
		{
			this.key = key;
		}

		public ConcurrentDictionary<string, UsageData> SenderQuotaDictionary
		{
			get
			{
				return this.senderQuotaDictionary;
			}
		}

		public override void Merge(UsageData source)
		{
			base.Merge(source);
			foreach (KeyValuePair<string, UsageData> keyValuePair in ((OrganizationUsageData)source).SenderQuotaDictionary)
			{
				UsageData.AddOrMerge<string, UsageData>(this.SenderQuotaDictionary, keyValuePair.Key, keyValuePair.Value);
			}
		}

		public IEnumerator<KeyValuePair<string, UsageData>> GetEnumerator()
		{
			yield return new KeyValuePair<string, UsageData>(this.key.ToString(), this);
			foreach (KeyValuePair<string, UsageData> pair in this.senderQuotaDictionary)
			{
				yield return pair;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		private readonly ConcurrentDictionary<string, UsageData> senderQuotaDictionary = new ConcurrentDictionary<string, UsageData>(StringComparer.InvariantCultureIgnoreCase);

		private readonly Guid key;
	}
}
