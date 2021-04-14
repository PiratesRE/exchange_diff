using System;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class AuthzException : Exception
	{
		public AuthzException(string message) : base(message)
		{
		}

		public AuthzException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
