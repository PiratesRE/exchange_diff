using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class MalformedLogRangeLineException : MessageTracingException
	{
		public MalformedLogRangeLineException(string message) : base(message)
		{
		}
	}
}
