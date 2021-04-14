using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CannotRenameDefaultCalendarException : InvalidRequestException
	{
		public CannotRenameDefaultCalendarException() : base(CalendaringStrings.CannotRenameDefaultCalendar)
		{
		}

		public CannotRenameDefaultCalendarException(Exception innerException) : base(CalendaringStrings.CannotRenameDefaultCalendar, innerException)
		{
		}

		protected CannotRenameDefaultCalendarException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
