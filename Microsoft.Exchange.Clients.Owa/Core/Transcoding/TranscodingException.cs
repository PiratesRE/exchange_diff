using System;

namespace Microsoft.Exchange.Clients.Owa.Core.Transcoding
{
	internal abstract class TranscodingException : OwaPermanentException
	{
		public TranscodingException(string message, Exception innerException, object thisObj) : base(message, innerException, thisObj)
		{
		}

		protected TranscodingException(string message, Exception innerException) : this(message, innerException, null)
		{
		}

		protected TranscodingException(string message) : base(message)
		{
		}
	}
}
