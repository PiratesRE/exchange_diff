using System;

namespace Microsoft.Exchange.InfoWorker.Common.Sharing
{
	[Serializable]
	public sealed class SubscriptionNotFoundException : SharingSynchronizationException
	{
		public SubscriptionNotFoundException() : base(Strings.SubscriptionNotFoundException)
		{
		}

		public SubscriptionNotFoundException(Exception innerException) : base(Strings.SubscriptionNotFoundException, innerException)
		{
		}
	}
}
