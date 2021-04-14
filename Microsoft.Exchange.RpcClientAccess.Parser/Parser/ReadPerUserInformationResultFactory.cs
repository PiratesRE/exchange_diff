using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ReadPerUserInformationResultFactory : StandardResultFactory
	{
		internal ReadPerUserInformationResultFactory() : base(RopId.ReadPerUserInformation)
		{
		}

		public RopResult CreateSuccessfulResult(bool hasFinished, byte[] data)
		{
			return new SuccessfulReadPerUserInformationResult(hasFinished, data);
		}
	}
}
