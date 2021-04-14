using System;

namespace Microsoft.Exchange.Search.Mdb
{
	internal sealed class StoreSessionCacheKey : IEquatable<StoreSessionCacheKey>
	{
		internal StoreSessionCacheKey(Guid mdbGuid, Guid mailboxGuid, bool isMoveDestination)
		{
			this.mdbGuid = mdbGuid;
			this.mailboxGuid = mailboxGuid;
			this.isMoveDestination = isMoveDestination;
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

		internal bool IsMoveDestination
		{
			get
			{
				return this.isMoveDestination;
			}
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as StoreSessionCacheKey);
		}

		public override int GetHashCode()
		{
			return this.MdbGuid.GetHashCode() ^ this.MailboxGuid.GetHashCode();
		}

		public bool Equals(StoreSessionCacheKey other)
		{
			return other != null && this.MdbGuid.Equals(other.MdbGuid) && this.MailboxGuid.Equals(other.MailboxGuid) && this.IsMoveDestination == other.IsMoveDestination;
		}

		public override string ToString()
		{
			return string.Format("[Mdb:{0},Mailbox:{1},IsMoveDestination{2}]", this.MdbGuid, this.MailboxGuid, this.IsMoveDestination);
		}

		private readonly Guid mdbGuid;

		private readonly Guid mailboxGuid;

		private readonly bool isMoveDestination;
	}
}
