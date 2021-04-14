using System;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	public enum SenderAddressLocation
	{
		[LocDescription(TransportRulesStrings.IDs.SenderAddressLocationHeader)]
		Header,
		[LocDescription(TransportRulesStrings.IDs.SenderAddressLocationEnvelope)]
		Envelope,
		[LocDescription(TransportRulesStrings.IDs.SenderAddressLocationHeaderOrEnvelope)]
		HeaderOrEnvelope
	}
}
