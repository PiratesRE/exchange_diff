using System;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	[Serializable]
	internal sealed class ConnectionRegistrationException : Exception
	{
		public ConnectionRegistrationException(string message) : base(message)
		{
		}
	}
}
