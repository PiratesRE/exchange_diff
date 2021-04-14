using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ModifyPermissionsResultFactory : StandardResultFactory
	{
		internal ModifyPermissionsResultFactory() : base(RopId.ModifyPermissions)
		{
		}

		public RopResult CreateSuccessfulResult()
		{
			return new StandardRopResult(RopId.ModifyPermissions, ErrorCode.None);
		}
	}
}
