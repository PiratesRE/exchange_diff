using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class StandardResultFactory : ResultFactory
	{
		internal StandardResultFactory(RopId ropId)
		{
			this.ropId = ropId;
		}

		public override RopResult CreateStandardFailedResult(ErrorCode errorCode)
		{
			return this.CreateFailedResult(errorCode);
		}

		public RopResult CreateFailedResult(ErrorCode errorCode)
		{
			return new StandardRopResult(this.ropId, errorCode);
		}

		private readonly RopId ropId;
	}
}
