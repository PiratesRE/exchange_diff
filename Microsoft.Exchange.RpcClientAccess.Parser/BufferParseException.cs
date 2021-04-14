using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class BufferParseException : Exception
	{
		internal BufferParseException(string message) : base(message)
		{
		}
	}
}
