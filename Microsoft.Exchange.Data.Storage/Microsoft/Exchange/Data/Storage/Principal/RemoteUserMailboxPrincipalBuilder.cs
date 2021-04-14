using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class RemoteUserMailboxPrincipalBuilder : ExchangePrincipalBuilder
	{
		public RemoteUserMailboxPrincipalBuilder(Func<IADUserFinder, IRecipientSession, IGenericADUser> findADUser) : base(findADUser)
		{
		}

		public RemoteUserMailboxPrincipalBuilder(IGenericADUser adUser) : base(adUser)
		{
		}

		protected override ExchangePrincipal BuildPrincipal(IGenericADUser recipient, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions)
		{
			return new RemoteUserMailboxPrincipal(recipient, allMailboxes, mailboxSelector, remotingOptions);
		}

		protected override bool IsRecipientTypeSupported(IGenericADUser user)
		{
			return user.RecipientType == RecipientType.MailUser && user.RecipientTypeDetails == (RecipientTypeDetails)((ulong)int.MinValue);
		}
	}
}
