using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal abstract class NotificationCommandBase<RequestType, SingleItemType> : SingleStepServiceCommand<RequestType, SingleItemType> where RequestType : BaseRequest
	{
		public NotificationCommandBase(CallContext callContext, RequestType request) : base(callContext, request)
		{
		}

		protected internal void ValidateSubscriptionUpdate(SubscriptionBase subscription, params Type[] subscriptionTypes)
		{
			bool flag = false;
			foreach (Type o in subscriptionTypes)
			{
				if (subscription.GetType().Equals(o))
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				if (subscriptionTypes.Length == 1 && subscriptionTypes[0] == typeof(PullSubscription))
				{
					throw new InvalidPullSubscriptionIdException();
				}
				if (ExchangeVersion.Current.Supports(ExchangeVersion.Exchange2010SP1))
				{
					throw new InvalidSubscriptionException();
				}
				throw new InvalidPullSubscriptionIdException();
			}
			else
			{
				if (!subscription.CheckCallerHasRights(base.CallContext))
				{
					throw new SubscriptionAccessDeniedException();
				}
				return;
			}
		}
	}
}
