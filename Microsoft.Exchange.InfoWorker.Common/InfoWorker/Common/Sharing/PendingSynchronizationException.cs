using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class PendingSynchronizationException : SharingSynchronizationException
	{
		public PendingSynchronizationException() : base(Strings.PendingSynchronizationException)
		{
		}

		public PendingSynchronizationException(Exception innerException) : base(Strings.PendingSynchronizationException, innerException)
		{
		}
	}
}
