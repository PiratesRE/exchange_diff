using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Mapi
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal interface IExRpcConnectionFactory
	{
		ExRpcConnection Create(ExRpcConnectionInfo connectionInfo);
	}
}
