using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class NotFoundException : RpcServerException
	{
		internal NotFoundException(string message) : base(message, RpcErrorCode.NotFound)
		{
		}
	}
}
