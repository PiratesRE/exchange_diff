using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class BadItemCounter
	{
		internal BadItemCounter(bool categorize)
		{
			this.categorize = categorize;
			this.classifier = new BadItemClassifier();
		}

		internal bool Count(string categoryName)
		{
			if (!this.categorize)
			{
				return true;
			}
			if (categoryName == null)
			{
				categoryName = string.Empty;
			}
			BadItemCounter.BadItemCount badItemCount;
			if (!this.counters.TryGetValue(categoryName, out badItemCount))
			{
				int limit = this.classifier.GetLimit(categoryName);
				badItemCount = new BadItemCounter.BadItemCount(limit);
				this.counters.Add(categoryName, badItemCount);
			}
			return badItemCount.Count();
		}

		private readonly bool categorize;

		private readonly BadItemClassifier classifier;

		private Dictionary<string, BadItemCounter.BadItemCount> counters = new Dictionary<string, BadItemCounter.BadItemCount>();

		private class BadItemCount
		{
			public BadItemCount(Unlimited<int> limit)
			{
				this.limit = limit;
				this.count = 0;
			}

			public bool Count()
			{
				this.count++;
				return this.count > this.limit;
			}

			private Unlimited<int> limit;

			private int count;
		}
	}
}
