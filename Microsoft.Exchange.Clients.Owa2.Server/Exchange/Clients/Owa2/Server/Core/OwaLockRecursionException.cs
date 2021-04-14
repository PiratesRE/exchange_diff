using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaLockRecursionException : OwaLockException
	{
		public OwaLockRecursionException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}
	}
}
