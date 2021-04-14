using System;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer
{
	internal interface ISession
	{
		bool TryResolveToNamedProperty(PropertyTag propertyTag, out NamedProperty namedProperty);

		bool TryResolveFromNamedProperty(NamedProperty namedProperty, ref PropertyTag propertyTag);
	}
}
