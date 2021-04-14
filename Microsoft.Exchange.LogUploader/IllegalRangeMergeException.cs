using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class IllegalRangeMergeException : MessageTracingException
	{
		public IllegalRangeMergeException(string message) : base(message)
		{
		}
	}
}
