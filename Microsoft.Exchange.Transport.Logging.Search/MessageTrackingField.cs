using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	public enum MessageTrackingField
	{
		Timestamp,
		ClientIp,
		ClientHostname,
		ServerIp,
		ServerHostname,
		SourceContext,
		ConnectorId,
		Source,
		EventId,
		InternalMessageId,
		MessageId,
		NetworkMessageId,
		RecipientAddress,
		RecipientStatus,
		TotalBytes,
		RecipientCount,
		RelatedRecipientAddress,
		Reference,
		MessageSubject,
		SenderAddress,
		ReturnPath,
		MessageInfo,
		Directionality,
		TenantId,
		OriginalClientIP,
		OriginalServerIP,
		CustomData
	}
}
