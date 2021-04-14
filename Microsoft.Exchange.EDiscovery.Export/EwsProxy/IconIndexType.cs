using System;
using System.CodeDom.Compiler;
using System.Xml.Serialization;

namespace Microsoft.Exchange.EDiscovery.Export.EwsProxy
{
	[GeneratedCode("wsdl", "4.0.30319.17627")]
	[XmlType(Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum IconIndexType
	{
		Default,
		PostItem,
		MailRead,
		MailUnread,
		MailReplied,
		MailForwarded,
		MailEncrypted,
		MailSmimeSigned,
		MailEncryptedReplied,
		MailSmimeSignedReplied,
		MailEncryptedForwarded,
		MailSmimeSignedForwarded,
		MailEncryptedRead,
		MailSmimeSignedRead,
		MailIrm,
		MailIrmForwarded,
		MailIrmReplied,
		SmsSubmitted,
		SmsRoutedToDeliveryPoint,
		SmsRoutedToExternalMessagingSystem,
		SmsDelivered,
		OutlookDefaultForContacts,
		AppointmentItem,
		AppointmentRecur,
		AppointmentMeet,
		AppointmentMeetRecur,
		AppointmentMeetNY,
		AppointmentMeetYes,
		AppointmentMeetNo,
		AppointmentMeetMaybe,
		AppointmentMeetCancel,
		AppointmentMeetInfo,
		TaskItem,
		TaskRecur,
		TaskOwned,
		TaskDelegated
	}
}
