using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class InvalidPropertyValueException : MessageTracingException
	{
		public InvalidPropertyValueException(string message) : base(message)
		{
		}
	}
}
