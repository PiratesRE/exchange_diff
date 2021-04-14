using System;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public abstract class LogicalIndexComponentBase : LockableMailboxComponent
	{
		public abstract int LogicalIndexNumber { get; }

		public override MailboxComponentId MailboxComponentId
		{
			get
			{
				return MailboxComponentId.LogicalIndex;
			}
		}

		public override LockManager.LockType ReaderLockType
		{
			get
			{
				return LockManager.LockType.LogicalIndexShared;
			}
		}

		public override LockManager.LockType WriterLockType
		{
			get
			{
				return LockManager.LockType.LogicalIndexExclusive;
			}
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ this.LogicalIndexNumber.GetHashCode();
		}

		public override int CompareTo(ILockName other)
		{
			int num = base.CompareTo(other);
			if (num == 0)
			{
				LogicalIndexComponentBase logicalIndexComponentBase = other as LogicalIndexComponentBase;
				num = this.LogicalIndexNumber.CompareTo(logicalIndexComponentBase.LogicalIndexNumber);
			}
			return num;
		}
	}
}
