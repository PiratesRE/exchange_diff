using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidNewReminderSettingIdException : InvalidRequestException
	{
		public InvalidNewReminderSettingIdException() : base(CalendaringStrings.InvalidNewReminderSettingId)
		{
		}

		public InvalidNewReminderSettingIdException(Exception innerException) : base(CalendaringStrings.InvalidNewReminderSettingId, innerException)
		{
		}

		protected InvalidNewReminderSettingIdException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
