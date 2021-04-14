using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	public enum InstantMessageAlert
	{
		None,
		Typing,
		StoppedTyping,
		TwoOrMoreTyping,
		YouJoined,
		LastMessageTime,
		ExternalUser,
		Subject = 8,
		SomeoneJoined,
		SomeoneLeft,
		FailedInvite,
		FailedDelivery,
		FailedDeliveryDueToServerPolicy,
		AllParticipantsLeftChat
	}
}
