using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MoveCopyMessagesExtendedWithEntryIdsResult : MoveCopyMessagesExtendedResultBase
	{
		internal StoreId[] MessageIds
		{
			get
			{
				return this.messageIds;
			}
		}

		internal ulong[] ChangeNumbers
		{
			get
			{
				return this.changeNumbers;
			}
		}

		internal MoveCopyMessagesExtendedWithEntryIdsResult(ErrorCode errorCode, bool isPartiallyCompleted, StoreId[] messageIds, ulong[] changeNumbers, uint destinationObjectHandleIndex) : base(RopId.MoveCopyMessagesExtendedWithEntryIds, errorCode, isPartiallyCompleted, destinationObjectHandleIndex)
		{
			if ((messageIds == null && changeNumbers != null) || (messageIds != null && changeNumbers == null) || (messageIds != null && changeNumbers != null && messageIds.Length != changeNumbers.Length))
			{
				throw new ArgumentException("Number of messageIds elements doesn't match number of changeNumbers elements.");
			}
			this.messageIds = messageIds;
			this.changeNumbers = changeNumbers;
		}

		internal MoveCopyMessagesExtendedWithEntryIdsResult(Reader reader) : base(reader)
		{
			uint length = reader.ReadUInt32();
			this.messageIds = reader.ReadStoreIdArray(length);
			this.changeNumbers = reader.ReadUInt64Array(length);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			writer.WriteUInt32((uint)((this.messageIds != null) ? this.messageIds.Length : 0));
			writer.WriteStoreIds(this.messageIds);
			writer.WriteUInt64Array(this.changeNumbers);
		}

		private readonly StoreId[] messageIds;

		private readonly ulong[] changeNumbers;
	}
}
