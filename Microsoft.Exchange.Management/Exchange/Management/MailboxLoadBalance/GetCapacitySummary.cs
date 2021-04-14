using System;
using System.Management.Automation;
using Microsoft.Exchange.AnchorService;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.MailboxLoadBalance.Anchor;
using Microsoft.Exchange.MailboxLoadBalance.Directory;
using Microsoft.Exchange.MailboxLoadBalanceClient;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.MailboxLoadBalance
{
	[Cmdlet("Get", "CapacitySummary", DefaultParameterSetName = "ForestSet")]
	public sealed class GetCapacitySummary : DataAccessTask<CapacitySummary>
	{
		[Parameter(Mandatory = true, ParameterSetName = "DatabaseSet")]
		[ValidateNotNull]
		public DatabaseIdParameter Database
		{
			get
			{
				return (DatabaseIdParameter)base.Fields["Database"];
			}
			set
			{
				base.Fields["Database"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "DagSet")]
		[ValidateNotNull]
		public DatabaseAvailabilityGroupIdParameter DatabaseAvailabilityGroup
		{
			get
			{
				return (DatabaseAvailabilityGroupIdParameter)base.Fields["DatabaseAvailabilityGroup"];
			}
			set
			{
				base.Fields["DatabaseAvailabilityGroup"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ForestSet")]
		public SwitchParameter Forest
		{
			get
			{
				return (SwitchParameter)(base.Fields["Forest"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Forest"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "DatabaseSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ServerSet")]
		public SwitchParameter Refresh
		{
			get
			{
				return (SwitchParameter)(base.Fields["Refresh"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Refresh"] = value;
			}
		}

		[ValidateNotNull]
		[Parameter(Mandatory = true, ParameterSetName = "ServerSet")]
		public ServerIdParameter Server
		{
			get
			{
				return (ServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.CreateTopologyConfigurationSession(base.DomainController, true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 193, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\MailboxLoadBalance\\GetCapacitySummary.cs");
		}

		protected override void InternalProcessRecord()
		{
			using (LoadBalanceAnchorContext loadBalanceAnchorContext = new LoadBalanceAnchorContext())
			{
				using (CmdletLogAdapter cmdletLogAdapter = new CmdletLogAdapter(loadBalanceAnchorContext.Logger, new Action<LocalizedString>(base.WriteVerbose), new Action<LocalizedString>(this.WriteWarning), new Action<LocalizedString>(base.WriteDebug)))
				{
					ILoadBalanceServicePort loadBalanceServicePort = LoadBalanceServiceAdapter.Create(cmdletLogAdapter);
					CapacitySummary capacitySummary = loadBalanceServicePort.GetCapacitySummary(this.objectIdentity, this.Refresh);
					base.WriteObject(capacitySummary);
				}
			}
		}

		protected override void InternalValidate()
		{
			if (this.Database != null)
			{
				this.AssignDirectoryIdentity<MailboxDatabase>(this.Database, DirectoryObjectType.Database, Strings.ErrorDatabaseNotFound(this.Database.RawIdentity), Strings.ErrorDatabaseNotUnique(this.Database.RawIdentity));
				return;
			}
			if (this.Server != null)
			{
				this.AssignDirectoryIdentity<Server>(this.Server, DirectoryObjectType.Server, Strings.ErrorServerNotFound(this.Server.RawIdentity), Strings.ErrorServerNotUnique(this.Server.RawIdentity));
				return;
			}
			if (this.DatabaseAvailabilityGroup != null)
			{
				this.AssignDirectoryIdentity<DatabaseAvailabilityGroup>(this.DatabaseAvailabilityGroup, DirectoryObjectType.DatabaseAvailabilityGroup, Strings.ErrorDagNotFound(this.DatabaseAvailabilityGroup.RawIdentity), Strings.ErrorDagNotUnique(this.DatabaseAvailabilityGroup.RawIdentity));
				return;
			}
			this.objectIdentity = DirectoryIdentity.CreateForestIdentity(string.Empty);
		}

		private void AssignDirectoryIdentity<TObject>(IIdentityParameter identityParameter, DirectoryObjectType objectType, LocalizedString notFoundErrorMsg, LocalizedString ambiguousObjErrorMsg) where TObject : ADConfigurationObject, new()
		{
			ADConfigurationObject adconfigurationObject = (ADConfigurationObject)base.GetDataObject<TObject>(identityParameter, this.ConfigurationSession, this.RootId, new LocalizedString?(notFoundErrorMsg), new LocalizedString?(ambiguousObjErrorMsg));
			this.objectIdentity = DirectoryIdentity.CreateFromADObjectId(adconfigurationObject.Id, objectType);
		}

		private const string DagSet = "DagSet";

		private const string DatabaseAvailabilityGroupParameter = "DatabaseAvailabilityGroup";

		private const string DatabaseSet = "DatabaseSet";

		private const string ForestParameter = "Forest";

		private const string ForestSet = "ForestSet";

		private const string ParameterDatabase = "Database";

		private const string RefreshParameter = "Refresh";

		private const string ServerParameter = "Server";

		private const string ServerSet = "ServerSet";

		private const string TaskNoun = "CapacitySummary";

		private DirectoryIdentity objectIdentity;
	}
}
