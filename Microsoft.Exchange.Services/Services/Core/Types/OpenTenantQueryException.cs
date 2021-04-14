using System;

namespace Microsoft.Exchange.Services.Core.Types
{
	internal class OpenTenantQueryException : Exception
	{
		public OpenTenantQueryException(string message) : base(message)
		{
		}

		public OpenTenantQueryException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
