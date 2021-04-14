using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.MailboxReplicationService;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxRelocationSplitSourceNotInContainerException : MailboxReplicationPermanentException
	{
		public MailboxRelocationSplitSourceNotInContainerException(string mailbox) : base(Strings.ErrorMailboxRelocationSplitSourceNotInContainer(mailbox))
		{
			this.mailbox = mailbox;
		}

		public MailboxRelocationSplitSourceNotInContainerException(string mailbox, Exception innerException) : base(Strings.ErrorMailboxRelocationSplitSourceNotInContainer(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected MailboxRelocationSplitSourceNotInContainerException(SerializationInfo info, StreamingContext context) : base(info, context)
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
