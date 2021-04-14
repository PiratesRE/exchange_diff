using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class WrongCASServerBecauseOfOutOfDateDNSCacheException : OwaPermanentException
	{
		public WrongCASServerBecauseOfOutOfDateDNSCacheException() : base(null)
		{
		}
	}
}
