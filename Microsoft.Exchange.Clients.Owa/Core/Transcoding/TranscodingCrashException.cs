using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal sealed class TranscodingCrashException : TranscodingException
	{
		public TranscodingCrashException(string message, Exception innerException, object theObj) : base(message, innerException, theObj)
		{
		}

		public TranscodingCrashException(string message, Exception innerException) : base(message, innerException)
		{
		}

		public TranscodingCrashException(string message) : base(message)
		{
		}
	}
}
