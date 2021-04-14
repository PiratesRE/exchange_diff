using System;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal enum OlcCalendarType
	{
		SecondaryTasks = -2,
		SecondaryEvents,
		RegularEvents,
		RegularTasks,
		Birthday,
		Holiday,
		IcalSubscription
	}
}
