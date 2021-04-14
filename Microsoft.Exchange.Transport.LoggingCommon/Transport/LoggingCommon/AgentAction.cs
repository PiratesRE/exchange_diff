using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum AgentAction
	{
		RejectConnection = 1,
		Disconnect,
		RejectAuthentication,
		RejectCommand,
		RejectRecipients,
		RejectMessage,
		AcceptMessage,
		QuarantineRecipients,
		QuarantineMessage,
		DeleteRecipients,
		DeleteMessage,
		ModifyHeaders,
		StampScl,
		AttributionResult,
		OnPremiseInboundConnectorInfo,
		InvalidCertificate,
		AutoNukeRecipient
	}
}
