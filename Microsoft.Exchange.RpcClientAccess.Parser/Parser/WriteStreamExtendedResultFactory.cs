using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WriteStreamExtendedResultFactory : ResultFactory
	{
		internal WriteStreamExtendedResultFactory()
		{
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode, 0U);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode, uint byteCount)
		{
			return new WriteStreamExtendedResult(errorCode, byteCount);
		}

		public RopResult CreateSuccessfulResult(uint byteCount)
		{
			return new WriteStreamExtendedResult(ErrorCode.None, byteCount);
		}

		public override long SuccessfulResultMinimalSize
		{
			get
			{
				if (WriteStreamExtendedResultFactory.successfulResultMinimalSize == 0L)
				{
					RopResult ropResult = new WriteStreamExtendedResultFactory().CreateSuccessfulResult(0U);
					ropResult.String8Encoding = CTSGlobals.AsciiEncoding;
					WriteStreamExtendedResultFactory.successfulResultMinimalSize = RopResult.CalculateResultSize(ropResult);
				}
				return WriteStreamExtendedResultFactory.successfulResultMinimalSize;
			}
		}

		private static long successfulResultMinimalSize;
	}
}
