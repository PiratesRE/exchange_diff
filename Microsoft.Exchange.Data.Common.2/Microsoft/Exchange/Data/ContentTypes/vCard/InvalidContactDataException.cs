using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ContentTypes.vCard
{
	[Serializable]
	public class InvalidContactDataException : ExchangeDataException
	{
		public InvalidContactDataException(string message) : base(message)
		{
		}

		public InvalidContactDataException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidContactDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
