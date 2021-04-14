using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NullPopupReminderSettingsException : InvalidRequestException
	{
		public NullPopupReminderSettingsException() : base(CalendaringStrings.NullPopupReminderSettings)
		{
		}

		public NullPopupReminderSettingsException(Exception innerException) : base(CalendaringStrings.NullPopupReminderSettings, innerException)
		{
		}

		protected NullPopupReminderSettingsException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
