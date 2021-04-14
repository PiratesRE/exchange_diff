using System;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	[Serializable]
	public sealed class OwaNotificationPipeWriteException : OwaNotificationPipeException
	{
		public OwaNotificationPipeWriteException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
