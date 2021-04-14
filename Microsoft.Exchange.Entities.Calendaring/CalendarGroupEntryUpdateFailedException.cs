using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarGroupEntryUpdateFailedException : CalendarUpdateFailedException
	{
		public CalendarGroupEntryUpdateFailedException() : base(CalendaringStrings.CalendarGroupEntryUpdateFailed)
		{
		}

		public CalendarGroupEntryUpdateFailedException(Exception innerException) : base(CalendaringStrings.CalendarGroupEntryUpdateFailed, innerException)
		{
		}

		protected CalendarGroupEntryUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
