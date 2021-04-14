using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ReplyCreation : ReplyForwardCommon
	{
		internal ReplyCreation(Item originalItem, Item newItem, ReplyForwardConfiguration parameters, bool isReplyAll, bool shouldUseSender, bool decodeSmime) : base(originalItem, newItem, parameters, decodeSmime)
		{
			this.Initialize(originalItem, isReplyAll, shouldUseSender);
		}

		protected override void BuildSubject()
		{
			if (this.parameters.SubjectPrefix != null)
			{
				this.newItem[InternalSchema.SubjectPrefix] = this.parameters.SubjectPrefix;
			}
			else
			{
				this.newItem[InternalSchema.SubjectPrefix] = ClientStrings.ItemReply.ToString(base.Culture);
			}
			this.newItem[InternalSchema.NormalizedSubject] = this.originalItem.GetValueOrDefault<string>(InternalSchema.NormalizedSubjectInternal, string.Empty);
		}

		protected override void UpdateNewItemProperties()
		{
			base.UpdateNewItemProperties();
			if (this.originalItem is CalendarItemBase || this.originalItem is MeetingRequest || this.originalItem is MeetingCancellation)
			{
				this.newItem.SafeSetProperty(InternalSchema.IsReplyRequested, false);
				this.newItem.SafeSetProperty(InternalSchema.IsResponseRequested, false);
				ReplyCreation.UpdateMeetingReplyIconIndex(this.newItem);
			}
			if (!(this.newItem is PostItem))
			{
				this.BuildRecipients();
			}
			this.newItem.SafeSetProperty(InternalSchema.Importance, Importance.Normal);
		}

		protected override void BuildAttachments(BodyConversionCallbacks callbacks, InboundConversionOptions optionsForSmime)
		{
			base.CopyAttachments(callbacks, this.originalItem.AttachmentCollection, this.newItem.AttachmentCollection, true, this.parameters.TargetFormat == BodyFormat.TextPlain, optionsForSmime);
		}

		private static void UpdateMeetingReplyIconIndex(Item item)
		{
			IconIndex valueOrDefault = item.GetValueOrDefault<IconIndex>(InternalSchema.IconIndex, IconIndex.Default);
			if ((valueOrDefault & IconIndex.BaseAppointment) > (IconIndex)0)
			{
				item[InternalSchema.IconIndex] = IconIndex.Default;
			}
		}

		private void Initialize(Item originalItem, bool isReplyAll, bool shouldUseSender)
		{
			this.isReplyAll = isReplyAll;
			this.shouldUseSender = shouldUseSender;
			LastAction lastAction;
			if (isReplyAll)
			{
				lastAction = LastAction.ReplyToAll;
			}
			else
			{
				lastAction = LastAction.ReplyToSender;
			}
			IconIndex iconIndex = IconIndex.MailReplied;
			if (this.originalItemSigned)
			{
				iconIndex = IconIndex.MailSmimeSignedReplied;
			}
			else if (this.originalItemEncrypted)
			{
				iconIndex = IconIndex.MailEncryptedReplied;
			}
			else if (this.originalItemIrm)
			{
				iconIndex = IconIndex.MailIrmReplied;
			}
			if (originalItem.Id != null && originalItem.Id.ObjectId != null && !originalItem.Id.ObjectId.IsFakeId && !(originalItem is PostItem))
			{
				this.newItem.SafeSetProperty(InternalSchema.ReplyForwardStatus, ReplyForwardUtils.EncodeReplyForwardStatus(lastAction, iconIndex, originalItem.Id));
			}
		}

		private void BuildRecipients()
		{
			bool useReplyTo = true;
			if (this is OofReplyCreation || this is RuleReplyCreation)
			{
				useReplyTo = false;
			}
			if (this.originalItem is MessageItem)
			{
				ReplyForwardCommon.BuildReplyRecipientsFromMessage(this.newItem as MessageItem, this.originalItem as MessageItem, this.isReplyAll, this.shouldUseSender, useReplyTo);
				return;
			}
			if (this.originalItem is CalendarItemBase)
			{
				this.BuildRecipientsFromCalendarItem();
				return;
			}
			if (this.originalItem is PostItem)
			{
				this.BuildRecipientsFromPostItem();
			}
		}

		private void BuildRecipientsFromCalendarItem()
		{
			MessageItem messageItem = (MessageItem)this.newItem;
			CalendarItemBase calendarItemBase = (CalendarItemBase)this.originalItem;
			if (calendarItemBase.Organizer != null)
			{
				messageItem.Recipients.Add(calendarItemBase.Organizer, RecipientItemType.To);
			}
			if (!this.isReplyAll)
			{
				return;
			}
			MailboxSession mailboxSession = calendarItemBase.Session as MailboxSession;
			foreach (Attendee attendee in calendarItemBase.AttendeeCollection)
			{
				if (!messageItem.Recipients.Contains(attendee.Participant) && (mailboxSession == null || !Participant.HasSameEmail(attendee.Participant, new Participant(mailboxSession.MailboxOwner))))
				{
					if (attendee.AttendeeType == AttendeeType.Required)
					{
						messageItem.Recipients.Add(attendee.Participant, RecipientItemType.To);
					}
					else if (attendee.AttendeeType == AttendeeType.Optional)
					{
						messageItem.Recipients.Add(attendee.Participant, RecipientItemType.Cc);
					}
				}
			}
		}

		private void BuildRecipientsFromPostItem()
		{
			MessageItem messageItem = (MessageItem)this.newItem;
			PostItem postItem = (PostItem)this.originalItem;
			if (postItem.From != null)
			{
				messageItem.Recipients.Add(postItem.From, RecipientItemType.To);
			}
		}

		private bool isReplyAll;

		private bool shouldUseSender;
	}
}
