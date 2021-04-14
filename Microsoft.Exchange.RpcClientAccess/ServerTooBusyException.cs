using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ServerTooBusyException : RpcServiceException
	{
		internal ServerTooBusyException(string message, Exception innerException) : base(message, 1723, innerException)
		{
		}
	}
}
