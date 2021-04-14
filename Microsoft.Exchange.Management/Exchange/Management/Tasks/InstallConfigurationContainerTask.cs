using System;
using System.Globalization;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Tasks
{
	[Cmdlet("install", "ConfigurationContainer")]
	public sealed class InstallConfigurationContainerTask : InstallContainerTaskBase<ExchangeConfigurationUnit>
	{
		private new string[] Name
		{
			get
			{
				return base.Name;
			}
			set
			{
				base.Name = value;
			}
		}

		[Parameter(Mandatory = true)]
		public string ParentContainerDN
		{
			get
			{
				return (string)base.Fields[InstallConfigurationContainerTask.ParentContainerField];
			}
			set
			{
				base.Fields[InstallConfigurationContainerTask.ParentContainerField] = value;
			}
		}

		[Parameter(Mandatory = true)]
		public ADOrganizationalUnitIdParameter OrganizationalUnit
		{
			get
			{
				return (ADOrganizationalUnitIdParameter)base.Fields[InstallConfigurationContainerTask.OrganizationalUnitField];
			}
			set
			{
				base.Fields[InstallConfigurationContainerTask.OrganizationalUnitField] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ProgramId
		{
			get
			{
				return (string)base.Fields["TenantProgramId"];
			}
			set
			{
				base.Fields["TenantProgramId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string OfferId
		{
			get
			{
				return (string)base.Fields["TenantOfferId"];
			}
			set
			{
				base.Fields["TenantOfferId"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string ServicePlan
		{
			get
			{
				return (string)base.Fields["TenantServicePlan"];
			}
			set
			{
				base.Fields["TenantServicePlan"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public Guid ExternalDirectoryOrganizationId
		{
			get
			{
				if (string.IsNullOrEmpty(this.DataObject.ExternalDirectoryOrganizationId))
				{
					return Guid.Empty;
				}
				return new Guid(this.DataObject.ExternalDirectoryOrganizationId);
			}
			set
			{
				this.DataObject.ExternalDirectoryOrganizationId = value.ToString();
			}
		}

		[Parameter(Mandatory = false)]
		public bool IsDirSyncRunning
		{
			get
			{
				return this.DataObject.IsDirSyncRunning;
			}
			set
			{
				this.DataObject.IsDirSyncRunning = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> DirSyncStatus
		{
			get
			{
				return this.DataObject.DirSyncStatus;
			}
			set
			{
				this.DataObject.DirSyncStatus = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<string> CompanyTags
		{
			get
			{
				return this.DataObject.CompanyTags;
			}
			set
			{
				this.DataObject.CompanyTags = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string Location
		{
			get
			{
				return this.DataObject.Location;
			}
			set
			{
				this.DataObject.Location = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<Capability> PersistedCapabilities
		{
			get
			{
				return this.DataObject.PersistedCapabilities;
			}
			set
			{
				this.DataObject.PersistedCapabilities = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool CreateSharedConfiguration
		{
			get
			{
				return (bool)(base.Fields[InstallConfigurationContainerTask.CreateSharedConfigurationField] ?? true);
			}
			set
			{
				base.Fields[InstallConfigurationContainerTask.CreateSharedConfigurationField] = value;
			}
		}

		public InstallConfigurationContainerTask()
		{
			this.Name = new string[]
			{
				InstallConfigurationContainerTask.ConfigurationContainerName
			};
		}

		protected override IConfigDataProvider CreateSession()
		{
			base.CreateSession();
			return DirectorySessionFactory.Default.CreateTenantConfigurationSession(base.DomainController, false, ConsistencyMode.PartiallyConsistent, null, ADSessionSettings.FromAllTenantsObjectId(this.GetBaseContainer()), 194, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallTenantConfigurationContainerTask.cs");
		}

		protected override ADObjectId GetBaseContainer()
		{
			return new ADObjectId(this.ParentContainerDN);
		}

		protected override void InternalProcessRecord()
		{
			IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
			string linkResolutionServer = configurationSession.LinkResolutionServer;
			configurationSession.LinkResolutionServer = this.ouOriginatingServer;
			try
			{
				base.InternalProcessRecord();
				ExchangeConfigurationUnit dataObject = this.DataObject;
				dataObject.OrganizationalUnitLink = this.ou.Id;
				configurationSession.Save(dataObject);
				this.ou.ConfigurationUnit = dataObject.ConfigurationUnit;
				bool useConfigNC = configurationSession.UseConfigNC;
				configurationSession.UseConfigNC = false;
				try
				{
					configurationSession.Save(this.ou);
				}
				finally
				{
					configurationSession.UseConfigNC = useConfigNC;
				}
			}
			finally
			{
				configurationSession.LinkResolutionServer = linkResolutionServer;
			}
		}

		protected override IConfigurable PrepareDataObject()
		{
			IConfigurationSession configurationSession = base.DataSession as IConfigurationSession;
			bool useConfigNC = configurationSession.UseConfigNC;
			configurationSession.UseConfigNC = false;
			try
			{
				this.ou = (ADOrganizationalUnit)base.GetDataObject<ADOrganizationalUnit>(this.OrganizationalUnit, configurationSession, null, new LocalizedString?(Strings.ErrorOrganizationalUnitNotFound(this.OrganizationalUnit.ToString())), new LocalizedString?(Strings.ErrorOrganizationalUnitNotUnique(this.OrganizationalUnit.ToString())));
				this.ouOriginatingServer = this.ou.OriginatingServer;
			}
			finally
			{
				configurationSession.UseConfigNC = useConfigNC;
			}
			ExchangeConfigurationUnit exchangeConfigurationUnit = (ExchangeConfigurationUnit)base.PrepareDataObject();
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 290, "PrepareDataObject", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\DirectorySetup\\InstallTenantConfigurationContainerTask.cs");
			string parentLegacyDN = string.Format(CultureInfo.InvariantCulture, "{0}{1}", new object[]
			{
				topologyConfigurationSession.GetAdministrativeGroup().LegacyExchangeDN,
				this.GetRelativeDNTillConfigurationUnits(exchangeConfigurationUnit.Id)
			});
			exchangeConfigurationUnit.Name = exchangeConfigurationUnit.Id.Rdn.UnescapedName;
			exchangeConfigurationUnit.LegacyExchangeDN = LegacyDN.GenerateLegacyDN(parentLegacyDN, exchangeConfigurationUnit);
			exchangeConfigurationUnit.OrganizationStatus = OrganizationStatus.PendingCompletion;
			if (!string.IsNullOrEmpty(this.ServicePlan))
			{
				exchangeConfigurationUnit.ServicePlan = this.ServicePlan;
			}
			if (!string.IsNullOrEmpty(this.ProgramId) && !string.IsNullOrEmpty(this.OfferId))
			{
				exchangeConfigurationUnit.ResellerId = string.Format("{0}.{1}", this.ProgramId, this.OfferId);
				if (this.CreateSharedConfiguration)
				{
					this.DataObject.SharedConfigurationInfo = SharedConfigurationInfo.FromInstalledVersion(this.ProgramId, this.OfferId);
				}
			}
			if (string.IsNullOrEmpty(exchangeConfigurationUnit.ExternalDirectoryOrganizationId))
			{
				exchangeConfigurationUnit.ExternalDirectoryOrganizationId = Guid.NewGuid().ToString();
			}
			return exchangeConfigurationUnit;
		}

		private string GetRelativeDNTillConfigurationUnits(ADObjectId newTenant)
		{
			ADObjectId parent = newTenant.Parent;
			string text = string.Empty;
			while (parent != null && !string.Equals(parent.Name, ADObject.ConfigurationUnits, StringComparison.OrdinalIgnoreCase))
			{
				text = string.Format(CultureInfo.InvariantCulture, "{0}/cn={1}", new object[]
				{
					text,
					parent.Name
				});
				parent = parent.Parent;
			}
			return text;
		}

		private static readonly string ConfigurationContainerName = "Configuration";

		private static readonly string ParentContainerField = "ParentContainer";

		private static readonly string OrganizationalUnitField = "OrganizationalUnit";

		private static readonly string CreateSharedConfigurationField = "CreateSharedConfiguration";

		private string ouOriginatingServer = string.Empty;

		private ADOrganizationalUnit ou;
	}
}
