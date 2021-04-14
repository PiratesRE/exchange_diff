using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaThemeManagerInitializationException : OwaPermanentException
	{
		public OwaThemeManagerInitializationException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaThemeManagerInitializationException(string message) : base(message)
		{
		}
	}
}
