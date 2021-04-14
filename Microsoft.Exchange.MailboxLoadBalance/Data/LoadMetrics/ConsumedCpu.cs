using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class ConsumedCpu : LoadMetric
	{
		static ConsumedCpu()
		{
			ConsumedCpu.Instance = new ConsumedCpu();
		}

		private ConsumedCpu() : base("CPU", false)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return mailbox.TotalCpu;
		}

		public static readonly ConsumedCpu Instance = new ConsumedCpu();
	}
}
