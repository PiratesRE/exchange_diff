using System;
using Microsoft.Exchange.Services.Core.Types;

namespace Microsoft.Exchange.Services.Core
{
	internal sealed class SubscribeForPush : SubscribeCommandBase
	{
		public SubscribeForPush(CallContext callContext, SubscribeRequest request) : base(callContext, request)
		{
			this.pushSubscriptionRequest = (base.Request.SubscriptionRequest as PushSubscriptionRequest);
		}

		protected override void ValidateOperation()
		{
			base.ValidateOperation();
			SubscribeForPush.ValidateSubscriptionUrl(this.pushSubscriptionRequest.Url);
		}

		private static void ValidateSubscriptionUrl(string url)
		{
			if (url.Length == 0 || url.Length > 2083)
			{
				throw new InvalidPushSubscriptionUrlException();
			}
			try
			{
				Uri uri = new Uri(url);
				string components = uri.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped);
				if (!components.Equals("http", StringComparison.OrdinalIgnoreCase) && !components.Equals("https", StringComparison.OrdinalIgnoreCase))
				{
					throw new InvalidPushSubscriptionUrlException();
				}
			}
			catch (UriFormatException innerException)
			{
				throw new InvalidPushSubscriptionUrlException(innerException);
			}
		}

		protected override SubscriptionBase CreateSubscriptionInstance(IdAndSession[] folderIds)
		{
			return new PushSubscription(this.pushSubscriptionRequest, folderIds, base.CallContext.EffectiveCaller.ObjectGuid, Subscriptions.Singleton);
		}

		private const int MaxUrlLength = 2083;

		private PushSubscriptionRequest pushSubscriptionRequest;
	}
}
