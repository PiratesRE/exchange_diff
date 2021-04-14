using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.GroupMailbox.Common;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.GroupMailbox
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WelcomeToGroupMessageComposer : BaseGroupMessageComposer
	{
		public WelcomeToGroupMessageComposer(WelcomeToGroupMessageTemplate template, ADUser recipient, ADUser group)
		{
			ArgumentValidator.ThrowIfNull("template", template);
			ArgumentValidator.ThrowIfNull("recipient", recipient);
			this.template = template;
			this.recipient = recipient;
			this.preferredCulture = BaseGroupMessageComposer.GetPreferredCulture(new ADUser[]
			{
				recipient,
				group
			});
		}

		protected override ADUser[] Recipients
		{
			get
			{
				return new ADUser[]
				{
					this.recipient
				};
			}
		}

		protected override Participant FromParticipant
		{
			get
			{
				return this.template.EmailFrom;
			}
		}

		protected override string Subject
		{
			get
			{
				return this.GetSubject();
			}
		}

		private bool AddedByRecipient
		{
			get
			{
				return this.template.ExecutingUser == null || this.template.ExecutingUser.Id.Equals(this.recipient.Id);
			}
		}

		protected override void SetAdditionalMessageProperties(MessageItem message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			message[StoreObjectSchema.ItemClass] = "IPM.Note.GroupMailbox.WelcomeEmail";
			if (this.template.EmailSender != null)
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<string>((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.SetAdditionalMessageProperties: Setting message sender to: {0}", this.template.EmailSender.DisplayName);
				message.Sender = this.template.EmailSender;
				return;
			}
			BaseGroupMessageComposer.Tracer.TraceDebug((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.SetAdditionalMessageProperties: Skipping setting email sender.");
		}

		protected override void WriteMessageBody(StreamWriter streamWriter)
		{
			ArgumentValidator.ThrowIfNull("streamWriter", streamWriter);
			WelcomeMessageBodyData data = new WelcomeMessageBodyData(this.template, this.GetJoinedByMessage(), this.recipient.RecipientType == RecipientType.MailUser, !this.AddedByRecipient, this.preferredCulture);
			WelcomeMessageBodyWriter.WriteTemplate(streamWriter, data);
		}

		protected override void AddAttachments(MessageItem message)
		{
			ArgumentValidator.ThrowIfNull("message", message);
			WelcomeMessageBodyData.WelcomeConversationsIcon.AddImageAsAttachment(message);
			WelcomeMessageBodyData.BlankGifImage.AddImageAsAttachment(message);
			if (!this.template.GroupIsAutoSubscribe)
			{
				if (this.preferredCulture.TextInfo.IsRightToLeft)
				{
					WelcomeMessageBodyData.WelcomeArrowFlippedIcon.AddImageAsAttachment(message);
				}
				else
				{
					WelcomeMessageBodyData.WelcomeArrowIcon.AddImageAsAttachment(message);
				}
			}
			if (this.recipient.RecipientType != RecipientType.MailUser)
			{
				WelcomeMessageBodyData.WelcomeO365Icon.AddImageAsAttachment(message);
			}
			if (!string.IsNullOrEmpty(this.template.GroupSharePointUrl))
			{
				WelcomeMessageBodyData.WelcomeDocumentIcon.AddImageAsAttachment(message);
			}
			if (this.template.GroupHasPhoto)
			{
				this.template.GroupPhoto.AddImageAsAttachment(message);
			}
		}

		private string GetJoinedByMessage()
		{
			LocalizedString localizedString = LocalizedString.Empty;
			if (this.AddedByRecipient)
			{
				BaseGroupMessageComposer.Tracer.TraceDebug((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.GetJoinedByMessage: executingUser is unknown or by user himself.");
				localizedString = ClientStrings.GroupMailboxWelcomeEmailSecondaryHeaderYouJoined(this.template.EncodedGroupDisplayName);
			}
			else
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<ADObjectId, ADObjectId>((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.GetJoinedByMessage: executingUser is different than the one joining the group, returning message header for added member. ExecutingUser.AdObjectId: {0}, NewMember.AdObjectId: {1}.", this.template.ExecutingUser.Id, this.recipient.Id);
				localizedString = ClientStrings.GroupMailboxWelcomeEmailSecondaryHeaderAddedBy(this.template.EncodedExecutingUserDisplayName, this.template.EncodedGroupDisplayName);
			}
			return localizedString.ToString(this.preferredCulture);
		}

		private string GetSubject()
		{
			LocalizedString localizedString;
			if (this.template.ExecutingUser == null)
			{
				BaseGroupMessageComposer.Tracer.TraceDebug((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.GetSubject: executingUser is unknown, returning subject without JoinedBy.");
				localizedString = ClientStrings.GroupMailboxAddedMemberNoJoinedBySubject(this.template.Group.DisplayName);
			}
			else if (this.template.ExecutingUser.Id.Equals(this.recipient.Id))
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<ADObjectId, ADObjectId>((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.GetSubject: executingUser is same as user joining the group, returning subject for self-join. ExecutingUser.AdObjectId: {0}, NewMember.AdObjectId: {1}.", this.template.ExecutingUser.Id, this.recipient.Id);
				localizedString = ClientStrings.GroupMailboxAddedSelfMessageSubject(this.template.Group.DisplayName);
			}
			else
			{
				BaseGroupMessageComposer.Tracer.TraceDebug<ADObjectId, ADObjectId>((long)this.GetHashCode(), "WelcomeToGroupMessageComposer.GetSubject: executingUser is different than the one joining the group, returning subject for added member. ExecutingUser.AdObjectId: {0}, NewMember.AdObjectId: {1}.", this.template.ExecutingUser.Id, this.recipient.Id);
				localizedString = ClientStrings.GroupMailboxAddedMemberMessageSubject(this.template.Group.DisplayName);
			}
			return localizedString.ToString(this.preferredCulture);
		}

		private readonly WelcomeToGroupMessageTemplate template;

		private readonly ADUser recipient;

		private readonly CultureInfo preferredCulture;
	}
}
