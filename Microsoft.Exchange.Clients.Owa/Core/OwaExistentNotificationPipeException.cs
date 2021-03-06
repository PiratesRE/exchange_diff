using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaExistentNotificationPipeException : OwaNotificationPipeException
	{
		public OwaExistentNotificationPipeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public OwaExistentNotificationPipeException(string message) : base(message)
		{
		}
	}
}
