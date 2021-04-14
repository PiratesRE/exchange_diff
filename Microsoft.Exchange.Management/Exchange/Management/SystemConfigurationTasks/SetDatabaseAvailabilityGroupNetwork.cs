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
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "DatabaseAvailabilityGroupNetwork", DefaultParameterSetName = "Identity", SupportsShouldProcess = true)]
	public sealed class SetDatabaseAvailabilityGroupNetwork : SetTenantADTaskBase<DatabaseAvailabilityGroupNetworkIdParameter, DatabaseAvailabilityGroupNetwork, DatabaseAvailabilityGroupNetwork>
	{
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

		protected override bool IsKnownException(Exception e)
		{
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				DatabaseAvailabilityGroupNetwork dataObject = this.DataObject;
				DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)dataObject.Identity;
				return Strings.ConfirmationMessageSetDatabaseAvailabilityGroupNetwork(dagNetworkObjectId.FullName);
			}
		}

		private IConfigurationSession SetupAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 135, "SetupAdSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\SetDatabaseAvailabilityGroupNetwork.cs");
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
			DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)dataObject.Identity;
			ExTraceGlobals.CmdletsTracer.TraceError<string>((long)this.GetHashCode(), "Validating SetDAGNetwork({0})", dagNetworkObjectId.FullName);
			IConfigurationSession configSession = this.SetupAdSession();
			DatabaseAvailabilityGroup dag = DagTaskHelper.ReadDagByName(dagNetworkObjectId.DagName, configSession);
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroupNetwork>(this, dag, true);
			DagTaskHelper.PreventTaskWhenAutoNetConfigIsEnabled(dag, this);
			DagNetworkConfigDataProvider dagNetworkConfigDataProvider = (DagNetworkConfigDataProvider)base.DataSession;
			DagNetworkConfiguration networkConfig = dagNetworkConfigDataProvider.NetworkConfig;
			DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork = (DatabaseAvailabilityGroupNetwork)dataObject.GetOriginalObject();
			string name = databaseAvailabilityGroupNetwork.Name;
			DatabaseAvailabilityGroupNetwork networkBeingChanged = null;
			foreach (DatabaseAvailabilityGroupNetwork databaseAvailabilityGroupNetwork2 in networkConfig.Networks)
			{
				if (databaseAvailabilityGroupNetwork2 == dataObject)
				{
					networkBeingChanged = databaseAvailabilityGroupNetwork2;
				}
				else if (DatabaseAvailabilityGroupNetwork.NameComparer.Equals(databaseAvailabilityGroupNetwork2.Name, dataObject.Name))
				{
					throw new DagNetworkManagementException(ServerStrings.DagNetworkRenameDupName(name, dataObject.Name));
				}
			}
			DagNetworkValidation.ValidateSwitches(dataObject, new Task.TaskErrorLoggingDelegate(base.WriteError));
			if (base.Fields["Subnets"] != null)
			{
				DagNetworkValidation.ValidateSubnets(this.Subnets, networkConfig, dataObject.Name, networkBeingChanged, new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose));
				dataObject.ReplaceSubnets(this.Subnets);
			}
			DagNetworkValidation.WarnIfAllNetsAreDisabled(networkConfig, new Task.TaskWarningLoggingDelegate(this.WriteWarning));
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			DatabaseAvailabilityGroupNetwork dataObject = this.DataObject;
			ExTraceGlobals.CmdletsTracer.TraceError<string>((long)this.GetHashCode(), "Processing SetDAGNetwork({0})", dataObject.Name);
			base.InternalProcessRecord();
			TaskLogger.LogExit();
		}

		private static class ParameterNames
		{
			public const string Name = "Name";

			public const string Description = "Description";

			public const string Subnets = "Subnets";

			public const string ReplicationEnabled = "ReplicationEnabled";

			public const string IgnoreNetwork = "IgnoreNetwork";
		}
	}
}
