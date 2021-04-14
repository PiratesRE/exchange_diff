using System;

namespace Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch
{
	internal enum ItemIndexError
	{
		None,
		GenericError,
		Timeout,
		StaleEvent,
		MailboxOffline,
		AttachmentLimitReached,
		MarsWriterTruncation,
		DocumentParserFailure
	}
}
