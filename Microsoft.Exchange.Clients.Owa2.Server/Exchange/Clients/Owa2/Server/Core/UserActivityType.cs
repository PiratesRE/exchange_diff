using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	public enum UserActivityType
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
		AllParticipantsLeftChat,
		DeliveryTimeout
	}
}
