using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MiniSubscriptionInformation : IComparable<MiniSubscriptionInformation>
	{
		internal MiniSubscriptionInformation(Guid externalDirectoryOrgId, Guid databaseGuid, Guid mailboxGuid, Guid subscriptionGuid, AggregationSubscriptionType subscriptionType, ExDateTime nextOwaMailboxPolicyProbeTime)
		{
			SyncUtilities.ThrowIfGuidEmpty("databaseGuid", databaseGuid);
			SyncUtilities.ThrowIfGuidEmpty("mailboxGuid", mailboxGuid);
			SyncUtilities.ThrowIfGuidEmpty("subscriptionGuid", subscriptionGuid);
			SyncUtilities.ThrowIfArgumentNull("nextOwaMailboxPolicyProbeTime", nextOwaMailboxPolicyProbeTime);
			this.externalDirectoryOrgId = externalDirectoryOrgId;
			this.databaseGuid = databaseGuid;
			this.mailboxGuid = mailboxGuid;
			this.subscriptionGuid = subscriptionGuid;
			this.subscriptionType = subscriptionType;
			this.nextOwaMailboxPolicyProbeTime = nextOwaMailboxPolicyProbeTime;
		}

		internal Guid ExternalDirectoryOrgId
		{
			get
			{
				return this.externalDirectoryOrgId;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.databaseGuid;
			}
		}

		internal Guid MailboxGuid
		{
			get
			{
				return this.mailboxGuid;
			}
		}

		internal Guid SubscriptionGuid
		{
			get
			{
				return this.subscriptionGuid;
			}
		}

		internal AggregationSubscriptionType SubscriptionType
		{
			get
			{
				return this.subscriptionType;
			}
		}

		internal ExDateTime NextOwaMailboxPolicyProbeTime
		{
			get
			{
				return this.nextOwaMailboxPolicyProbeTime;
			}
			set
			{
				this.nextOwaMailboxPolicyProbeTime = value;
			}
		}

		public int CompareTo(MiniSubscriptionInformation miniSubscriptionInformation)
		{
			SyncUtilities.ThrowIfArgumentNull("miniSubscriptionInformation", miniSubscriptionInformation);
			return this.SubscriptionGuid.CompareTo(miniSubscriptionInformation.SubscriptionGuid);
		}

		private readonly Guid externalDirectoryOrgId;

		private readonly Guid databaseGuid;

		private readonly Guid mailboxGuid;

		private readonly Guid subscriptionGuid;

		private readonly AggregationSubscriptionType subscriptionType;

		private ExDateTime nextOwaMailboxPolicyProbeTime;
	}
}
