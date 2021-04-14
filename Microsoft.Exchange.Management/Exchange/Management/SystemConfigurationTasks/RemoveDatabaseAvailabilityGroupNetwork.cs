using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "DatabaseAvailabilityGroupNetwork", DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemoveDatabaseAvailabilityGroupNetwork : RemoveTenantADTaskBase<DatabaseAvailabilityGroupNetworkIdParameter, DatabaseAvailabilityGroupNetwork>
	{
		protected override bool IsKnownException(Exception e)
		{
			return DagTaskHelper.IsKnownException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				DatabaseAvailabilityGroupNetwork dataObject = base.DataObject;
				DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)dataObject.Identity;
				return Strings.ConfirmationMessageRemoveDatabaseAvailabilityGroupNetwork(dagNetworkObjectId.FullName);
			}
		}

		private IConfigurationSession SetupAdSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, base.SessionSettings, 72, "SetupAdSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\RemoveDatabaseAvailabilityGroupNetwork.cs");
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
			this.m_output = new HaTaskOutputHelper("remove-databaseavailabilitygroupnetwork", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			DatabaseAvailabilityGroupNetwork dataObject = base.DataObject;
			DagNetworkObjectId dagNetworkObjectId = (DagNetworkObjectId)dataObject.Identity;
			ExTraceGlobals.CmdletsTracer.TraceError<string>((long)this.GetHashCode(), "Validating RemoveDAGNetwork({0})", dagNetworkObjectId.FullName);
			IConfigurationSession configSession = this.SetupAdSession();
			DatabaseAvailabilityGroup dag = DagTaskHelper.ReadDagByName(dagNetworkObjectId.DagName, configSession);
			DagTaskHelper.VerifyDagAndServersAreWithinScopes<DatabaseAvailabilityGroupNetwork>(this, dag, true);
			DagTaskHelper.PreventTaskWhenAutoNetConfigIsEnabled(dag, this);
			foreach (DatabaseAvailabilityGroupNetworkSubnet databaseAvailabilityGroupNetworkSubnet in dataObject.Subnets)
			{
				if (databaseAvailabilityGroupNetworkSubnet.State != DatabaseAvailabilityGroupNetworkSubnet.SubnetState.Unknown)
				{
					this.m_output.WriteErrorSimple(new DagNetworkManagementException(ServerStrings.DagNetworkCannotRemoveActiveSubnet(dataObject.Name)));
				}
			}
			TaskLogger.LogExit();
		}

		private HaTaskOutputHelper m_output;
	}
}
