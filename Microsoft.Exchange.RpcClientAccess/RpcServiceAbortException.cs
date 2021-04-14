using System;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class RpcServiceAbortException : Exception
	{
		public RpcServiceAbortException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
