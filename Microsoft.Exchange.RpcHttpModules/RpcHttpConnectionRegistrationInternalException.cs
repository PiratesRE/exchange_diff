using System;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpConnectionRegistrationInternalException : RpcHttpConnectionRegistrationException
	{
		public RpcHttpConnectionRegistrationInternalException()
		{
		}

		public RpcHttpConnectionRegistrationInternalException(string message) : base(message)
		{
		}

		public RpcHttpConnectionRegistrationInternalException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
