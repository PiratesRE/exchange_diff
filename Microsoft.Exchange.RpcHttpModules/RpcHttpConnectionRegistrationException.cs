using System;

namespace Microsoft.Exchange.RpcHttpModules
{
	public class RpcHttpConnectionRegistrationException : Exception
	{
		public RpcHttpConnectionRegistrationException()
		{
		}

		public RpcHttpConnectionRegistrationException(string message) : base(message)
		{
		}

		public RpcHttpConnectionRegistrationException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public RpcHttpConnectionRegistrationException(string message, int errorCode) : base(message)
		{
			this.ErrorCode = errorCode;
		}

		public int ErrorCode { get; set; }
	}
}
