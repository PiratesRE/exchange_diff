using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class DefaultCalendarGroupCreationException : StorageTransientException
	{
		public DefaultCalendarGroupCreationException(string calendarType) : base(ServerStrings.idUnableToCreateDefaultCalendarGroupException(calendarType))
		{
			this.calendarType = calendarType;
		}

		public DefaultCalendarGroupCreationException(string calendarType, Exception innerException) : base(ServerStrings.idUnableToCreateDefaultCalendarGroupException(calendarType), innerException)
		{
			this.calendarType = calendarType;
		}

		protected DefaultCalendarGroupCreationException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.calendarType = (string)info.GetValue("calendarType", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("calendarType", this.calendarType);
		}

		public string CalendarType
		{
			get
			{
				return this.calendarType;
			}
		}

		private readonly string calendarType;
	}
}
