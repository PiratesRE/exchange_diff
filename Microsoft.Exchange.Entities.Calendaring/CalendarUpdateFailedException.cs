using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarUpdateFailedException : StoragePermanentException
	{
		public CalendarUpdateFailedException(LocalizedString message) : base(message)
		{
		}

		public CalendarUpdateFailedException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected CalendarUpdateFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
