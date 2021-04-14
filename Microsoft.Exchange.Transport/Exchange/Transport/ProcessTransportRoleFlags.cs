using System;

namespace Microsoft.Exchange.Transport
{
	[Flags]
	internal enum ProcessTransportRoleFlags
	{
		None = 0,
		Hub = 1,
		Edge = 2,
		FrontEnd = 4,
		MailboxSubmission = 8,
		MailboxDelivery = 16,
		All = 31
	}
}
