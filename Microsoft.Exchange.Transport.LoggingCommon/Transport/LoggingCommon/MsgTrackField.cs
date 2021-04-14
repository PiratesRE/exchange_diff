using System;

namespace Microsoft.Exchange.Transport.LoggingCommon
{
	internal enum MsgTrackField
	{
		Time,
		ClientIP,
		ClientHostName,
		ServerIP,
		ServerHostName,
		SourceContext,
		ConnectorId,
		Source,
		EventID,
		InternalMsgID,
		MessageID,
		NetworkMsgID,
		RecipientAddress,
		RecipStatus,
		TotalBytes,
		RecipientCount,
		RelatedRecipientAddress,
		Reference,
		Subject,
		Sender,
		ReturnPath,
		MessageInfo,
		Directionality,
		TenantID,
		OriginalClientIP,
		OriginalServerIP,
		CustomData
	}
}
