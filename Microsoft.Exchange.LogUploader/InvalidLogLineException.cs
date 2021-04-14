using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class InvalidLogLineException : MessageTracingException
	{
		public InvalidLogLineException(string message) : base(message)
		{
		}

		public InvalidLogLineException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
