using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Data.Mapi.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class FailedToRefreshMailboxException : MapiOperationException
	{
		public FailedToRefreshMailboxException(string exception, string mailbox) : base(Strings.FailedToRefreshMailboxExceptionError(exception, mailbox))
		{
			this.exception = exception;
			this.mailbox = mailbox;
		}

		public FailedToRefreshMailboxException(string exception, string mailbox, Exception innerException) : base(Strings.FailedToRefreshMailboxExceptionError(exception, mailbox), innerException)
		{
			this.exception = exception;
			this.mailbox = mailbox;
		}

		protected FailedToRefreshMailboxException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.exception = (string)info.GetValue("exception", typeof(string));
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("exception", this.exception);
			info.AddValue("mailbox", this.mailbox);
		}

		public string Exception
		{
			get
			{
				return this.exception;
			}
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly string exception;

		private readonly string mailbox;
	}
}
