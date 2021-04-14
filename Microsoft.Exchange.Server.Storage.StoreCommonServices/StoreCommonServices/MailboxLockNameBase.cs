using System;
using System.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class MailboxLockNameBase : IMailboxLockName, ILockName, IEquatable<ILockName>, IComparable<ILockName>
	{
		public MailboxLockNameBase(Guid databaseGuid, int mailboxPartitionNumber)
		{
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.hashCode = (mailboxPartitionNumber.GetHashCode() ^ databaseGuid.GetHashCode());
		}

		public LockManager.LockLevel LockLevel
		{
			get
			{
				return LockManager.LockLevel.Mailbox;
			}
		}

		public virtual LockManager.NamedLockObject CachedLockObject
		{
			get
			{
				return this.cachedLockObject;
			}
			set
			{
				this.cachedLockObject = value;
			}
		}

		public int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public abstract Guid DatabaseGuid { get; }

		public static IMailboxLockName GetMailboxLockName(Guid databaseGuid, int mailboxPartitionNumber)
		{
			return new MailboxLockName(databaseGuid, mailboxPartitionNumber);
		}

		public static bool IsMailboxLocked(Guid databaseGuid, int mailboxPartitionNumber, bool shared)
		{
			if (shared)
			{
				return LockManager.TestLock(MailboxLockNameBase.GetMailboxLockName(databaseGuid, mailboxPartitionNumber), LockManager.LockType.MailboxShared);
			}
			return LockManager.TestLock(MailboxLockNameBase.GetMailboxLockName(databaseGuid, mailboxPartitionNumber), LockManager.LockType.MailboxExclusive);
		}

		[Conditional("DEBUG")]
		public static void AssertMailboxLocked(Guid databaseGuid, int mailboxPartitionNumber)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertMailboxLocked(Guid databaseGuid, int mailboxPartitionNumber, bool shared)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertMailboxNotLocked(Guid databaseGuid, int mailboxPartitionNumber)
		{
		}

		[Conditional("DEBUG")]
		public static void AssertNoMailboxLocked()
		{
		}

		public abstract ILockName GetLockNameToCache();

		public abstract string GetFriendlyNameForLogging();

		public bool Equals(ILockName other)
		{
			return this.CompareTo(other) == 0;
		}

		public virtual int CompareTo(ILockName other)
		{
			int num = ((int)this.LockLevel).CompareTo((int)other.LockLevel);
			if (num == 0)
			{
				MailboxLockNameBase mailboxLockNameBase = other as MailboxLockNameBase;
				num = this.DatabaseGuid.CompareTo(mailboxLockNameBase.DatabaseGuid);
				if (num == 0)
				{
					num = this.mailboxPartitionNumber.CompareTo(mailboxLockNameBase.MailboxPartitionNumber);
				}
			}
			return num;
		}

		public override bool Equals(object other)
		{
			return this.Equals(other as ILockName);
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public override string ToString()
		{
			return "DB " + this.DatabaseGuid.ToString() + "/MBX " + this.MailboxPartitionNumber.ToString();
		}

		public bool IsMailboxLocked()
		{
			return LockManager.TestLock(this, LockManager.LockType.MailboxExclusive) || LockManager.TestLock(this, LockManager.LockType.MailboxShared);
		}

		public bool IsMailboxLockedExclusively()
		{
			return LockManager.TestLock(this, LockManager.LockType.MailboxExclusive);
		}

		public bool IsMailboxSharedLockHeld()
		{
			return LockManager.TestLock(this, LockManager.LockType.MailboxShared);
		}

		[Conditional("DEBUG")]
		public void AssertMailboxLocked()
		{
		}

		[Conditional("DEBUG")]
		public void AssertMailboxExclusiveLockHeld()
		{
		}

		[Conditional("DEBUG")]
		public void AssertMailboxLocked(bool shared)
		{
		}

		[Conditional("DEBUG")]
		public void AssertMailboxNotLocked()
		{
		}

		private readonly int hashCode;

		private readonly int mailboxPartitionNumber;

		private LockManager.NamedLockObject cachedLockObject;
	}
}
