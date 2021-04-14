using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaClientNotSupportedException : OwaPermanentException
	{
		public OwaClientNotSupportedException(string message, Exception innerException, object thisObject) : base(message, innerException, thisObject)
		{
		}

		public OwaClientNotSupportedException(string message) : base(message)
		{
		}
	}
}
