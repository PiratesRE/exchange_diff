using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Parser
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class AddressTypesResultFactory : StandardResultFactory
	{
		internal AddressTypesResultFactory() : base(RopId.AddressTypes)
		{
		}

		public RopResult CreateSuccessfulResult(string[] addressTypes)
		{
			return new SuccessfulAddressTypesResult(addressTypes);
		}
	}
}
