using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	internal class SenderRateTracker
	{
		public SenderRateTracker(TimeSpan slidingWindowLength, TimeSpan bucketLength)
		{
			this.slidingWindowLength = slidingWindowLength;
			this.bucketLength = bucketLength;
		}

		public long IncrementSenderRate(Guid senderMailbox, DateTime messageCreateTime)
		{
			HistoricalSlidingTotalCounter historicalSlidingTotalCounter = null;
			try
			{
				this.readerWriterLock.EnterReadLock();
				this.dictionary.TryGetValue(senderMailbox, out historicalSlidingTotalCounter);
			}
			finally
			{
				this.readerWriterLock.ExitReadLock();
			}
			if (historicalSlidingTotalCounter == null)
			{
				try
				{
					this.readerWriterLock.EnterWriteLock();
					if (!this.dictionary.TryGetValue(senderMailbox, out historicalSlidingTotalCounter))
					{
						historicalSlidingTotalCounter = new HistoricalSlidingTotalCounter(this.slidingWindowLength, this.bucketLength, messageCreateTime);
						this.dictionary[senderMailbox] = historicalSlidingTotalCounter;
					}
				}
				finally
				{
					this.readerWriterLock.ExitWriteLock();
				}
			}
			return historicalSlidingTotalCounter.AddValue(1L, messageCreateTime);
		}

		public void ResetSenderRate(Guid senderMailbox, DateTime messageCreateTime)
		{
			try
			{
				this.readerWriterLock.EnterWriteLock();
				this.dictionary[senderMailbox] = new HistoricalSlidingTotalCounter(this.slidingWindowLength, this.bucketLength, messageCreateTime);
			}
			finally
			{
				this.readerWriterLock.ExitWriteLock();
			}
		}

		private readonly Dictionary<Guid, HistoricalSlidingTotalCounter> dictionary = new Dictionary<Guid, HistoricalSlidingTotalCounter>();

		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

		private readonly TimeSpan slidingWindowLength;

		private readonly TimeSpan bucketLength;
	}
}
