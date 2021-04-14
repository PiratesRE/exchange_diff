using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.RpcClientAccess.Handler
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecipientTranslationException : RopExecutionException
	{
		internal RecipientTranslationException(string message) : base(message, (ErrorCode)2147942487U)
		{
		}

		internal RecipientTranslationException(string message, Exception innerException) : base(message, (ErrorCode)2147942487U, innerException)
		{
		}

		private const ErrorCode DefaultErrorCode = ErrorCode.InvalidParam;
	}
}
