using System;

namespace Microsoft.Exchange.Rpc.MailSubmission
{
	internal class MailSubmissionResult
	{
		public MailSubmissionResult(uint ec)
		{
			this.errorCode = ec;
			base..ctor();
		}

		public MailSubmissionResult()
		{
		}

		public uint ErrorCode
		{
			get
			{
				return this.errorCode;
			}
			set
			{
				this.errorCode = value;
			}
		}

		public string Sender
		{
			get
			{
				return this.sender;
			}
			set
			{
				this.sender = value;
			}
		}

		public string From
		{
			get
			{
				return this.from;
			}
			set
			{
				this.from = value;
			}
		}

		public string MessageId
		{
			get
			{
				return this.internetMessageId;
			}
			set
			{
				this.internetMessageId = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			set
			{
				this.subject = value;
			}
		}

		public string DiagnosticInfo
		{
			get
			{
				return this.diagnosticInfo;
			}
			set
			{
				this.diagnosticInfo = value;
			}
		}

		private uint errorCode;

		private string sender;

		private string from;

		private string internetMessageId;

		private string subject;

		private string diagnosticInfo;
	}
}
