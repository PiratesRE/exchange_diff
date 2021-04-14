using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class InvalidPopupReminderSettingsCountException : InvalidRequestException
	{
		public InvalidPopupReminderSettingsCountException(int count) : base(CalendaringStrings.InvalidPopupReminderSettingsCount(count))
		{
			this.count = count;
		}

		public InvalidPopupReminderSettingsCountException(int count, Exception innerException) : base(CalendaringStrings.InvalidPopupReminderSettingsCount(count), innerException)
		{
			this.count = count;
		}

		protected InvalidPopupReminderSettingsCountException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.count = (int)info.GetValue("count", typeof(int));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("count", this.count);
		}

		public int Count
		{
			get
			{
				return this.count;
			}
		}

		private readonly int count;
	}
}
