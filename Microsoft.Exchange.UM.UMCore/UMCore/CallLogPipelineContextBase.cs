using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal abstract class CallLogPipelineContextBase : PipelineContext, IUMCAMessage, IUMResolveCaller
	{
		internal CallLogPipelineContextBase(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				this.recipient = base.CreateRecipientFromObjectGuid(helper.RecipientObjectGuid, helper.TenantGuid);
				this.recipientDialPlan = base.InitializeCallerIdAndTryGetDialPlan(this.recipient);
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

		internal CallLogPipelineContextBase(SubmissionHelper helper, UMRecipient recipient) : base(helper)
		{
			this.recipient = recipient;
			this.recipient.AddReference();
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

		internal override void WriteCustomHeaderFields(StreamWriter headerStream)
		{
		}

		public override string GetMailboxServerId()
		{
			return base.GetMailboxServerIdHelper();
		}

		public override string GetRecipientIdForThrottling()
		{
			return base.GetRecipientIdHelper();
		}

		protected abstract string GetMessageSubject(MessageContentBuilder contentBuilder);

		protected abstract void AddMessageBody(MessageContentBuilder contentBuilder);

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo, this.recipientDialPlan);
			base.MessageToSubmit.Subject = this.GetMessageSubject(messageContentBuilder);
			this.AddMessageBody(messageContentBuilder);
			using (MemoryStream memoryStream = messageContentBuilder.ToStream())
			{
				using (Stream stream = base.MessageToSubmit.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			base.MessageToSubmit.ClassName = "IPM.Note.Microsoft.Conversation.Voice";
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "CallLogPipelineContextBase.Dispose() called", new object[0]);
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

		private UMRecipient recipient;

		private UMDialPlan recipientDialPlan;

		private ContactInfo contactInfo;
	}
}
