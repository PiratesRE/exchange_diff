using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaLockTimeoutException : OwaLockException
	{
		public OwaLockTimeoutException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaLockTimeoutException(string message) : base(message)
		{
		}
	}
}
