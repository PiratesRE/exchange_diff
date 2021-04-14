using System;
using System.IO;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Globalization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class MissedCallPipelineContext : PipelineContext, IUMCAMessage, IUMResolveCaller
	{
		internal MissedCallPipelineContext(SubmissionHelper helper) : base(helper)
		{
			bool flag = false;
			try
			{
				base.MessageType = "MissedCall";
				if (helper.CustomHeaders.Contains("Important"))
				{
					this.important = bool.Parse((string)helper.CustomHeaders["Important"]);
				}
				if (helper.CustomHeaders.Contains("Subject"))
				{
					this.subject = (string)helper.CustomHeaders["Subject"];
				}
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

		internal MissedCallPipelineContext(SubmissionHelper helper, bool important, string subject, UMRecipient recipient) : base(helper)
		{
			base.MessageType = "MissedCall";
			this.important = important;
			this.subject = subject;
			this.recipient = recipient;
			this.recipient.AddReference();
		}

		internal MissedCallPipelineContext(SubmissionHelper helper, bool important, string subject, UMRecipient recipient, string messageID, ExDateTime sentTime) : base(helper)
		{
			base.MessageType = "MissedCall";
			this.important = important;
			this.subject = subject;
			this.recipient = recipient;
			this.recipient.AddReference();
			base.MessageID = messageID;
			base.SentTime = sentTime;
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
			CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "MissedCallPipelineContext.PostCompletion - Incrementing missed calls counter", new object[0]);
			Util.IncrementCounter(CallAnswerCounters.CallAnsweringMissedCalls);
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
			if (this.subject != null)
			{
				headerStream.WriteLine("Subject : " + this.subject);
			}
			headerStream.WriteLine("Important : " + this.important.ToString());
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<MissedCallPipelineContext>(this);
		}

		protected override void SetMessageProperties()
		{
			base.SetMessageProperties();
			MessageContentBuilder messageContentBuilder = MessageContentBuilder.Create(base.CultureInfo, this.recipientDialPlan);
			base.MessageToSubmit.Subject = messageContentBuilder.GetMissedCallSubject(this.subject);
			messageContentBuilder.AddMissedCallBody(base.CallerId, this.ContactInfo);
			using (MemoryStream memoryStream = messageContentBuilder.ToStream())
			{
				using (Stream stream = base.MessageToSubmit.Body.OpenWriteStream(new BodyWriteConfiguration(BodyFormat.TextHtml, Charset.UTF8.Name)))
				{
					memoryStream.WriteTo(stream);
				}
			}
			if (this.important)
			{
				base.MessageToSubmit.Importance = Importance.High;
			}
			base.MessageToSubmit.ClassName = "IPM.Note.Microsoft.Missed.Voice";
		}

		protected override void InternalDispose(bool disposing)
		{
			try
			{
				if (disposing)
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.VoiceMailTracer, this.GetHashCode(), "MissedCallPipelineContext.Dispose() called", new object[0]);
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

		private bool important;

		private string subject;

		private UMRecipient recipient;

		private UMDialPlan recipientDialPlan;

		private ContactInfo contactInfo;
	}
}
