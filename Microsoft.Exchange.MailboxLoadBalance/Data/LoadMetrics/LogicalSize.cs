using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class LogicalSize : LoadMetric
	{
		private LogicalSize() : base("LogicalSize", true)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return this.GetUnitsForSize(mailbox.LogicalSize);
		}

		public static readonly LogicalSize Instance = new LogicalSize();
	}
}
