using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class FastTransferSourceGetBufferResult : FastTransferSourceGetBufferResultBase
	{
		internal ushort Progress
		{
			get
			{
				return (ushort)this.ResultData.Progress;
			}
		}

		internal ushort Steps
		{
			get
			{
				return (ushort)this.ResultData.Steps;
			}
		}

		internal FastTransferSourceGetBufferResult(ErrorCode errorCode, FastTransferSourceGetBufferData resultData) : base(RopId.FastTransferSourceGetBuffer, errorCode, resultData)
		{
		}

		internal FastTransferSourceGetBufferResult(Reader reader, bool isServerBusy) : base(reader, isServerBusy, false)
		{
		}

		internal static RopResult Parse(Reader reader)
		{
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			ErrorCode errorCode2 = errorCode;
			if (errorCode2 == ErrorCode.None)
			{
				return new SuccessfulFastTransferSourceGetBufferResult(reader);
			}
			if (errorCode2 == ErrorCode.ServerBusy)
			{
				return new BackOffFastTransferSourceGetBufferResult(reader);
			}
			return new FailedFastTransferSourceGetBufferResult(reader);
		}

		internal const int SpecificRopHeaderSize = 9;

		internal static readonly int FullHeaderSize = Rop.ComputeResultHeaderSize(9);
	}
}
