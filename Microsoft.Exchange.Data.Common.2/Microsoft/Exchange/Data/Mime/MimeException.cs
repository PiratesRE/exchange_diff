using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.CtsResources;

namespace Microsoft.Exchange.Data.Mime
{
	[Serializable]
	public class MimeException : ExchangeDataException
	{
		public MimeException(string message) : base(Strings.InternalMimeError + " " + message)
		{
		}

		public MimeException(string message, Exception innerException) : base(Strings.InternalMimeError + " " + message, innerException)
		{
		}

		protected MimeException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
