using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingTimeoutException : TranscodingException
	{
		public TranscodingTimeoutException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}

		public TranscodingTimeoutException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TranscodingTimeoutException(string message) : base(message)
		{
		}
	}
}
