using System;
using System.Xml.Linq;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.Configuration;

namespace Microsoft.Exchange.Transport
{
	internal class ConfigurationComponent : ITransportComponent, ITransportConfiguration, IDiagnosable
	{
		public ConfigurationComponent(ProcessTransportRole processTransportRole) : this()
		{
			this.isFrontendTransportRoleProcess = (processTransportRole == ProcessTransportRole.FrontEnd);
			this.isMailboxTransportSubmissionRoleProcess = (processTransportRole == ProcessTransportRole.MailboxSubmission);
			this.isMailboxTransportDeliveryRoleProcess = (processTransportRole == ProcessTransportRole.MailboxDelivery);
		}

		public ConfigurationComponent()
		{
			this.parallelComponent = new ParallelTransportComponent("Transport Configuration Component");
			this.CreateAndRegister<RemoteDomainTable, RemoteDomainTable.Builder>(null, true, out this.remoteDomainTable);
			this.CreateAndRegister<AcceptedDomainTable, AcceptedDomainTable.Builder>(delegate(AcceptedDomainTable.Builder builder)
			{
				builder.IsBridgehead = this.localServer.Cache.IsBridgehead;
			}, true, out this.firstOrgAcceptedDomainTable);
			this.CreateAndRegister<X400AuthoritativeDomainTable, X400AuthoritativeDomainTable.Builder>(null, true, out this.x400DomainTable);
			this.CreateAndRegister<ReceiveConnectorConfiguration, ReceiveConnectorConfiguration.Builder>(delegate(ReceiveConnectorConfiguration.Builder builder)
			{
				builder.Server = this.localServer.Cache;
			}, true, out this.receiveConnectors);
			this.CreateAndRegister<TransportServerConfiguration, TransportServerConfiguration.Builder>(null, false, out this.localServer);
			this.CreateAndRegister<FrontendTransportServer, TransportServerConfiguration.FrontendBuilder>(null, false, out this.frontendServer);
			this.CreateAndRegister<MailboxTransportServer, TransportServerConfiguration.MailboxBuilder>(null, false, out this.mailboxServer);
			this.CreateAndRegister<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder>(null, true, out this.transportSettings);
		}

		private static PerTenantPerimeterSettings CreateDefaultTenantPerimeterSettings()
		{
			bool routeOutboundViaEhfEnabled = false;
			bool routeOutboundViaFfoFrontendEnabled = false;
			RoutingDomain empty = RoutingDomain.Empty;
			return new PerTenantPerimeterSettings(routeOutboundViaEhfEnabled, routeOutboundViaFfoFrontendEnabled, RoutingDomain.Empty, empty);
		}

		public event ConfigurationUpdateHandler<AcceptedDomainTable> FirstOrgAcceptedDomainTableChanged
		{
			add
			{
				this.firstOrgAcceptedDomainTable.Changed += value;
			}
			remove
			{
				this.firstOrgAcceptedDomainTable.Changed -= value;
			}
		}

		public event ConfigurationUpdateHandler<ReceiveConnectorConfiguration> LocalReceiveConnectorsChanged
		{
			add
			{
				this.receiveConnectors.Changed += value;
			}
			remove
			{
				this.receiveConnectors.Changed -= value;
			}
		}

		public event ConfigurationUpdateHandler<TransportServerConfiguration> LocalServerChanged;

		public event ConfigurationUpdateHandler<TransportSettingsConfiguration> TransportSettingsChanged
		{
			add
			{
				this.transportSettings.Changed += value;
			}
			remove
			{
				this.transportSettings.Changed -= value;
			}
		}

		public event ConfigurationUpdateHandler<RemoteDomainTable> RemoteDomainTableChanged
		{
			add
			{
				this.remoteDomainTable.Changed += value;
			}
			remove
			{
				this.remoteDomainTable.Changed -= value;
			}
		}

		public AcceptedDomainTable FirstOrgAcceptedDomainTable
		{
			get
			{
				return this.firstOrgAcceptedDomainTable.Cache;
			}
		}

		public RemoteDomainTable RemoteDomainTable
		{
			get
			{
				return this.remoteDomainTable.Cache;
			}
		}

		public X400AuthoritativeDomainTable X400AuthoritativeDomainTable
		{
			get
			{
				return this.x400DomainTable.Cache;
			}
		}

		public ReceiveConnectorConfiguration LocalReceiveConnectors
		{
			get
			{
				return this.receiveConnectors.Cache;
			}
		}

		public TransportServerConfiguration LocalServer
		{
			get
			{
				return this.reconciledServer;
			}
		}

		public TransportSettingsConfiguration TransportSettings
		{
			get
			{
				return this.transportSettings.Cache;
			}
		}

		public MicrosoftExchangeRecipientPerTenantSettings MicrosoftExchangeRecipient
		{
			get
			{
				return this.GetMicrosoftExchangeRecipient(OrganizationId.ForestWideOrgId);
			}
		}

		public TransportAppConfig AppConfig
		{
			get
			{
				return Components.TransportAppConfig;
			}
		}

		public ProcessTransportRole ProcessTransportRole
		{
			get
			{
				if (!this.processTransportRoleComputed)
				{
					throw new InvalidOperationException("ProcessTransportRole has not been computed yet.");
				}
				return this.processTransportRole;
			}
		}

		public static bool IsFrontEndTransportProcess(ITransportConfiguration transportConfiguration)
		{
			return transportConfiguration.ProcessTransportRole == ProcessTransportRole.FrontEnd;
		}

		string IDiagnosable.GetDiagnosticComponentName()
		{
			return "TransportConfiguration";
		}

		XElement IDiagnosable.GetDiagnosticInfo(DiagnosableParameters parameters)
		{
			string diagnosticComponentName = ((IDiagnosable)this).GetDiagnosticComponentName();
			XElement xelement = new XElement(diagnosticComponentName);
			bool flag = parameters.Argument.IndexOf("basic", StringComparison.OrdinalIgnoreCase) != -1 || parameters.Argument.IndexOf("verbose", StringComparison.OrdinalIgnoreCase) != -1;
			bool flag2 = parameters.Argument.IndexOf("config", StringComparison.OrdinalIgnoreCase) != -1 || flag;
			bool flag3 = parameters.Argument.IndexOf("AppConfig", StringComparison.OrdinalIgnoreCase) != -1 || flag;
			if (flag2)
			{
				xelement.Add(this.receiveConnectors.GetDiagnosticInfo(parameters));
			}
			if (flag3)
			{
				xelement.Add(this.AppConfig.GetDiagnosticInfo());
			}
			if (!flag2 && !flag3)
			{
				xelement.Add(new XElement("help", "Supported arguments: appconfig, config, basic, verbose, help."));
			}
			return xelement;
		}

		public void Load()
		{
			this.localServer.Load();
			this.ComputeProcessTransportRole();
			if (this.isFrontendTransportRoleProcess)
			{
				this.frontendServer.Load();
			}
			else if (this.isMailboxTransportSubmissionRoleProcess || this.isMailboxTransportDeliveryRoleProcess)
			{
				this.mailboxServer.Load();
			}
			this.ReconcileTransportServerObjects(false);
			this.localServer.Changed += this.LocalServer_Changed;
			if (this.isFrontendTransportRoleProcess)
			{
				this.frontendServer.Changed += this.FrontendServer_Changed;
			}
			else if (this.isMailboxTransportSubmissionRoleProcess || this.isMailboxTransportDeliveryRoleProcess)
			{
				this.mailboxServer.Changed += this.MailboxServer_Changed;
			}
			this.parallelComponent.Load();
			this.InitializeCaches();
		}

		public void Unload()
		{
			this.parallelComponent.Unload();
			if (this.isFrontendTransportRoleProcess)
			{
				this.frontendServer.Changed -= this.FrontendServer_Changed;
				this.frontendServer.Unload();
			}
			else if (this.isMailboxTransportSubmissionRoleProcess || this.isMailboxTransportDeliveryRoleProcess)
			{
				this.mailboxServer.Changed -= this.MailboxServer_Changed;
				this.mailboxServer.Unload();
			}
			this.localServer.Changed -= this.LocalServer_Changed;
			this.localServer.Unload();
			this.ClearCaches();
		}

		public string OnUnhandledException(Exception e)
		{
			return null;
		}

		public void ClearCaches()
		{
			this.transportSettingsCache.Clear();
			this.perimeterSettingsCache.Clear();
			this.remoteDomainsCache.Clear();
			this.microsoftExchangeRecipientCache.Clear();
			this.journalingRulesCache.Clear();
			this.reconciliationAccountsCache.Clear();
			this.acceptedDomainsCache.Clear();
		}

		public void ReloadFirstOrgAcceptedDomainTable()
		{
			this.firstOrgAcceptedDomainTable.Reload(null, EventArgs.Empty);
		}

		public bool TryGetTransportSettings(OrganizationId orgId, out PerTenantTransportSettings perTenantTransportSettings)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				perTenantTransportSettings = this.TransportSettings.PerTenantTransportSettings;
				return true;
			}
			return this.transportSettingsCache.TryGetValue(orgId, out perTenantTransportSettings);
		}

		public PerTenantTransportSettings GetTransportSettings(OrganizationId orgId)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return this.TransportSettings.PerTenantTransportSettings;
			}
			return this.transportSettingsCache.GetValue(orgId);
		}

		public bool TryGetPerimeterSettings(OrganizationId orgId, out PerTenantPerimeterSettings perTenantPerimeterSettings)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				perTenantPerimeterSettings = ConfigurationComponent.CreateDefaultTenantPerimeterSettings();
				return true;
			}
			return this.perimeterSettingsCache.TryGetValue(orgId, out perTenantPerimeterSettings);
		}

		public PerTenantPerimeterSettings GetPerimeterSettings(OrganizationId orgId)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return ConfigurationComponent.CreateDefaultTenantPerimeterSettings();
			}
			return this.perimeterSettingsCache.GetValue(orgId);
		}

		public bool TryGetMicrosoftExchangeRecipient(OrganizationId orgId, out MicrosoftExchangeRecipientPerTenantSettings perTenantMicrosoftExchangeRecipient)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				perTenantMicrosoftExchangeRecipient = new MicrosoftExchangeRecipientPerTenantSettings(GlobalConfigurationBase<Microsoft.Exchange.Data.Directory.SystemConfiguration.MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance.ConfigObject, orgId);
				return true;
			}
			return this.microsoftExchangeRecipientCache.TryGetValue(orgId, out perTenantMicrosoftExchangeRecipient);
		}

		public MicrosoftExchangeRecipientPerTenantSettings GetMicrosoftExchangeRecipient(OrganizationId orgId)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return new MicrosoftExchangeRecipientPerTenantSettings(GlobalConfigurationBase<Microsoft.Exchange.Data.Directory.SystemConfiguration.MicrosoftExchangeRecipient, MicrosoftExchangeRecipientConfiguration>.Instance.ConfigObject, orgId);
			}
			return this.microsoftExchangeRecipientCache.GetValue(orgId);
		}

		public bool TryGetRemoteDomainTable(OrganizationId orgId, out PerTenantRemoteDomainTable perTenantRemoteDomains)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				perTenantRemoteDomains = new PerTenantRemoteDomainTable(this.RemoteDomainTable);
				return true;
			}
			if (!this.remoteDomainsCache.TryGetValue(orgId, out perTenantRemoteDomains))
			{
				return false;
			}
			if (perTenantRemoteDomains == null)
			{
				throw new InvalidOperationException("remoteDomainsCache.GetValue() returned null accepted domain table. Null is not expected by the code base in this case.");
			}
			return true;
		}

		public PerTenantRemoteDomainTable GetRemoteDomainTable(OrganizationId orgId)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return new PerTenantRemoteDomainTable(this.RemoteDomainTable);
			}
			PerTenantRemoteDomainTable value = this.remoteDomainsCache.GetValue(orgId);
			if (value == null)
			{
				throw new InvalidOperationException("remoteDomainsCache.GetValue() returned null accepted domain table. Null is not expected by the code base in this case.");
			}
			return value;
		}

		public bool TryGetAcceptedDomainTable(OrganizationId orgId, out PerTenantAcceptedDomainTable perTenantAcceptedDomains)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				perTenantAcceptedDomains = new PerTenantAcceptedDomainTable(this.FirstOrgAcceptedDomainTable);
				return true;
			}
			if (!this.acceptedDomainsCache.TryGetValue(orgId, out perTenantAcceptedDomains))
			{
				return false;
			}
			if (perTenantAcceptedDomains == null)
			{
				throw new InvalidOperationException("acceptedDomainsCache.TryGetValue() returned true and null accepted domain table. Null is not expected by the code base in this case.");
			}
			return true;
		}

		public bool TryGetTenantOutboundConnectors(OrganizationId orgId, out PerTenantOutboundConnectors perTenantOutboundConnectors)
		{
			if (!this.outboundConnectorsCache.TryGetValue(orgId, out perTenantOutboundConnectors))
			{
				return false;
			}
			if (perTenantOutboundConnectors == null)
			{
				throw new InvalidOperationException("outboundConnectorsCache.TryGetValue() returned true with null tenant outbound connectors. Null is not expected by the code base in this case.");
			}
			return true;
		}

		public PerTenantAcceptedDomainTable GetAcceptedDomainTable(OrganizationId orgId)
		{
			if (orgId == OrganizationId.ForestWideOrgId)
			{
				return new PerTenantAcceptedDomainTable(this.FirstOrgAcceptedDomainTable);
			}
			PerTenantAcceptedDomainTable value = this.acceptedDomainsCache.GetValue(orgId);
			if (value == null)
			{
				throw new InvalidOperationException("acceptedDomainsCache.GetValue() returned null accepted domain table. Null is not expected by the code base in this case.");
			}
			return value;
		}

		public bool TryGetJournalingRules(OrganizationId orgId, out JournalingRulesPerTenantSettings perTenantJournalingRules)
		{
			orgId == OrganizationId.ForestWideOrgId;
			return this.journalingRulesCache.TryGetValue(orgId, out perTenantJournalingRules);
		}

		public JournalingRulesPerTenantSettings GetJournalingRules(OrganizationId orgId)
		{
			orgId == OrganizationId.ForestWideOrgId;
			return this.journalingRulesCache.GetValue(orgId);
		}

		public bool TryGetReconciliationAccounts(OrganizationId orgId, out ReconciliationAccountPerTenantSettings perTenantReconciliationSettings)
		{
			orgId == OrganizationId.ForestWideOrgId;
			return this.reconciliationAccountsCache.TryGetValue(orgId, out perTenantReconciliationSettings);
		}

		public ReconciliationAccountPerTenantSettings GetReconciliationAccounts(OrganizationId orgId)
		{
			orgId == OrganizationId.ForestWideOrgId;
			return this.reconciliationAccountsCache.GetValue(orgId);
		}

		private void LocalServer_Changed(TransportServerConfiguration update)
		{
			this.ReconcileTransportServerObjects(false);
		}

		private void FrontendServer_Changed(FrontendTransportServer update)
		{
			this.ReconcileTransportServerObjects(true);
		}

		private void MailboxServer_Changed(MailboxTransportServer update)
		{
			this.ReconcileTransportServerObjects(true);
		}

		private void ReconcileTransportServerObjects(bool cloneAndUpdate)
		{
			if (this.isFrontendTransportRoleProcess)
			{
				TransportServerConfiguration cache = this.localServer.Cache;
				cache.UpdateFrontEndConfiguration(this.frontendServer.Cache, cloneAndUpdate);
				this.reconciledServer = cache;
			}
			else if (this.isMailboxTransportDeliveryRoleProcess || this.isMailboxTransportSubmissionRoleProcess)
			{
				string pathSuffix = this.isMailboxTransportDeliveryRoleProcess ? "Delivery" : "Submission";
				TransportServerConfiguration cache2 = this.localServer.Cache;
				cache2.UpdateMailboxConfiguration(this.mailboxServer.Cache, pathSuffix, cloneAndUpdate);
				this.reconciledServer = cache2;
			}
			else
			{
				this.reconciledServer = this.localServer.Cache;
			}
			ConfigurationUpdateHandler<TransportServerConfiguration> localServerChanged = this.LocalServerChanged;
			if (localServerChanged != null)
			{
				localServerChanged(this.reconciledServer);
			}
		}

		private void CreateAndRegister<TCache, TBuilder>(ConfigurationLoader<TCache, TBuilder>.ExternalConfigurationSetter extraConfiguration, bool addToParallelComponent, out ConfigurationLoader<TCache, TBuilder> cache) where TCache : class where TBuilder : ConfigurationLoader<TCache, TBuilder>.Builder, new()
		{
			this.CreateAndRegister<TCache, TBuilder>(extraConfiguration, this.AppConfig.ADPolling.ConfigurationComponentReloadInterval, addToParallelComponent, out cache);
		}

		private void CreateAndRegister<TCache, TBuilder>(ConfigurationLoader<TCache, TBuilder>.ExternalConfigurationSetter extraConfiguration, TimeSpan reloadInterval, bool addToParallelComponent, out ConfigurationLoader<TCache, TBuilder> cache) where TCache : class where TBuilder : ConfigurationLoader<TCache, TBuilder>.Builder, new()
		{
			cache = new ConfigurationLoader<TCache, TBuilder>(extraConfiguration, reloadInterval);
			if (addToParallelComponent)
			{
				this.parallelComponent.TransportComponents.Add(cache);
			}
			Components.ConfigChanged += cache.Reload;
		}

		private void InitializeCaches()
		{
			this.transportSettingsCache = new TenantConfigurationCache<PerTenantTransportSettings>((long)this.AppConfig.PerTenantCache.TransportSettingsCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.TransportSettingsCacheExpiryInterval, this.AppConfig.PerTenantCache.TransportSettingsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.TransportSettingsCacheTracer, "TransportSettings"), new PerTenantCachePerformanceCounters(this.processTransportRole, "TransportSettings"));
			this.perimeterSettingsCache = new TenantConfigurationCache<PerTenantPerimeterSettings>((long)this.AppConfig.PerTenantCache.PerimeterSettingsCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.PerimeterSettingsCacheExpiryInterval, this.AppConfig.PerTenantCache.PerimeterSettingsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.PerimeterSettingsCacheTracer, "PerimeterSettings"), new PerTenantCachePerformanceCounters(this.processTransportRole, "PerimeterSettings"));
			this.microsoftExchangeRecipientCache = new TenantConfigurationCache<MicrosoftExchangeRecipientPerTenantSettings>((long)this.AppConfig.PerTenantCache.MicrosoftExchangeRecipientCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.MicrosoftExchangeRecipientCacheExpiryInterval, this.AppConfig.PerTenantCache.MicrosoftExchangeRecipientCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.MicrosoftExchangeRecipientCacheTracer, "MicrosoftExchangeRecipient"), new PerTenantCachePerformanceCounters(this.processTransportRole, "MicrosoftExchangeRecipient"));
			this.remoteDomainsCache = new TenantConfigurationCache<PerTenantRemoteDomainTable>((long)this.AppConfig.PerTenantCache.RemoteDomainsCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.RemoteDomainsCacheExpiryInterval, this.AppConfig.PerTenantCache.RemoteDomainsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.RemoteDomainsCacheTracer, "RemoteDomains"), new PerTenantCachePerformanceCounters(this.processTransportRole, "RemoteDomains"));
			this.acceptedDomainsCache = new TenantConfigurationCache<PerTenantAcceptedDomainTable>((long)this.AppConfig.PerTenantCache.AcceptedDomainsCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.AcceptedDomainsCacheExpiryInterval, this.AppConfig.PerTenantCache.AcceptedDomainsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.AcceptedDomainsCacheTracer, "AcceptedDomains"), new PerTenantCachePerformanceCounters(this.processTransportRole, "AcceptedDomains"));
			this.journalingRulesCache = new TenantConfigurationCache<JournalingRulesPerTenantSettings>((long)this.AppConfig.PerTenantCache.JournalingRulesCacheMaxSize.ToBytes(), this.AppConfig.PerTenantCache.JournalingCacheExpiryInterval, this.AppConfig.PerTenantCache.JournalingCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.JournalingRulesCacheTracer, "JournalingRules"), new PerTenantCachePerformanceCounters(this.processTransportRole, "JournalingRules"));
			this.reconciliationAccountsCache = new TenantConfigurationCache<ReconciliationAccountPerTenantSettings>((long)this.AppConfig.PerTenantCache.ReconciliationCacheConfigMaxSize.ToBytes(), this.AppConfig.PerTenantCache.JournalingCacheExpiryInterval, this.AppConfig.PerTenantCache.JournalingCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.JournalingRulesCacheTracer, "JournalingRules"), new PerTenantCachePerformanceCounters(this.processTransportRole, "ReconciliationAccounts"));
			this.outboundConnectorsCache = new TenantConfigurationCache<PerTenantOutboundConnectors>((long)this.AppConfig.PerTenantCache.OutboundConnectorsCacheSize.ToBytes(), this.AppConfig.PerTenantCache.OutboundConnectorsCacheExpirationInterval, this.AppConfig.PerTenantCache.OutboundConnectorsCacheCleanupInterval, new PerTenantCacheTracer(ExTraceGlobals.OutboundConnectorsCacheTracer, "OutboundConnectorsCache"), new PerTenantCachePerformanceCounters(this.processTransportRole, "OutboundConnectorsCache"));
		}

		private void ComputeProcessTransportRole()
		{
			if (!this.localServer.IsInitialized)
			{
				throw new InvalidOperationException("ProcessTransportRole can be computed only after initializing localserver.");
			}
			if (this.isFrontendTransportRoleProcess)
			{
				this.processTransportRole = ProcessTransportRole.FrontEnd;
			}
			else if (this.isMailboxTransportSubmissionRoleProcess)
			{
				this.processTransportRole = ProcessTransportRole.MailboxSubmission;
			}
			else if (this.isMailboxTransportDeliveryRoleProcess)
			{
				this.processTransportRole = ProcessTransportRole.MailboxDelivery;
			}
			else if (this.localServer.Cache.TransportServer.IsHubTransportServer)
			{
				this.processTransportRole = ProcessTransportRole.Hub;
			}
			else
			{
				if (!this.localServer.Cache.TransportServer.IsEdgeServer)
				{
					throw new TransportComponentLoadFailedException("Unexpected server role: " + this.localServer.Cache.TransportServer.CurrentServerRole);
				}
				this.processTransportRole = ProcessTransportRole.Edge;
			}
			this.processTransportRoleComputed = true;
		}

		private readonly bool isFrontendTransportRoleProcess;

		private readonly bool isMailboxTransportSubmissionRoleProcess;

		private readonly bool isMailboxTransportDeliveryRoleProcess;

		private ProcessTransportRole processTransportRole;

		private bool processTransportRoleComputed;

		private ParallelTransportComponent parallelComponent;

		private ConfigurationLoader<AcceptedDomainTable, AcceptedDomainTable.Builder> firstOrgAcceptedDomainTable;

		private ConfigurationLoader<RemoteDomainTable, RemoteDomainTable.Builder> remoteDomainTable;

		private ConfigurationLoader<X400AuthoritativeDomainTable, X400AuthoritativeDomainTable.Builder> x400DomainTable;

		private ConfigurationLoader<ReceiveConnectorConfiguration, ReceiveConnectorConfiguration.Builder> receiveConnectors;

		private ConfigurationLoader<TransportServerConfiguration, TransportServerConfiguration.Builder> localServer;

		private ConfigurationLoader<FrontendTransportServer, TransportServerConfiguration.FrontendBuilder> frontendServer;

		private ConfigurationLoader<MailboxTransportServer, TransportServerConfiguration.MailboxBuilder> mailboxServer;

		private ConfigurationLoader<TransportSettingsConfiguration, TransportSettingsConfiguration.Builder> transportSettings;

		private TenantConfigurationCache<PerTenantTransportSettings> transportSettingsCache;

		private TenantConfigurationCache<PerTenantPerimeterSettings> perimeterSettingsCache;

		private TenantConfigurationCache<MicrosoftExchangeRecipientPerTenantSettings> microsoftExchangeRecipientCache;

		private TenantConfigurationCache<PerTenantRemoteDomainTable> remoteDomainsCache;

		private TenantConfigurationCache<PerTenantAcceptedDomainTable> acceptedDomainsCache;

		private TenantConfigurationCache<JournalingRulesPerTenantSettings> journalingRulesCache;

		private TenantConfigurationCache<ReconciliationAccountPerTenantSettings> reconciliationAccountsCache;

		private TenantConfigurationCache<PerTenantOutboundConnectors> outboundConnectorsCache;

		private TransportServerConfiguration reconciledServer;

		private sealed class PerTenantCacheConstants
		{
			internal const string TransportSettingsString = "TransportSettings";

			internal const string PerimeterSettingsString = "PerimeterSettings";

			internal const string JournalingRulesString = "JournalingRules";

			internal const string ReconciliationAccountsString = "ReconciliationAccounts";

			internal const string MicrosoftExchangeRecipientString = "MicrosoftExchangeRecipient";

			internal const string RemoteDomainsString = "RemoteDomains";

			internal const string AcceptedDomainsString = "AcceptedDomains";

			internal const string OrganizationSettingsString = "OrganizationSettings";

			internal const string OutboundConnectorsCacheString = "OutboundConnectorsCache";
		}
	}
}
