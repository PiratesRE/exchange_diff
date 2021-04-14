using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	internal enum ModificationType
	{
		Subject = 1,
		MeetingType = 2,
		ReminderDelta = 4,
		Reminder = 8,
		Location = 16,
		BusyStatus = 32,
		Attachment = 64,
		SubType = 128,
		Color = 256,
		Body = 512
	}
}
