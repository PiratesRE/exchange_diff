using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Data.ContentTypes.iCalendar
{
	[Serializable]
	public class InvalidCalendarDataException : ExchangeDataException
	{
		public InvalidCalendarDataException(string message) : base(message)
		{
		}

		public InvalidCalendarDataException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected InvalidCalendarDataException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
