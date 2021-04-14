using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupPrincipalBuilder : ExchangePrincipalBuilder
	{
		public GroupPrincipalBuilder(Func<IADUserFinder, IRecipientSession, IGenericADUser> findADUser) : base(findADUser)
		{
		}

		public GroupPrincipalBuilder(IGenericADUser adUser) : base(adUser)
		{
		}

		protected override ExchangePrincipal BuildPrincipal(IGenericADUser recipient, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions)
		{
			return new GroupPrincipal(recipient, allMailboxes, mailboxSelector, remotingOptions);
		}

		protected override bool IsRecipientTypeSupported(IGenericADUser group)
		{
			return group.RecipientType == RecipientType.MailUniversalDistributionGroup && group.RecipientTypeDetails == RecipientTypeDetails.GroupMailbox;
		}
	}
}
