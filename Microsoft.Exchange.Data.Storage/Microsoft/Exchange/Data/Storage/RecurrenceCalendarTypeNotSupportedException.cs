using System;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Data.Storage
{
	[Serializable]
	public class RecurrenceCalendarTypeNotSupportedException : RecurrenceFormatException
	{
		internal RecurrenceCalendarTypeNotSupportedException(LocalizedString message, CalendarType calendarType, Stream stream) : base(message, stream)
		{
			this.calendarType = calendarType;
		}

		protected RecurrenceCalendarTypeNotSupportedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.calendarType = (CalendarType)info.GetValue("calendarType", typeof(CalendarType));
		}

		public CalendarType CalendarType
		{
			get
			{
				return this.calendarType;
			}
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("calendarType", this.calendarType);
		}

		private const string CalendarTypeLabel = "calendarType";

		private CalendarType calendarType;
	}
}
