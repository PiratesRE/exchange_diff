using System;
using System.Text;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class MoveCopyMessagesResult : RopResult
	{
		internal MoveCopyMessagesResult(ErrorCode errorCode, bool isPartiallyCompleted, uint destinationObjectHandleIndex) : base(RopId.MoveCopyMessages, errorCode, null)
		{
			this.isPartiallyCompleted = isPartiallyCompleted;
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal MoveCopyMessagesResult(Reader reader) : base(reader)
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

		internal override void AppendToString(StringBuilder stringBuilder)
		{
			base.AppendToString(stringBuilder);
			stringBuilder.Append(" Partial=").Append(this.isPartiallyCompleted);
		}

		private readonly bool isPartiallyCompleted;

		private uint destinationObjectHandleIndex;
	}
}
