using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UploadStateStreamContinueResultFactory : StandardResultFactory
	{
		internal UploadStateStreamContinueResultFactory() : base(RopId.UploadStateStreamContinue)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.UploadStateStreamContinue, ErrorCode.None);
		}
	}
}
