using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class UploadStateStreamBeginResultFactory : StandardResultFactory
	{
		internal UploadStateStreamBeginResultFactory() : base(RopId.UploadStateStreamBegin)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.UploadStateStreamBegin, ErrorCode.None);
		}
	}
}
