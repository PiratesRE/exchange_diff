using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaADObjectNotFoundException : OwaPermanentException
	{
		public OwaADObjectNotFoundException() : base(null)
		{
		}

		public OwaADObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaADObjectNotFoundException(string message) : base(message)
		{
		}
	}
}
