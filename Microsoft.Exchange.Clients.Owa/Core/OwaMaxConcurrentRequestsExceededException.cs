using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaMaxConcurrentRequestsExceededException : OwaTransientException
	{
		public OwaMaxConcurrentRequestsExceededException(string message) : base(message)
		{
		}
	}
}
