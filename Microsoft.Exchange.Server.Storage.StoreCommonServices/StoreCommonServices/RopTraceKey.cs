using System;

namespace Microsoft.Exchange.Server.Storage.StoreCommonServices
{
	public struct RopTraceKey
	{
		internal RopTraceKey(OperationType operationType, int mailboxNumber, ClientType clientType, uint activityId, byte operationId, uint detailId, bool sharedLock)
		{
			this.operationType = operationType;
			this.mailboxNumber = mailboxNumber;
			this.clientType = clientType;
			this.activityId = activityId;
			this.operationId = operationId;
			this.detailId = detailId;
			this.sharedLock = sharedLock;
		}

		internal OperationType OperationType
		{
			get
			{
				return this.operationType;
			}
		}

		internal int MailboxNumber
		{
			get
			{
				return this.mailboxNumber;
			}
		}

		internal ClientType ClientType
		{
			get
			{
				return this.clientType;
			}
		}

		internal uint ActivityId
		{
			get
			{
				return this.activityId;
			}
		}

		internal byte OperationId
		{
			get
			{
				return this.operationId;
			}
		}

		internal uint DetailId
		{
			get
			{
				return this.detailId;
			}
		}

		internal bool SharedLock
		{
			get
			{
				return this.sharedLock;
			}
		}

		public override int GetHashCode()
		{
			return this.operationType.GetHashCode() ^ this.mailboxNumber ^ this.clientType.GetHashCode() ^ (int)this.activityId ^ (int)this.operationId ^ (int)this.detailId ^ (this.sharedLock ? 1 : 0);
		}

		public override bool Equals(object other)
		{
			return other is RopTraceKey && this.Equals((RopTraceKey)other);
		}

		public bool Equals(RopTraceKey other)
		{
			return this.operationType == other.operationType && this.mailboxNumber == other.mailboxNumber && this.clientType == other.clientType && this.activityId == other.activityId && this.operationId == other.operationId && this.detailId == other.detailId && this.sharedLock == other.sharedLock;
		}

		private OperationType operationType;

		private int mailboxNumber;

		private ClientType clientType;

		private uint activityId;

		private byte operationId;

		private uint detailId;

		private bool sharedLock;
	}
}
