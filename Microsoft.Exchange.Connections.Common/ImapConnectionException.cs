using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Connections.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ImapConnectionException : OperationLevelTransientException
	{
		public ImapConnectionException(string imapConnectionErrorMsg, RetryPolicy retryPolicy) : base(CXStrings.ImapConnectionErrorMsg(imapConnectionErrorMsg, retryPolicy))
		{
			this.imapConnectionErrorMsg = imapConnectionErrorMsg;
			this.retryPolicy = retryPolicy;
		}

		public ImapConnectionException(string imapConnectionErrorMsg, RetryPolicy retryPolicy, Exception innerException) : base(CXStrings.ImapConnectionErrorMsg(imapConnectionErrorMsg, retryPolicy), innerException)
		{
			this.imapConnectionErrorMsg = imapConnectionErrorMsg;
			this.retryPolicy = retryPolicy;
		}

		protected ImapConnectionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.imapConnectionErrorMsg = (string)info.GetValue("imapConnectionErrorMsg", typeof(string));
			this.retryPolicy = (RetryPolicy)info.GetValue("retryPolicy", typeof(RetryPolicy));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("imapConnectionErrorMsg", this.imapConnectionErrorMsg);
			info.AddValue("retryPolicy", this.retryPolicy);
		}

		public string ImapConnectionErrorMsg
		{
			get
			{
				return this.imapConnectionErrorMsg;
			}
		}

		public RetryPolicy RetryPolicy
		{
			get
			{
				return this.retryPolicy;
			}
		}

		private readonly string imapConnectionErrorMsg;

		private readonly RetryPolicy retryPolicy;
	}
}
