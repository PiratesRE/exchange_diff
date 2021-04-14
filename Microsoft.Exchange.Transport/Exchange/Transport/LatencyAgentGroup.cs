using System;

namespace Microsoft.Exchange.Transport
{
	internal enum LatencyAgentGroup
	{
		Categorizer,
		SmtpReceive,
		StoreDriver,
		Delivery,
		MailboxTransportSubmissionStoreDriverSubmission,
		Storage,
		UnassignedAgentGroup = 2147483647,
		AgentGroupCount = 6
	}
}
