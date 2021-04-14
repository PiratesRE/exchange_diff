using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.MessagingPolicies.UnJournalAgent
{
	internal sealed class ArchiveJournalTenantConfiguration
	{
		public ArchiveJournalTenantConfiguration(OrganizationId orgId, PerTenantTransportSettings perTenantTransportSettings, MicrosoftExchangeRecipientPerTenantSettings perTenantMerConfig)
		{
			if (perTenantTransportSettings == null || perTenantMerConfig == null)
			{
				throw new ArgumentNullException("must specify pertenanttransportsettings and pertenantmerconfig");
			}
			this.merRoutingAddress = RoutingAddress.Parse(perTenantMerConfig.PrimarySmtpAddress.ToString());
			SmtpAddress journalingReportNdrTo = perTenantTransportSettings.JournalingReportNdrTo;
			if (perTenantTransportSettings.JournalingReportNdrTo.IsValidAddress && perTenantTransportSettings.JournalingReportNdrTo != SmtpAddress.NullReversePath)
			{
				this.journalingReportNdrToRoutingAddress = RoutingAddress.Parse(perTenantTransportSettings.JournalingReportNdrTo.ToString());
			}
			else
			{
				this.journalingReportNdrToRoutingAddress = this.merRoutingAddress;
			}
			if (orgId != OrganizationId.ForestWideOrgId || VariantConfiguration.InvariantNoFlightingSnapshot.Ipaed.ProcessForestWideOrgJournal.Enabled)
			{
				this.legacyArchiveJournalingEnabled = perTenantTransportSettings.LegacyArchiveJournalingEnabled;
				this.dropUnprovisionedUserMessages = perTenantTransportSettings.DropUnprovisionedUserMessagesForLegacyArchiveJournaling;
				this.redirectDistributionListsMessages = perTenantTransportSettings.RedirectDLMessagesForLegacyArchiveJournaling;
				this.legacyArchiveLiveJournalingEnabled = perTenantTransportSettings.LegacyArchiveLiveJournalingEnabled;
				this.journalArchivingEnabled = perTenantTransportSettings.JournalArchivingEnabled;
			}
		}

		internal RoutingAddress EhaMigrationMailboxAddress
		{
			get
			{
				return this.ehaMigrationMailboxAddress;
			}
			set
			{
				this.ehaMigrationMailboxAddress = value;
			}
		}

		internal RoutingAddress MSExchangeRecipient
		{
			get
			{
				return this.merRoutingAddress;
			}
		}

		internal RoutingAddress JournalReportNdrTo
		{
			get
			{
				return this.journalingReportNdrToRoutingAddress;
			}
		}

		internal RoutingAddress JournalReportNdrToForEhaMigration
		{
			get
			{
				return this.journalingReportNdrToRoutingAddress;
			}
		}

		internal bool LegacyArchiveJournalingEnabled
		{
			get
			{
				return this.legacyArchiveJournalingEnabled;
			}
		}

		internal bool LegacyArchiveLiveJournalingEnabled
		{
			get
			{
				return this.legacyArchiveLiveJournalingEnabled;
			}
		}

		internal bool JournalArchivingEnabled
		{
			get
			{
				return this.journalArchivingEnabled;
			}
		}

		internal bool RedirectDistributionListMessages
		{
			get
			{
				return this.redirectDistributionListsMessages;
			}
		}

		internal bool DropUnprovisionedUsersMessages
		{
			get
			{
				return this.dropUnprovisionedUserMessages;
			}
		}

		internal bool DropJournalsWithPermanentErrors
		{
			get
			{
				return ArchiveJournalTenantConfiguration.dropJournalsWithPermanentErrors;
			}
		}

		internal static ArchiveJournalTenantConfiguration GetTenantConfig(OrganizationId organizationId)
		{
			PerTenantTransportSettings perTenantTransportSettings;
			if (!Components.Configuration.TryGetTransportSettings(organizationId, out perTenantTransportSettings))
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "UnjournalAgent: Transport Settings could not be loaded for tenant: {0}", organizationId);
				return null;
			}
			MicrosoftExchangeRecipientPerTenantSettings microsoftExchangeRecipientPerTenantSettings;
			if (!Components.Configuration.TryGetMicrosoftExchangeRecipient(organizationId, out microsoftExchangeRecipientPerTenantSettings))
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "UnjournalAgent: MER config could not be loaded tenant: {0}", organizationId);
				return null;
			}
			SmtpAddress primarySmtpAddress = microsoftExchangeRecipientPerTenantSettings.PrimarySmtpAddress;
			if (!microsoftExchangeRecipientPerTenantSettings.PrimarySmtpAddress.IsValidAddress)
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "UnjournalAgent: MER config not valid for this tenant: {0}", organizationId);
				return null;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug<OrganizationId>(0L, "Unjournal agent: Loaded tenant config: {0}", organizationId);
			return new ArchiveJournalTenantConfiguration(organizationId, perTenantTransportSettings, microsoftExchangeRecipientPerTenantSettings);
		}

		private static readonly bool dropJournalsWithPermanentErrors = true;

		private readonly bool legacyArchiveJournalingEnabled;

		private readonly bool legacyArchiveLiveJournalingEnabled;

		private readonly bool journalArchivingEnabled;

		private readonly bool dropUnprovisionedUserMessages = true;

		private readonly bool redirectDistributionListsMessages;

		private RoutingAddress journalingReportNdrToRoutingAddress;

		private RoutingAddress ehaMigrationMailboxAddress;

		private RoutingAddress merRoutingAddress;
	}
}
