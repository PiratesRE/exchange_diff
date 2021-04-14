using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Client
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class PushNotificationEndpointNotFoundException : PushNotificationTransientException
	{
		public PushNotificationEndpointNotFoundException(LocalizedString message) : base(message)
		{
		}

		public PushNotificationEndpointNotFoundException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected PushNotificationEndpointNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
