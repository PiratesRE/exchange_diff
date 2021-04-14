using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotDeleteDefaultCalendarException : InvalidRequestException
	{
		public CannotDeleteDefaultCalendarException() : base(CalendaringStrings.CannotDeleteDefaultCalendar)
		{
		}

		public CannotDeleteDefaultCalendarException(Exception innerException) : base(CalendaringStrings.CannotDeleteDefaultCalendar, innerException)
		{
		}

		protected CannotDeleteDefaultCalendarException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
