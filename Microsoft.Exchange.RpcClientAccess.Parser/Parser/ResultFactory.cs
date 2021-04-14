using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal abstract class ResultFactory : IResultFactory
	{
		public abstract RopResult CreateStandardFailedResult(ErrorCode errorCode);

		public virtual long SuccessfulResultMinimalSize
		{
			get
			{
				return 0L;
			}
		}

		protected static RopResult ParseResultOrProgress(RopId ropId, Reader reader, Func<Reader, RopResult> resultFunc)
		{
			return ResultFactory.ParseResultOrProgress(ropId, reader, resultFunc, resultFunc);
		}

		protected static RopResult ParseResultOrProgress(RopId ropId, Reader reader, Func<Reader, RopResult> successfulResultFunc, Func<Reader, RopResult> failedResultFunc)
		{
			RopId ropId2 = (RopId)reader.PeekByte(0L);
			ErrorCode errorCode = (ErrorCode)reader.PeekUInt32(2L);
			if (ropId2 == RopId.Progress)
			{
				if (errorCode == ErrorCode.None)
				{
					return new SuccessfulProgressResult(reader);
				}
				throw new BufferParseException("Unexpected failed progress");
			}
			else
			{
				if (ropId2 != ropId)
				{
					throw new BufferParseException(string.Format("Unexpected result RopId: {0}", ropId2));
				}
				if (errorCode == ErrorCode.None)
				{
					return successfulResultFunc(reader);
				}
				return failedResultFunc(reader);
			}
		}
	}
}
