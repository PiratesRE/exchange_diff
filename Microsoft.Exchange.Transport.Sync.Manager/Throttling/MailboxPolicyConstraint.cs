using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Transport.Sync.Common.Logging;
using Microsoft.Exchange.Transport.Sync.Common.Subscription;

namespace Microsoft.Exchange.Transport.Sync.Manager.Throttling
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class MailboxPolicyConstraint
	{
		internal static MailboxPolicyConstraint Instance
		{
			get
			{
				return MailboxPolicyConstraint.instance;
			}
		}

		private MailboxPolicyConstraint()
		{
			this.probeInterval = ContentAggregationConfig.OwaMailboxPolicyProbeInterval;
		}

		internal bool WantsDispositionChangedToDeletion(DispatchEntry dispatchEntry, SyncLogSession syncLogSession)
		{
			return this.IsRelevantSubscriptionType(dispatchEntry) && this.PolicyCheckIsWarranted(dispatchEntry) && this.PolicyIsDisabledFor(dispatchEntry, syncLogSession);
		}

		private bool IsRelevantSubscriptionType(DispatchEntry dispatchEntry)
		{
			AggregationSubscriptionType subscriptionType = dispatchEntry.MiniSubscriptionInformation.SubscriptionType;
			return AggregationSubscriptionType.AllThatSupportPolicyInducedDeletion.HasFlag(subscriptionType);
		}

		internal bool PolicyCheckIsWarranted(DispatchEntry dispatchEntry)
		{
			return dispatchEntry.WorkType != WorkType.PolicyInducedDelete && this.RequisiteTimeHasElapsed(dispatchEntry);
		}

		internal bool RequisiteTimeHasElapsed(DispatchEntry dispatchEntry)
		{
			ExDateTime nextOwaMailboxPolicyProbeTime = dispatchEntry.MiniSubscriptionInformation.NextOwaMailboxPolicyProbeTime;
			return ExDateTime.Compare(ExDateTime.UtcNow, nextOwaMailboxPolicyProbeTime) > 0;
		}

		private bool PolicyIsDisabledFor(DispatchEntry dispatchEntry, SyncLogSession syncLogSession)
		{
			ConnectSubscriptionPolicySettings connectSubscriptionPolicySettings = this.PolicySettingsFor(dispatchEntry, syncLogSession);
			AggregationSubscriptionType subscriptionType = dispatchEntry.MiniSubscriptionInformation.SubscriptionType;
			AggregationSubscriptionType aggregationSubscriptionType = subscriptionType;
			if (aggregationSubscriptionType == AggregationSubscriptionType.Facebook)
			{
				return connectSubscriptionPolicySettings.IsFacebookDisabled;
			}
			if (aggregationSubscriptionType != AggregationSubscriptionType.LinkedIn)
			{
				throw new ArgumentException("Unsupported subscriptionType:" + subscriptionType);
			}
			return connectSubscriptionPolicySettings.IsLinkedInDisabled;
		}

		private ConnectSubscriptionPolicySettings PolicySettingsFor(DispatchEntry dispatchEntry, SyncLogSession syncLogSession)
		{
			MiniSubscriptionInformation miniSubscriptionInformation = dispatchEntry.MiniSubscriptionInformation;
			miniSubscriptionInformation.NextOwaMailboxPolicyProbeTime = ExDateTime.UtcNow + this.probeInterval;
			if (!miniSubscriptionInformation.ExternalDirectoryOrgId.Equals(Guid.Empty))
			{
				try
				{
					ADSessionSettings adsessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(miniSubscriptionInformation.ExternalDirectoryOrgId);
					ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMailboxGuid(adsessionSettings, miniSubscriptionInformation.MailboxGuid, null);
					IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 143, "PolicySettingsFor", "f:\\15.00.1497\\sources\\dev\\transportSync\\src\\Manager\\Throttling\\MailboxPolicyConstraint.cs");
					OwaSegmentationSettings owaSegmentationSettings = OwaSegmentationSettings.GetInstance(tenantOrTopologyConfigurationSession, exchangePrincipal.MailboxInfo.Configuration.OwaMailboxPolicy, exchangePrincipal.MailboxInfo.OrganizationId);
					return new ConnectSubscriptionPolicySettings(owaSegmentationSettings);
				}
				catch (LocalizedException ex)
				{
					syncLogSession.LogError((TSLID)238UL, "MPC.PolicySettingsFor: exception trying to read mailbox policy, ex:{0}", new object[]
					{
						ex
					});
				}
			}
			return ConnectSubscriptionPolicySettings.GetFallbackInstance();
		}

		private static readonly MailboxPolicyConstraint instance = new MailboxPolicyConstraint();

		private readonly TimeSpan probeInterval;
	}
}
