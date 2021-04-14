using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReadStreamResultFactory : ResultFactory
	{
		internal ReadStreamResultFactory()
		{
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new ReadStreamResult(errorCode, Array<byte>.EmptySegment);
		}

		public RopResult CreateSuccessfulResult(ArraySegment<byte> dataSegment)
		{
			return new ReadStreamResult(ErrorCode.None, dataSegment);
		}
	}
}
