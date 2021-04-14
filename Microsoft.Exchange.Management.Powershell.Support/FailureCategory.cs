using System;

namespace Microsoft.Exchange.Management.Powershell.Support
{
	[Flags]
	public enum FailureCategory
	{
		None = 0,
		DuplicateMeetings = 1,
		WrongTime = 2,
		WrongLocation = 4,
		WrongTrackingStatus = 8,
		CorruptMeetings = 16,
		MissingMeetings = 32,
		RecurrenceProblems = 64,
		MailboxUnavailable = 128,
		All = 255
	}
}
