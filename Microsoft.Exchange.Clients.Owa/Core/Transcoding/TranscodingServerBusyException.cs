using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingServerBusyException : TranscodingException
	{
		public TranscodingServerBusyException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}
	}
}
