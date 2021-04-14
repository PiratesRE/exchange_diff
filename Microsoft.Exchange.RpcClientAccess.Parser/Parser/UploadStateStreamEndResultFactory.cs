using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UploadStateStreamEndResultFactory : StandardResultFactory
	{
		internal UploadStateStreamEndResultFactory() : base(RopId.UploadStateStreamEnd)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.UploadStateStreamEnd, ErrorCode.None);
		}
	}
}
