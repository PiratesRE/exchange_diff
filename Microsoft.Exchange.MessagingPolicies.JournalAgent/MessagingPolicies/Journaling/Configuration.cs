using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.MessagingPolicies;
using Microsoft.Exchange.Transport;
using Microsoft.Exchange.Transport.Configuration;
using Microsoft.Exchange.Transport.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Journaling
{
	internal sealed class Configuration
	{
		public static Configuration GetConfig(OrganizationId organizationId)
		{
			if (organizationId != OrganizationId.ForestWideOrgId)
			{
				ExTraceGlobals.JournalingTracer.TraceDebug<OrganizationId>(0L, "Getting config for: {0}", organizationId);
				return Configuration.GetTenantConfig(organizationId);
			}
			ExTraceGlobals.JournalingTracer.TraceDebug(0L, "Getting GetEnterpriseConfig config");
			return Configuration.GetEnterpriseConfig();
		}

		internal static Configuration GetEnterpriseConfig()
		{
			TransportConfigContainer transportConfigObject = Configuration.TransportConfigObject;
			if (transportConfigObject == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "Transport Settings could not be loaded");
				return null;
			}
			if (!transportConfigObject.JournalingReportNdrTo.IsValidAddress)
			{
				ExTraceGlobals.JournalingTracer.TraceError<SmtpAddress>(0L, "Invalid JournalingReportNdrTo: {0}", transportConfigObject.JournalingReportNdrTo);
				return null;
			}
			MicrosoftExchangeRecipientConfiguration microsoftExchangeRecipientConfiguration = GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance;
			if (microsoftExchangeRecipientConfiguration == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "MER could not be loaded");
				return null;
			}
			JournalingRules config = JournalingRules.GetConfig(OrganizationId.ForestWideOrgId);
			if (config == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "Rules could not be loaded");
				return null;
			}
			ReconcileConfig reconcileConfig = ReconcileConfig.GetInstance(OrganizationId.ForestWideOrgId);
			if (reconcileConfig == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError(0L, "Reconciliation config could not be loaded");
				return null;
			}
			Configuration configuration = Configuration.instance;
			if (configuration == null || configuration.transportSettingsConfiguration != transportConfigObject || configuration.merConfiguration != microsoftExchangeRecipientConfiguration || configuration.rules != config || configuration.reconcileConfiguration != reconcileConfig)
			{
				lock (Configuration.lockObject)
				{
					configuration = Configuration.instance;
					if (configuration == null || configuration.transportSettingsConfiguration != transportConfigObject || configuration.merConfiguration != microsoftExchangeRecipientConfiguration || configuration.rules != config || configuration.reconcileConfiguration != reconcileConfig)
					{
						Configuration.instance = new Configuration(transportConfigObject, microsoftExchangeRecipientConfiguration, config, reconcileConfig, null);
					}
				}
			}
			return Configuration.instance;
		}

		internal static Configuration GetTenantConfig(OrganizationId organizationId)
		{
			PerTenantTransportSettings settings;
			if (!Components.Configuration.TryGetTransportSettings(organizationId, out settings))
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Transport Settings could not be loaded for tenant: {0}", organizationId);
				return null;
			}
			MicrosoftExchangeRecipientPerTenantSettings merConfig;
			if (!Components.Configuration.TryGetMicrosoftExchangeRecipient(organizationId, out merConfig))
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "MER config could not be loaded tenant: {0}", organizationId);
				return null;
			}
			JournalingRules config = JournalingRules.GetConfig(organizationId);
			if (config == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Rules config could not be loaded for tenant: {0}", organizationId);
				return null;
			}
			List<GccRuleEntry> gccConfig = JournalingRules.GetGccConfig();
			if (gccConfig == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "GCC Rules config could not be loaded while processing tenant: {0}", organizationId);
				JournalingGlobals.Logger.LogEvent(MessagingPoliciesEventLogConstants.Tuple_JournalingRulesConfigurationLoadError, null, new object[0]);
			}
			ReconcileConfig reconcileConfig = ReconcileConfig.GetInstance(organizationId);
			if (reconcileConfig == null)
			{
				ExTraceGlobals.JournalingTracer.TraceError<OrganizationId>(0L, "Reconciliation config was corrupt for tenant: {0}", organizationId);
				return null;
			}
			ExTraceGlobals.JournalingTracer.TraceDebug<OrganizationId>(0L, "Loaded tenant config: {0}", organizationId);
			return new Configuration(organizationId, settings, merConfig, config, reconcileConfig, gccConfig);
		}

		internal OrganizationId OrganizationId
		{
			get
			{
				return this.organizationId;
			}
		}

		internal RoutingAddress JournalReportNdrTo
		{
			get
			{
				if (null != this.organizationId && this.organizationId != OrganizationId.ForestWideOrgId && this.journalingReportNdrToRoutingAddress.Equals(RoutingAddress.NullReversePath))
				{
					return this.MSExchangeRecipient;
				}
				return this.journalingReportNdrToRoutingAddress;
			}
		}

		internal RoutingAddress JournalReportNdrToForGcc
		{
			get
			{
				return this.journalingReportNdrToRoutingAddressForGcc;
			}
		}

		internal RoutingAddress MSExchangeRecipient
		{
			get
			{
				return this.merRoutingAddress;
			}
		}

		internal bool SkipUMVoicemailMessages
		{
			get
			{
				if (this.organizationId == OrganizationId.ForestWideOrgId)
				{
					return !this.transportSettingsConfiguration.VoicemailJournalingEnabled;
				}
				return !this.perTenantTransportSettings.VoicemailJournalingEnabled;
			}
		}

		internal bool LegacyJournalingMigrationEnabled
		{
			get
			{
				if (this.organizationId == OrganizationId.ForestWideOrgId)
				{
					return this.transportSettingsConfiguration.LegacyJournalingMigrationEnabled;
				}
				return this.perTenantTransportSettings.LegacyJournalingMigrationEnabled;
			}
		}

		internal bool LegacyArchiveJournalingEnabled
		{
			get
			{
				return !(this.organizationId == OrganizationId.ForestWideOrgId) && this.perTenantTransportSettings.LegacyArchiveJournalingEnabled;
			}
		}

		internal bool JournalReportDLMemberSubstitutionEnabled
		{
			get
			{
				return this.organizationId == OrganizationId.ForestWideOrgId && this.transportSettingsConfiguration.JournalReportDLMemberSubstitutionEnabled;
			}
		}

		internal bool LegacyArchiveLiveJournalingEnabled
		{
			get
			{
				return !(this.organizationId == OrganizationId.ForestWideOrgId) && this.perTenantTransportSettings.LegacyArchiveLiveJournalingEnabled;
			}
		}

		internal JournalingRules Rules
		{
			get
			{
				return this.rules;
			}
		}

		internal List<GccRuleEntry> GCCRules
		{
			get
			{
				return this.gccRules;
			}
		}

		internal ReconcileConfig ReconcileConfig
		{
			get
			{
				return this.reconcileConfiguration;
			}
		}

		private Configuration(TransportConfigContainer settings, MicrosoftExchangeRecipientConfiguration merConfig, JournalingRules rules, ReconcileConfig reconcileConfig, List<GccRuleEntry> gccRules)
		{
			this.organizationId = OrganizationId.ForestWideOrgId;
			this.transportSettingsConfiguration = settings;
			this.merConfiguration = merConfig;
			this.rules = rules;
			this.reconcileConfiguration = reconcileConfig;
			this.gccRules = gccRules;
			this.merRoutingAddress = RoutingAddress.Parse(merConfig.Address.ToString());
			this.journalingReportNdrToRoutingAddress = RoutingAddress.Parse(settings.JournalingReportNdrTo.ToString());
			this.journalingReportNdrToRoutingAddressForGcc = this.journalingReportNdrToRoutingAddress;
		}

		private Configuration(OrganizationId organizationId, PerTenantTransportSettings settings, MicrosoftExchangeRecipientPerTenantSettings merConfig, JournalingRules rules, ReconcileConfig reconcileConfig, List<GccRuleEntry> gccRules)
		{
			this.organizationId = organizationId;
			this.perTenantTransportSettings = settings;
			this.rules = rules;
			this.reconcileConfiguration = reconcileConfig;
			this.gccRules = gccRules;
			this.merRoutingAddress = RoutingAddress.Parse(merConfig.PrimarySmtpAddress.ToString());
			this.journalingReportNdrToRoutingAddress = RoutingAddress.Parse(settings.JournalingReportNdrTo.ToString());
			this.journalingReportNdrToRoutingAddressForGcc = RoutingAddress.NullReversePath;
			TransportConfigContainer transportConfigObject = Configuration.TransportConfigObject;
			if (transportConfigObject != null && transportConfigObject.JournalingReportNdrTo.IsValidAddress && transportConfigObject.JournalingReportNdrTo != SmtpAddress.NullReversePath)
			{
				this.journalingReportNdrToRoutingAddressForGcc = RoutingAddress.Parse(transportConfigObject.JournalingReportNdrTo.ToString());
				return;
			}
			MicrosoftExchangeRecipientConfiguration microsoftExchangeRecipientConfiguration = GlobalConfigurationBase<MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance;
			if (microsoftExchangeRecipientConfiguration != null)
			{
				this.journalingReportNdrToRoutingAddressForGcc = RoutingAddress.Parse(microsoftExchangeRecipientConfiguration.Address.ToString());
			}
		}

		private static object lockObject = new object();

		private static Configuration instance = null;

		private OrganizationId organizationId;

		private TransportConfigContainer transportSettingsConfiguration;

		private PerTenantTransportSettings perTenantTransportSettings;

		private ReconcileConfig reconcileConfiguration;

		private MicrosoftExchangeRecipientConfiguration merConfiguration;

		private JournalingRules rules;

		private List<GccRuleEntry> gccRules;

		private RoutingAddress journalingReportNdrToRoutingAddress;

		private RoutingAddress journalingReportNdrToRoutingAddressForGcc;

		private RoutingAddress merRoutingAddress;
	}
}
