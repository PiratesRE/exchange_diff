using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class RequestIsProtectedPermanentException : MailboxReplicationPermanentException
	{
		public RequestIsProtectedPermanentException(string mailbox) : base(Strings.ErrorRequestIsProtected(mailbox))
		{
			this.mailbox = mailbox;
		}

		public RequestIsProtectedPermanentException(string mailbox, Exception innerException) : base(Strings.ErrorRequestIsProtected(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected RequestIsProtectedPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (string)info.GetValue("mailbox", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
		}

		public string Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly string mailbox;
	}
}
