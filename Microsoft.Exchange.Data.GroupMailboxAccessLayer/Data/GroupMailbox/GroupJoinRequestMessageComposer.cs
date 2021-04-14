using System;
using System.IO;
using System.Linq;
using Microsoft.Exchange.Data.ApplicationLogic.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupJoinRequestMessageComposer : BaseGroupMessageComposer
	{
		public GroupJoinRequestMessageComposer(MailboxSession mailboxSession, ADUser groupAdUser, string attachedMessageBody)
		{
			this.mailboxSession = mailboxSession;
			this.groupAdUser = groupAdUser;
			this.attachedMessageBody = attachedMessageBody;
			this.groupOwners = (from g in groupAdUser.Session.FindByADObjectIds<ADUser>(groupAdUser.Owners.ToArray())
			where g.Data != null
			select g.Data).ToArray<ADUser>();
		}

		protected override ADUser[] Recipients
		{
			get
			{
				return this.groupOwners;
			}
		}

		protected override Participant FromParticipant
		{
			get
			{
				return new Participant(this.mailboxSession.MailboxOwner);
			}
		}

		protected override string Subject
		{
			get
			{
				return Strings.JoinRequestMessageSubject(this.mailboxSession.MailboxOwner.MailboxInfo.DisplayName, this.groupAdUser.DisplayName).ToString(BaseGroupMessageComposer.GetPreferredCulture(new ADUser[]
				{
					this.groupAdUser
				}));
			}
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(this.groupAdUser, null);
			GroupJoinRequestMessageBodyBuilder.WriteMessageToStream(streamWriter, this.mailboxSession.MailboxOwner.MailboxInfo.DisplayName, this.groupAdUser.DisplayName, this.attachedMessageBody, new MailboxUrls(exchangePrincipal, false), BaseGroupMessageComposer.GetPreferredCulture(new ADUser[]
			{
				this.groupAdUser
			}));
		}

		protected override void SetAdditionalMessageProperties(MessageItem message)
		{
			if (!(message is GroupMailboxJoinRequestMessageItem))
			{
				throw new ArgumentException();
			}
			GroupMailboxJoinRequestMessageItem groupMailboxJoinRequestMessageItem = message as GroupMailboxJoinRequestMessageItem;
			groupMailboxJoinRequestMessageItem.GroupSmtpAddress = this.groupAdUser.PrimarySmtpAddress.ToString();
			groupMailboxJoinRequestMessageItem.AutoResponseSuppress = AutoResponseSuppress.All;
		}

		protected override void AddAttachments(MessageItem message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
		}

		private readonly MailboxSession mailboxSession;

		private readonly ADUser groupAdUser;

		private readonly ADUser[] groupOwners;

		private readonly string attachedMessageBody;
	}
}
