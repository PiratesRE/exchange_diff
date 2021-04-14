using System;
using System.Collections.Generic;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Aggregation;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Imap;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pim;
using Microsoft.Exchange.Transport.Sync.Common.Subscription.Pop;

namespace Microsoft.Exchange.Management.TransportSync
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class AggregationSubscriptionConstraintChecker
	{
		public AggregationSubscriptionConstraintChecker(SyncLogSession syncLogSession)
		{
			this.syncLogSession = syncLogSession;
		}

		public AggregationSubscriptionConstraintChecker() : this(CommonLoggingHelper.SyncLogSession)
		{
		}

		public void CheckNewSubscriptionConstraints(PimAggregationSubscription newSubscription, IList<AggregationSubscription> subscriptions, int maxSubscriptionAllowed)
		{
			SyncUtilities.ThrowIfArgumentNull("newSubscription", newSubscription);
			if (subscriptions == null)
			{
				return;
			}
			this.CheckUpdateSubscriptionConstraints(newSubscription, subscriptions);
			int num = 0;
			foreach (AggregationSubscription aggregationSubscription in subscriptions)
			{
				if (aggregationSubscription.AggregationType == newSubscription.AggregationType)
				{
					num++;
				}
			}
			this.CheckIfCanCreateNewSubscriptionConstraint(newSubscription, maxSubscriptionAllowed, num);
		}

		public virtual void CheckUpdateSubscriptionConstraints(PimAggregationSubscription updatedSubscription, IList<AggregationSubscription> subscriptions)
		{
			SyncUtilities.ThrowIfArgumentNull("UpdatedSubscription", updatedSubscription);
			if (subscriptions == null)
			{
				return;
			}
			foreach (AggregationSubscription aggregationSubscription in subscriptions)
			{
				if (!aggregationSubscription.SubscriptionGuid.Equals(updatedSubscription.SubscriptionGuid))
				{
					this.CheckUpdateSubscriptionConstraints(updatedSubscription, aggregationSubscription as PimAggregationSubscription);
				}
			}
		}

		private void CheckUpdateSubscriptionConstraints(PimAggregationSubscription newSubscription, PimAggregationSubscription existingSubscription)
		{
			this.CheckSameSubscriptionNameConstraint(newSubscription, existingSubscription);
			if (newSubscription.AggregationType != AggregationType.PeopleConnection)
			{
				this.CheckSubscriptionEmailAddressesConstraint(newSubscription, existingSubscription);
				if (newSubscription is PopAggregationSubscription)
				{
					this.CheckPOPSubscriptionConstraint(newSubscription, existingSubscription);
					return;
				}
				if (newSubscription is IMAPAggregationSubscription)
				{
					this.CheckIMAPSubscriptionConstraint(newSubscription, existingSubscription);
				}
			}
		}

		private void CheckSameSubscriptionNameConstraint(PimAggregationSubscription newSubscription, PimAggregationSubscription existingSubscription)
		{
			if (string.Equals(existingSubscription.Name, newSubscription.Name, StringComparison.InvariantCultureIgnoreCase))
			{
				this.syncLogSession.LogError((TSLID)1248UL, AggregationTaskUtils.Tracer, (long)existingSubscription.Name.GetHashCode(), "Subscription creation failed: {0}. There is already a subscription with the same name", new object[]
				{
					existingSubscription.Name
				});
				throw new SubscriptionNameAlreadyExistsException(existingSubscription.Name);
			}
		}

		private void CheckSubscriptionEmailAddressesConstraint(PimAggregationSubscription newSubscription, PimAggregationSubscription existingSubscription)
		{
			if (newSubscription.AggregationType != existingSubscription.AggregationType)
			{
				return;
			}
			if (!(newSubscription.UserEmailAddress == existingSubscription.UserEmailAddress))
			{
				return;
			}
			string text = newSubscription.UserEmailAddress.ToString();
			this.syncLogSession.LogError((TSLID)1247UL, AggregationTaskUtils.Tracer, (long)newSubscription.Name.GetHashCode(), "Cannot create subscription. A subscription with email address {0} already exists", new object[]
			{
				text
			});
			throw new RedundantPimSubscriptionException(text);
		}

		private void CheckPOPSubscriptionConstraint(PimAggregationSubscription newSubscription, PimAggregationSubscription existingSubscription)
		{
			PopAggregationSubscription popAggregationSubscription = existingSubscription as PopAggregationSubscription;
			if (popAggregationSubscription == null)
			{
				return;
			}
			PopAggregationSubscription popAggregationSubscription2 = newSubscription as PopAggregationSubscription;
			if (popAggregationSubscription2 != null && string.Compare(popAggregationSubscription2.PopServer, popAggregationSubscription.PopServer, true) == 0 && string.Compare(popAggregationSubscription2.PopLogonName, popAggregationSubscription.PopLogonName, true) == 0)
			{
				string text = popAggregationSubscription2.PopServer.ToString();
				this.syncLogSession.LogError((TSLID)1249UL, AggregationTaskUtils.Tracer, (long)newSubscription.Name.GetHashCode(), "Cannot create subscription. A POP subscription for username {0} on server {1} already exists", new object[]
				{
					popAggregationSubscription2.PopLogonName,
					text
				});
				throw new RedundantAccountSubscriptionException(popAggregationSubscription2.PopLogonName, text);
			}
		}

		private void CheckIMAPSubscriptionConstraint(PimAggregationSubscription newSubscription, PimAggregationSubscription existingSubscription)
		{
			IMAPAggregationSubscription imapaggregationSubscription = existingSubscription as IMAPAggregationSubscription;
			if (imapaggregationSubscription == null)
			{
				return;
			}
			IMAPAggregationSubscription imapaggregationSubscription2 = newSubscription as IMAPAggregationSubscription;
			if (imapaggregationSubscription2 != null && string.Compare(imapaggregationSubscription2.IMAPServer, imapaggregationSubscription.IMAPServer, true) == 0 && string.Compare(imapaggregationSubscription2.IMAPLogOnName, imapaggregationSubscription.IMAPLogOnName, true) == 0)
			{
				string text = imapaggregationSubscription2.IMAPServer.ToString();
				this.syncLogSession.LogError((TSLID)1250UL, AggregationTaskUtils.Tracer, (long)newSubscription.Name.GetHashCode(), "Cannot create subscription. An IMAP subscription for username {0} on server {1} already exists", new object[]
				{
					imapaggregationSubscription2.IMAPLogOnName,
					text
				});
				throw new RedundantAccountSubscriptionException(imapaggregationSubscription2.IMAPLogOnName, text);
			}
		}

		private void CheckIfCanCreateNewSubscriptionConstraint(PimAggregationSubscription newSubscription, int maxSubscriptionAllowed, int subscriptionCount)
		{
			if (subscriptionCount >= maxSubscriptionAllowed)
			{
				this.syncLogSession.LogError((TSLID)1251UL, AggregationTaskUtils.Tracer, (long)newSubscription.Name.GetHashCode(), "Every user can only have {0} subscriptions", new object[]
				{
					maxSubscriptionAllowed
				});
				throw new SubscriptionNumberExceedLimitException(maxSubscriptionAllowed);
			}
		}

		private readonly SyncLogSession syncLogSession;
	}
}
