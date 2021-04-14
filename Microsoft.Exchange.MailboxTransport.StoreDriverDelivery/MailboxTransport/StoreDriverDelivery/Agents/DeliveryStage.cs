using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverDelivery.Agents
{
	internal enum DeliveryStage
	{
		None,
		InitializedMessageEventHandled,
		PromotedMessageEventHandled,
		CreatedMessageEventHandled,
		DeliveredMessageEventHandled,
		CompletedMessageEventHandled
	}
}
