using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal sealed class FailedCopyPropertiesResult : RopResult
	{
		internal FailedCopyPropertiesResult(ErrorCode errorCode, uint destinationObjectHandleIndex) : base(RopId.CopyProperties, errorCode, null)
		{
			if (errorCode == ErrorCode.DestinationNullObject)
			{
				this.destinationObjectHandleIndex = destinationObjectHandleIndex;
			}
		}

		internal FailedCopyPropertiesResult(Reader reader) : base(reader)
		{
			if (ErrorCode.DestinationNullObject == base.ErrorCode)
			{
				this.destinationObjectHandleIndex = reader.ReadUInt32();
			}
		}

		internal static FailedCopyPropertiesResult Parse(Reader reader)
		{
			return new FailedCopyPropertiesResult(reader);
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
