using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public class MailboxComponentLockName : LockableMailboxComponent
	{
		public MailboxComponentLockName(MailboxComponentId componentId, int mailboxPartitionNumber, Guid databaseGuid, Connection.OperationType operationType, Table table) : this(LockManager.LockType.MailboxComponentsShared, componentId, mailboxPartitionNumber, databaseGuid, operationType, table)
		{
		}

		public MailboxComponentLockName(LockManager.LockType readerLockType, MailboxComponentId componentId, int mailboxPartitionNumber, Guid databaseGuid) : this(readerLockType, componentId, mailboxPartitionNumber, databaseGuid, Connection.OperationType.Query, null)
		{
		}

		private MailboxComponentLockName(LockManager.LockType readerLockType, MailboxComponentId componentId, int mailboxPartitionNumber, Guid databaseGuid, Connection.OperationType operationType, Table table)
		{
			this.readerLockType = readerLockType;
			this.componentId = componentId;
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.databaseGuid = databaseGuid;
			this.operationType = operationType;
			this.table = table;
		}

		public override MailboxComponentId MailboxComponentId
		{
			get
			{
				return this.componentId;
			}
		}

		public override Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		public override int MailboxPartitionNumber
		{
			get
			{
				return this.mailboxPartitionNumber;
			}
		}

		public override LockManager.LockType ReaderLockType
		{
			get
			{
				return this.readerLockType;
			}
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			if (operationType != this.operationType || !table.Equals(this.table))
			{
				return false;
			}
			if (operationType == Connection.OperationType.Query)
			{
				return this.TestSharedLock() || this.TestExclusiveLock();
			}
			return this.TestExclusiveLock();
		}

		private readonly MailboxComponentId componentId;

		private readonly int mailboxPartitionNumber;

		private readonly Guid databaseGuid;

		private Connection.OperationType operationType;

		private Table table;

		private LockManager.LockType readerLockType;
	}
}
