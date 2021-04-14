using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.Mime.Encoders
{
	[Serializable]
	public class ByteEncoderException : ExchangeDataException
	{
		public ByteEncoderException(string message) : base(message)
		{
		}

		public ByteEncoderException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected ByteEncoderException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
