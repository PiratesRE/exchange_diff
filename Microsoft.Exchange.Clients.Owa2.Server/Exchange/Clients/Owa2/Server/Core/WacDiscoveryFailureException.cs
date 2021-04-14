using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class WacDiscoveryFailureException : OwaPermanentException
	{
		public WacDiscoveryFailureException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public WacDiscoveryFailureException(string message) : base(message)
		{
		}
	}
}
