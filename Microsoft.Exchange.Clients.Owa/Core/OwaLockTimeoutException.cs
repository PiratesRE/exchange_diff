using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaLockTimeoutException : OwaTransientException
	{
		public OwaLockTimeoutException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
