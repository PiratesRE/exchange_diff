using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum AgentLogField
	{
		Timestamp,
		SessionId,
		LocalEndpoint,
		RemoteEndpoint,
		EnteredOrgFromIP,
		MessageId,
		P1FromAddress,
		P2FromAddresses,
		Recipient,
		NumRecipients,
		Agent,
		Event,
		Action,
		SmtpResponse,
		Reason,
		ReasonData,
		Diagnostics,
		NetworkMsgID,
		TenantID,
		Directionality,
		NumFields
	}
}
