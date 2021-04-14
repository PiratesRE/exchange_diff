using System;
using System.DirectoryServices.ActiveDirectory;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class VoiceMessageCollectionStage : SynchronousPipelineStageBase
	{
		public VoiceMessageCollectionStage()
		{
			if (Utils.RunningInTestMode)
			{
				string text = null;
				using (Domain currentDomain = Domain.GetCurrentDomain())
				{
					text = currentDomain.Name;
				}
				if (text.EndsWith(".extest.microsoft.com", StringComparison.InvariantCultureIgnoreCase))
				{
					this.VoiceMessageCollectionAddress = string.Format(CultureInfo.InvariantCulture, "telexdb2user1@{0}", new object[]
					{
						text
					});
				}
				else
				{
					this.VoiceMessageCollectionAddress = string.Empty;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage - Collection address = '{0}', Domain = '{1}'", new object[]
				{
					this.VoiceMessageCollectionAddress,
					text
				});
			}
		}

		internal override PipelineDispatcher.PipelineResourceType ResourceType
		{
			get
			{
				return PipelineDispatcher.PipelineResourceType.CpuBound;
			}
		}

		internal override TimeSpan ExpectedRunTime
		{
			get
			{
				return TimeSpan.FromMinutes(5.0);
			}
		}

		internal override bool ShouldRunStage(PipelineWorkItem workItem)
		{
			ExAssert.RetailAssert(!string.IsNullOrEmpty(this.VoiceMessageCollectionAddress), "VoiceMessageCollectionAddress should be non-empty");
			IUMCAMessage iumcamessage = base.WorkItem.Message as IUMCAMessage;
			ExAssert.RetailAssert(iumcamessage != null, "VoiceMessageCollectionStage must operate on PipelineContext which implements IUMCAMessage");
			if (!iumcamessage.CollectMessageForAnalysis)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage.ShouldRunStage - Message should not be collected for analysis", new object[0]);
				return false;
			}
			return true;
		}

		protected override StageRetryDetails InternalGetRetrySchedule()
		{
			return new StageRetryDetails(StageRetryDetails.FinalAction.SkipStage);
		}

		protected override void InternalDoSynchronousWork()
		{
			IUMCompressAudio iumcompressAudio = base.WorkItem.Message as IUMCompressAudio;
			ExAssert.RetailAssert(iumcompressAudio != null, "VoiceMessageCollectionStage must operate on PipelineContext that implements IUMCompressAudio.");
			IUMCAMessage iumcamessage = base.WorkItem.Message as IUMCAMessage;
			ExAssert.RetailAssert(iumcamessage != null, "VoiceMessageCollectionStage must operate on PipelineContext which implements IUMCAMessage");
			UMSubscriber umsubscriber = iumcamessage.CAMessageRecipient as UMSubscriber;
			ExAssert.RetailAssert(umsubscriber != null, "VoiceMessageCollectionStage should be run only if message recipient is UM enabled");
			SmtpAddress voiceMessageCollectionAddress = new SmtpAddress(this.VoiceMessageCollectionAddress);
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromOrganizationId(umsubscriber.ADRecipient.OrganizationId);
			MicrosoftExchangeRecipient microsoftExchangeRecipient = iadsystemConfigurationLookup.GetMicrosoftExchangeRecipient();
			SmtpAddress primarySmtpAddress = microsoftExchangeRecipient.PrimarySmtpAddress;
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage - Compress PCM", new object[0]);
			if (iumcompressAudio.FileToCompressPath == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage - No recording", new object[0]);
				return;
			}
			using (ITempFile tempFile = MediaMethods.FromPcm(iumcompressAudio.FileToCompressPath, AudioCodecEnum.G711))
			{
				string requestId = Path.GetFileNameWithoutExtension(base.WorkItem.HeaderFilename) + "-collection";
				using (MessageItem messageItem = MessageItem.CreateInMemory(StoreObjectSchema.ContentConversionProperties))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage - Set message properties", new object[0]);
					this.SetMessageProperties(messageItem, microsoftExchangeRecipient, voiceMessageCollectionAddress, tempFile, umsubscriber, base.WorkItem.Message.CallerId, base.WorkItem.Message.MessageToSubmit);
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessageCollectionStage - Send message via SMTP", new object[0]);
					this.SendMessageForCollection(messageItem, primarySmtpAddress, base.WorkItem.Message.TenantGuid, voiceMessageCollectionAddress, umsubscriber, requestId);
				}
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VoiceMessageCollectionStage>(this);
		}

		private void SetMessageProperties(MessageItem message, MicrosoftExchangeRecipient sender, SmtpAddress voiceMessageCollectionAddress, ITempFile compressedAudioFile, UMSubscriber subscriber, PhoneNumber callerId, MessageItem originalVoiceMessage)
		{
			string text = voiceMessageCollectionAddress.ToString();
			message.Recipients.Add(new Participant(text, text, "SMTP"), RecipientItemType.To);
			message.From = new Participant(subscriber.ADRecipient);
			message.Sender = new Participant(sender);
			message.Subject = originalVoiceMessage.Subject;
			message.ClassName = "IPM.Note.Microsoft.Partner.UM";
			message[MessageItemSchema.XMsExchangeUMPartnerContent] = "voice";
			message[MessageItemSchema.XMsExchangeUMPartnerContext] = string.Empty;
			if (callerId != null)
			{
				message[MessageItemSchema.SenderTelephoneNumber] = (callerId.ToDial ?? string.Empty);
			}
			message[MessageItemSchema.VoiceMessageDuration] = originalVoiceMessage[MessageItemSchema.VoiceMessageDuration];
			message[MessageItemSchema.VoiceMessageSenderName] = originalVoiceMessage[MessageItemSchema.VoiceMessageSenderName];
			message[MessageItemSchema.XMsExchangeUMDialPlanLanguage] = subscriber.DialPlan.DefaultLanguage.Culture.Name;
			message[MessageItemSchema.XMsExchangeUMCallerInformedOfAnalysis] = (subscriber.UMMailboxPolicy.InformCallerOfVoiceMailAnalysis ? "true" : "false");
			message.AutoResponseSuppress = AutoResponseSuppress.All;
			XsoUtil.AddAttachment(message.AttachmentCollection, compressedAudioFile, Path.GetFileName(compressedAudioFile.FilePath), AudioCodec.GetMimeType(AudioCodecEnum.G711));
		}

		private void SendMessageForCollection(MessageItem message, SmtpAddress senderAddress, Guid senderOrgGuid, SmtpAddress voiceMessageCollectionAddress, UMSubscriber subscriber, string requestId)
		{
			OutboundConversionOptions outboundConversionOptions = XsoUtil.GetOutboundConversionOptions(subscriber);
			SmtpSubmissionHelper.SubmitMessage(message, senderAddress.ToString(), senderOrgGuid, voiceMessageCollectionAddress.ToString(), outboundConversionOptions, requestId);
		}

		private readonly string VoiceMessageCollectionAddress = "vmailng@microsoft.com";
	}
}
