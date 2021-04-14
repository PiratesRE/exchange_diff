using System;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[XmlType(TypeName = "ClientIntentMeetingInquiryActionType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public enum ClientIntentMeetingInquiryAction
	{
		SendCancellation,
		ReviveMeeting,
		SendUpdateForMaster,
		MeetingAlreadyExists,
		ExistingOccurrence,
		HasDelegates,
		DeletedVersionNotFound,
		PairedCancellationFound,
		FailedToRevive
	}
}
