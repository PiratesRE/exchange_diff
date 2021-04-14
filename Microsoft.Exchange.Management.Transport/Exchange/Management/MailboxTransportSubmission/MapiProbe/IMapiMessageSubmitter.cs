using System;

namespace Microsoft.Exchange.Management.MailboxTransportSubmission.MapiProbe
{
	internal interface IMapiMessageSubmitter
	{
		void SendMapiMessage(string lamNotificationId, SendMapiMailDefinition mapiMailDefinition, out string entryId, out string internetMessageId, out Guid senderMbxGuid);

		void SendMapiMessage(SendMapiMailDefinition mapiMailDefinition, out string entryId, out string internetMessageId, out Guid senderMbxGuid);

		DeletionResult DeleteMessageFromOutbox(DeleteMapiMailDefinition deleteMapiMailDefinition);
	}
}
