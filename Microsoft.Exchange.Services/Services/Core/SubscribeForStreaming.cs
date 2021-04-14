using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeForStreaming : SubscribeCommandBase
	{
		public SubscribeForStreaming(CallContext callContext, SubscribeRequest request) : base(callContext, request)
		{
			this.streamingSubscriptionRequest = (base.Request.SubscriptionRequest as StreamingSubscriptionRequest);
		}

		protected override SubscriptionBase CreateSubscriptionInstance(IdAndSession[] folderIds)
		{
			return new StreamingSubscription(this.streamingSubscriptionRequest, folderIds, base.CallContext.OriginalCallerContext.IdentifierString);
		}

		private StreamingSubscriptionRequest streamingSubscriptionRequest;
	}
}
