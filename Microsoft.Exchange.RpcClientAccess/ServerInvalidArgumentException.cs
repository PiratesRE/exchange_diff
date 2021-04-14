using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ServerInvalidArgumentException : RpcServiceException
	{
		internal ServerInvalidArgumentException(string message, Exception innerException) : base(message, 87, innerException)
		{
		}
	}
}
