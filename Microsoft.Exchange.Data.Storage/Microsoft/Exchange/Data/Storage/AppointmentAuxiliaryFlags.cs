using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum AppointmentAuxiliaryFlags
	{
		Copied = 1,
		ForceMeetingResponse = 2,
		ForwardedAppointment = 4,
		Orphaned = 8,
		ExtractOrganizer = 16,
		RepairUpdateMessage = 32,
		ExtractForceReceived = 64,
		ExtractedMeeting = 128,
		EventAddedFromGroupCalendar = 256
	}
}
