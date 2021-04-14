using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "DatabaseAvailabilityGroupNetwork", SupportsShouldProcess = true)]
	public sealed class NewDatabaseAvailabilityGroupNetwork : NewTenantADTaskBase<DatabaseAvailabilityGroupNetwork>
	{
		[Parameter(Mandatory = true, Position = 0)]
		public string Name
		{
			get
			{
				return this.DataObject.Name;
			}
			set
			{
				this.DataObject.Name = value;
			}
		}

		[Parameter(Mandatory = true, Position = 1)]
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

		[Parameter]
		public DatabaseAvailabilityGroupSubnetId[] Subnets
		{
			get
			{
				return (DatabaseAvailabilityGroupSubnetId[])base.Fields["Subnets"];
			}
			set
			{
				base.Fields["Subnets"] = value;
			}
		}

		[Parameter]
		public string Description
		{
			get
			{
				return this.DataObject.Description;
			}
			set
			{
				this.DataObject.Description = value;
			}
		}

		[Parameter]
		public bool ReplicationEnabled
		{
			get
			{
				return this.DataObject.ReplicationEnabled;
			}
			set
			{
				this.DataObject.ReplicationEnabled = value;
			}
		}

		[Parameter]
		public bool IgnoreNetwork
		{
			get
			{
				return this.DataObject.IgnoreNetwork;
			}
			set
			{
				this.DataObject.IgnoreNetwork = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				DatabaseAvailabilityGroupNetwork dataObject = this.DataObject;
				return Strings.ConfirmationMessageNewDatabaseAvailabilityGroupNetwork(dataObject.Name);
			}
		}

		private IConfigurationSession SetupAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 129, "SetupAdSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\NewDatabaseAvailabilityGroupNetwork.cs");
		}

		protected override IConfigDataProvider CreateSession()
		{
			IConfigurationSession adSession = this.SetupAdSession();
			return new DagNetworkConfigDataProvider(adSession, null, null);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			DatabaseAvailabilityGroupNetwork dataObject = this.DataObject;
			IConfigurationSession configSession = this.SetupAdSession();
			DatabaseAvailabilityGroup databaseAvailabilityGroup = DagTaskHelper.DagIdParameterToDag(this.DatabaseAvailabilityGroup, configSession);
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroupNetwork>(this, databaseAvailabilityGroup, true);
			if (databaseAvailabilityGroup.IsDagEmpty())
			{
				base.WriteError(new DagNetworkEmptyDagException(databaseAvailabilityGroup.Name), ErrorCategory.InvalidArgument, null);
			}
			DagTaskHelper.PreventTaskWhenAutoNetConfigIsEnabled(databaseAvailabilityGroup, this);
			DagNetworkObjectId identity = new DagNetworkObjectId(databaseAvailabilityGroup.Name, dataObject.Name);
			dataObject.SetIdentity(identity);
			DagNetworkConfigDataProvider dagNetworkConfigDataProvider = (DagNetworkConfigDataProvider)base.DataSession;
			DagNetworkConfiguration dagNetworkConfiguration = dagNetworkConfigDataProvider.ReadNetConfig(databaseAvailabilityGroup);
			DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork = dagNetworkConfiguration.FindNetwork(dataObject.Name);
			if (databaseAvailabilityGroupNetwork != null)
			{
				throw new DagNetworkManagementException(ServerStrings.DagNetworkCreateDupName(dataObject.Name));
			}
			DagNetworkValidation.ValidateSwitches(dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (base.Fields["Subnets"] != null)
			{
				DagNetworkValidation.ValidateSubnets(this.Subnets, dagNetworkConfiguration, this.Name, null, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				dataObject.ReplaceSubnets(this.Subnets);
			}
			TaskLogger.LogExit();
		}

		private static class ParameterNames
		{
			public const string Subnets = "Subnets";

			public const string DagId = "DatabaseAvailabilityGroup";
		}
	}
}
