using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingOverMaximumFileSizeException : TranscodingException
	{
		public TranscodingOverMaximumFileSizeException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}

		public TranscodingOverMaximumFileSizeException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TranscodingOverMaximumFileSizeException(string message) : base(message)
		{
		}
	}
}
