using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaIMOperationNotAllowedToSelf : OwaPermanentException
	{
		public OwaIMOperationNotAllowedToSelf(string message) : base(message)
		{
		}

		public OwaIMOperationNotAllowedToSelf(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
