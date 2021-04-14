using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.TextConverters
{
	[Serializable]
	public class TextConvertersException : ExchangeDataException
	{
		internal TextConvertersException() : base("internal text conversion error (document too complex)")
		{
		}

		public TextConvertersException(string message) : base(message)
		{
		}

		public TextConvertersException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected TextConvertersException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
