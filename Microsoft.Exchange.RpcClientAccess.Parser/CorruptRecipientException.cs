using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class CorruptRecipientException : RopExecutionException
	{
		public CorruptRecipientException(string message, ErrorCode error) : base(message, error)
		{
		}
	}
}
