using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceCommandTransientException : PushNotificationTransientException
	{
		public ServiceCommandTransientException(LocalizedString message) : base(message)
		{
		}

		public ServiceCommandTransientException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ServiceCommandTransientException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
