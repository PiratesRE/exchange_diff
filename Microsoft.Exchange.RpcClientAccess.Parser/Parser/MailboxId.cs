using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal struct MailboxId : IEquatable<MailboxId>
	{
		public MailboxId(string mailboxLegacyDn)
		{
			this.isLegacyDn = true;
			this.mailboxLegacyDn = mailboxLegacyDn;
			this.mailboxGuid = Guid.Empty;
			this.databaseGuid = Guid.Empty;
		}

		public MailboxId(Guid mailboxGuid, Guid databaseGuid)
		{
			this.isLegacyDn = false;
			this.mailboxGuid = mailboxGuid;
			this.databaseGuid = databaseGuid;
			this.mailboxLegacyDn = null;
		}

		public bool IsLegacyDn
		{
			get
			{
				return this.isLegacyDn;
			}
		}

		public string LegacyDn
		{
			get
			{
				return this.mailboxLegacyDn;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public uint SerializedLength()
		{
			if (this.isLegacyDn)
			{
				return (uint)(this.mailboxLegacyDn.Length + 1);
			}
			return 32U;
		}

		public bool Equals(MailboxId other)
		{
			if (this.isLegacyDn != other.isLegacyDn)
			{
				return false;
			}
			if (this.isLegacyDn)
			{
				return this.mailboxLegacyDn == other.mailboxLegacyDn;
			}
			return this.databaseGuid == other.databaseGuid && this.mailboxGuid == other.mailboxGuid;
		}

		public override bool Equals(object obj)
		{
			return obj is MailboxId && this.Equals((MailboxId)obj);
		}

		public override int GetHashCode()
		{
			if (this.isLegacyDn)
			{
				return this.mailboxLegacyDn.GetHashCode();
			}
			return this.mailboxGuid.GetHashCode() ^ this.databaseGuid.GetHashCode();
		}

		public override string ToString()
		{
			return "MailboxId: " + this.ToBareString();
		}

		public string ToBareString()
		{
			if (this.isLegacyDn)
			{
				return string.Format("Dn[{0}]", this.mailboxLegacyDn);
			}
			return string.Format("Mailbox[{0}] Database[{1}]", this.mailboxGuid, this.databaseGuid);
		}

		internal void Serialize(Writer writer)
		{
			if (this.isLegacyDn)
			{
				writer.WriteAsciiString(this.mailboxLegacyDn, StringFlags.IncludeNull);
				return;
			}
			writer.WriteGuid(this.mailboxGuid);
			writer.WriteGuid(this.databaseGuid);
		}

		private readonly bool isLegacyDn;

		private readonly Guid mailboxGuid;

		private readonly Guid databaseGuid;

		private readonly string mailboxLegacyDn;
	}
}
