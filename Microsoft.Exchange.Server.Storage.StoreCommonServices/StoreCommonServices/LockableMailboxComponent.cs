using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public abstract class LockableMailboxComponent : ILockName, IEquatable<ILockName>, IComparable<ILockName>
	{
		public LockManager.LockLevel LockLevel
		{
			get
			{
				return LockManager.LockLevelFromLockType(this.ReaderLockType);
			}
		}

		public LockManager.NamedLockObject CachedLockObject
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

		public abstract int MailboxPartitionNumber { get; }

		public abstract Guid DatabaseGuid { get; }

		public virtual LockManager.LockType ReaderLockType
		{
			get
			{
				return LockManager.LockType.MailboxComponentsShared;
			}
		}

		public virtual LockManager.LockType WriterLockType
		{
			get
			{
				return LockManager.LockType.MailboxComponentsExclusive;
			}
		}

		public virtual bool Committable
		{
			get
			{
				return true;
			}
		}

		public abstract MailboxComponentId MailboxComponentId { get; }

		[Conditional("DEBUG")]
		public void AssertSharedLockHeld()
		{
		}

		[Conditional("DEBUG")]
		public void AssertExclusiveLockHeld()
		{
		}

		public virtual ILockName GetLockNameToCache()
		{
			return new MailboxComponentLockName(this.ReaderLockType, this.MailboxComponentId, this.MailboxPartitionNumber, this.DatabaseGuid);
		}

		public virtual bool TestSharedLock()
		{
			return LockManager.TestLock(this, this.ReaderLockType);
		}

		public virtual bool TestExclusiveLock()
		{
			return LockManager.TestLock(this, this.WriterLockType);
		}

		public virtual void LockShared(ILockStatistics lockStats)
		{
			LockManager.GetLock(this, this.ReaderLockType, lockStats);
		}

		public virtual void ReleaseShared()
		{
			LockManager.ReleaseLock(this, this.ReaderLockType);
		}

		public virtual void LockExclusive(ILockStatistics lockStats)
		{
			LockManager.GetLock(this, this.WriterLockType, lockStats);
		}

		public virtual void ReleaseExclusive()
		{
			LockManager.ReleaseLock(this, this.WriterLockType);
		}

		public abstract bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues);

		public override bool Equals(object other)
		{
			return this.Equals(other as ILockName);
		}

		public virtual bool Equals(ILockName other)
		{
			return other != null && this.CompareTo(other) == 0;
		}

		public virtual int CompareTo(ILockName other)
		{
			int num = ((int)this.LockLevel).CompareTo((int)other.LockLevel);
			if (num == 0)
			{
				LockableMailboxComponent lockableMailboxComponent = other as LockableMailboxComponent;
				num = this.DatabaseGuid.CompareTo(lockableMailboxComponent.DatabaseGuid);
				if (num == 0)
				{
					num = this.MailboxPartitionNumber.CompareTo(lockableMailboxComponent.MailboxPartitionNumber);
					if (num == 0)
					{
						num = ((int)this.MailboxComponentId).CompareTo((int)lockableMailboxComponent.MailboxComponentId);
					}
				}
			}
			return num;
		}

		public override int GetHashCode()
		{
			return (int)(this.LockLevel ^ (LockManager.LockLevel)this.MailboxComponentId ^ (LockManager.LockLevel)this.MailboxPartitionNumber.GetHashCode());
		}

		public override string ToString()
		{
			return "MailboxComponent " + this.MailboxComponentId.ToString() + "/" + this.LockLevel.ToString();
		}

		private LockManager.NamedLockObject cachedLockObject;
	}
}
