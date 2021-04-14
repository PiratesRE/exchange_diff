using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum DifferencesBetweenBlobAndAttach
	{
		None = 0,
		StartTime = 1,
		EndTime = 2,
		Subject = 4,
		Location = 8,
		AppointmentColor = 16,
		IsAllDayEvent = 32,
		HasAttachment = 64,
		FreeBusyStatus = 128,
		ReminderIsSet = 256,
		ReminderMinutesBeforeStartInternal = 512,
		AppointmentState = 1024
	}
}
