using System;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IQueuedGroupJoinInvoker
	{
		void Enqueue(GroupMailbox group);

		bool ProcessQueue(UserMailboxLocator userMailbox, Guid parentActivityId);
	}
}
