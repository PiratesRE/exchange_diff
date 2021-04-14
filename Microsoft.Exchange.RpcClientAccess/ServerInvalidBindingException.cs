using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class ServerInvalidBindingException : RpcServiceException
	{
		internal ServerInvalidBindingException(string message, Exception innerException) : base(message, 1702, innerException)
		{
		}
	}
}
