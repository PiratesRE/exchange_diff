using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LyncServiceRequestHandler : ICallHandler
	{
		public void HandleCall(CafeRoutingContext cafeContext)
		{
			ValidateArgument.NotNull(cafeContext, "cafeContext");
			string simplifiedUri = cafeContext.FromUri.SimplifiedUri;
			PlatformSipUri requestUriOfCall = cafeContext.RequestUriOfCall;
			SipRoutingHelper.Context routingContext = cafeContext.RoutingHelper.GetRoutingContext(simplifiedUri, requestUriOfCall);
			ExAssert.RetailAssert(routingContext != null, "SipRoutingHelper.Context was not set.");
			if (routingContext.Recipient == null)
			{
				throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.CouldNotFindUserBySipUri, "User: {0}.", new object[]
				{
					simplifiedUri
				});
			}
			using (UMRecipient umrecipient = UMRecipient.Factory.FromADRecipient<UMRecipient>(routingContext.Recipient))
			{
				if (umrecipient == null)
				{
					throw CallRejectedException.Create(Strings.InvalidRequest, CallEndingReason.CouldNotFindUserBySipUri, "User: {0}.", new object[]
					{
						simplifiedUri
					});
				}
				cafeContext.RedirectUri = RedirectionTarget.Instance.GetForCallAnsweringCall(umrecipient, cafeContext).Uri;
			}
		}
	}
}
