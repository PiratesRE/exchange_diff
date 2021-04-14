using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PushNotificationTransientException : LocalizedException
	{
		public PushNotificationTransientException(LocalizedString message) : base(message)
		{
		}

		public PushNotificationTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PushNotificationTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
