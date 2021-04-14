using System;

namespace Microsoft.Exchange.LogUploader
{
	internal abstract class MessageBatchBase : LogDataBatch
	{
		public MessageBatchBase(int batchSizeInBytes, long beginOffSet, string fullLogName, string logPrefix) : base(batchSizeInBytes, beginOffSet, fullLogName, logPrefix)
		{
		}

		internal abstract int MessageBatchFlushInterval { get; set; }

		internal abstract bool Flushed { get; set; }

		internal abstract bool ReadyToFlush(DateTime newestLogLineTS);

		internal abstract bool ContainsMessage(ParsedReadOnlyRow parsedRow);

		internal abstract bool ReachedDalOptimizationLimit();
	}
}
