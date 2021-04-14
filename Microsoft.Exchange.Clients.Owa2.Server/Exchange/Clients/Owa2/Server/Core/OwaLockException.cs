using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public class OwaLockException : OwaTransientException
	{
		public OwaLockException(string message) : base(message)
		{
		}

		public OwaLockException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
