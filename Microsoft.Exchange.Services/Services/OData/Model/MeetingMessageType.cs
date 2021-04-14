using System;

namespace Microsoft.Exchange.Services.OData.Model
{
	internal enum MeetingMessageType
	{
		None,
		MeetingRequest,
		MeetingCancelled,
		MeetingAccepted,
		MeetingTenativelyAccepted,
		MeetingDeclined
	}
}
