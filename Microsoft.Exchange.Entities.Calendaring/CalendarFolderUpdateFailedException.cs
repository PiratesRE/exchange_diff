using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarFolderUpdateFailedException : CalendarUpdateFailedException
	{
		public CalendarFolderUpdateFailedException() : base(CalendaringStrings.CalendarFolderUpdateFailed)
		{
		}

		public CalendarFolderUpdateFailedException(Exception innerException) : base(CalendaringStrings.CalendarFolderUpdateFailed, innerException)
		{
		}

		protected CalendarFolderUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
