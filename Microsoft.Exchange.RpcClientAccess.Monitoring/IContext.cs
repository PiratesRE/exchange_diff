using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Monitoring
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IContext
	{
		ILogger Logger { get; }

		IEnvironment Environment { get; }

		IPropertyBag Properties { get; }

		IContext CreateDerived();
	}
}
