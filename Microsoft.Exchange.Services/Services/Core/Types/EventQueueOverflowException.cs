using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal sealed class EventQueueOverflowException : ServicePermanentException
	{
		public EventQueueOverflowException() : base(CoreResources.IDs.ErrorMissedNotificationEvents)
		{
			this.Data[StreamingConnection.IsNonFatalSubscriptionExceptionKey] = bool.TrueString;
		}

		internal override ExchangeVersion EffectiveVersion
		{
			get
			{
				return ExchangeVersion.Exchange2010SP1;
			}
		}
	}
}
