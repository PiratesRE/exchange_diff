using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[DataContract]
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PhysicalSize : LoadMetric
	{
		private PhysicalSize() : base("PhysicalSize", true)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return this.GetUnitsForSize(mailbox.PhysicalSize);
		}

		public static readonly PhysicalSize Instance = new PhysicalSize();
	}
}
