using System;
using System.Runtime.Serialization;
using System.Security.Permissions;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Common
{
	[SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
	[Serializable]
	internal class FailedDeletePeopleConnectSubscriptionException : LocalizedException
	{
		public FailedDeletePeopleConnectSubscriptionException(AggregationSubscriptionType subscriptionType) : base(Strings.FailedDeletePeopleConnectSubscription(subscriptionType))
		{
			this.subscriptionType = subscriptionType;
		}

		public FailedDeletePeopleConnectSubscriptionException(AggregationSubscriptionType subscriptionType, Exception innerException) : base(Strings.FailedDeletePeopleConnectSubscription(subscriptionType), innerException)
		{
			this.subscriptionType = subscriptionType;
		}

		protected FailedDeletePeopleConnectSubscriptionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.subscriptionType = (AggregationSubscriptionType)info.GetValue("subscriptionType", typeof(AggregationSubscriptionType));
		}

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("subscriptionType", this.subscriptionType);
		}

		public AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		private readonly AggregationSubscriptionType subscriptionType;
	}
}
