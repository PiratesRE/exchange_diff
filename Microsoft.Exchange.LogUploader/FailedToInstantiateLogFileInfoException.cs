using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class FailedToInstantiateLogFileInfoException : MessageTracingException
	{
		public FailedToInstantiateLogFileInfoException(string fileName) : base(fileName)
		{
		}
	}
}
