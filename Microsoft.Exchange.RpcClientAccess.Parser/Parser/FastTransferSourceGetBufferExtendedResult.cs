using System;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	internal abstract class FastTransferSourceGetBufferExtendedResult : FastTransferSourceGetBufferResultBase
	{
		internal uint Progress
		{
			get
			{
				return this.ResultData.Progress;
			}
		}

		internal uint Steps
		{
			get
			{
				return this.ResultData.Steps;
			}
		}

		internal FastTransferSourceGetBufferExtendedResult(ErrorCode errorCode, FastTransferSourceGetBufferData resultData) : base(RopId.FastTransferSourceGetBufferExtended, errorCode, resultData)
		{
		}

		internal FastTransferSourceGetBufferExtendedResult(Reader reader, bool isServerBusy) : base(reader, isServerBusy, true)
		{
		}

		internal static RopResult Parse(Reader reader)
		{
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			ErrorCode errorCode2 = errorCode;
			if (errorCode2 == ErrorCode.None)
			{
				return new SuccessfulFastTransferSourceGetBufferExtendedResult(reader);
			}
			if (errorCode2 == ErrorCode.ServerBusy)
			{
				return new BackOffFastTransferSourceGetBufferExtendedResult(reader);
			}
			return new FailedFastTransferSourceGetBufferExtendedResult(reader);
		}

		internal const int SpecificRopHeaderSize = 13;

		internal static readonly int FullHeaderSize = Rop.ComputeResultHeaderSize(13);
	}
}
