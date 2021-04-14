using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ConsumerMailboxSize : LoadMetric
	{
		private ConsumerMailboxSize() : base("ConsumerMailboxSize", true)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			if (mailbox.MailboxType != DirectoryMailboxType.Consumer)
			{
				return 0L;
			}
			return (long)mailbox.PhysicalSize.ToBytes();
		}

		public static readonly ConsumerMailboxSize Instance = new ConsumerMailboxSize();
	}
}
