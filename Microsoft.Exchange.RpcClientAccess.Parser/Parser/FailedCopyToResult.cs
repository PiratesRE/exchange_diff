using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FailedCopyToResult : RopResult
	{
		internal FailedCopyToResult(ErrorCode errorCode, uint destinationObjectHandleIndex) : base(RopId.CopyTo, errorCode, null)
		{
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal FailedCopyToResult(Reader reader) : base(reader)
		{
			if (ErrorCode.DestinationNullObject == base.ErrorCode)
			{
				this.destinationObjectHandleIndex = reader.ReadUInt32();
			}
		}

		internal static FailedCopyToResult Parse(Reader reader)
		{
			return new FailedCopyToResult(reader);
		}

		internal override void Serialize(Writer writer)
		{
			base.Serialize(writer);
			if (base.ErrorCode == ErrorCode.DestinationNullObject)
			{
				writer.WriteUInt32(this.destinationObjectHandleIndex);
			}
		}

		private uint destinationObjectHandleIndex;
	}
}
