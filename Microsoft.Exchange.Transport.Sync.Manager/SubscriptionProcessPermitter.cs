using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;

namespace Microsoft.Exchange.Transport.Sync.Manager
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class SubscriptionProcessPermitter
	{
		internal SubscriptionProcessPermitter(GlobalSyncLogSession syncLogSession, Trace tracer, ISubscriptionProcessPermitterConfig configuration)
		{
			SyncUtilities.ThrowIfArgumentNull("syncLogSession", syncLogSession);
			SyncUtilities.ThrowIfArgumentNull("tracer", tracer);
			SyncUtilities.ThrowIfArgumentNull("configuration", configuration);
			this.syncLogSession = syncLogSession;
			this.tracer = tracer;
			this.configuration = configuration;
		}

		internal bool IsSubscriptionPermitted(ISubscriptionInformation subscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("subscriptionInformation", subscriptionInformation);
			return this.IsSubscriptionOfAggregationTypePermitted(subscriptionInformation.AggregationType, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid) && this.IsSubscriptionOfAggregationSubscriptionTypePermitted(subscriptionInformation.SubscriptionType, subscriptionInformation.SubscriptionGuid, subscriptionInformation.MailboxGuid);
		}

		private bool IsSubscriptionOfAggregationTypePermitted(AggregationType aggregationType, Guid subscriptionGuid, Guid mailboxGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			bool flag;
			if (aggregationType != AggregationType.Aggregation)
			{
				if (aggregationType != AggregationType.Migration)
				{
					if (aggregationType != AggregationType.PeopleConnection)
					{
						this.syncLogSession.LogError((TSLID)520UL, this.tracer, subscriptionGuid, mailboxGuid, "IsSubscriptionOfAggregationTypePermitted: Encountered invalid aggregation type: {0}", new object[]
						{
							aggregationType
						});
						return false;
					}
					flag = this.configuration.PeopleConnectionSubscriptionsEnabled;
				}
				else
				{
					flag = this.configuration.MigrationSubscriptionsEnabled;
				}
			}
			else
			{
				flag = this.configuration.AggregationSubscriptionsEnabled;
			}
			if (!flag)
			{
				this.syncLogSession.LogVerbose((TSLID)1323UL, this.tracer, subscriptionGuid, mailboxGuid, "IsSubscriptionOfAggregationTypePermitted: AggregationType '{0}' is not allowed.", new object[]
				{
					aggregationType
				});
			}
			return flag;
		}

		private bool IsSubscriptionOfAggregationSubscriptionTypePermitted(AggregationSubscriptionType aggregationSubscriptionType, Guid subscriptionGuid, Guid mailboxGuid)
		{
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			bool flag;
			if (aggregationSubscriptionType <= AggregationSubscriptionType.IMAP)
			{
				switch (aggregationSubscriptionType)
				{
				case AggregationSubscriptionType.Pop:
					flag = this.configuration.PopAggregationEnabled;
					goto IL_C5;
				case (AggregationSubscriptionType)3:
					break;
				case AggregationSubscriptionType.DeltaSyncMail:
					flag = this.configuration.DeltaSyncAggregationEnabled;
					goto IL_C5;
				default:
					if (aggregationSubscriptionType == AggregationSubscriptionType.IMAP)
					{
						flag = this.configuration.ImapAggregationEnabled;
						goto IL_C5;
					}
					break;
				}
			}
			else
			{
				if (aggregationSubscriptionType == AggregationSubscriptionType.Facebook)
				{
					flag = this.configuration.FacebookAggregationEnabled;
					goto IL_C5;
				}
				if (aggregationSubscriptionType == AggregationSubscriptionType.LinkedIn)
				{
					flag = this.configuration.LinkedInAggregationEnabled;
					goto IL_C5;
				}
			}
			this.syncLogSession.LogError((TSLID)524UL, this.tracer, subscriptionGuid, mailboxGuid, "IsSubscriptionOfAggregationSubscriptionTypePermitted: Encountered invalid subscription type: {0}", new object[]
			{
				aggregationSubscriptionType
			});
			return false;
			IL_C5:
			if (!flag)
			{
				this.syncLogSession.LogInformation((TSLID)214UL, this.tracer, subscriptionGuid, mailboxGuid, "IsSubscriptionOfAggregationSubscriptionTypePermitted: AggregationSubscriptionType '{0}' is not allowed.", new object[]
				{
					aggregationSubscriptionType
				});
			}
			return flag;
		}

		private GlobalSyncLogSession syncLogSession;

		private Trace tracer;

		private ISubscriptionProcessPermitterConfig configuration;
	}
}
