using System;

namespace Microsoft.Exchange.Data.Storage
{
	internal enum IconIndex
	{
		Default = -1,
		PostItem = 1,
		BaseMail = 256,
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
		BaseSmsDeliveryStatus = 336,
		SmsSubmitted = 336,
		SmsRoutedToDeliveryPoint,
		SmsRoutedToExternalMessagingSystem,
		SmsDelivered,
		LastSmsDeliveryStatus = 339,
		OutlookDefaultForContacts = 512,
		BaseAppointment = 1024,
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
		BaseTask = 1280,
		TaskItem = 1280,
		TaskRecur,
		TaskOwned,
		TaskDelagated
	}
}
