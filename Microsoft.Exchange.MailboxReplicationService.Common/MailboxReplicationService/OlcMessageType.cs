using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum OlcMessageType
	{
		NONE,
		FAX,
		VOICEMAIL,
		CALENDAR_GENERIC,
		CALENDAR_REQUEST,
		CALENDAR_CANCEL,
		CALENDAR_ACCEPTED,
		CALENDAR_TENTATIVE,
		CALENDAR_DECLINED,
		DRAFT_MESSAGE,
		PHOTO_MAIL,
		MSGR_TEXT_OIM,
		MSGR_SMS_OIM,
		MSGR_BUBBLE_OIM,
		CALL_LOG,
		MSGR_OIM,
		SMS,
		NDR,
		MSGR_CONVERSATION,
		MMS
	}
}
