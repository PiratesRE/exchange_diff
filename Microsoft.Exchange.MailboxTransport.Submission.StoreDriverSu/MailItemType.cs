using System;

namespace Microsoft.Exchange.MailboxTransport.Submission.StoreDriverSubmission
{
	[Flags]
	internal enum MailItemType
	{
		ActualMessage = 1,
		MainMessage = 2,
		OtherMessage = 4
	}
}
