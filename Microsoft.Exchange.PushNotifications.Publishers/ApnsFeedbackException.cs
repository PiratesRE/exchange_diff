using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.PushNotifications.Publishers
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	public class ApnsFeedbackException : PushNotificationPermanentException
	{
		public ApnsFeedbackException(LocalizedString message) : base(message)
		{
		}

		public ApnsFeedbackException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected ApnsFeedbackException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
