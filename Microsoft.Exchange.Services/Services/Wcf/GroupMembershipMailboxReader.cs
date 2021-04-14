using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Services.Wcf
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class GroupMembershipMailboxReader : IGroupMembershipReader<GroupMailbox>
	{
		public GroupMembershipMailboxReader(UserMailboxLocator mailbox, IRecipientSession adSession, MailboxSession mailboxSession)
		{
			ArgumentValidator.ThrowIfNull("mailbox", mailbox);
			ArgumentValidator.ThrowIfNull("adSession", adSession);
			ArgumentValidator.ThrowIfNull("mailboxSession", mailboxSession);
			this.mailbox = mailbox;
			this.adSession = adSession;
			this.mailboxSession = mailboxSession;
		}

		public IEnumerable<GroupMailbox> GetJoinedGroups()
		{
			IEnumerable<GroupMailbox> groupMailboxes = null;
			GroupMailboxAccessLayer.Execute("GroupMembershipMailboxReader", this.adSession, this.mailboxSession, delegate(GroupMailboxAccessLayer accessLayer)
			{
				groupMailboxes = accessLayer.GetJoinedGroups(this.mailbox, true);
			});
			return groupMailboxes ?? ((IEnumerable<GroupMailbox>)new GroupMailbox[0]);
		}

		private readonly IRecipientSession adSession;

		private readonly MailboxSession mailboxSession;

		private readonly UserMailboxLocator mailbox;
	}
}
