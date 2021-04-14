using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SetReceiveFolderResultFactory : StandardResultFactory
	{
		internal SetReceiveFolderResultFactory() : base(RopId.SetReceiveFolder)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.SetReceiveFolder, ErrorCode.None);
		}
	}
}
