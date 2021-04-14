using System;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	[Serializable]
	public sealed class OwaCannotSanitizeHtmlException : OwaPermanentException
	{
		public OwaCannotSanitizeHtmlException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaCannotSanitizeHtmlException(string message) : base(message)
		{
		}
	}
}
