using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class MoveCopyMessagesExtendedResultBase : RopResult
	{
		internal MoveCopyMessagesExtendedResultBase(RopId ropId, ErrorCode errorCode, bool isPartiallyCompleted, uint destinationObjectHandleIndex) : base(ropId, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal MoveCopyMessagesExtendedResultBase(Reader reader) : base(reader)
		{
			if (base.ErrorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = reader.ReadUInt32();
			}
			this.isPartiallyCompleted = reader.ReadBool();
		}

		internal bool IsPartiallyCompleted
		{
			get
			{
				return this.isPartiallyCompleted;
			}
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			if (base.ErrorCode == ErrorCode.DestinationNullObject)
			{
				writer.WriteUInt32(this.destinationObjectHandleIndex);
			}
			writer.WriteBool(this.isPartiallyCompleted);
		}

		private readonly bool isPartiallyCompleted;

		private uint destinationObjectHandleIndex;
	}
}
