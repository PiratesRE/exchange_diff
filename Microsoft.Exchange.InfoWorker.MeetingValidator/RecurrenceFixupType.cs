using System;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[Flags]
	internal enum RecurrenceFixupType
	{
		None = 0,
		RecurPattern = 1,
		BlobAndExceptions = 2,
		JustExceptions = 4
	}
}
