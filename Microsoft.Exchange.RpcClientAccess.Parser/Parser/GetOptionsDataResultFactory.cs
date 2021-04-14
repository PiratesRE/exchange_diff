using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetOptionsDataResultFactory : StandardResultFactory
	{
		internal GetOptionsDataResultFactory() : base(RopId.GetOptionsData)
		{
		}

		public RopResult CreateSuccessfulResult(byte[] optionsInfo)
		{
			return new SuccessfulGetOptionsDataResult(optionsInfo, Array<byte>.Empty, null);
		}

		public RopResult CreateSuccessfulResult(byte[] optionsInfo, byte[] helpFileData, string helpFileName)
		{
			return new SuccessfulGetOptionsDataResult(optionsInfo, helpFileData, helpFileName);
		}
	}
}
