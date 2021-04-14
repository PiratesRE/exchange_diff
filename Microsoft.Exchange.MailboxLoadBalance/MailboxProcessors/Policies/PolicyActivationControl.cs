using System;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class PolicyActivationControl : IMailboxPolicy
	{
		public PolicyActivationControl(IMailboxPolicy policy, ILoadBalanceSettings settings)
		{
			AnchorUtil.ThrowOnNullArgument(policy, "policy");
			this.policy = policy;
			this.settings = settings;
		}

		public BatchName GetBatchName()
		{
			return this.policy.GetBatchName();
		}

		public bool IsMailboxOutOfPolicy(DirectoryMailbox mailbox, DirectoryDatabase currentDatabase)
		{
			return !(this.settings.DisabledMailboxPolicies ?? string.Empty).Contains(this.policy.GetType().Name) && this.policy.IsMailboxOutOfPolicy(mailbox, currentDatabase);
		}

		public void HandleExistingButNotInProgressMove(DirectoryMailbox mailbox, DirectoryDatabase database)
		{
			this.policy.HandleExistingButNotInProgressMove(mailbox, database);
		}

		private readonly IMailboxPolicy policy;

		private readonly ILoadBalanceSettings settings;
	}
}
