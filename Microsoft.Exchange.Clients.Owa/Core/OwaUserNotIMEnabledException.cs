using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaUserNotIMEnabledException : OwaPermanentException
	{
		public OwaUserNotIMEnabledException(string message) : base(message)
		{
		}

		public OwaUserNotIMEnabledException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
