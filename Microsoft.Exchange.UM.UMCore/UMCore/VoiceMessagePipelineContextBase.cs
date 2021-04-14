using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Microsoft.Exchange.Compliance.Xml;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Security.RightsManagement;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.FaultInjection;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class VoiceMessagePipelineContextBase : PipelineContext, IUMCompressAudio, IUMTranscribeAudio
	{
		protected VoiceMessagePipelineContextBase(AudioCodecEnum audioCodec, SubmissionHelper helper, string attachmentName, int duration) : base(helper)
		{
			this.tempAttachmentPath = attachmentName;
			this.audioCodec = audioCodec;
			this.duration = TimeSpan.FromSeconds((double)duration);
		}

		protected VoiceMessagePipelineContextBase(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				if (!helper.CustomHeaders.Contains("Duration"))
				{
					throw new HeaderFileArgumentInvalidException("Duration");
				}
				int num = int.Parse((string)helper.CustomHeaders["Duration"], CultureInfo.InvariantCulture);
				this.duration = TimeSpan.FromSeconds((double)num);
				if (!helper.CustomHeaders.Contains("AttachmentPath"))
				{
					throw new HeaderFileArgumentInvalidException("AttachmentPath");
				}
				if (this.duration.TotalSeconds > 0.0)
				{
					this.attachmentPath = (string)helper.CustomHeaders["AttachmentPath"];
					if (!string.Equals(Path.GetExtension(this.attachmentPath), ".wav", StringComparison.InvariantCultureIgnoreCase))
					{
						CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Attachment name is not a wav file  {0}", new object[]
						{
							this.attachmentPath
						});
						throw new HeaderFileArgumentInvalidException(string.Format(CultureInfo.InvariantCulture, "WAV {0}: {1}", new object[]
						{
							"AttachmentPath",
							this.attachmentPath
						}));
					}
					if (!File.Exists(this.attachmentPath))
					{
						CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Attachment file {0} does not exist", new object[]
						{
							this.attachmentPath
						});
						throw new HeaderFileArgumentInvalidException(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", new object[]
						{
							"AttachmentPath",
							this.attachmentPath
						}));
					}
				}
				else
				{
					this.attachmentPath = null;
				}
				if (!helper.CustomHeaders.Contains("Codec"))
				{
					throw new HeaderFileArgumentInvalidException("Codec");
				}
				this.audioCodec = (AudioCodecEnum)Enum.Parse(typeof(AudioCodecEnum), (string)helper.CustomHeaders["Codec"]);
				if (helper.CustomHeaders.Contains("TranscriptionData"))
				{
					XmlDocument xmlDocument = new SafeXmlDocument();
					try
					{
						xmlDocument.LoadXml((string)helper.CustomHeaders["TranscriptionData"]);
					}
					catch (XmlException innerException)
					{
						throw new HeaderFileArgumentInvalidException("TranscriptionData", innerException);
					}
					this.TranscriptionData = new PartnerTranscriptionData(xmlDocument);
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

		public AudioCodecEnum AudioCodec
		{
			get
			{
				return this.audioCodec;
			}
		}

		public ITempFile CompressedAudioFile
		{
			get
			{
				return this.compressedAudioFile;
			}
			set
			{
				this.compressedAudioFile = value;
			}
		}

		public string FileToCompressPath
		{
			get
			{
				return this.attachmentPath;
			}
		}

		public string AttachmentPath
		{
			get
			{
				return this.attachmentPath;
			}
		}

		public ITranscriptionData TranscriptionData
		{
			get
			{
				return this.transcriptionData;
			}
			set
			{
				this.transcriptionData = value;
			}
		}

		public UMSubscriber TranscriptionUser { get; protected set; }

		public TimeSpan Duration
		{
			get
			{
				return this.duration;
			}
		}

		public virtual bool EnableTopNGrammar
		{
			get
			{
				return false;
			}
		}

		public override void PostCompletion()
		{
			if (this.attachmentPath != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "VoiceMessagePipelineContextBase.PostCompletion - Deleting attachment file '{0}'", new object[]
				{
					this.attachmentPath
				});
				Util.TryDeleteFile(this.attachmentPath);
			}
			if (this.compressedAudioFile != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "VoiceMessagePipelineContextBase.PostCompletion - Deleting compressed audio file '{0}'", new object[]
				{
					this.compressedAudioFile.FilePath
				});
				this.compressedAudioFile.Dispose();
			}
			base.PostCompletion();
		}

		internal override void SaveMessage()
		{
			if (this.tempAttachmentPath != null)
			{
				this.attachmentPath = Path.Combine(Path.GetDirectoryName(base.HeaderFileName), Path.GetFileNameWithoutExtension(base.HeaderFileName) + ".wav");
				File.Copy(this.tempAttachmentPath, this.attachmentPath);
				this.tempAttachmentPath = null;
			}
			base.SaveMessage();
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			string str = this.attachmentPath ?? "null";
			headerStream.WriteLine("AttachmentPath : " + str);
			headerStream.WriteLine("Duration : " + (int)this.duration.TotalSeconds);
			headerStream.WriteLine("Codec : " + this.audioCodec.ToString("g"));
			if (this.TranscriptionData != null)
			{
				headerStream.WriteLine("TranscriptionData : " + this.TranscriptionData.TranscriptionXml.OuterXml);
			}
		}

		protected void CreateRightsManagedItem(LocalizedString? outerMessageBody, UMRecipient user, string contentClass)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContextBase:CreateRightsManagedItem.", new object[0]);
			using (DisposeGuard disposeGuard = default(DisposeGuard))
			{
				FaultInjectionUtils.FaultInjectException(3034983741U);
				RightsManagedMessageItem rightsManagedMessageItem = RightsManagedMessageItem.Create(base.MessageToSubmit, XsoUtil.GetOutboundConversionOptions(user));
				disposeGuard.Add<RightsManagedMessageItem>(rightsManagedMessageItem);
				rightsManagedMessageItem.SetRestriction(RmsTemplate.DoNotForward);
				if (this.TranscriptionData != null)
				{
					this.AddASRDataAttachment(rightsManagedMessageItem);
				}
				rightsManagedMessageItem.SetDefaultEnvelopeBody(outerMessageBody);
				rightsManagedMessageItem.ClassName = contentClass;
				rightsManagedMessageItem[StoreObjectSchema.ContentClass] = contentClass;
				rightsManagedMessageItem.Save(SaveMode.FailOnAnyConflict);
				disposeGuard.Success();
				base.MessageToSubmit = rightsManagedMessageItem;
			}
		}

		protected void AddASRDataAttachment(RightsManagedMessageItem message)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "VoiceMessagePipelineContextBase:AddASRDataAttachment.", new object[0]);
			using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(this.TranscriptionData.TranscriptionXml.OuterXml)))
			{
				XsoUtil.AddHiddenAttachment(message.ProtectedAttachmentCollection, memoryStream, "voicemail.umrmasr", "text");
			}
		}

		protected void SetNDRProperties(MessageContentBuilder content)
		{
			base.MessageToSubmit.AutoResponseSuppress = AutoResponseSuppress.All;
			using (MemoryStream memoryStream = content.ToStream())
			{
				using (Stream stream = base.MessageToSubmit.Body.OpenWriteStream(new BodyWriteConfiguration(Microsoft.Exchange.Data.Storage.BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			base.MessageToSubmit.ClassName = "IPM.Note";
			base.MessageToSubmit.Importance = Importance.High;
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<VoiceMessagePipelineContextBase>(this);
		}

		protected LocalizedString? GetCustomDRMText(UMMailboxPolicy policy)
		{
			LocalizedString? result = null;
			if (policy != null)
			{
				if (string.IsNullOrEmpty(policy.ProtectedVoiceMailText))
				{
					result = new LocalizedString?(Strings.UMBodyDownload);
				}
				else
				{
					result = new LocalizedString?(new LocalizedString(policy.ProtectedVoiceMailText));
				}
			}
			return result;
		}

		private const string ASRAttachmentNameForDRMVoicemails = "voicemail.umrmasr";

		private string attachmentPath;

		private string tempAttachmentPath;

		private ITempFile compressedAudioFile;

		private TimeSpan duration;

		private AudioCodecEnum audioCodec;

		private ITranscriptionData transcriptionData;

		protected int temporaryMessageRetentionDays = 7;
	}
}
