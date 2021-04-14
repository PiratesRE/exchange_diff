using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotificationEventFormatException : NotificationEventException
	{
		public NotificationEventFormatException() : base(Strings.NotificationEventFormatException)
		{
		}

		public NotificationEventFormatException(Exception innerException) : base(Strings.NotificationEventFormatException, innerException)
		{
		}

		protected NotificationEventFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
