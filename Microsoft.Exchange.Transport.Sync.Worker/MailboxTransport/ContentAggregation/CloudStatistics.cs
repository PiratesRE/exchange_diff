using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxTransport.ContentAggregation
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CloudStatistics
	{
		internal long? TotalItemsInSourceMailbox { get; set; }

		internal long? TotalFoldersInSourceMailbox { get; set; }

		internal long? TotalSizeOfSourceMailbox { get; set; }
	}
}
