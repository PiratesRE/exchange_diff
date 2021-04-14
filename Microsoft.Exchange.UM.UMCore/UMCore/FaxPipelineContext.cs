using System;
using System.Globalization;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCommon.MessageContent;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class FaxPipelineContext : PipelineContext, IUMCAMessage, IUMResolveCaller
	{
		internal FaxPipelineContext(SubmissionHelper helper, string attachmentName, uint numberOfPages, bool incomplete, UMRecipient recipient, string messageID, ExDateTime sentTime) : base(helper)
		{
			this.tempAttachmentPath = attachmentName;
			this.numberOfPages = numberOfPages;
			this.incomplete = incomplete;
			base.MessageType = "Fax";
			this.recipient = recipient;
			this.recipient.AddReference();
			base.MessageID = messageID;
			base.SentTime = sentTime;
		}

		internal FaxPipelineContext(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				this.recipient = base.CreateRecipientFromObjectGuid(helper.RecipientObjectGuid, helper.TenantGuid);
				this.recipientDialPlan = base.InitializeCallerIdAndTryGetDialPlan(this.recipient);
				if (helper.CustomHeaders.Contains("AttachmentPath"))
				{
					this.attachmentPath = (string)helper.CustomHeaders["AttachmentPath"];
					if (Path.GetExtension(this.attachmentPath) != ".tif")
					{
						CallIdTracer.TraceError(ExTraceGlobals.VoiceMailTracer, 0, "Attachment name is not a Tif file  {0}", new object[]
						{
							this.attachmentPath
						});
						throw new HeaderFileArgumentInvalidException(string.Format(CultureInfo.InvariantCulture, "TIFF {0}: {1}", new object[]
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
				if (!helper.CustomHeaders.Contains("NumberOfPages"))
				{
					throw new HeaderFileArgumentInvalidException("NumberOfPages");
				}
				this.numberOfPages = uint.Parse((string)helper.CustomHeaders["NumberOfPages"], CultureInfo.InvariantCulture);
				if (helper.CustomHeaders.Contains("InComplete"))
				{
					this.incomplete = bool.Parse((string)helper.CustomHeaders["InComplete"]);
				}
				base.MessageType = "Fax";
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
				return false;
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
				return TextPipeline.Instance;
			}
		}

		public override void PostCompletion()
		{
			if (this.attachmentPath != null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, 0, "FaxPipelineContext.PostCompletion - Deleting attachment file '{0}'", new object[]
				{
					this.attachmentPath
				});
				Util.TryDeleteFile(this.attachmentPath);
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

		internal override void SaveMessage()
		{
			if (this.tempAttachmentPath != null)
			{
				this.attachmentPath = Path.Combine(Path.GetDirectoryName(base.HeaderFileName), Path.GetFileNameWithoutExtension(base.HeaderFileName) + ".tif");
				File.Copy(this.tempAttachmentPath, this.attachmentPath);
				this.tempAttachmentPath = null;
			}
			base.SaveMessage();
		}

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
			headerStream.WriteLine("AttachmentPath : " + this.attachmentPath);
			headerStream.WriteLine("InComplete : " + this.incomplete);
			headerStream.WriteLine("NumberOfPages : " + this.numberOfPages);
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<FaxPipelineContext>(this);
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo, this.recipientDialPlan);
			base.MessageToSubmit.Subject = messageContentBuilder.GetFaxSubject(this.ContactInfo, base.CallerId, this.numberOfPages, this.incomplete);
			base.MessageToSubmit[MessageItemSchema.FaxNumberOfPages] = (int)this.numberOfPages;
			string additionalText = null;
			IADSystemConfigurationLookup iadsystemConfigurationLookup = ADSystemConfigurationLookupFactory.CreateFromADRecipient(this.CAMessageRecipient.ADRecipient);
			UMMailboxPolicy policyFromRecipient = iadsystemConfigurationLookup.GetPolicyFromRecipient(this.CAMessageRecipient.ADRecipient);
			if (policyFromRecipient != null)
			{
				additionalText = policyFromRecipient.FaxMessageText;
			}
			messageContentBuilder.AddFaxBody(base.CallerId, this.ContactInfo, additionalText);
			using (MemoryStream memoryStream = messageContentBuilder.ToStream())
			{
				using (Stream stream = base.MessageToSubmit.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			if (this.attachmentPath != null)
			{
				string attachmentName;
				if (this.numberOfPages > 1U)
				{
					attachmentName = Strings.FaxAttachmentInPages(base.CallerId.ToDisplay, this.numberOfPages, ".tif").ToString(base.CultureInfo);
				}
				else
				{
					attachmentName = Strings.FaxAttachmentInPage(base.CallerId.ToDisplay, ".tif").ToString(base.CultureInfo);
				}
				using (FileStream fileStream = File.Open(this.attachmentPath, FileMode.Open, FileAccess.Read))
				{
					XsoUtil.AddAttachment(base.MessageToSubmit.AttachmentCollection, fileStream, attachmentName, "image/tiff");
					CallIdTracer.TraceDebug(ExTraceGlobals.EmailTracer, this, "Successfully attached recorded message.", new object[0]);
				}
			}
			base.MessageToSubmit.ClassName = "IPM.Note.Microsoft.Fax.CA";
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "FaxPipelineContext.Dispose() called", new object[0]);
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

		private string attachmentPath;

		private string tempAttachmentPath;

		private uint numberOfPages;

		private bool incomplete;

		private UMRecipient recipient;

		private UMDialPlan recipientDialPlan;

		private ContactInfo contactInfo;
	}
}
