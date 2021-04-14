using System;
using System.Collections.Generic;
using Microsoft.Exchange.Server.Storage.Common;
using Microsoft.Exchange.Server.Storage.PhysicalAccess;
using Microsoft.Exchange.Server.Storage.StoreCommonServices;

namespace Microsoft.Exchange.Server.Storage.LazyIndexing
{
	public class LogicalIndexLockName : LogicalIndexComponentBase
	{
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

		public override int LogicalIndexNumber
		{
			get
			{
				return this.logicalIndexNumber;
			}
		}

		public LogicalIndexLockName(Guid databaseGuid, int mailboxPartitionNumber, int logicalIndexNumber)
		{
			this.databaseGuid = databaseGuid;
			this.mailboxPartitionNumber = mailboxPartitionNumber;
			this.logicalIndexNumber = logicalIndexNumber;
		}

		public override ILockName GetLockNameToCache()
		{
			return this;
		}

		public override bool IsValidTableOperation(Context context, Connection.OperationType operationType, Table table, IList<object> partitionValues)
		{
			return false;
		}

		private readonly Guid databaseGuid;

		private readonly int mailboxPartitionNumber;

		private readonly int logicalIndexNumber;
	}
}
