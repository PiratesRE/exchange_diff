using System;
using System.Collections.Generic;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.MailboxLoadBalance.Data;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	internal class FactorBasedBatchSizeReducer : IBatchSizeReducer
	{
		public FactorBasedBatchSizeReducer(double factor, ILogger logger)
		{
			AnchorUtil.ThrowOnNullArgument(logger, "logger");
			if (factor > 1.0 || factor < 0.0)
			{
				throw new ArgumentOutOfRangeException("factor", factor, "Factor must be between 0 and 1.");
			}
			this.logger = logger;
			this.factor = factor;
		}

		public IEnumerable<BandMailboxRebalanceData> ReduceBatchSize(IEnumerable<BandMailboxRebalanceData> results)
		{
			this.logger.Log(MigrationEventType.Information, "Reducing rebalance weights by a factor of {0}.", new object[]
			{
				this.factor
			});
			foreach (BandMailboxRebalanceData result in results)
			{
				BandMailboxRebalanceData reducedResult = new BandMailboxRebalanceData(result.SourceDatabase, result.TargetDatabase, new LoadMetricStorage())
				{
					RebalanceBatchName = result.RebalanceBatchName,
					ConstraintSetIdentity = result.ConstraintSetIdentity
				};
				foreach (LoadMetric metric in result.RebalanceInformation.Metrics)
				{
					reducedResult.RebalanceInformation[metric] = (long)((double)result.RebalanceInformation[metric] * this.factor);
				}
				yield return reducedResult;
			}
			yield break;
		}

		private readonly double factor;

		private readonly ILogger logger;
	}
}
