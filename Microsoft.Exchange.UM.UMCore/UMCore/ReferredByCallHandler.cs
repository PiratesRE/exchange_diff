using System;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class ReferredByCallHandler : ICallHandler
	{
		public void HandleCall(CafeRoutingContext context)
		{
			ValidateArgument.NotNull(context, "RoutingContext");
			if (!string.IsNullOrEmpty(context.ReferredByHeader))
			{
				this.HandleTransferredCall(context);
			}
		}

		private void HandleTransferredCall(CafeRoutingContext context)
		{
			context.Tracer.Trace("ReferredByCallHandler : HandleTransferredCall, referredbyheader = {0}", new object[]
			{
				context.ReferredByHeader
			});
			UMRecipient umrecipient = null;
			UserTransferWithContext.DeserializedReferredByHeader deserializedReferredByHeader = null;
			try
			{
				if (UserTransferWithContext.TryParseReferredByHeader(context.ReferredByHeader, context.DialPlan, out umrecipient, out deserializedReferredByHeader))
				{
					context.Tracer.Trace("ReferredByCallHandler : HandleTransferredCall, TypeOfTransferredCall = {0}", new object[]
					{
						deserializedReferredByHeader.TypeOfTransferredCall
					});
					switch (deserializedReferredByHeader.TypeOfTransferredCall)
					{
					case 3:
						context.RedirectUri = RedirectionTarget.Instance.GetForSubscriberAccessCall(umrecipient, context).Uri;
						break;
					case 4:
						context.RedirectUri = RedirectionTarget.Instance.GetForCallAnsweringCall(umrecipient, context).Uri;
						break;
					}
				}
			}
			finally
			{
				if (umrecipient != null)
				{
					umrecipient.Dispose();
				}
			}
		}
	}
}
