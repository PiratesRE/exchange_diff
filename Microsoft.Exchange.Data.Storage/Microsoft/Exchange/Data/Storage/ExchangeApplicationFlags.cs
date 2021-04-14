using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ExchangeApplicationFlags
	{
		IsFromFavoriteSender = 1,
		IsSpecificMessageReply = 2,
		IsSpecificMessageReplyStamped = 4,
		RelyOnConversationIndex = 8,
		IsClutterOverridden = 16,
		SupportsSideConversation = 32,
		IsGroupEscalationMessage = 64,
		FromAddressBookContact = 128,
		JunkedByBlockListMessageFilter = 256,
		SenderPRAEmailPresent = 512,
		FromFirstTimeSender = 1024,
		IsFromPerson = 2048
	}
}
