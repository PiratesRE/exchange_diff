using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class GcmResponseFormatException : PushNotificationPermanentException
	{
		public GcmResponseFormatException(LocalizedString message) : base(message)
		{
		}

		public GcmResponseFormatException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected GcmResponseFormatException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
