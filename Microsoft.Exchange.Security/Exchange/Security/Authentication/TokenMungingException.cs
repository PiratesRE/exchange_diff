using System;

namespace Microsoft.Exchange.Security.Authentication
{
	internal class TokenMungingException : Exception
	{
		public TokenMungingException(string message) : base(message)
		{
		}

		public TokenMungingException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
