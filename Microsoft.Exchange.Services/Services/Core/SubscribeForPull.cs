using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeForPull : SubscribeCommandBase
	{
		public SubscribeForPull(CallContext callContext, SubscribeRequest request) : base(callContext, request)
		{
			this.pullSubscriptionRequest = (base.Request.SubscriptionRequest as PullSubscriptionRequest);
		}

		protected override SubscriptionBase CreateSubscriptionInstance(IdAndSession[] folderIds)
		{
			return new PullSubscription(this.pullSubscriptionRequest, folderIds, base.CallContext.EffectiveCaller.ObjectGuid);
		}

		private PullSubscriptionRequest pullSubscriptionRequest;
	}
}
