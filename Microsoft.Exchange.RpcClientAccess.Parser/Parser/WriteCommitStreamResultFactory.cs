using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WriteCommitStreamResultFactory : ResultFactory
	{
		internal WriteCommitStreamResultFactory()
		{
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, ushort byteCount)
		{
			return new WriteCommitStreamResult(errorCode, byteCount);
		}

		public RopResult CreateSuccessfulResult(ushort byteCount)
		{
			return new WriteCommitStreamResult(ErrorCode.None, byteCount);
		}

		public override long SuccessfulResultMinimalSize
		{
			get
			{
				if (WriteCommitStreamResultFactory.successfulResultMinimalSize == 0L)
				{
					RopResult ropResult = new WriteCommitStreamResultFactory().CreateSuccessfulResult(0);
					ropResult.String8Encoding = CTSGlobals.AsciiEncoding;
					WriteCommitStreamResultFactory.successfulResultMinimalSize = RopResult.CalculateResultSize(ropResult);
				}
				return WriteCommitStreamResultFactory.successfulResultMinimalSize;
			}
		}

		private static long successfulResultMinimalSize;
	}
}
