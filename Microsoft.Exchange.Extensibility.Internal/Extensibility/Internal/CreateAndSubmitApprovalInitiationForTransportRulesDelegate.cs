using System;
using Microsoft.Exchange.Data.Transport.Smtp;

namespace Microsoft.Exchange.Extensibility.Internal
{
	internal delegate SmtpResponse CreateAndSubmitApprovalInitiationForTransportRulesDelegate(ITransportMailItemFacade transportMailItemFacade, string originalSenderAddress, string approverAddresses, string transportRuleName);
}
