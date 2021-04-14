using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class WritePerUserInformationResultFactory : StandardResultFactory
	{
		internal WritePerUserInformationResultFactory() : base(RopId.WritePerUserInformation)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.WritePerUserInformation, ErrorCode.None);
		}
	}
}
