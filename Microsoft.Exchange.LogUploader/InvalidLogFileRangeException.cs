using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class InvalidLogFileRangeException : MessageTracingException
	{
		public InvalidLogFileRangeException(string message) : base(message)
		{
		}
	}
}
