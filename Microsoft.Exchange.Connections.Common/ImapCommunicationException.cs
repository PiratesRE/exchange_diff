using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapCommunicationException : OperationLevelTransientException
	{
		public ImapCommunicationException(string imapCommunicationErrorMsg, RetryPolicy retryPolicy) : base(CXStrings.ImapCommunicationErrorMsg(imapCommunicationErrorMsg, retryPolicy))
		{
			this.imapCommunicationErrorMsg = imapCommunicationErrorMsg;
			this.retryPolicy = retryPolicy;
		}

		public ImapCommunicationException(string imapCommunicationErrorMsg, RetryPolicy retryPolicy, Exception innerException) : base(CXStrings.ImapCommunicationErrorMsg(imapCommunicationErrorMsg, retryPolicy), innerException)
		{
			this.imapCommunicationErrorMsg = imapCommunicationErrorMsg;
			this.retryPolicy = retryPolicy;
		}

		protected ImapCommunicationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.imapCommunicationErrorMsg = (string)info.GetValue("imapCommunicationErrorMsg", typeof(string));
			this.retryPolicy = (RetryPolicy)info.GetValue("retryPolicy", typeof(RetryPolicy));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("imapCommunicationErrorMsg", this.imapCommunicationErrorMsg);
			info.AddValue("retryPolicy", this.retryPolicy);
		}

		public string ImapCommunicationErrorMsg
		{
			get
			{
				return this.imapCommunicationErrorMsg;
			}
		}

		public RetryPolicy RetryPolicy
		{
			get
			{
				return this.retryPolicy;
			}
		}

		private readonly string imapCommunicationErrorMsg;

		private readonly RetryPolicy retryPolicy;
	}
}
