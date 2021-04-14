using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ServerUnavailableException : RpcServiceException
	{
		internal ServerUnavailableException(string message, Exception innerException) : base(message, 1722, innerException)
		{
		}
	}
}
