using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class CalendarProcessingException : CorruptDataException
	{
		public CalendarProcessingException(LocalizedString message) : base(message)
		{
		}

		public CalendarProcessingException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CalendarProcessingException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}
