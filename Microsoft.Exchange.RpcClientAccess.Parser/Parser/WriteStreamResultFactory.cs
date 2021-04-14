using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WriteStreamResultFactory : ResultFactory
	{
		internal WriteStreamResultFactory()
		{
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, ushort byteCount)
		{
			return new WriteStreamResult(errorCode, byteCount);
		}

		public RopResult CreateSuccessfulResult(ushort byteCount)
		{
			return new WriteStreamResult(ErrorCode.None, byteCount);
		}

		public override long SuccessfulResultMinimalSize
		{
			get
			{
				if (WriteStreamResultFactory.successfulResultMinimalSize == 0L)
				{
					RopResult ropResult = new WriteStreamResultFactory().CreateSuccessfulResult(0);
					ropResult.String8Encoding = CTSGlobals.AsciiEncoding;
					WriteStreamResultFactory.successfulResultMinimalSize = RopResult.CalculateResultSize(ropResult);
				}
				return WriteStreamResultFactory.successfulResultMinimalSize;
			}
		}

		private static long successfulResultMinimalSize;
	}
}
