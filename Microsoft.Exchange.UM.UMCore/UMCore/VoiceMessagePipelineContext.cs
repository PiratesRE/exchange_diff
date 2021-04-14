using System;
using System.IO;
using System.Text;
using Microsoft.Exchange.Data.ApplicationLogic.UM;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoiceMessagePipelineContext : VoiceMessagePipelineContextBase, IUMCAMessage, IUMResolveCaller
	{
		internal VoiceMessagePipelineContext(AudioCodecEnum audioCodec, SubmissionHelper helper, string attachmentName, int duration, bool important, string subject, bool callAnswering, bool messageMarkedPrivate, UMRecipient recipient) : base(audioCodec, helper, attachmentName, duration)
		{
			base.MessageType = "SMTPVoiceMail";
			this.important = important;
			this.subject = subject;
			this.callAnswering = callAnswering;
			this.recipient = recipient;
			this.messageMarkedPrivate = messageMarkedPrivate;
			this.recipient.AddReference();
		}

		internal VoiceMessagePipelineContext(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				if (helper.CustomHeaders.Contains("Important"))
				{
					this.important = bool.Parse((string)helper.CustomHeaders["Important"]);
				}
				if (helper.CustomHeaders.Contains("Subject"))
				{
					this.subject = (string)helper.CustomHeaders["Subject"];
				}
				if (helper.CustomHeaders.Contains("CallAnswering"))
				{
					this.callAnswering = bool.Parse((string)helper.CustomHeaders["CallAnswering"]);
				}
				if (helper.CustomHeaders.Contains("Private"))
				{
					this.messageMarkedPrivate = bool.Parse((string)helper.CustomHeaders["Private"]);
				}
				this.recipient = base.CreateRecipientFromObjectGuid(helper.RecipientObjectGuid, helper.TenantGuid);
				this.recipientDialPlan = base.InitializeCallerIdAndTryGetDialPlan(this.recipient);
				UMSubscriber umsubscriber = this.recipient as UMSubscriber;
				if (umsubscriber != null && umsubscriber.ShouldMessageBeProtected(true, this.messageMarkedPrivate))
				{
					this.messageNeedsToBeProtected = true;
				}
				base.TranscriptionUser = (this.recipient as UMSubscriber);
				base.MessageType = "SMTPVoiceMail";
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.Dispose();
				}
			}
		}

		public UMRecipient CAMessageRecipient
		{
			get
			{
				return this.recipient;
			}
		}

		public bool CollectMessageForAnalysis
		{
			get
			{
				UMSubscriber umsubscriber = this.CAMessageRecipient as UMSubscriber;
				if (umsubscriber == null)
				{
					PIIMessage data = PIIMessage.Create(PIIType._PII, this.CAMessageRecipient.ADRecipient.DistinguishedName);
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data, "VoiceMessagePipelineContext.CollectMessageForAnalysis - Recipient '_PII' is not UM Enabled, will not collect voice message", new object[0]);
					return false;
				}
				PIIMessage data2 = PIIMessage.Create(PIIType._PII, umsubscriber.ADRecipient.DistinguishedName);
				UMMailboxPolicy ummailboxPolicy = umsubscriber.UMMailboxPolicy;
				if (!ummailboxPolicy.AllowVoiceMailAnalysis)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data2, "VoiceMessagePipelineContext.CollectMessageForAnalysis - Voice mail collection is not allowed on mailbox policy of recipient '_PII', will not collect voice message", new object[0]);
					return false;
				}
				if (!umsubscriber.ADUMMailboxSettings.VoiceMailAnalysisEnabled)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data2, "VoiceMessagePipelineContext.CollectMessageForAnalysis - Voice mail collection is not enabled on UM mailbox of recipient '_PII', will not collect voice message", new object[0]);
					return false;
				}
				if (this.messageMarkedPrivate || umsubscriber.ShouldMessageBeProtected(true, this.messageMarkedPrivate))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data2, "VoiceMessageCollectionStage.CollectMessageForAnalysis - Voice message of recipient '_PII' is protected or marked private, will not collect voice message", new object[0]);
					return false;
				}
				TranscriptionEnabledSetting transcriptionEnabledInMailboxConfig = umsubscriber.IsTranscriptionEnabledInMailboxConfig(VoiceMailTypeEnum.ReceivedVoiceMails);
				if (UMSubscriber.IsPartnerTranscriptionEnabled(umsubscriber.UMMailboxPolicy, transcriptionEnabledInMailboxConfig) == TranscriptionEnabledSetting.Enabled)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), data2, "VoiceMessageCollectionStage.CollectMessageForAnalysis - Partner transcription is enabled for recipient '_PII', will not collect voice message", new object[0]);
					return false;
				}
				return true;
			}
		}

		public ContactInfo ContactInfo
		{
			get
			{
				return this.contactInfo;
			}
			set
			{
				this.contactInfo = value;
			}
		}

		internal override Pipeline Pipeline
		{
			get
			{
				if (this.messageNeedsToBeProtected)
				{
					return ProtectedAudioPipeline.Instance;
				}
				return AudioPipeline.Instance;
			}
		}

		internal bool IsThisAProtectedMessage
		{
			get
			{
				return this.messageNeedsToBeProtected;
			}
		}

		public override string GetMailboxServerId()
		{
			return base.GetMailboxServerIdHelper();
		}

		public override string GetRecipientIdForThrottling()
		{
			return base.GetRecipientIdHelper();
		}

		public override void PrepareProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContext:PrepareProtectedMessage.", new object[0]);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = ((UMMailboxRecipient)this.CAMessageRecipient).CreateSessionLock())
				{
					using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(mailboxSessionLock.Session))
					{
						base.MessageToSubmit = XsoUtil.CreateTemporaryMessage(mailboxSessionLock.Session, folder, this.temporaryMessageRetentionDays);
						disposeGuard.Add<MessageItem>(base.MessageToSubmit);
						this.SetMessageProperties();
						this.GenerateProtectedContent();
						disposeGuard.Success();
					}
				}
			}
		}

		public override void PrepareNDRForFailureToGenerateProtectedMessage()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContext:PrepareNDRForFailureToGenerateProtectedMessage.", new object[0]);
			this.sentNDRForFailureToGenerateRMSMessage = true;
			UmGlobals.ExEvent.LogEvent(this.CAMessageRecipient.OrganizationId, UMEventLogConstants.Tuple_RMSCallAnsweringSendFailure, this.CAMessageRecipient.OrganizationId.ToString(), this.CAMessageRecipient.ToString(), this.ContactInfo.ToString());
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				using (UMMailboxRecipient.MailboxSessionLock mailboxSessionLock = ((UMMailboxRecipient)this.CAMessageRecipient).CreateSessionLock())
				{
					using (Folder folder = UMStagingFolder.OpenOrCreateUMStagingFolder(mailboxSessionLock.Session))
					{
						base.MessageToSubmit = XsoUtil.CreateTemporaryMessage(mailboxSessionLock.Session, folder, this.temporaryMessageRetentionDays);
						disposeGuard.Add<MessageItem>(base.MessageToSubmit);
						MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo, this.recipientDialPlan);
						base.MessageToSubmit.Subject = messageContentBuilder.GetNDRSubjectForCallAnsweringDRM();
						messageContentBuilder.AddNDRBodyForCADRM(base.CallerId, this.ContactInfo, base.SentTime);
						base.SetNDRProperties(messageContentBuilder);
						base.MessageToSubmit.Save(SaveMode.FailOnAnyConflict);
						disposeGuard.Success();
					}
				}
			}
		}

		public override void PostCompletion()
		{
			Util.IncrementCounter(CallAnswerCounters.CallAnsweringVoiceMessages);
			if (this.messageNeedsToBeProtected)
			{
				try
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContext.PostCompletion - Deleting message from hidden folder", new object[0]);
					base.MessageToSubmit.Session.Delete(DeleteItemFlags.HardDelete, new StoreId[]
					{
						base.MessageToSubmit.Id
					});
				}
				catch (Exception)
				{
				}
				if (this.sentNDRForFailureToGenerateRMSMessage)
				{
					Util.IncrementCounter(CallAnswerCounters.CallAnsweringVoiceMessageProtectionFailures);
					UMEventNotificationHelper.PublishUMFailureEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
				}
				else
				{
					Util.IncrementCounter(CallAnswerCounters.CallAnsweringProtectedVoiceMessages);
					UMEventNotificationHelper.PublishUMSuccessEventNotificationItem(ExchangeComponent.UMProtocol, UMNotificationEvent.ProtectedVoiceMessageEncryptDecrypt.ToString());
				}
			}
			base.PostCompletion();
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			base.WriteCustomHeaderFields(headerStream);
			headerStream.WriteLine("Important : " + this.important);
			headerStream.WriteLine("CallAnswering : " + this.callAnswering);
			if (!string.IsNullOrEmpty(this.subject))
			{
				headerStream.WriteLine("Subject : " + this.subject);
			}
			headerStream.WriteLine("Private : " + this.messageMarkedPrivate);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VoiceMessagePipelineContext>(this);
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContext.Dispose() called", new object[0]);
					if (this.recipient != null)
					{
						this.recipient.ReleaseReference();
					}
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			string durationString = Util.BuildDurationString((int)base.Duration.TotalSeconds, base.CultureInfo);
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo, this.recipientDialPlan);
			base.MessageToSubmit.Subject = messageContentBuilder.GetVoicemailSubject(durationString, this.subject);
			string additionalText = null;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.CAMessageRecipient.ADRecipient);
			UMMailboxPolicy policyFromRecipient = iadsystemConfigurationLookup.GetPolicyFromRecipient(this.CAMessageRecipient.ADRecipient);
			if (policyFromRecipient != null)
			{
				additionalText = policyFromRecipient.VoiceMailText;
			}
			messageContentBuilder.AddVoicemailBody(base.CallerId, this.ContactInfo, additionalText, base.TranscriptionData);
			using (MemoryStream memoryStream = messageContentBuilder.ToStream())
			{
				using (Stream stream = base.MessageToSubmit.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			if (base.TranscriptionData != null && !this.messageNeedsToBeProtected)
			{
				using (Stream stream2 = base.MessageToSubmit.OpenPropertyStream(MessageItemSchema.AsrData, PropertyOpenMode.Create))
				{
					using (StreamWriter streamWriter = new StreamWriter(stream2, Encoding.UTF8))
					{
						streamWriter.Write(base.TranscriptionData.TranscriptionXml.OuterXml);
					}
				}
			}
			if (this.important)
			{
				base.MessageToSubmit.Importance = Importance.High;
			}
			base.MessageToSubmit[MessageItemSchema.VoiceMessageDuration] = (int)base.Duration.TotalSeconds;
			base.MessageToSubmit[MessageItemSchema.UcSubject] = (this.subject ?? string.Empty);
			string text = Util.BuildAttachmentName(base.CallerId.ToDisplay, (int)base.Duration.TotalSeconds, null, base.CultureInfo, base.AudioCodec, this.messageNeedsToBeProtected);
			if (this.messageNeedsToBeProtected)
			{
				XsoUtil.AddHiddenAttachment(base.MessageToSubmit.AttachmentCollection, base.CompressedAudioFile, text, Microsoft.Exchange.UM.UMCommon.AudioCodec.GetMimeType(base.AudioCodec));
			}
			else
			{
				XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, base.CompressedAudioFile, text, Microsoft.Exchange.UM.UMCommon.AudioCodec.GetMimeType(base.AudioCodec));
			}
			XsoUtil.UpdateAttachementOrder(base.MessageToSubmit, text);
			CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Successfully attached recorded message.", new object[0]);
			base.MessageToSubmit.ClassName = (this.callAnswering ? "IPM.Note.Microsoft.Voicemail.UM.CA" : "IPM.Note.Microsoft.Voicemail.UM");
			if (this.messageMarkedPrivate)
			{
				base.MessageToSubmit.Sensitivity = Sensitivity.Private;
			}
		}

		private void GenerateProtectedContent()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContext:GenerateProtectedContent.", new object[0]);
			UMMailboxPolicy ummailboxPolicy = ((UMSubscriber)this.CAMessageRecipient).UMMailboxPolicy;
			LocalizedString? customDRMText = base.GetCustomDRMText(ummailboxPolicy);
			if (this.messageMarkedPrivate)
			{
				base.MessageToSubmit.Sensitivity = Sensitivity.Private;
			}
			if (this.ContactInfo != null & !string.IsNullOrEmpty(this.ContactInfo.EMailAddress))
			{
				new Participant(this.ContactInfo.EMailAddress, this.ContactInfo.EMailAddress, "SMTP");
			}
			base.CreateRightsManagedItem(customDRMText, this.CAMessageRecipient, "IPM.Note.rpmsg.Microsoft.Voicemail.UM.CA");
		}

		private bool important;

		private string subject;

		private bool callAnswering;

		private UMRecipient recipient;

		private UMDialPlan recipientDialPlan;

		private bool messageMarkedPrivate;

		private bool messageNeedsToBeProtected;

		private ContactInfo contactInfo;

		private bool sentNDRForFailureToGenerateRMSMessage;
	}
}
