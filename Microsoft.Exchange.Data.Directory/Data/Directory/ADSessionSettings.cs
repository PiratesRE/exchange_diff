using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.Exchange.Data.Directory.Diagnostics;
using Microsoft.Exchange.Data.Directory.EventLog;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	[Serializable]
	internal class ADSessionSettings
	{
		internal static ADServerSettings ExternalServerSettings
		{
			get
			{
				if (ADSessionSettings.threadContext != null)
				{
					return ADSessionSettings.threadContext.ServerSettings;
				}
				if (ADSessionSettings.processContext != null)
				{
					return ADSessionSettings.processContext.ServerSettings;
				}
				return null;
			}
		}

		private ADSessionSettings(ScopeSet scopeSet, ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ConfigScopes configScopes, PartitionId partitionId)
		{
			if (scopeSet == null)
			{
				throw new ArgumentNullException("scopeSet");
			}
			if (null == currentOrganizationId)
			{
				throw new ArgumentNullException("currentOrganizationId");
			}
			if (executingUserOrganizationId != null && !executingUserOrganizationId.Equals(OrganizationId.ForestWideOrgId) && !executingUserOrganizationId.Equals(currentOrganizationId) && !currentOrganizationId.OrganizationalUnit.IsDescendantOf(executingUserOrganizationId.OrganizationalUnit))
			{
				throw new ArgumentException(DirectoryStrings.ErrorInvalidExecutingOrg(executingUserOrganizationId.OrganizationalUnit.DistinguishedName, currentOrganizationId.OrganizationalUnit.DistinguishedName));
			}
			if (partitionId == null)
			{
				throw new ArgumentNullException("partitionId");
			}
			this.scopeSet = scopeSet;
			this.preferredServers = new SimpleServerSettings();
			this.rootOrgId = rootOrgId;
			this.currentOrganizationId = currentOrganizationId;
			this.executingUserOrganizationId = executingUserOrganizationId;
			this.configScopes = configScopes;
			this.partitionId = partitionId;
			this.tenantConsistencyMode = ((configScopes == ConfigScopes.AllTenants) ? TenantConsistencyMode.IgnoreRetiredTenants : TenantConsistencyMode.ExpectOnlyLiveTenants);
			if (!ADGlobalConfigSettings.SoftLinkEnabled || this.PartitionId == null || this.PartitionId.IsLocalForestPartition() || ADSessionSettings.IsForefrontObject(this.PartitionId))
			{
				this.PartitionSoftLinkMode = SoftLinkMode.Disabled;
				return;
			}
			if (this.PartitionId.ForestFQDN.EndsWith(TopologyProvider.LocalForestFqdn, StringComparison.OrdinalIgnoreCase))
			{
				this.PartitionSoftLinkMode = SoftLinkMode.Disabled;
				return;
			}
			if (this.ConfigScopes == ConfigScopes.Database || this.ConfigScopes == ConfigScopes.Server || this.ConfigScopes == ConfigScopes.RootOrg)
			{
				this.PartitionSoftLinkMode = SoftLinkMode.Disabled;
				return;
			}
			this.PartitionSoftLinkMode = SoftLinkMode.DualMatch;
		}

		internal static bool IsGlsDisabled
		{
			get
			{
				if (GlsMServDirectorySession.GlsLookupMode == GlsLookupMode.MServOnly)
				{
					return true;
				}
				ADServerSettings externalServerSettings = ADSessionSettings.ExternalServerSettings;
				return externalServerSettings != null && externalServerSettings.DisableGls;
			}
		}

		public IBudget AccountingObject
		{
			get
			{
				return this.budget;
			}
			set
			{
				this.budget = value;
			}
		}

		public ADScope RecipientReadScope
		{
			get
			{
				return this.scopeSet.RecipientReadScope;
			}
		}

		public IList<ADScopeCollection> RecipientWriteScopes
		{
			get
			{
				return this.scopeSet.RecipientWriteScopes;
			}
		}

		public ADScopeCollection ExclusiveRecipientScopes
		{
			get
			{
				return this.scopeSet.ExclusiveRecipientScopes;
			}
		}

		public ADScope ConfigReadScope
		{
			get
			{
				return this.scopeSet.ConfigReadScope;
			}
		}

		public ADObjectId RootOrgId
		{
			get
			{
				if (this.rootOrgId == null)
				{
					this.rootOrgId = ADSessionSettings.SessionSettingsFactory.Default.GetRootOrgContainerId(this.partitionId);
				}
				return this.rootOrgId;
			}
		}

		public OrganizationId CurrentOrganizationId
		{
			get
			{
				return this.currentOrganizationId;
			}
		}

		public OrganizationId GetCurrentOrganizationIdPopulated()
		{
			this.currentOrganizationId.EnsureFullyPopulated();
			return this.currentOrganizationId;
		}

		public OrganizationId ExecutingUserOrganizationId
		{
			get
			{
				return this.executingUserOrganizationId ?? this.currentOrganizationId;
			}
		}

		internal string ExecutingUserIdentityName { get; set; }

		public ADObjectId RecipientViewRoot
		{
			get
			{
				return this.ServerSettings.RecipientViewRoot;
			}
		}

		public SoftLinkMode PartitionSoftLinkMode { get; private set; }

		public string GetPreferredDC(ADObjectId domain)
		{
			return this.ServerSettings.GetPreferredDC(domain);
		}

		public bool IsGlobal
		{
			get
			{
				return this.configScopes == ConfigScopes.Global;
			}
		}

		public ConfigScopes ConfigScopes
		{
			get
			{
				return this.configScopes;
			}
		}

		public PartitionId PartitionId
		{
			get
			{
				return this.partitionId;
			}
		}

		internal bool IsSharedConfigChecked { get; set; }

		internal bool IsRedirectedToSharedConfig { get; set; }

		internal bool IsTenantScoped
		{
			get
			{
				return ADSessionSettings.SessionSettingsFactory.IsTenantScopedOrganization(this.CurrentOrganizationId);
			}
		}

		internal TenantConsistencyMode TenantConsistencyMode
		{
			get
			{
				return this.tenantConsistencyMode;
			}
			set
			{
				if (value == TenantConsistencyMode.IgnoreRetiredTenants && this.configScopes == ConfigScopes.TenantLocal)
				{
					throw new InvalidOperationException("TenantConsistencyMode cannot be set to IgnoreRetiredTenants for tenant scoped session.");
				}
				this.tenantConsistencyMode = value;
			}
		}

		internal bool RetiredTenantModificationAllowed
		{
			get
			{
				return this.retiredTenantModificationAllowed;
			}
			set
			{
				this.retiredTenantModificationAllowed = value;
			}
		}

		public bool IncludeCNFObject
		{
			get
			{
				return this.includeCNFObject;
			}
			set
			{
				this.includeCNFObject = value;
			}
		}

		[Conditional("DEBUG")]
		internal static void DebugCheckCaller(params string[] approvedCallerTypes)
		{
			StackTrace stackTrace = new StackTrace();
			foreach (StackFrame stackFrame in stackTrace.GetFrames())
			{
				Type declaringType = stackFrame.GetMethod().DeclaringType;
				if (approvedCallerTypes.Contains(declaringType.Name))
				{
					break;
				}
			}
		}

		public ADServerSettings ServerSettings
		{
			get
			{
				return ADSessionSettings.ExternalServerSettings ?? this.preferredServers;
			}
		}

		public bool IncludeSoftDeletedObjects
		{
			get
			{
				return this.includeSoftDeletedObjects;
			}
			set
			{
				this.includeSoftDeletedObjects = value;
			}
		}

		public bool IncludeSoftDeletedObjectLinks
		{
			get
			{
				return this.includeSoftDeletedObjectLinks;
			}
			set
			{
				this.includeSoftDeletedObjectLinks = value;
			}
		}

		public bool IncludeInactiveMailbox
		{
			get
			{
				return this.includeInactiveMailbox;
			}
			set
			{
				this.includeInactiveMailbox = value;
			}
		}

		public bool SkipCheckVirtualIndex
		{
			get
			{
				return this.skipCheckVirtualIndex;
			}
			set
			{
				this.skipCheckVirtualIndex = value;
			}
		}

		public bool ForceADInTemplateScope { get; set; }

		public ScopeSet ScopeSet
		{
			get
			{
				return this.scopeSet;
			}
		}

		internal static ADSessionSettings FromOrganizationIdWithAddressListScopeServiceOnly(OrganizationId scopingOrganizationId, ADObjectId scopingAddressListId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(delegate
			{
				ADSessionSettings.CheckIfRunningOnCmdlet();
				return ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithAddressListScopeServiceOnly(scopingOrganizationId, scopingAddressListId);
			}, "FromOrganizationIdWithAddressListScopeServiceOnly");
		}

		internal static ADSessionSettings FromOrganizationIdWithAddressListScope(ADObjectId rootOrgId, OrganizationId scopingOrganizationId, ADObjectId scopingAddressListId, OrganizationId executingUserOrganizationId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithAddressListScope(rootOrgId, scopingOrganizationId, scopingAddressListId, executingUserOrganizationId), "FromOrganizationIdWithAddressListScope");
		}

		internal static ADSessionSettings FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId scopingOrganizationId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId, true), "FromOrganizationIdWithoutRbacScopesServiceOnly");
		}

		internal static ADSessionSettings FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId scopingOrganizationId, bool allowRehoming)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithoutRbacScopesServiceOnly(scopingOrganizationId, allowRehoming), "FromOrganizationIdWithoutRbacScopesServiceOnly");
		}

		internal static ADSessionSettings FromOrganizationIdWithoutRbacScopes(ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, bool scopeToExecutingUserOrgId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithoutRbacScopes(rootOrgId, currentOrganizationId, executingUserOrganizationId, scopeToExecutingUserOrgId, true), "FromOrganizationIdWithoutRbacScopes");
		}

		internal static ADSessionSettings FromOrganizationIdWithoutRbacScopes(ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, bool scopeToExecutingUserOrgId, bool allowRehoming)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromOrganizationIdWithoutRbacScopes(rootOrgId, currentOrganizationId, executingUserOrganizationId, scopeToExecutingUserOrgId, allowRehoming), "FromOrganizationIdWithoutRbacScopes");
		}

		internal static ADSessionSettings FromRootOrgBootStrapSession(ADObjectId configNC)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromRootOrgBootStrapSession(configNC), "FromRootOrgBootStrapSession");
		}

		internal static ADSessionSettings FromCustomScopeSet(ScopeSet scopeSet, ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, bool allowRehoming = true)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromCustomScopeSet(scopeSet, rootOrgId, currentOrganizationId, executingUserOrganizationId, allowRehoming), "FromCustomScopeSet");
		}

		internal static ADSessionSettings FromRootOrgScopeSet()
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromRootOrgScopeSet(), "FromRootOrgScopeSet");
		}

		internal static ADSessionSettings FromAccountPartitionRootOrgScopeSet(PartitionId partitionId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAccountPartitionRootOrgScopeSet(partitionId), "FromAccountPartitionRootOrgScopeSet");
		}

		internal static ADSessionSettings FromAccountPartitionWideScopeSet(PartitionId partitionId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAccountPartitionWideScopeSet(partitionId), "FromAccountPartitionWideScopeSet");
		}

		internal static ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(ADObjectId id)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsOrRootOrgAutoDetect(id), "FromAllTenantsOrRootOrgAutoDetect");
		}

		internal static ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(OrganizationId orgId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsOrRootOrgAutoDetect(orgId), "FromAllTenantsOrRootOrgAutoDetect");
		}

		internal static ADSessionSettings RescopeToSubtree(ADSessionSettings sessionSettings)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.RescopeToSubtree(sessionSettings), "RescopeToSubtree");
		}

		internal static ADSessionSettings RescopeToAllTenants(ADSessionSettings sessionSettings)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.RescopeToAllTenants(sessionSettings), "RescopeToAllTenants");
		}

		internal static ADSessionSettings RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(string domain)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(domain), "RootOrgOrSingleTenantFromAcceptedDomainAutoDetect");
		}

		internal static ADSessionSettings FromTenantAcceptedDomain(string domain)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromTenantAcceptedDomain(domain), "FromTenantAcceptedDomain");
		}

		internal static ADSessionSettings FromTenantMSAUser(string msaUserNetID)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromTenantMSAUser(msaUserNetID), "FromTenantMSAUser");
		}

		internal static ADSessionSettings FromConsumerOrganization()
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(delegate
			{
				ExchangeConfigurationUnit localTemplateTenant = TemplateTenantConfiguration.GetLocalTemplateTenant();
				if (localTemplateTenant == null)
				{
					throw new ADTransientException(DirectoryStrings.CannotFindTemplateTenant);
				}
				return ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(localTemplateTenant.OrganizationId);
			}, "FromConsumerOrganization");
		}

		internal static ADSessionSettings FromTenantCUName(string name)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromTenantCUName(name), "FromTenantCUName");
		}

		internal static ADSessionSettings FromTenantPartitionHint(TenantPartitionHint partitionHint)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromTenantPartitionHint(partitionHint), "FromTenantPartitionHint");
		}

		internal static ADSessionSettings FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromExternalDirectoryOrganizationId(externalDirectoryOrganizationId), "FromExternalDirectoryOrganizationId");
		}

		internal static ADSessionSettings FromTenantForestAndCN(string exoAccountForest, string exoTenantContainer)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromTenantForestAndCN(exoAccountForest, exoTenantContainer), "FromTenantForestAndCN");
		}

		internal static ADSessionSettings FromAllTenantsPartitionId(PartitionId partitionId)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsPartitionId(partitionId), "FromAllTenantsPartitionId");
		}

		internal static ADSessionSettings FromAllTenantsObjectId(ADObjectId id)
		{
			return ADSessionSettings.InvokeWithAPILogging<ADSessionSettings>(() => ADSessionSettings.SessionSettingsFactory.Default.FromAllTenantsObjectId(id), "FromAllTenantsObjectId");
		}

		internal static bool IsForefrontObject(PartitionId id)
		{
			return DatacenterRegistry.IsForefrontForOffice() && id.ForestFQDN == "FFO.extest.microsoft.com";
		}

		internal static bool IsForefrontObject(ADObjectId id)
		{
			return DatacenterRegistry.IsForefrontForOffice() && id.Depth >= 3 && string.Equals(id.AncestorDN(id.Depth - 3).Name, "FFO", StringComparison.OrdinalIgnoreCase) && string.Equals(id.AncestorDN(id.Depth - 2).Name, "EXTEST", StringComparison.OrdinalIgnoreCase) && string.Equals(id.AncestorDN(id.Depth - 1).Name, "MICROSOFT", StringComparison.OrdinalIgnoreCase) && string.Equals(id.AncestorDN(id.Depth).Name, "COM", StringComparison.OrdinalIgnoreCase);
		}

		internal static void CheckIfRunningOnService()
		{
			if (!ADSessionSettings.IsRunningOnCmdlet())
			{
				throw new InvalidOperationException("This method should only be called from non-service code");
			}
		}

		internal static ADServerSettings GetProcessServerSettings()
		{
			if (ADSessionSettings.processContext == null)
			{
				return null;
			}
			return ADSessionSettings.processContext.ServerSettings;
		}

		internal static ADDriverContext GetProcessADContext()
		{
			return ADSessionSettings.processContext;
		}

		internal static void SetProcessADContext(ADDriverContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (context.Mode != ContextMode.Setup && context.Mode != ContextMode.Test && context.Mode != ContextMode.TopologyService)
			{
				throw new ArgumentException("Only Setup,Test context and Topology Service modes are supported");
			}
			if (context.ServerSettings == null)
			{
				throw new ArgumentException("context.ServerSettings cannot be null");
			}
			if (context.Mode != ContextMode.TopologyService && TopologyProvider.CurrentTopologyMode != TopologyMode.Ldap)
			{
				throw new InvalidOperationException(DirectoryStrings.ExceptionSetPreferredDCsOnlyForManagement);
			}
			ADSessionSettings.processContext = context;
			ADSessionSettings.LogEventProcessADContextChanged();
		}

		internal static void ClearProcessADContext()
		{
			if (ADSessionSettings.processContext != null)
			{
				ADSessionSettings.processContext = null;
				ADSessionSettings.LogEventProcessADContextChanged();
			}
		}

		internal static ADServerSettings GetThreadServerSettings()
		{
			if (ADSessionSettings.threadContext == null)
			{
				return null;
			}
			return ADSessionSettings.threadContext.ServerSettings;
		}

		internal static ADDriverContext GetThreadADContext()
		{
			return ADSessionSettings.threadContext;
		}

		internal static void SetThreadADContext(ADDriverContext context)
		{
			if (context == null)
			{
				throw new ArgumentNullException("context");
			}
			if (context.Mode != ContextMode.Cmdlet && context.Mode != ContextMode.Test && context.Mode != ContextMode.TopologyService)
			{
				throw new ArgumentException("Only Cmdlet, Test and Topology Service context modes are supported");
			}
			if (context.ServerSettings == null)
			{
				throw new ArgumentException("context.ServerSettings cannot be null");
			}
			ADSessionSettings.threadContext = context;
		}

		internal static void ClearThreadADContext()
		{
			if (ADSessionSettings.threadContext != null)
			{
				ADSessionSettings.threadContext = null;
			}
		}

		internal static void CloneSettableProperties(ADSessionSettings oldSessionSettings, ADSessionSettings newSessionSettings)
		{
			newSessionSettings.IsSharedConfigChecked = oldSessionSettings.IsSharedConfigChecked;
			newSessionSettings.IsRedirectedToSharedConfig = oldSessionSettings.IsRedirectedToSharedConfig;
			newSessionSettings.RetiredTenantModificationAllowed = oldSessionSettings.RetiredTenantModificationAllowed;
			newSessionSettings.IncludeInactiveMailbox = oldSessionSettings.IncludeInactiveMailbox;
			newSessionSettings.IncludeSoftDeletedObjectLinks = oldSessionSettings.IncludeSoftDeletedObjectLinks;
			newSessionSettings.IncludeSoftDeletedObjects = oldSessionSettings.IncludeSoftDeletedObjects;
			if (newSessionSettings.ConfigScopes != ConfigScopes.TenantLocal || oldSessionSettings.TenantConsistencyMode != TenantConsistencyMode.IgnoreRetiredTenants)
			{
				newSessionSettings.TenantConsistencyMode = oldSessionSettings.TenantConsistencyMode;
			}
		}

		internal static ADSessionSettings RescopeToOrganization(ADSessionSettings sessionSettings, OrganizationId orgId, bool rehomeDataSession = true)
		{
			return ADSessionSettings.RescopeToOrganization(sessionSettings, orgId, true, rehomeDataSession);
		}

		internal static ADSessionSettings RescopeToOrganization(ADSessionSettings sessionSettings, OrganizationId orgId, bool checkOrgScope, bool rehomeDataSession = true)
		{
			if (sessionSettings == null)
			{
				throw new ArgumentNullException("sessionSettings");
			}
			if (orgId != null && orgId.OrganizationalUnit == null && sessionSettings.RecipientReadScope.Root == null)
			{
				return sessionSettings;
			}
			if (sessionSettings.CurrentOrganizationId != null && sessionSettings.CurrentOrganizationId.Equals(orgId))
			{
				return sessionSettings;
			}
			ScopeSet scopeSet = ScopeSet.ResolveUnderScope(orgId, sessionSettings.ScopeSet, checkOrgScope);
			ADSessionSettings adsessionSettings = ADSessionSettings.FromCustomScopeSet(scopeSet, sessionSettings.RootOrgId, orgId, checkOrgScope ? sessionSettings.ExecutingUserOrganizationId : OrganizationId.ForestWideOrgId, rehomeDataSession);
			ADSessionSettings.CloneSettableProperties(sessionSettings, adsessionSettings);
			return adsessionSettings;
		}

		internal string GetAccountOrResourceForestFqdn()
		{
			if (this.PartitionId != null && !ADSessionSettings.IsForefrontObject(this.PartitionId))
			{
				return this.PartitionId.ForestFQDN;
			}
			return TopologyProvider.LocalForestFqdn;
		}

		private static void CheckIfRunningOnCmdlet()
		{
			if (ADSessionSettings.IsRunningOnCmdlet())
			{
				throw new InvalidOperationException("This method should never be called from non-service code");
			}
		}

		internal static bool IsRunningOnCmdlet()
		{
			return (ADSessionSettings.processContext != null && ADSessionSettings.processContext.Mode == ContextMode.Setup) || (ADSessionSettings.threadContext != null && ADSessionSettings.threadContext.Mode == ContextMode.Cmdlet);
		}

		private static void LogEventProcessADContextChanged()
		{
			string text = "<null>";
			string text2 = text;
			string text3 = text;
			string text4 = text;
			if (ADSessionSettings.processContext != null)
			{
				ADServerSettings serverSettings = ADSessionSettings.processContext.ServerSettings;
				string text5 = serverSettings.PreferredGlobalCatalog(TopologyProvider.LocalForestFqdn);
				if (!string.IsNullOrEmpty(text5))
				{
					text3 = text5;
				}
				text5 = serverSettings.ConfigurationDomainController(TopologyProvider.LocalForestFqdn);
				if (!string.IsNullOrEmpty(text5))
				{
					text4 = text5;
				}
				if (serverSettings.PreferredDomainControllers.Count > 0)
				{
					StringBuilder stringBuilder = new StringBuilder();
					foreach (Fqdn fqdn in serverSettings.PreferredDomainControllers)
					{
						stringBuilder.AppendLine(fqdn);
					}
					text2 = stringBuilder.ToString();
				}
			}
			Globals.LogEvent(DirectoryEventLogConstants.Tuple_DSC_EVENT_PREFERRED_TOPOLOGY, null, new object[]
			{
				text2,
				text3,
				text4
			});
		}

		private static T InvokeWithAPILogging<T>(Func<T> action, [CallerMemberName] string memberName = null) where T : ADSessionSettings
		{
			Func<string> func = null;
			if (ADSessionSettings.SessionSettingsFactory.Default is ADSessionSettingsFactory)
			{
				DateTime utcNow = DateTime.UtcNow;
				Guid activityId = default(Guid);
				string className = ADSessionSettings.ClassName;
				string caller = "";
				Func<T> action2 = () => action();
				if (func == null)
				{
					func = (() => null);
				}
				return ADScenarioLog.InvokeWithAPILog<T>(utcNow, memberName, activityId, className, caller, action2, func);
			}
			return action();
		}

		private static string ClassName = "ADSessionSettings";

		[ThreadStatic]
		private static ADDriverContext threadContext = null;

		private static ADDriverContext processContext;

		private ScopeSet scopeSet;

		private ADServerSettings preferredServers;

		private ADObjectId rootOrgId;

		private OrganizationId currentOrganizationId;

		private OrganizationId executingUserOrganizationId;

		private ConfigScopes configScopes;

		private bool includeSoftDeletedObjects;

		private bool includeSoftDeletedObjectLinks;

		private bool includeInactiveMailbox;

		private PartitionId partitionId;

		private bool skipCheckVirtualIndex;

		private TenantConsistencyMode tenantConsistencyMode;

		private bool retiredTenantModificationAllowed;

		private bool includeCNFObject = true;

		[NonSerialized]
		private IBudget budget;

		internal abstract class SessionSettingsFactory
		{
			public static ADSessionSettings.SessionSettingsFactory Default
			{
				get
				{
					if (ADSessionSettings.SessionSettingsFactory.defaultInstance == null)
					{
						ADSessionSettings.SessionSettingsFactory.defaultInstance = (ADSessionSettings.SessionSettingsFactory)DirectorySessionFactory.InstantiateFfoOrExoClass("Microsoft.Exchange.Hygiene.Data.Directory.FfoSessionSettingsFactory", typeof(ADSessionSettingsFactory));
					}
					return ADSessionSettings.SessionSettingsFactory.defaultInstance;
				}
			}

			internal static ADSessionSettings.SessionSettingsFactory.PostActionForSettings ThreadPostActionForSettings
			{
				get
				{
					return ADSessionSettings.SessionSettingsFactory.threadPostActionForSettings;
				}
				set
				{
					ExTraceGlobals.SessionSettingsTracer.TraceInformation(0, 0L, string.Format("ThreadPostActionForSettings is set, prevous is {0}, changed to {1}.", (ADSessionSettings.SessionSettingsFactory.threadPostActionForSettings == null) ? "null" : ADSessionSettings.SessionSettingsFactory.threadPostActionForSettings.ToString(), (value == null) ? "null" : value.ToString()));
					ADSessionSettings.SessionSettingsFactory.threadPostActionForSettings = value;
				}
			}

			internal static bool IsTenantScopedOrganization(OrganizationId organizationId)
			{
				if (organizationId == null)
				{
					throw new ArgumentNullException("organizationId");
				}
				return organizationId.OrganizationalUnit != null;
			}

			internal virtual ADSessionSettings FromAccountPartitionRootOrgScopeSet(PartitionId partitionId)
			{
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ADSessionSettings.SessionSettingsFactory.GlobalScopeSet, null, OrganizationId.ForestWideOrgId, null, ConfigScopes.RootOrg, partitionId);
			}

			internal virtual ADSessionSettings FromAccountPartitionWideScopeSet(PartitionId partitionId)
			{
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ADSessionSettings.SessionSettingsFactory.GlobalScopeSet, null, OrganizationId.ForestWideOrgId, null, ConfigScopes.Global, partitionId);
			}

			internal abstract ADSessionSettings FromAllTenantsPartitionId(PartitionId partitionId);

			internal abstract ADSessionSettings FromTenantPartitionHint(TenantPartitionHint partitionHint);

			internal virtual ADObjectId GetRootOrgContainerId(PartitionId partitionId)
			{
				if (partitionId == null)
				{
					throw new ArgumentNullException("partitionId");
				}
				if (ADSession.IsBoundToAdam || partitionId.IsLocalForestPartition())
				{
					return ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
				}
				return ADSystemConfigurationSession.GetRootOrgContainerId(partitionId.ForestFQDN, null, null);
			}

			internal abstract ADSessionSettings FromAllTenantsObjectId(ADObjectId id);

			internal abstract ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(ADObjectId id);

			internal virtual ADSessionSettings FromRootOrgBootStrapSession(ADObjectId configNC)
			{
				if (configNC == null)
				{
					throw new ArgumentNullException("configNC");
				}
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ADSessionSettings.SessionSettingsFactory.GlobalScopeSet, configNC, OrganizationId.ForestWideOrgId, null, ConfigScopes.RootOrg, TopologyProvider.IsAdamTopology() ? PartitionId.LocalForest : configNC.GetPartitionId());
			}

			internal ADSessionSettings FromCustomScopeSet(ScopeSet scopeSet, ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, bool allowRehoming = true)
			{
				if (rootOrgId == null)
				{
					throw new ArgumentNullException("rootOrgId");
				}
				ConfigScopes configScopes = ConfigScopes.TenantLocal;
				if (allowRehoming)
				{
					currentOrganizationId = this.RehomeScopingOrganizationIdIfNeeded(currentOrganizationId);
					executingUserOrganizationId = this.RehomeScopingOrganizationIdIfNeeded(executingUserOrganizationId);
				}
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(scopeSet, rootOrgId, currentOrganizationId, executingUserOrganizationId, configScopes, currentOrganizationId.PartitionId);
			}

			internal abstract ADSessionSettings FromAllTenantsOrRootOrgAutoDetect(OrganizationId orgId);

			internal virtual ADSessionSettings FromOrganizationIdWithAddressListScopeServiceOnly(OrganizationId scopingOrganizationId, ADObjectId scopingAddressListId)
			{
				QueryFilter recipientReadFilter;
				if (scopingAddressListId == null)
				{
					recipientReadFilter = ADScope.NoObjectFilter;
				}
				else
				{
					recipientReadFilter = new AndFilter(new QueryFilter[]
					{
						new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, scopingAddressListId),
						new ExistsFilter(ADRecipientSchema.DisplayName)
					});
				}
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(scopingOrganizationId, recipientReadFilter), null, scopingOrganizationId, null, ConfigScopes.TenantLocal, scopingOrganizationId.PartitionId);
			}

			internal ADSessionSettings FromOrganizationIdWithAddressListScope(ADObjectId rootOrgId, OrganizationId scopingOrganizationId, ADObjectId scopingAddressListId, OrganizationId executingUserOrganizationId)
			{
				ArgumentValidator.ThrowIfNull("scopingAddressListId", scopingAddressListId);
				QueryFilter recipientReadFilter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.AddressListMembership, scopingAddressListId),
					new ExistsFilter(ADRecipientSchema.DisplayName)
				});
				return this.FromCustomScopeSet(ScopeSet.GetOrgWideDefaultScopeSet(scopingOrganizationId, recipientReadFilter), rootOrgId, scopingOrganizationId, executingUserOrganizationId, true);
			}

			internal virtual ADSessionSettings FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId scopingOrganizationId, bool allowRehoming)
			{
				if (allowRehoming)
				{
					scopingOrganizationId = this.RehomeScopingOrganizationIdIfNeeded(scopingOrganizationId);
				}
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ScopeSet.GetOrgWideDefaultScopeSet(scopingOrganizationId, null), null, scopingOrganizationId, null, ConfigScopes.TenantLocal, scopingOrganizationId.PartitionId);
			}

			internal virtual ADSessionSettings FromOrganizationIdWithoutRbacScopes(ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, bool scopeToExecutingUserOrgId, bool allowRehoming)
			{
				if (rootOrgId == null)
				{
					throw new ArgumentNullException("rootOrgId");
				}
				if (null == currentOrganizationId)
				{
					throw new ArgumentNullException("currentOrganizationId");
				}
				if (scopeToExecutingUserOrgId && executingUserOrganizationId == null)
				{
					throw new ArgumentException("scopeToExecutingUserOrgId + null executingUserOrganizationId");
				}
				if (allowRehoming)
				{
					currentOrganizationId = this.RehomeScopingOrganizationIdIfNeeded(currentOrganizationId);
					executingUserOrganizationId = this.RehomeScopingOrganizationIdIfNeeded(executingUserOrganizationId);
				}
				OrganizationId organizationId = currentOrganizationId;
				if (scopeToExecutingUserOrgId)
				{
					organizationId = executingUserOrganizationId;
				}
				ScopeSet orgWideDefaultScopeSet = ScopeSet.GetOrgWideDefaultScopeSet(organizationId);
				ConfigScopes configScopes = ConfigScopes.TenantLocal;
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(orgWideDefaultScopeSet, rootOrgId, currentOrganizationId, executingUserOrganizationId, configScopes, (currentOrganizationId.PartitionId != null) ? currentOrganizationId.PartitionId : (Globals.IsMicrosoftHostedOnly ? rootOrgId.GetPartitionId() : null));
			}

			internal abstract ADSessionSettings FromExternalDirectoryOrganizationId(Guid externalDirectoryOrganizationId);

			internal abstract ADSessionSettings FromTenantForestAndCN(string exoAccountForest, string exoTenantContainer);

			internal virtual ADSessionSettings RootOrgOrSingleTenantFromAcceptedDomainAutoDetect(string domain)
			{
				if (!Globals.IsDatacenter)
				{
					return ADSessionSettings.FromRootOrgScopeSet();
				}
				ADSessionSettings result;
				try
				{
					result = ADSessionSettings.FromTenantAcceptedDomain(domain);
				}
				catch (CannotResolveTenantNameException)
				{
					result = ADSessionSettings.FromRootOrgScopeSet();
				}
				return result;
			}

			internal abstract ADSessionSettings FromTenantAcceptedDomain(string domain);

			internal abstract ADSessionSettings FromTenantMSAUser(string msaUserNetID);

			internal abstract ADSessionSettings FromTenantCUName(string name);

			internal virtual ADSessionSettings FromRootOrgScopeSet()
			{
				return ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(ADSessionSettings.SessionSettingsFactory.GlobalScopeSet, null, OrganizationId.ForestWideOrgId, null, ConfigScopes.RootOrg, PartitionId.LocalForest);
			}

			internal virtual ADSessionSettings RescopeToSubtree(ADSessionSettings sessionSettings)
			{
				if (sessionSettings == null)
				{
					throw new ArgumentNullException("sessionSettings");
				}
				if (sessionSettings.ConfigScopes != ConfigScopes.TenantLocal)
				{
					throw new ArgumentException(DirectoryStrings.ErrorInvalidConfigScope(sessionSettings.ConfigScopes.ToString()));
				}
				ADSessionSettings adsessionSettings = ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(sessionSettings.ScopeSet, sessionSettings.RootOrgId, sessionSettings.CurrentOrganizationId, sessionSettings.ExecutingUserOrganizationId, ConfigScopes.TenantSubTree, sessionSettings.PartitionId);
				adsessionSettings.IncludeSoftDeletedObjects = sessionSettings.IncludeSoftDeletedObjects;
				adsessionSettings.IncludeSoftDeletedObjectLinks = sessionSettings.IncludeSoftDeletedObjectLinks;
				adsessionSettings.IncludeInactiveMailbox = sessionSettings.IncludeInactiveMailbox;
				return adsessionSettings;
			}

			internal virtual ADSessionSettings RescopeToAllTenants(ADSessionSettings sessionSettings)
			{
				if (sessionSettings == null)
				{
					throw new ArgumentNullException("sessionSettings");
				}
				ADSessionSettings adsessionSettings = ADSessionSettings.SessionSettingsFactory.CreateADSessionSettings(sessionSettings.ScopeSet, sessionSettings.RootOrgId, sessionSettings.CurrentOrganizationId, sessionSettings.ExecutingUserOrganizationId, ConfigScopes.AllTenants, sessionSettings.PartitionId);
				adsessionSettings.IncludeSoftDeletedObjects = sessionSettings.IncludeSoftDeletedObjects;
				adsessionSettings.IncludeSoftDeletedObjectLinks = sessionSettings.IncludeSoftDeletedObjectLinks;
				adsessionSettings.IncludeInactiveMailbox = sessionSettings.IncludeInactiveMailbox;
				return adsessionSettings;
			}

			internal abstract bool InDomain();

			protected static ADSessionSettings CreateADSessionSettings(ScopeSet scopeSet, ADObjectId rootOrgId, OrganizationId currentOrganizationId, OrganizationId executingUserOrganizationId, ConfigScopes configScopes, PartitionId partitionId)
			{
				ADSessionSettings adsessionSettings = new ADSessionSettings(scopeSet, rootOrgId, currentOrganizationId, executingUserOrganizationId, configScopes, partitionId);
				if (ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings == null)
				{
					return adsessionSettings;
				}
				return ADSessionSettings.SessionSettingsFactory.ThreadPostActionForSettings(adsessionSettings);
			}

			protected abstract OrganizationId RehomeScopingOrganizationIdIfNeeded(OrganizationId currentOrganizationId);

			[Conditional("DEBUG")]
			private static void CheckCallStackForBootstrapSession()
			{
				string stackTrace = Environment.StackTrace;
				if (!stackTrace.Contains("Microsoft.Exchange.Data.Directory.SystemConfiguration.ADSystemConfigurationSession"))
				{
					stackTrace.Contains("Microsoft.Exchange.Directory.TopologyService");
				}
			}

			[Conditional("DEBUG")]
			private static void CheckProcessNameForTopologyServiceSession()
			{
				using (Process currentProcess = Process.GetCurrentProcess())
				{
					if (!currentProcess.MainModule.ModuleName.Equals("Microsoft.Exchange.Directory.TopologyService.exe", StringComparison.OrdinalIgnoreCase))
					{
						currentProcess.MainModule.ModuleName.Equals("PerseusStudio.exe", StringComparison.OrdinalIgnoreCase);
					}
				}
			}

			protected static readonly ScopeSet GlobalScopeSet = ScopeSet.GetOrgWideDefaultScopeSet(OrganizationId.ForestWideOrgId);

			private static ADSessionSettings.SessionSettingsFactory defaultInstance;

			[ThreadStatic]
			private static ADSessionSettings.SessionSettingsFactory.PostActionForSettings threadPostActionForSettings;

			internal delegate ADSessionSettings PostActionForSettings(ADSessionSettings settings);
		}
	}
}
