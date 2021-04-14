using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Configuration.Tasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class SubscriptionConfigDataProviderFactory
	{
		private SubscriptionConfigDataProviderFactory()
		{
		}

		public static SubscriptionConfigDataProviderFactory Instance
		{
			get
			{
				return SubscriptionConfigDataProviderFactory.instance;
			}
		}

		public AggregationSubscriptionDataProvider CreateSubscriptionDataProvider(AggregationType aggregationType, AggregationTaskType taskType, IRecipientSession session, ADUser adUser)
		{
			if (aggregationType <= AggregationType.Migration)
			{
				if (aggregationType != AggregationType.Aggregation && aggregationType != AggregationType.Migration)
				{
					goto IL_33;
				}
			}
			else
			{
				if (aggregationType == AggregationType.PeopleConnection)
				{
					return new ConnectSubscriptionDataProvider(taskType, session, adUser);
				}
				if (aggregationType != AggregationType.All)
				{
					goto IL_33;
				}
			}
			return new AggregationSubscriptionDataProvider(taskType, session, adUser);
			IL_33:
			throw new InvalidOperationException("Invalid Aggregation Type:" + aggregationType);
		}

		private static SubscriptionConfigDataProviderFactory instance = new SubscriptionConfigDataProviderFactory();
	}
}
