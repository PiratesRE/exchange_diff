using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class PartnerTranscriptionRequestPipelineContext : VoiceMessagePipelineContextBase, IUMCAMessage
	{
		internal PartnerTranscriptionRequestPipelineContext(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				base.MessageType = "PartnerTranscriptionRequest";
				if (!helper.CustomHeaders.Contains("PartnerTranscriptionContext"))
				{
					throw new HeaderFileArgumentInvalidException("PartnerTranscriptionContext");
				}
				string base64Data = helper.CustomHeaders["PartnerTranscriptionContext"] as string;
				this.partnerContext = UMPartnerContext.Parse<UMPartnerTranscriptionContext>(base64Data);
				if (!helper.CustomHeaders.Contains("SenderObjectGuid"))
				{
					throw new HeaderFileArgumentInvalidException("SenderObjectGuid");
				}
				Guid objectGuid = new Guid(helper.CustomHeaders["SenderObjectGuid"] as string);
				this.subscriber = (UMSubscriber)base.CreateRecipientFromObjectGuid(objectGuid, helper.TenantGuid);
				if (helper.CustomHeaders.Contains("MessageFilePath"))
				{
					this.interpersonalMessagePath = (helper.CustomHeaders["MessageFilePath"] as string);
				}
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

		internal PartnerTranscriptionRequestPipelineContext(SubmissionHelper helper, UMSubscriber subscriber, UMSubscriber caller, string waveFilePath, MessageItem ipmMessage, string ipmAttachName, bool isCallAnswering, AudioCodecEnum codec, bool isImportant, string subject, int duration) : base(AudioCodecEnum.G711, helper, waveFilePath, duration)
		{
			FileInfo fileInfo = new FileInfo(waveFilePath);
			base.MessageType = "PartnerTranscriptionRequest";
			this.subscriber = subscriber;
			this.subscriber.AddReference();
			this.interpersonalMessage = ipmMessage;
			UMMailboxPolicy ummailboxPolicy = this.subscriber.UMMailboxPolicy;
			this.partnerContext = UMPartnerContext.Create<UMPartnerTranscriptionContext>();
			this.partnerContext.IpmAttachmentName = ipmAttachName;
			this.partnerContext.IsCallAnsweringMessage = isCallAnswering;
			this.partnerContext.IsImportant = isImportant;
			this.partnerContext.CallId = helper.CallId;
			this.partnerContext.SessionId = helper.CallId;
			this.partnerContext.Subject = subject;
			this.partnerContext.Duration = duration;
			this.partnerContext.Culture = new CultureInfo(helper.CultureInfo);
			this.partnerContext.CreationTime = new ExDateTime(ExTimeZone.UtcTimeZone, fileInfo.CreationTimeUtc);
			this.partnerContext.CallingParty = helper.CallerId.ToDial;
			this.partnerContext.AudioCodec = codec.ToString();
			this.partnerContext.PartnerAddress = ummailboxPolicy.VoiceMailPreviewPartnerAddress.Value;
			this.partnerContext.PartnerMaxDeliveryDelay = ummailboxPolicy.VoiceMailPreviewPartnerMaxDeliveryDelay;
			this.partnerContext.CallerGuid = ((caller != null) ? caller.ADRecipient.Guid : Guid.Empty);
			this.partnerContext.CallerName = ((caller != null) ? caller.DisplayName : string.Empty);
			this.partnerContext.CallerIdDisplayName = helper.CallerIdDisplayName;
			this.partnerContext.UMDialPlanLanguage = this.subscriber.DialPlan.DefaultLanguage.Culture.Name;
			this.partnerContext.CallerInformedOfAnalysis = (ummailboxPolicy.InformCallerOfVoiceMailAnalysis ? "true" : "false");
		}

		public UMRecipient CAMessageRecipient
		{
			get
			{
				return this.subscriber;
			}
		}

		public bool CollectMessageForAnalysis
		{
			get
			{
				return false;
			}
		}

		internal override Pipeline Pipeline
		{
			get
			{
				return PartnerTranscriptionRequestPipeline.Instance;
			}
		}

		public override void PostCompletion()
		{
			if (!string.IsNullOrEmpty(this.interpersonalMessagePath))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "PartnerTranscriptionRequestPipelineContext.PostCompletion - Removing msg file '{0}'", new object[]
				{
					this.interpersonalMessagePath
				});
				Util.TryDeleteFile(this.interpersonalMessagePath);
				this.interpersonalMessagePath = null;
			}
			base.PostCompletion();
		}

		public override string GetMailboxServerId()
		{
			return base.GetMailboxServerIdHelper();
		}

		public override string GetRecipientIdForThrottling()
		{
			return base.GetRecipientIdHelper();
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			base.WriteCustomHeaderFields(headerStream);
			headerStream.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				"PartnerTranscriptionContext",
				" : ",
				this.partnerContext
			}));
			headerStream.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
			{
				"SenderObjectGuid",
				" : ",
				this.subscriber.ADRecipient.Guid
			}));
			if (!string.IsNullOrEmpty(this.interpersonalMessagePath))
			{
				headerStream.WriteLine(string.Format(CultureInfo.InvariantCulture, "{0}{1}{2}", new object[]
				{
					"MessageFilePath",
					" : ",
					this.interpersonalMessagePath
				}));
			}
		}

		internal override void SaveMessage()
		{
			if (this.interpersonalMessage != null)
			{
				this.interpersonalMessagePath = Path.Combine(Path.GetDirectoryName(base.HeaderFileName), Path.GetFileNameWithoutExtension(base.HeaderFileName) + ".msg");
				XSOVoiceMessagePipelineContext.SaveAndDeleteMessageItem(this.interpersonalMessage, this.interpersonalMessagePath, this.subscriber);
				this.interpersonalMessage = null;
			}
			base.SaveMessage();
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<PartnerTranscriptionRequestPipelineContext>(this);
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			this.partnerContext.PcmAudioAttachmentName = new FileInfo(base.FileToCompressPath).Name;
			this.partnerContext.PartnerAudioAttachmentName = new FileInfo(base.CompressedAudioFile.FilePath).Name;
			using (FileStream fileStream = new FileStream(base.CompressedAudioFile.FilePath, FileMode.Open, FileAccess.Read))
			{
				XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, fileStream, this.partnerContext.PartnerAudioAttachmentName, "audio/wav");
			}
			using (FileStream fileStream2 = new FileStream(base.FileToCompressPath, FileMode.Open, FileAccess.Read))
			{
				XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, fileStream2, this.partnerContext.PcmAudioAttachmentName, "audio/wav");
			}
			if (this.interpersonalMessagePath != null)
			{
				using (FileStream fileStream3 = new FileStream(this.interpersonalMessagePath, FileMode.Open, FileAccess.Read))
				{
					XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, fileStream3, this.partnerContext.IpmAttachmentName, "application/octet-stream");
				}
			}
			UMMailboxPolicy ummailboxPolicy = this.subscriber.UMMailboxPolicy;
			base.MessageToSubmit.ClassName = "IPM.Note.Microsoft.Partner.UM.TranscriptionRequest";
			base.MessageToSubmit.Subject = base.MessageToSubmit.ClassName;
			base.MessageToSubmit[MessageItemSchema.XMsExchangeUMPartnerContext] = this.partnerContext.ToString();
			base.MessageToSubmit[MessageItemSchema.XMsExchangeUMPartnerContent] = "voice";
			base.MessageToSubmit[MessageItemSchema.XMsExchangeUMPartnerAssignedID] = ummailboxPolicy.VoiceMailPreviewPartnerAssignedID;
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing && this.subscriber != null)
				{
					this.subscriber.ReleaseReference();
				}
			}
			finally
			{
				base.InternalDispose(disposing);
			}
		}

		private UMPartnerTranscriptionContext partnerContext;

		private UMSubscriber subscriber;

		private MessageItem interpersonalMessage;

		private string interpersonalMessagePath;
	}
}
