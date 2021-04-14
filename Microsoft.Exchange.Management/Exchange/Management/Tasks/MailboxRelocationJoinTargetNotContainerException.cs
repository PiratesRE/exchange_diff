using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxRelocationJoinTargetNotContainerException : MailboxReplicationPermanentException
	{
		public MailboxRelocationJoinTargetNotContainerException(string mailbox) : base(Strings.ErrorMailboxRelocationJoinTargetNotContainer(mailbox))
		{
			this.mailbox = mailbox;
		}

		public MailboxRelocationJoinTargetNotContainerException(string mailbox, Exception innerException) : base(Strings.ErrorMailboxRelocationJoinTargetNotContainer(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected MailboxRelocationJoinTargetNotContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
