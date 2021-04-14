using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;

namespace Microsoft.Exchange.ProvisioningAgent
{
	internal sealed class MailboxSessionCacheKey : IEquatable<MailboxSessionCacheKey>
	{
		internal MailboxSessionCacheKey(IExchangePrincipal exchangePrincipal)
		{
			this.mdbGuid = exchangePrincipal.MailboxInfo.GetDatabaseGuid();
			this.mailboxGuid = exchangePrincipal.MailboxInfo.MailboxGuid;
		}

		internal MailboxSessionCacheKey(Guid mdbGuid, Guid mailboxGuid)
		{
			this.mdbGuid = mdbGuid;
			this.mailboxGuid = mailboxGuid;
		}

		internal Guid MdbGuid
		{
			get
			{
				return this.mdbGuid;
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as MailboxSessionCacheKey);
		}

		public override int GetHashCode()
		{
			return this.MdbGuid.GetHashCode() ^ this.MailboxGuid.GetHashCode();
		}

		public bool Equals(MailboxSessionCacheKey other)
		{
			return other != null && this.MdbGuid.Equals(other.MdbGuid) && this.MailboxGuid.Equals(other.MailboxGuid);
		}

		public override string ToString()
		{
			return string.Format("[Mdb:{0},Mailbox:{1}]", this.MdbGuid, this.MailboxGuid);
		}

		private readonly Guid mdbGuid;

		private readonly Guid mailboxGuid;
	}
}
