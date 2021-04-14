using System;
using System.Globalization;
using System.IO;
using System.Web.Security.AntiXss;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GroupWarmingMessageComposer : BaseGroupMessageComposer
	{
		public GroupWarmingMessageComposer(ADUser groupMailbox, ADUser executingUser)
		{
			ArgumentValidator.ThrowIfNull("groupMailbox", groupMailbox);
			this.encodedGroupDisplayName = AntiXssEncoder.HtmlEncode(groupMailbox.DisplayName, false);
			this.plainGroupDisplayName = groupMailbox.DisplayName;
			this.groupMailbox = groupMailbox;
			this.participant = new Participant(groupMailbox);
			this.preferredCulture = BaseGroupMessageComposer.GetPreferredCulture(new ADUser[]
			{
				groupMailbox
			});
		}

		protected override ADUser[] Recipients
		{
			get
			{
				return new ADUser[]
				{
					this.groupMailbox
				};
			}
		}

		protected override Participant FromParticipant
		{
			get
			{
				return this.participant;
			}
		}

		protected override string Subject
		{
			get
			{
				return ClientStrings.GroupMailboxWelcomeMessageSubject(this.plainGroupDisplayName).ToString(this.preferredCulture);
			}
		}

		protected override void SetAdditionalMessageProperties(MessageItem message)
		{
			message.IsDraft = false;
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			ArgumentValidator.ThrowIfNull("streamWriter", streamWriter);
			WelcomeMessageBodyBuilder.WriteWarmingMessageBody(streamWriter, this.encodedGroupDisplayName, this.preferredCulture);
		}

		private readonly string encodedGroupDisplayName;

		private readonly string plainGroupDisplayName;

		private readonly ADUser groupMailbox;

		private readonly Participant participant;

		private readonly CultureInfo preferredCulture;
	}
}
