using System;

namespace Microsoft.Exchange.MailboxTransport.StoreDriverCommon
{
	[Serializable]
	internal class HandlerParseException : Exception
	{
		public HandlerParseException(string message) : base(message)
		{
		}

		public HandlerParseException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
