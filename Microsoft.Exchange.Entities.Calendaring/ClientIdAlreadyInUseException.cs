using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Entities.DataProviders;

namespace Microsoft.Exchange.Entities.Calendaring
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class ClientIdAlreadyInUseException : InvalidRequestException
	{
		public ClientIdAlreadyInUseException() : base(CalendaringStrings.ClientIdAlreadyInUse)
		{
		}

		public ClientIdAlreadyInUseException(Exception innerException) : base(CalendaringStrings.ClientIdAlreadyInUse, innerException)
		{
		}

		protected ClientIdAlreadyInUseException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
