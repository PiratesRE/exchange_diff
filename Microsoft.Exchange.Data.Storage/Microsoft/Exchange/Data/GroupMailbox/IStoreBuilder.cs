using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IStoreBuilder
	{
		IAssociationStore Create(IMailboxLocator targetMailbox, IMailboxAssociationPerformanceTracker performanceTracker);
	}
}
