using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "IconIndexType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum IconIndexType
	{
		Default = -1,
		PostItem = 1,
		MailRead = 256,
		MailUnread,
		MailReplied = 261,
		MailForwarded,
		MailEncrypted = 272,
		MailSmimeSigned,
		MailEncryptedReplied = 275,
		MailSmimeSignedReplied,
		MailEncryptedForwarded,
		MailSmimeSignedForwarded,
		MailEncryptedRead,
		MailSmimeSignedRead,
		MailIrm = 306,
		MailIrmForwarded,
		MailIrmReplied,
		SmsSubmitted = 336,
		SmsRoutedToDeliveryPoint,
		SmsRoutedToExternalMessagingSystem,
		SmsDelivered,
		OutlookDefaultForContacts = 512,
		AppointmentItem = 1024,
		AppointmentRecur,
		AppointmentMeet,
		AppointmentMeetRecur,
		AppointmentMeetNY,
		AppointmentMeetYes,
		AppointmentMeetNo,
		AppointmentMeetMaybe,
		AppointmentMeetCancel,
		AppointmentMeetInfo,
		TaskItem = 1280,
		TaskRecur,
		TaskOwned,
		TaskDelegated
	}
}
