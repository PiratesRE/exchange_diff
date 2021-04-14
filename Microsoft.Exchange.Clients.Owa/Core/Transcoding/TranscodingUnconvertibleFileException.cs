using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingUnconvertibleFileException : TranscodingException
	{
		public TranscodingUnconvertibleFileException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}
	}
}
