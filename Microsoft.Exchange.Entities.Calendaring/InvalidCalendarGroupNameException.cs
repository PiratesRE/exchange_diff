using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidCalendarGroupNameException : InvalidRequestException
	{
		public InvalidCalendarGroupNameException() : base(CalendaringStrings.InvalidCalendarGroupName)
		{
		}

		public InvalidCalendarGroupNameException(Exception innerException) : base(CalendaringStrings.InvalidCalendarGroupName, innerException)
		{
		}

		protected InvalidCalendarGroupNameException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
