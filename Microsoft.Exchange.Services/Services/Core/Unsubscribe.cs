using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class Unsubscribe : NotificationCommandBase<UnsubscribeRequest, ServiceResultNone>
	{
		public Unsubscribe(CallContext callContext, UnsubscribeRequest request) : base(callContext, request)
		{
			this.subscriptionId = base.Request.SubscriptionId;
			ServiceCommandBase.ThrowIfNullOrEmpty(this.subscriptionId, "subscriptionId", "Unsubscribe:PreExecuteCommand");
		}

		internal override IExchangeWebMethodResponse GetResponse()
		{
			UnsubscribeResponse unsubscribeResponse = new UnsubscribeResponse();
			unsubscribeResponse.ProcessServiceResult(base.Result);
			return unsubscribeResponse;
		}

		internal override ServiceResult<ServiceResultNone> Execute()
		{
			SubscriptionBase subscription = Subscriptions.Singleton.Get(this.subscriptionId);
			base.ValidateSubscriptionUpdate(subscription, new Type[]
			{
				typeof(PullSubscription),
				typeof(StreamingSubscription)
			});
			Subscriptions.Singleton.Delete(this.subscriptionId);
			return new ServiceResult<ServiceResultNone>(new ServiceResultNone());
		}

		private string subscriptionId;
	}
}
