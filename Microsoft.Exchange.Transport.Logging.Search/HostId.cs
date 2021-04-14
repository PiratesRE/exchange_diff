using System;

namespace Microsoft.Exchange.Transport.Logging.Search
{
	internal enum HostId
	{
		NotInitialized,
		ECPApplicationPool,
		EWSApplicationPool,
		PowershellApplicationPool,
		MailSubmissionService,
		MailboxDeliveryService,
		MailboxTransportSubmissionService
	}
}
