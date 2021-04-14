using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingErrorFileException : TranscodingException
	{
		public TranscodingErrorFileException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}

		public TranscodingErrorFileException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TranscodingErrorFileException(string message) : base(message)
		{
		}
	}
}
