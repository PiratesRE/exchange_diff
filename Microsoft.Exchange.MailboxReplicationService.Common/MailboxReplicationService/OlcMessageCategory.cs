using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum OlcMessageCategory
	{
		NotDefined,
		Unread,
		NotFromContact = 3,
		FromContact = 5,
		Flagged = 7,
		HasAttachment = 9,
		ResponsesToMe = 11,
		SMS = 17,
		Chat = 19,
		MMS = 21,
		RepliedTo = 27,
		Newsletter = 15,
		Photo = 53,
		SocialNetwork = 55,
		Comment = 57,
		Tag = 59,
		Video = 61,
		Document = 63,
		File = 65,
		MailingList = 67,
		ShippingNotification = 69,
		InteractiveLiveView = 71,
		DocumentsPlus = 73
	}
}
