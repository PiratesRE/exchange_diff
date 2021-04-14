using System;
using Microsoft.Exchange.Diagnostics.Components.Services;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class PullSubscription : SubscriptionBase
	{
		public PullSubscription(PullSubscriptionRequest subscriptionRequest, IdAndSession[] folderIds, Guid subscriptionOwnerObjectGuid) : base(subscriptionRequest, folderIds, subscriptionOwnerObjectGuid)
		{
			this.timeOutMinutes = (double)subscriptionRequest.Timeout;
			this.SetExpirationDateTime();
		}

		private void SetExpirationDateTime()
		{
			this.expirationDateTime = ExDateTime.Now.AddMinutes(this.timeOutMinutes);
		}

		public override EwsNotificationType GetEvents(string theLastWatermarkSent)
		{
			RequestDetailsLogger.Current.AppendGenericInfo("SubscriptionType", "Pull");
			EwsNotificationType result;
			lock (this.lockObject)
			{
				if (this.isExpired)
				{
					ExTraceGlobals.SubscriptionBaseTracer.TraceDebug<int>((long)this.GetHashCode(), "SubscriptionBase.GetEvents. ExpiredSubscriptionException. Hashcode: {0}.", this.GetHashCode());
					throw new ExpiredSubscriptionException();
				}
				EwsNotificationType events = base.GetEvents(theLastWatermarkSent);
				this.SetExpirationDateTime();
				result = events;
			}
			return result;
		}

		public override bool IsExpired
		{
			get
			{
				if (!this.isExpired)
				{
					this.isExpired = (this.expirationDateTime < ExDateTime.Now);
				}
				return this.isExpired;
			}
		}

		protected override int EventQueueSize
		{
			get
			{
				return 50;
			}
		}

		private const int MaximumTimeOutMinutes = 1440;

		private const int MinimumTimeOutMinutes = 1;

		private const int PullEventQueueSize = 50;

		private bool isExpired;

		private double timeOutMinutes;

		private ExDateTime expirationDateTime;
	}
}
