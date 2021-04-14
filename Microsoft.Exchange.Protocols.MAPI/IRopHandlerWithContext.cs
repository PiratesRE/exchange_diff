using System;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.Protocols.MAPI
{
	internal interface IRopHandlerWithContext : IRopHandler, IDisposable
	{
		MapiContext MapiContext { get; set; }
	}
}
