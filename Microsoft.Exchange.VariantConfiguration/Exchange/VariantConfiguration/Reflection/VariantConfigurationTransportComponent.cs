using System;
using Microsoft.Exchange.MessageDepot;

namespace Microsoft.Exchange.VariantConfiguration.Reflection
{
	public sealed class VariantConfigurationTransportComponent : VariantConfigurationComponent
	{
		internal VariantConfigurationTransportComponent() : base("Transport")
		{
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "VerboseLogging", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "TargetAddressRoutingForRemoteGroupMailbox", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "MessageDepot", typeof(IMessageDepotSettings), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SelectHubServersForClientProxy", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "TestProcessingQuota", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SystemMessageOverrides", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "DirectTrustCache", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "UseNewConnectorMatchingOrder", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "TargetAddressDistributionGroupAsExternal", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "ConsolidateAdvancedRouting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "ClientAuthRequireMailboxDatabase", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "UseTenantPartitionToCreateOrganizationId", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "LimitTransportRules", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SmtpAcceptAnyRecipient", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "RiskBasedCounters", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "DefaultTransportServiceStateToInactive", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "LimitRemoteDomains", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "IgnoreArbitrationMailboxForModeratedRecipient", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "TransferAdditionalTenantDataThroughXATTR", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "ADExceptionHandling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "EnforceProcessingQuota", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SystemProbeDropAgent", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SetMustDeliverJournalReport", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SendUserAddressInXproxyCommand", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "UseAdditionalTenantDataFromXATTR", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "DelayDsn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "ExplicitDeletedObjectNotifications", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "EnforceQueueQuota", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "OrganizationMailboxRouting", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "StringentHeaderTransformationMode", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SmtpReceiveCountersStripServerName", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "ClientSubmissionToDelivery", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "EnforceOutboundConnectorAndAcceptedDomainsRestriction", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "TenantThrottling", typeof(IFeature), false));
			base.Add(new VariantConfigurationSection("Transport.settings.ini", "SystemProbeLogging", typeof(IFeature), false));
		}

		public VariantConfigurationSection VerboseLogging
		{
			get
			{
				return base["VerboseLogging"];
			}
		}

		public VariantConfigurationSection TargetAddressRoutingForRemoteGroupMailbox
		{
			get
			{
				return base["TargetAddressRoutingForRemoteGroupMailbox"];
			}
		}

		public VariantConfigurationSection MessageDepot
		{
			get
			{
				return base["MessageDepot"];
			}
		}

		public VariantConfigurationSection SelectHubServersForClientProxy
		{
			get
			{
				return base["SelectHubServersForClientProxy"];
			}
		}

		public VariantConfigurationSection TestProcessingQuota
		{
			get
			{
				return base["TestProcessingQuota"];
			}
		}

		public VariantConfigurationSection SystemMessageOverrides
		{
			get
			{
				return base["SystemMessageOverrides"];
			}
		}

		public VariantConfigurationSection DirectTrustCache
		{
			get
			{
				return base["DirectTrustCache"];
			}
		}

		public VariantConfigurationSection UseNewConnectorMatchingOrder
		{
			get
			{
				return base["UseNewConnectorMatchingOrder"];
			}
		}

		public VariantConfigurationSection TargetAddressDistributionGroupAsExternal
		{
			get
			{
				return base["TargetAddressDistributionGroupAsExternal"];
			}
		}

		public VariantConfigurationSection ConsolidateAdvancedRouting
		{
			get
			{
				return base["ConsolidateAdvancedRouting"];
			}
		}

		public VariantConfigurationSection ClientAuthRequireMailboxDatabase
		{
			get
			{
				return base["ClientAuthRequireMailboxDatabase"];
			}
		}

		public VariantConfigurationSection UseTenantPartitionToCreateOrganizationId
		{
			get
			{
				return base["UseTenantPartitionToCreateOrganizationId"];
			}
		}

		public VariantConfigurationSection LimitTransportRules
		{
			get
			{
				return base["LimitTransportRules"];
			}
		}

		public VariantConfigurationSection SmtpAcceptAnyRecipient
		{
			get
			{
				return base["SmtpAcceptAnyRecipient"];
			}
		}

		public VariantConfigurationSection RiskBasedCounters
		{
			get
			{
				return base["RiskBasedCounters"];
			}
		}

		public VariantConfigurationSection DefaultTransportServiceStateToInactive
		{
			get
			{
				return base["DefaultTransportServiceStateToInactive"];
			}
		}

		public VariantConfigurationSection LimitRemoteDomains
		{
			get
			{
				return base["LimitRemoteDomains"];
			}
		}

		public VariantConfigurationSection IgnoreArbitrationMailboxForModeratedRecipient
		{
			get
			{
				return base["IgnoreArbitrationMailboxForModeratedRecipient"];
			}
		}

		public VariantConfigurationSection TransferAdditionalTenantDataThroughXATTR
		{
			get
			{
				return base["TransferAdditionalTenantDataThroughXATTR"];
			}
		}

		public VariantConfigurationSection ADExceptionHandling
		{
			get
			{
				return base["ADExceptionHandling"];
			}
		}

		public VariantConfigurationSection EnforceProcessingQuota
		{
			get
			{
				return base["EnforceProcessingQuota"];
			}
		}

		public VariantConfigurationSection SystemProbeDropAgent
		{
			get
			{
				return base["SystemProbeDropAgent"];
			}
		}

		public VariantConfigurationSection SetMustDeliverJournalReport
		{
			get
			{
				return base["SetMustDeliverJournalReport"];
			}
		}

		public VariantConfigurationSection SendUserAddressInXproxyCommand
		{
			get
			{
				return base["SendUserAddressInXproxyCommand"];
			}
		}

		public VariantConfigurationSection UseAdditionalTenantDataFromXATTR
		{
			get
			{
				return base["UseAdditionalTenantDataFromXATTR"];
			}
		}

		public VariantConfigurationSection DelayDsn
		{
			get
			{
				return base["DelayDsn"];
			}
		}

		public VariantConfigurationSection ExplicitDeletedObjectNotifications
		{
			get
			{
				return base["ExplicitDeletedObjectNotifications"];
			}
		}

		public VariantConfigurationSection EnforceQueueQuota
		{
			get
			{
				return base["EnforceQueueQuota"];
			}
		}

		public VariantConfigurationSection OrganizationMailboxRouting
		{
			get
			{
				return base["OrganizationMailboxRouting"];
			}
		}

		public VariantConfigurationSection StringentHeaderTransformationMode
		{
			get
			{
				return base["StringentHeaderTransformationMode"];
			}
		}

		public VariantConfigurationSection SmtpReceiveCountersStripServerName
		{
			get
			{
				return base["SmtpReceiveCountersStripServerName"];
			}
		}

		public VariantConfigurationSection ClientSubmissionToDelivery
		{
			get
			{
				return base["ClientSubmissionToDelivery"];
			}
		}

		public VariantConfigurationSection SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn
		{
			get
			{
				return base["SmtpXproxyConstructUpnFromSamAccountNameAndParitionFqdn"];
			}
		}

		public VariantConfigurationSection EnforceOutboundConnectorAndAcceptedDomainsRestriction
		{
			get
			{
				return base["EnforceOutboundConnectorAndAcceptedDomainsRestriction"];
			}
		}

		public VariantConfigurationSection TenantThrottling
		{
			get
			{
				return base["TenantThrottling"];
			}
		}

		public VariantConfigurationSection SystemProbeLogging
		{
			get
			{
				return base["SystemProbeLogging"];
			}
		}
	}
}
