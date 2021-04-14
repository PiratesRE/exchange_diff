using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class CalendarNameCannotBeEmptyException : InvalidRequestException
	{
		public CalendarNameCannotBeEmptyException() : base(CalendaringStrings.CalendarNameCannotBeEmpty)
		{
		}

		public CalendarNameCannotBeEmptyException(Exception innerException) : base(CalendaringStrings.CalendarNameCannotBeEmpty, innerException)
		{
		}

		protected CalendarNameCannotBeEmptyException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
