using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class BatchSynchronizationFailedException : SharingSynchronizationException
	{
		public BatchSynchronizationFailedException() : base(Strings.BatchSynchronizationFailedException)
		{
		}

		public BatchSynchronizationFailedException(Exception innerException) : base(Strings.BatchSynchronizationFailedException, innerException)
		{
		}
	}
}
