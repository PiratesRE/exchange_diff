using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.Data.LoadMetrics
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	[DataContract]
	internal class InProgressLoadBalancingMoveCount : LoadMetric
	{
		private InProgressLoadBalancingMoveCount() : base("InProgressLoadBalancingMoveCount", false)
		{
		}

		public override long GetUnitsForMailbox(DirectoryMailbox mailbox)
		{
			return mailbox.IsBeingLoadBalanced ? 1L : 0L;
		}

		public static readonly InProgressLoadBalancingMoveCount Instance = new InProgressLoadBalancingMoveCount();
	}
}
