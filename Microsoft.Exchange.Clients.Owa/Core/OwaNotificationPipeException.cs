using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public class OwaNotificationPipeException : OwaTransientException
	{
		public OwaNotificationPipeException(string message) : base(message)
		{
		}

		public OwaNotificationPipeException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
