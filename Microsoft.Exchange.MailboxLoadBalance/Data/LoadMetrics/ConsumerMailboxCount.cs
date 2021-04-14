using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class ConsumerMailboxCount : LoadMetric
	{
		private ConsumerMailboxCount() : base("ConsumerMailboxCount", false)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return (mailbox.MailboxType == DirectoryMailboxType.Consumer) ? 1L : 0L;
		}

		public static readonly ConsumerMailboxCount Instance = new ConsumerMailboxCount();
	}
}
