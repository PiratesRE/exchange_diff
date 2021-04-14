using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxRelocationJoinTargetNotContainerOwnerException : MailboxReplicationPermanentException
	{
		public MailboxRelocationJoinTargetNotContainerOwnerException(string mailbox) : base(Strings.ErrorMailboxRelocationJoinTargetNotContainerOwner(mailbox))
		{
			this.mailbox = mailbox;
		}

		public MailboxRelocationJoinTargetNotContainerOwnerException(string mailbox, Exception innerException) : base(Strings.ErrorMailboxRelocationJoinTargetNotContainerOwner(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected MailboxRelocationJoinTargetNotContainerOwnerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
