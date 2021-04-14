using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class SessionDeadException : Exception
	{
		internal SessionDeadException(string message) : base(message)
		{
		}

		internal SessionDeadException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
