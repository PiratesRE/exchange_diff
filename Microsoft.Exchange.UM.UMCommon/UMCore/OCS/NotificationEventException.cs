using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.UM.UMCore.OCS
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class NotificationEventException : LocalizedException
	{
		public NotificationEventException(LocalizedString message) : base(message)
		{
		}

		public NotificationEventException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected NotificationEventException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
