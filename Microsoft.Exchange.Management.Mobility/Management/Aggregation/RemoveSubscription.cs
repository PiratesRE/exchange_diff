using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Management.Aggregation
{
	[Cmdlet("Remove", "Subscription", DefaultParameterSetName = "Identity", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveSubscription : RemoveSubscriptionBase<PimSubscriptionProxy>
	{
		[Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "Identity")]
		public MailboxIdParameter Mailbox
		{
			get
			{
				return (MailboxIdParameter)base.Fields["Mailbox"];
			}
			set
			{
				base.Fields["Mailbox"] = value;
			}
		}

		protected override AggregationType AggregationType
		{
			get
			{
				return AggregationType.Migration;
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || exception is FailedDeleteAggregationSubscriptionException;
		}

		protected override MailboxIdParameter GetMailboxIdParameter()
		{
			if (this.Mailbox == null)
			{
				this.Mailbox = base.GetMailboxIdParameter();
			}
			return this.Mailbox;
		}
	}
}
