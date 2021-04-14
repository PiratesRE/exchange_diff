using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common.SendAsVerification
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SendAsVerificationUrlGenerator
	{
		internal string GenerateURLFor(ExchangePrincipal subscriptionExchangePrincipal, AggregationSubscriptionType subscriptionType, Guid subscriptionGuid, Guid sharedSecret, SyncLogSession syncLogSession)
		{
			string result;
			if (!EcpUtilities.TryGetSendAsVerificationUrl(subscriptionExchangePrincipal, (int)subscriptionType, subscriptionGuid, sharedSecret, syncLogSession, out result))
			{
				throw new FailedToGenerateVerificationEmailException();
			}
			return result;
		}
	}
}
