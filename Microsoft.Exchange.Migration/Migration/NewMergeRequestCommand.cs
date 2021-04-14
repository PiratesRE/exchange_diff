using System;
using Microsoft.Exchange.Migration.DataAccessLayer;

namespace Microsoft.Exchange.Migration
{
	internal class NewMergeRequestCommand : NewMergeRequestCommandBase
	{
		internal NewMergeRequestCommand(ExchangeOutlookAnywhereEndpoint endpoint, ExchangeJobItemSubscriptionSettings subscriptionSettings, object targetMailboxId, string mergeRequestName, bool whatIf, bool useAdmin) : base("New-MergeRequest", endpoint, subscriptionSettings, whatIf, useAdmin)
		{
			MigrationUtil.ThrowOnNullArgument(targetMailboxId, "targetMailboxId");
			MigrationUtil.ThrowOnNullOrEmptyArgument(mergeRequestName, "mergeRequestName");
			base.RequestName = mergeRequestName;
			this.TargetMailboxId = targetMailboxId;
		}

		public object TargetMailboxId
		{
			set
			{
				base.AddParameter("TargetMailbox", value);
			}
		}

		public const string NewMergeRequestCommandName = "New-MergeRequest";

		internal const string TargetMailboxParameter = "TargetMailbox";
	}
}
