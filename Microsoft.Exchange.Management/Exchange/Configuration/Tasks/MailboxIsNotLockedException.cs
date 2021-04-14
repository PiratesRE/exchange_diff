using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class MailboxIsNotLockedException : LocalizedException
	{
		public MailboxIsNotLockedException(string mailboxId) : base(Strings.MailboxIsNotLocked(mailboxId))
		{
			this.mailboxId = mailboxId;
		}

		public MailboxIsNotLockedException(string mailboxId, Exception innerException) : base(Strings.MailboxIsNotLocked(mailboxId), innerException)
		{
			this.mailboxId = mailboxId;
		}

		protected MailboxIsNotLockedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.mailboxId = (string)info.GetValue("mailboxId", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("mailboxId", this.mailboxId);
		}

		public string MailboxId
		{
			get
			{
				return this.mailboxId;
			}
		}

		private readonly string mailboxId;
	}
}
