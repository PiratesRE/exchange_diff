using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetPerUserGuidResultFactory : StandardResultFactory
	{
		internal GetPerUserGuidResultFactory() : base(RopId.GetPerUserGuid)
		{
		}

		public RopResult CreateSuccessfulResult(Guid databaseGuid)
		{
			return new SuccessfulGetPerUserGuidResult(databaseGuid);
		}
	}
}
