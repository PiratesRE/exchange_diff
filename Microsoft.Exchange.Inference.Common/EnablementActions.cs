using System;

namespace Microsoft.Exchange.Inference.Common
{
	[Flags]
	internal enum EnablementActions
	{
		NoAction = 0,
		AutoEnabled = 1,
		SentInvitation = 2,
		SentReminder = 4,
		AddedToReadyBreadCrumb = 8,
		AddedToNotReadyBreadCrumb = 16,
		ScheduledAutoEnablementNotice = 32,
		SentAutoEnablementNotice = 64
	}
}
