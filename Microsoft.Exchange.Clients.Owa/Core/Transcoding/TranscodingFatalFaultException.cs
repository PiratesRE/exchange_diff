using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingFatalFaultException : TranscodingException
	{
		public TranscodingFatalFaultException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}

		public TranscodingFatalFaultException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TranscodingFatalFaultException(string message) : base(message)
		{
		}
	}
}
