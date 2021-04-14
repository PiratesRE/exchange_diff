using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.FastTransfer.Parser
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal interface IFastTransferConverter<TContext> where TContext : BaseObject
	{
		PropertyValue Convert(TContext context, PropertyValue propertyValue);
	}
}
