using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ItemCount : LoadMetric
	{
		private ItemCount() : base("ItemCount", false)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return mailbox.ItemCount;
		}

		public static readonly ItemCount Instance = new ItemCount();
	}
}
