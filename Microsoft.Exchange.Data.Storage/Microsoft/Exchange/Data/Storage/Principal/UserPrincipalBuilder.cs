using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage.Principal
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UserPrincipalBuilder : ExchangePrincipalBuilder
	{
		public UserPrincipalBuilder(Func<IADUserFinder, IRecipientSession, IGenericADUser> findADUser) : base(findADUser)
		{
		}

		public UserPrincipalBuilder(IGenericADUser adUser) : base(adUser)
		{
		}

		protected override ExchangePrincipal BuildPrincipal(IGenericADUser recipient, IEnumerable<IMailboxInfo> allMailboxes, Func<IMailboxInfo, bool> mailboxSelector, RemotingOptions remotingOptions)
		{
			return new UserPrincipal(recipient, allMailboxes, mailboxSelector, remotingOptions);
		}

		protected override bool IsRecipientTypeSupported(IGenericADUser user)
		{
			return user.RecipientType == RecipientType.UserMailbox || user.RecipientType == RecipientType.SystemMailbox || user.RecipientType == RecipientType.SystemAttendantMailbox || this.IsArchiveMailUser(user.RecipientType, user.ArchiveGuid, user.ArchiveDatabase);
		}

		private bool IsArchiveMailUser(RecipientType recipientType, Guid archiveMailboxGuid, ADObjectId archiveDatabase)
		{
			return recipientType == RecipientType.MailUser && archiveMailboxGuid != Guid.Empty && !archiveDatabase.IsNullOrEmpty();
		}
	}
}
