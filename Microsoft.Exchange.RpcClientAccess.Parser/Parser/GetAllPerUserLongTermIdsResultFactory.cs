using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetAllPerUserLongTermIdsResultFactory : StandardResultFactory
	{
		internal GetAllPerUserLongTermIdsResultFactory(int availableBufferSize) : base(RopId.GetAllPerUserLongTermIds)
		{
			this.availableBufferSize = availableBufferSize;
		}

		public PerUserDataCollector CreatePerUserDataCollector()
		{
			int num = 6;
			return new PerUserDataCollector(this.availableBufferSize - num);
		}

		public RopResult CreateSuccessfulResult(PerUserDataCollector perUserDataCollector, bool finished)
		{
			return new SuccessfulGetAllPerUserLongTermIdsResult(perUserDataCollector, finished);
		}

		private readonly int availableBufferSize;
	}
}
