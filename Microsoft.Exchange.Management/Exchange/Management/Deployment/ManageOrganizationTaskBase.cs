using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Provisioning;

namespace Microsoft.Exchange.Management.Deployment
{
	[ClassAccessLevel(AccessLevel.Consumer)]
	public abstract class ManageOrganizationTaskBase : ComponentInfoBasedTask
	{
		internal IConfigurationSession Session
		{
			get
			{
				return this.configurationSession;
			}
		}

		protected ADObjectId RootOrgContainerId
		{
			get
			{
				return this.rootOrgId;
			}
		}

		protected bool InternalCreateSharedConfiguration
		{
			get
			{
				return this.createSharedConfig;
			}
			set
			{
				this.createSharedConfig = value;
			}
		}

		protected bool InternalIsSharedConfigServicePlan
		{
			get
			{
				return this.isSharedConfigServicePlan;
			}
			set
			{
				this.isSharedConfigServicePlan = value;
			}
		}

		protected OrganizationId InternalSharedConfigurationId
		{
			get
			{
				return this.sharedConfigurationId;
			}
			set
			{
				this.sharedConfigurationId = value;
			}
		}

		protected bool InternalLocalStaticConfigEnabled
		{
			get
			{
				return this.localStaticConfigEnabled;
			}
			set
			{
				this.localStaticConfigEnabled = value;
			}
		}

		protected bool InternalLocalHydrateableConfigEnabled
		{
			get
			{
				return this.localHydrateableConfigEnabled;
			}
			set
			{
				this.localHydrateableConfigEnabled = value;
			}
		}

		protected bool InternalPilotEnabled { get; set; }

		protected override bool IsInnerRunspaceThrottlingEnabled
		{
			get
			{
				return true;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter EnableFileLogging
		{
			get
			{
				return (SwitchParameter)(base.Fields["EnableFileLogging"] ?? false);
			}
			set
			{
				base.Fields["EnableFileLogging"] = value;
			}
		}

		protected override void InternalBeginProcessing()
		{
			base.InternalBeginProcessing();
			if (ProvisioningLayer.Disabled)
			{
				ProvisioningLayer.Disabled = false;
			}
			base.ShouldWriteLogFile = this.EnableFileLogging;
		}

		public ManageOrganizationTaskBase()
		{
			base.ImplementsResume = false;
			base.IsTenantOrganization = true;
			base.IsDatacenter = Datacenter.IsMicrosoftHostedOnly(false);
			base.IsPartnerHosted = Datacenter.IsPartnerHostedOnly(false);
			base.ComponentInfoFileNames = new List<string>();
			base.ShouldLoadDatacenterConfigFile = false;
			this.InitializeComponentInfoFileNames();
		}

		protected override ITaskModuleFactory CreateTaskModuleFactory()
		{
			return new ManageOrganizationTaskModuleFactory();
		}

		protected virtual void InitializeComponentInfoFileNames()
		{
			base.ComponentInfoFileNames.Add("setup\\data\\CommonGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\TransportGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\BridgeheadGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\ClientAccessGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\MailboxGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\UnifiedMessagingGlobalConfig.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\ProvisioningFeatureCatalog.xml");
			base.ComponentInfoFileNames.Add("setup\\data\\PostPrepForestGlobalConfig.xml");
		}

		public new LongPath UpdatesDir
		{
			get
			{
				throw new NotSupportedException();
			}
			set
			{
				throw new NotSupportedException();
			}
		}

		internal virtual IConfigurationSession CreateSession()
		{
			this.rootOrgId = ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest();
			ADSessionSettings sessionSettings = ADSessionSettings.FromCustomScopeSet(base.ScopeSet, this.RootOrgContainerId, base.CurrentOrganizationId, base.ExecutingUserOrganizationId, true);
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.RescopeToSubtree(sessionSettings), 213, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Deployment\\ManageOrganizationTaskBase.cs");
		}

		protected override void InternalStateReset()
		{
			TaskLogger.LogEnter();
			base.InternalStateReset();
			this.configurationSession = this.CreateSession();
			TaskLogger.LogExit();
		}

		protected override bool IsKnownException(Exception e)
		{
			return e is DataSourceOperationException || e is DataSourceTransientException || e is DataValidationException || e is ManagementObjectNotFoundException || e is ManagementObjectAmbiguousException || e is XmlDeserializationException || e is ScriptExecutionException || e is RedirectionEntryManagerException || base.IsKnownException(e);
		}

		protected override void SetRunspaceVariables()
		{
			base.SetRunspaceVariables();
			if (this.InternalSharedConfigurationId != null && !this.InternalSharedConfigurationId.Equals(OrganizationId.ForestWideOrgId))
			{
				base.Fields["SharedConfiguration"] = this.InternalSharedConfigurationId.OrganizationalUnit.Name;
			}
		}

		protected override void PopulateContextVariables()
		{
			base.PopulateContextVariables();
			this.monadConnection.RunspaceProxy.SetVariable(ManageOrganizationTaskBase.ParameterCreateSharedConfig, this.InternalCreateSharedConfiguration);
			this.monadConnection.RunspaceProxy.SetVariable(ManageOrganizationTaskBase.ParameterCommonHydrateableObjectsSharedEnabled, !this.InternalLocalHydrateableConfigEnabled);
			this.monadConnection.RunspaceProxy.SetVariable(ManageOrganizationTaskBase.ParameterAdvancedHydrateableObjectsSharedEnabled, !this.InternalLocalStaticConfigEnabled);
			this.monadConnection.RunspaceProxy.SetVariable(ManageOrganizationTaskBase.ParameterPilotEnabled, this.InternalPilotEnabled);
		}

		protected bool IsMSITTenant(OrganizationId id)
		{
			string strA = id.ConfigurationUnit.ToString();
			return string.Compare(strA, "sdflabs.com", true) == 0 || string.Compare(strA, "msft.ccsctp.net", true) == 0 || string.Compare(strA, "microsoft.onmicrosoft.com", true) == 0;
		}

		protected const string OrganizationHierarchicalPath = "OrganizationHierarchicalPath";

		private OrganizationId sharedConfigurationId;

		private bool createSharedConfig;

		private bool isSharedConfigServicePlan;

		private bool localStaticConfigEnabled = true;

		private bool localHydrateableConfigEnabled = true;

		internal static readonly string ParameterCreateSharedConfig = "CreateSharedConfiguration";

		internal static readonly string ParameterCommonHydrateableObjectsSharedEnabled = "CommonHydrateableObjectsSharedEnabled";

		internal static readonly string ParameterAdvancedHydrateableObjectsSharedEnabled = "AdvancedHydrateableObjectsSharedEnabled";

		internal static readonly string ParameterPilotEnabled = "PilotEnabled";

		protected ADObjectId rootOrgId;

		private IConfigurationSession configurationSession;
	}
}
