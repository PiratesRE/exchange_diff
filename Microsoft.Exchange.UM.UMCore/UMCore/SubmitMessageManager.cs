using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class SubmitMessageManager : SendMessageManager
	{
		internal SubmitMessageManager(ActivityManager manager, SubmitMessageManager.ConfigClass config) : base(manager, config)
		{
		}

		internal bool DrmIsEnabled
		{
			get
			{
				return this.drmIsEnabled;
			}
		}

		internal override void Start(BaseUMCallSession vo, string refInfo)
		{
			this.drmIsEnabled = (vo.CurrentCallContext.CallerInfo != null && vo.CurrentCallContext.CallerInfo.DRMPolicyForInterpersonal != DRMProtectionOptions.None);
			base.Start(vo, refInfo);
		}

		internal override TransitionBase ExecuteAction(string action, BaseUMCallSession vo)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SubmitMessageManager asked to do action: {0}.", new object[]
			{
				action
			});
			return base.ExecuteAction(action, vo);
		}

		internal override void DropCall(BaseUMCallSession vo, DropCallReason reason)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "SubmitMessageManager::DropCall.", new object[0]);
			if (base.CurrentActivity is Record)
			{
				UMSubscriber callerInfo = vo.CurrentCallContext.CallerInfo;
				if (base.RecordContext.TotalSeconds > 0)
				{
					this.SendMessage(vo);
				}
			}
			base.DropCall(vo, reason);
		}

		protected override string SendMessage(BaseUMCallSession vo)
		{
			base.SendMsg = new SubmitMessageManager.SearchRecordedMessage(vo, vo.CurrentCallContext.CallerInfo, this);
			return base.SendMessage(vo);
		}

		private bool drmIsEnabled;

		internal class ConfigClass : ActivityManagerConfig
		{
			public ConfigClass(ActivityManagerConfig manager) : base(manager)
			{
			}

			internal override ActivityManager CreateActivityManager(ActivityManager manager)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.StateMachineTracer, this, "Constructing Submit Message activity manager.", new object[0]);
				return new SubmitMessageManager(manager, this);
			}
		}

		internal class SearchRecordedMessage : XsoRecordedMessage
		{
			internal SearchRecordedMessage(BaseUMCallSession vo, UMSubscriber user, ActivityManager manager) : base(vo, user, manager, true)
			{
			}

			protected override bool IsPureVoiceMessage
			{
				get
				{
					return true;
				}
			}

			protected override MessageItem GenerateMessage(MailboxSession session)
			{
				MessageItem result;
				using (Folder folder = Folder.Bind(session, XsoUtil.GetDraftsFolderId(session)))
				{
					string text = Strings.VoiceMessageSubject(Util.BuildDurationString(base.Manager.RecordContext.TotalSeconds, base.User.TelephonyCulture)).ToString(base.User.TelephonyCulture);
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "RecordedMessage::GenerateMessage creating message with subject={0}.", new object[]
					{
						text
					});
					MessageItem messageItem = MessageItem.Create(session, folder.Id);
					messageItem.Subject = text;
					base.SetAttachmentName(null);
					messageItem.ClassName = "IPM.Note.Microsoft.Voicemail.UM";
					string s = this.PrepareMessageBody();
					using (Stream stream = messageItem.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
					{
						byte[] bytes = Encoding.UTF8.GetBytes(s);
						stream.Write(bytes, 0, bytes.Length);
					}
					result = messageItem;
				}
				return result;
			}

			protected override MessageItem GenerateProtectedMessage(MailboxSession session)
			{
				return this.GenerateMessage(session);
			}

			private string PrepareMessageBody()
			{
				MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.User.TelephonyCulture);
				messageContentBuilder.AddVoicemailBody(new ADContactInfo((IADOrgPerson)base.User.ADRecipient, FoundByType.NotSpecified), null);
				return messageContentBuilder.ToString();
			}
		}
	}
}
