using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	internal abstract class DropEntriesBatchSizeReducer : IBatchSizeReducer
	{
		protected DropEntriesBatchSizeReducer(ByteQuantifiedSize targetSize, ILogger logger)
		{
			this.targetSize = targetSize;
			this.logger = logger;
		}

		public IEnumerable<BandMailboxRebalanceData> ReduceBatchSize(IEnumerable<BandMailboxRebalanceData> results)
		{
			this.logger.LogInformation("Reducing batch size by dropping entries until size is at or below {0} ({1})", new object[]
			{
				this.targetSize,
				base.GetType().Name
			});
			IEnumerable<BandMailboxRebalanceData> sortedResults = this.SortResults(results);
			ByteQuantifiedSize currentSize = ByteQuantifiedSize.Zero;
			foreach (BandMailboxRebalanceData datum in sortedResults)
			{
				ByteQuantifiedSize datumSize = this.GetRebalanceDatumSize(datum);
				if (currentSize + datumSize > this.targetSize)
				{
					this.logger.LogVerbose("Current selected size is {0}, the next data entry would need {1} which would exceed the maximum. Skipping.", new object[]
					{
						currentSize,
						datumSize
					});
				}
				else
				{
					currentSize += datumSize;
					this.logger.LogVerbose("Selected {0}, total size is now {1}", new object[]
					{
						datumSize,
						currentSize
					});
					yield return datum;
				}
			}
			yield break;
		}

		protected ByteQuantifiedSize GetRebalanceDatumSize(BandMailboxRebalanceData datum)
		{
			return (from metric in datum.RebalanceInformation.Metrics
			where metric.IsSize
			select metric).Aggregate(ByteQuantifiedSize.Zero, (ByteQuantifiedSize current, LoadMetric metric) => current + datum.RebalanceInformation.GetSizeMetric(metric));
		}

		protected abstract IEnumerable<BandMailboxRebalanceData> SortResults(IEnumerable<BandMailboxRebalanceData> results);

		private readonly ILogger logger;

		private readonly ByteQuantifiedSize targetSize;
	}
}
