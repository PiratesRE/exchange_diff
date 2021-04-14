using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;

namespace Microsoft.Exchange.MailboxLoadBalance.Band
{
	internal class BatchSizeReducerFactory
	{
		protected BatchSizeReducerFactory()
		{
		}

		public static BatchSizeReducerFactory Instance
		{
			get
			{
				return BatchSizeReducerFactory.HookableInstance.Value;
			}
		}

		public virtual IBatchSizeReducer GetBatchSizeReducer(ByteQuantifiedSize maximumSize, ByteQuantifiedSize totalSize, ILogger logger)
		{
			switch (LoadBalanceADSettings.Instance.Value.BatchBatchSizeReducer)
			{
			case BatchSizeReducerType.DropLargest:
				return this.CreateDropLargestBatchSizeReducer(maximumSize, logger);
			case BatchSizeReducerType.DropSmallest:
				return this.CreateDropSmallestBatchSizeReducer(maximumSize, logger);
			default:
				return this.GetFactorBasedBatchSizeReducer(maximumSize, totalSize, logger);
			}
		}

		protected virtual IBatchSizeReducer CreateDropLargestBatchSizeReducer(ByteQuantifiedSize targetSize, ILogger logger)
		{
			return new DropLargestEntriesBatchSizeReducer(targetSize, logger);
		}

		protected virtual IBatchSizeReducer CreateDropSmallestBatchSizeReducer(ByteQuantifiedSize targetSize, ILogger logger)
		{
			return new DropSmallestEntriesBatchSizeReducer(targetSize, logger);
		}

		protected virtual IBatchSizeReducer CreateFactorBasedReducer(ILogger logger, double weightReductionFactor)
		{
			return new FactorBasedBatchSizeReducer(weightReductionFactor, logger);
		}

		protected virtual IBatchSizeReducer GetFactorBasedBatchSizeReducer(ByteQuantifiedSize maximumSize, ByteQuantifiedSize totalSize, ILogger logger)
		{
			double weightReductionFactor = maximumSize.ToMB() / totalSize.ToMB();
			return this.CreateFactorBasedReducer(logger, weightReductionFactor);
		}

		protected static readonly Hookable<BatchSizeReducerFactory> HookableInstance = Hookable<BatchSizeReducerFactory>.Create(true, new BatchSizeReducerFactory());
	}
}
