using System;

namespace Microsoft.Exchange.Net.Mserve
{
	internal sealed class RecipientPendingOperation
	{
		public RecipientPendingOperation(RecipientSyncOperation recipientSyncOperation, OperationType type)
		{
			this.recipientSyncOperation = recipientSyncOperation;
			this.type = type;
		}

		public RecipientSyncOperation RecipientSyncOperation
		{
			get
			{
				return this.recipientSyncOperation;
			}
		}

		public OperationType Type
		{
			get
			{
				return this.type;
			}
		}

		public bool IsAdd
		{
			get
			{
				return this.type == OperationType.Add;
			}
		}

		public bool IsDelete
		{
			get
			{
				return this.type == OperationType.Delete;
			}
		}

		public bool IsRead
		{
			get
			{
				return this.type == OperationType.Read;
			}
		}

		private OperationType type;

		private readonly RecipientSyncOperation recipientSyncOperation;
	}
}
