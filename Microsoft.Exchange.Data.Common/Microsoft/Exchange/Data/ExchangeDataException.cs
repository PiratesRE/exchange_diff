using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data
{
	[Serializable]
	public class ExchangeDataException : LocalizedException
	{
		public ExchangeDataException(string message) : base(new LocalizedString(message))
		{
		}

		public ExchangeDataException(string message, Exception innerException) : base(new LocalizedString(message), innerException)
		{
		}

		protected ExchangeDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
