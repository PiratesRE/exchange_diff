using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class SubscriptionSyncException : LocalizedException
	{
		public SubscriptionSyncException(string subscriptionName) : base(Strings.SubscriptionSyncException(subscriptionName))
		{
			this.subscriptionName = subscriptionName;
		}

		public SubscriptionSyncException(string subscriptionName, Exception innerException) : base(Strings.SubscriptionSyncException(subscriptionName), innerException)
		{
			this.subscriptionName = subscriptionName;
		}

		protected SubscriptionSyncException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.subscriptionName = (string)info.GetValue("subscriptionName", typeof(string));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("subscriptionName", this.subscriptionName);
		}

		public string SubscriptionName
		{
			get
			{
				return this.subscriptionName;
			}
		}

		private readonly string subscriptionName;
	}
}
