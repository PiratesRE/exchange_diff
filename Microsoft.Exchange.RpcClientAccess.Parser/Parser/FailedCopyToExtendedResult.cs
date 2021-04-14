using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FailedCopyToExtendedResult : RopResult
	{
		internal FailedCopyToExtendedResult(ErrorCode errorCode, uint destinationObjectHandleIndex) : base(RopId.CopyToExtended, errorCode, null)
		{
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal FailedCopyToExtendedResult(Reader reader) : base(reader)
		{
			if (ErrorCode.DestinationNullObject == base.ErrorCode)
			{
				this.destinationObjectHandleIndex = reader.ReadUInt32();
			}
		}

		internal static FailedCopyToExtendedResult Parse(Reader reader)
		{
			return new FailedCopyToExtendedResult(reader);
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
