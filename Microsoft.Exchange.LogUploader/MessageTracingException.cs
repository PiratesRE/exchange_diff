using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class MessageTracingException : Exception
	{
		public MessageTracingException(string message) : base(message)
		{
		}

		public MessageTracingException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
