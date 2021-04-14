using System;
using System.Text;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	internal class MailboxContextFilter : QueryFilter
	{
		public MailboxContextFilter(Guid mailboxGuid)
		{
			this.mailboxGuid = mailboxGuid;
			this.mailboxFlags = 0UL;
			this.noADLookup = false;
		}

		public MailboxContextFilter(Guid mailboxGuid, ulong mailboxFlags)
		{
			this.mailboxGuid = mailboxGuid;
			this.mailboxFlags = mailboxFlags;
			this.noADLookup = false;
		}

		public MailboxContextFilter(Guid mailboxGuid, ulong mailboxFlags, bool noADLookup)
		{
			this.mailboxGuid = mailboxGuid;
			this.mailboxFlags = mailboxFlags;
			this.noADLookup = noADLookup;
		}

		public override void ToString(StringBuilder sb)
		{
			sb.Append("(");
			sb.Append(this.mailboxGuid.ToString());
			sb.Append(" ");
			sb.Append(this.mailboxFlags.ToString());
			sb.Append(" ");
			sb.Append(this.noADLookup.ToString());
			sb.Append(")");
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public ulong MailboxFlags
		{
			get
			{
				return this.mailboxFlags;
			}
		}

		public bool NoADLookup
		{
			get
			{
				return this.noADLookup;
			}
		}

		private readonly Guid mailboxGuid;

		private readonly ulong mailboxFlags;

		private readonly bool noADLookup;
	}
}
