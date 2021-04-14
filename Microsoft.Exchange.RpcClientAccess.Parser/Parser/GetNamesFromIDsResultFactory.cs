using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class GetNamesFromIDsResultFactory : StandardResultFactory
	{
		internal GetNamesFromIDsResultFactory() : base(RopId.GetNamesFromIDs)
		{
		}

		public RopResult CreateSuccessfulResult(NamedProperty[] namedProperties)
		{
			return new SuccessfulGetNamesFromIDsResult(namedProperties);
		}
	}
}
