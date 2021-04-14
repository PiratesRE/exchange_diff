using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Management.Aggregation
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FacebookNewSubscriptionException : LocalizedException
	{
		public FacebookNewSubscriptionException(LocalizedString message) : base(message)
		{
		}

		public FacebookNewSubscriptionException(LocalizedString message, Exception innerException) : base(message, innerException)
		{
		}

		protected FacebookNewSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}
