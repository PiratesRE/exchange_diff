using System;

namespace Microsoft.Exchange.Data.Storage
{
	[Flags]
	public enum ClientIntentFlags
	{
		None = 0,
		Principal = 1,
		Delegate = 2,
		DeletedWithNoResponse = 4,
		DeletedExceptionWithNoResponse = 8,
		RespondedTentative = 16,
		RespondedAccept = 32,
		RespondedDecline = 64,
		ModifiedStartTime = 128,
		ModifiedEndTime = 256,
		ModifiedTime = 384,
		ModifiedLocation = 512,
		RespondedExceptionDecline = 1024,
		MeetingCanceled = 2048,
		MeetingExceptionCanceled = 4096,
		MeetingHistoryCreatedByPatternChange = 8192,
		MeetingConvertedToHistoryByRemove = 16384,
		MeetingResponseFailedButMessageContentSent = 32768,
		MeetingResponseFailedNoContentToSend = 65536
	}
}
