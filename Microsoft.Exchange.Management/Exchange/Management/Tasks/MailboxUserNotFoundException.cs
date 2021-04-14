using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxUserNotFoundException : LocalizedException
	{
		public MailboxUserNotFoundException(string mailboxName) : base(Strings.MailboxUserNotFoundException(mailboxName))
		{
			this.mailboxName = mailboxName;
		}

		public MailboxUserNotFoundException(string mailboxName, Exception innerException) : base(Strings.MailboxUserNotFoundException(mailboxName), innerException)
		{
			this.mailboxName = mailboxName;
		}

		protected MailboxUserNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxName = (string)info.GetValue("mailboxName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxName", this.mailboxName);
		}

		public string MailboxName
		{
			get
			{
				return this.mailboxName;
			}
		}

		private readonly string mailboxName;
	}
}
