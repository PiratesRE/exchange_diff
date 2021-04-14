using System;

namespace Microsoft.Exchange.Data.Storage
{
	public enum RequestedAction
	{
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionAny)]
		Any,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionCall)]
		Call,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionDoNotForward)]
		DoNotForward,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionFollowUp)]
		FollowUp,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionForYourInformation)]
		ForYourInformation,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionForward)]
		Forward,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionNoResponseNecessary)]
		NoResponseNecessary,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionRead)]
		Read,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionReply)]
		Reply,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionReplyToAll)]
		ReplyToAll,
		[ClientStringsLocDescription(ClientStrings.IDs.RequestedActionReview)]
		Review
	}
}
