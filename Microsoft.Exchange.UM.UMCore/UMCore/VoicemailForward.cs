using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoicemailForward : ReplyForwardBaseClass
	{
		internal VoicemailForward(BaseUMCallSession vo, StoreObjectId originalId, UMSubscriber user, RetrieveVoicemailManager context) : base(vo, user, context)
		{
			this.originalId = originalId;
		}

		public override void DoPostSubmit()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VoicemailForward::DoPostSubmit", new object[0]);
			base.Session.IncrementCounter(SubscriberAccessCounters.ForwardMessagesSent);
			base.DoPostSubmit();
		}

		protected override MessageItem GenerateMessage(MailboxSession session)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this, "VoicemailForward::GenerateMessage.", new object[0]);
			MessageItem result;
			using (MessageItem messageItem = MessageItem.Bind(session, this.originalId))
			{
				base.SetAttachmentName(messageItem.AttachmentCollection);
				ReplyForwardConfiguration replyForwardConfiguration = new ReplyForwardConfiguration(BodyFormat.TextHtml);
				replyForwardConfiguration.AddBodyPrefix(base.PrepareMessageBodyPrefix(messageItem));
				IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(base.User.ADUser.OrganizationId);
				AcceptedDomain defaultAcceptedDomain = iadsystemConfigurationLookup.GetDefaultAcceptedDomain();
				replyForwardConfiguration.ConversionOptionsForSmime = new InboundConversionOptions(defaultAcceptedDomain.DomainName.ToString());
				replyForwardConfiguration.ConversionOptionsForSmime.UserADSession = ADRecipientLookupFactory.CreateFromUmUser(base.User).ScopedRecipientSession;
				MessageItem messageItem2 = messageItem.CreateForward(XsoUtil.GetDraftsFolderId(session), replyForwardConfiguration);
				messageItem2.ClassName = "IPM.Note.Microsoft.Voicemail.UM";
				messageItem2[MessageItemSchema.VoiceMessageAttachmentOrder] = XsoUtil.GetAttachmentOrderString(messageItem);
				result = messageItem2;
			}
			return result;
		}

		protected override void AddRecordedMessageText(MessageContentBuilder content)
		{
			content.AddRecordedForwardText(base.User.DisplayName);
		}

		private StoreObjectId originalId;
	}
}
