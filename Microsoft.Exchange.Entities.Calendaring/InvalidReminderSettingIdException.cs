using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidReminderSettingIdException : InvalidRequestException
	{
		public InvalidReminderSettingIdException() : base(CalendaringStrings.InvalidReminderSettingId)
		{
		}

		public InvalidReminderSettingIdException(Exception innerException) : base(CalendaringStrings.InvalidReminderSettingId, innerException)
		{
		}

		protected InvalidReminderSettingIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
