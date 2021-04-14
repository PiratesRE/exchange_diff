using System;

namespace Microsoft.Exchange.LogUploader
{
	internal class TransientDALException : MessageTracingException
	{
		public TransientDALException(string message) : base(message)
		{
		}

		public TransientDALException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
