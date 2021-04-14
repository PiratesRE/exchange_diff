using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class MissingPropertyException : MessageTracingException
	{
		public MissingPropertyException(string message) : base(message)
		{
		}
	}
}
