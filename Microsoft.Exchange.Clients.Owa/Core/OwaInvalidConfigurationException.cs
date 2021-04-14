using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaInvalidConfigurationException : OwaPermanentException
	{
		internal OwaInvalidConfigurationException(string message) : base(message)
		{
		}

		internal OwaInvalidConfigurationException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
