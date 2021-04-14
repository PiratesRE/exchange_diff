using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum InstantMessageOperationError
	{
		Success,
		UnknownDoNotUse = -1,
		NotEnabled = -2,
		NotConfigured = -3,
		SessionDisconnected = -4,
		EmptyMessage = -5,
		NoRecipients = -6,
		InternalErrorInstantMessagingNotSupported = -7,
		UnableToCreateConversation = -8,
		ConversationEnded = -9,
		SelfPresenceNotEstablished = -10,
		NotSignedIn = -11,
		UnableToCreateProvider = -12,
		InitializationInProgress = -13
	}
}
