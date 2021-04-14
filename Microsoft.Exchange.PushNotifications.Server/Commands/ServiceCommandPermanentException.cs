using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Server.Commands
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ServiceCommandPermanentException : PushNotificationPermanentException
	{
		public ServiceCommandPermanentException(LocalizedString message) : base(message)
		{
		}

		public ServiceCommandPermanentException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ServiceCommandPermanentException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
