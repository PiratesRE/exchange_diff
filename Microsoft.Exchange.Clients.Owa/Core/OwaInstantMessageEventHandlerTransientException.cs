using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaInstantMessageEventHandlerTransientException : OwaTransientException
	{
		public OwaInstantMessageEventHandlerTransientException(string message) : base(message)
		{
		}

		public OwaInstantMessageEventHandlerTransientException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
