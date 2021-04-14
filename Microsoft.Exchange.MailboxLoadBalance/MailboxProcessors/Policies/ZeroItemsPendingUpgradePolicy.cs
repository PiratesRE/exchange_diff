using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.MailboxLoadBalance.Config;
using Microsoft.Exchange.MailboxLoadBalance.Data;
using Microsoft.Exchange.MailboxLoadBalance.Directory;

namespace Microsoft.Exchange.MailboxLoadBalance.MailboxProcessors.Policies
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ZeroItemsPendingUpgradePolicy : IMailboxPolicy
	{
		public ZeroItemsPendingUpgradePolicy(ILoadBalanceSettings settings)
		{
			this.settings = settings;
		}

		public bool IsMailboxOutOfPolicy(DirectoryMailbox mailbox, DirectoryDatabase currentDatabase)
		{
			TimeSpan minimumAgeInDatabase = mailbox.MinimumAgeInDatabase;
			return mailbox.ItemsPendingUpgrade > 0 && minimumAgeInDatabase >= this.settings.MinimumTimeInDatabaseForItemUpgrade;
		}

		public void HandleExistingButNotInProgressMove(DirectoryMailbox mailbox, DirectoryDatabase database)
		{
		}

		public BatchName GetBatchName()
		{
			return BatchName.CreateItemUpgradeBatch();
		}

		private readonly ILoadBalanceSettings settings;
	}
}
