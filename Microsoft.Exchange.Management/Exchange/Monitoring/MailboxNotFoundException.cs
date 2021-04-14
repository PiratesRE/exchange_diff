using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxNotFoundException : LocalizedException
	{
		public MailboxNotFoundException(MailboxIdParameter mailbox) : base(Strings.messageMailboxNotFoundException(mailbox))
		{
			this.mailbox = mailbox;
		}

		public MailboxNotFoundException(MailboxIdParameter mailbox, Exception innerException) : base(Strings.messageMailboxNotFoundException(mailbox), innerException)
		{
			this.mailbox = mailbox;
		}

		protected MailboxNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailbox = (MailboxIdParameter)info.GetValue("mailbox", typeof(MailboxIdParameter));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailbox", this.mailbox);
		}

		public MailboxIdParameter Mailbox
		{
			get
			{
				return this.mailbox;
			}
		}

		private readonly MailboxIdParameter mailbox;
	}
}
