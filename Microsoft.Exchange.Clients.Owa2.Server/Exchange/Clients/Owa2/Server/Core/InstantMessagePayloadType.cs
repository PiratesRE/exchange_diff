using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum InstantMessagePayloadType
	{
		None,
		AddToBuddyList,
		DeleteBuddy,
		DeleteGroup,
		EmptySubscribers,
		EndChatSession,
		NewChatMessage,
		NewChatMessageToast,
		ParticipantJoined,
		ParticipantLeft,
		PendingContactList,
		QueryUserPresenceChange,
		RenameGroup,
		ReportError,
		ServiceUnavailable,
		SignOn,
		UnsupportedLegacyUser,
		UpdateUserPresence,
		UserActivity,
		UserPresenceChange
	}
}
