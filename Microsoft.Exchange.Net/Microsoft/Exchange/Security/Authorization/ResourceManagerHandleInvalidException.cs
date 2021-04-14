using System;

namespace Microsoft.Exchange.Security.Authorization
{
	internal class ResourceManagerHandleInvalidException : Exception
	{
		public ResourceManagerHandleInvalidException(string message) : base(message)
		{
		}

		public ResourceManagerHandleInvalidException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
