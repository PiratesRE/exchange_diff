using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoicemailReply : ReplyForwardBaseClass
	{
		internal VoicemailReply(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, ContactInfo sender, RetrieveVoicemailManager context, bool isOriginalMessageIsProtected) : base(vo, user, context)
		{
			this.originalId = originalId;
			this.originalMessageProtected = isOriginalMessageIsProtected;
			if (sender.ADOrgPerson == null || string.IsNullOrEmpty(sender.ADOrgPerson.LegacyExchangeDN))
			{
				this.sender = new Participant(sender.DisplayName, sender.EMailAddress, null);
				return;
			}
			this.sender = new Participant(sender.DisplayName, sender.ADOrgPerson.LegacyExchangeDN, "EX");
		}

		protected override bool IsReplyToAProtectedMessage
		{
			get
			{
				return this.originalMessageProtected;
			}
		}

		public override void DoPostSubmit()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VoicemailReply::DoPostSubmit.", new object[0]);
			base.Session.IncrementCounter(SubscriberAccessCounters.ReplyMessagesSent);
			base.DoPostSubmit();
		}

		protected virtual MessageItem InternalCreateReply(MessageItem original, MailboxSession session, ReplyForwardConfiguration replyConfiguration)
		{
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = original.CreateReply(XsoUtil.GetDraftsFolderId(session), replyConfiguration);
				disposeGuard.Add<MessageItem>(messageItem);
				messageItem.Recipients.Clear();
				messageItem.Recipients.Add(this.sender);
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		protected override MessageItem GenerateMessage(MailboxSession session)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VoicemailReply::GenerateMessage.", new object[0]);
			MessageItem result;
			using (MessageItem messageItem = MessageItem.Bind(session, this.originalId))
			{
				base.SetAttachmentName(messageItem.AttachmentCollection);
				MessageItem messageItem2 = this.CreateReply(messageItem, session);
				this.CopyOverAttachments(messageItem2.AttachmentCollection, messageItem.AttachmentCollection);
				result = messageItem2;
			}
			return result;
		}

		protected override MessageItem GenerateProtectedMessage(MailboxSession session)
		{
			if (this.IsReplyToAProtectedMessage)
			{
				using (RightsManagedMessageItem rightsManagedMessageItem = RightsManagedMessageItem.Bind(session, this.originalId, XsoUtil.GetOutboundConversionOptions(base.User)))
				{
					base.SetAttachmentName(rightsManagedMessageItem.ProtectedAttachmentCollection);
					RightsManagedMessageItem rightsManagedMessageItem2 = (RightsManagedMessageItem)this.CreateReply(rightsManagedMessageItem, session);
					this.CopyOverAttachments(rightsManagedMessageItem2.ProtectedAttachmentCollection, rightsManagedMessageItem.ProtectedAttachmentCollection);
					return rightsManagedMessageItem2;
				}
			}
			return base.GenerateProtectedMessage(session);
		}

		protected override void AddRecordedMessageText(MessageContentBuilder content)
		{
			content.AddRecordedReplyText(base.User.DisplayName);
		}

		private void CopyOverAttachments(AttachmentCollection destAttachCollection, AttachmentCollection originalAttachCollection)
		{
			foreach (AttachmentHandle handle in originalAttachCollection)
			{
				using (Attachment attachment = originalAttachCollection.Open(handle))
				{
					StreamAttachment streamAttachment = attachment as StreamAttachment;
					if (streamAttachment != null)
					{
						Stream stream;
						if (XsoUtil.IsValidProtectedAudioAttachment(streamAttachment))
						{
							stream = DRMUtils.OpenProtectedAttachment(streamAttachment, base.User.ADUser.OrganizationId);
						}
						else
						{
							if (!XsoUtil.IsValidAudioAttachment(streamAttachment))
							{
								continue;
							}
							stream = streamAttachment.GetContentStream();
						}
						using (StreamAttachment streamAttachment2 = (StreamAttachment)destAttachCollection.Create(AttachmentType.Stream))
						{
							using (Stream contentStream = streamAttachment2.GetContentStream())
							{
								using (DisposeGuard disposeGuard = default(DisposeGuard))
								{
									disposeGuard.Add<Stream>(stream);
									CommonUtil.CopyStream(stream, contentStream);
									streamAttachment2.FileName = streamAttachment.FileName;
									streamAttachment2.ContentType = streamAttachment.ContentType;
									streamAttachment2.Save();
								}
							}
						}
					}
				}
			}
		}

		private MessageItem CreateReply(MessageItem original, MailboxSession session)
		{
			ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(BodyFormat.TextHtml);
			replyForwardConfiguration.AddBodyPrefix(base.PrepareMessageBodyPrefix(original));
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.User.ADUser.OrganizationId);
			AcceptedDomain defaultAcceptedDomain = iadsystemConfigurationLookup.GetDefaultAcceptedDomain();
			replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
			replyForwardConfiguration.ConversionOptionsForSmime.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(base.User).ScopedRecipientSession;
			MessageItem result;
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				MessageItem messageItem = this.InternalCreateReply(original, session, replyForwardConfiguration);
				disposeGuard.Add<MessageItem>(messageItem);
				messageItem.ClassName = "IPM.Note.Microsoft.Voicemail.UM";
				messageItem[MessageItemSchema.VoiceMessageAttachmentOrder] = XsoUtil.GetAttachmentOrderString(original);
				disposeGuard.Success();
				result = messageItem;
			}
			return result;
		}

		private StoreObjectId originalId;

		private Participant sender;

		private bool originalMessageProtected;
	}
}
