using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "StampGroupServer", SupportsShouldProcess = true)]
	public sealed class AddStampGroupServer : SystemConfigurationObjectActionTask<StampGroupIdParameter, StampGroup>, IDisposable
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 1)]
		[ValidateNotNullOrEmpty]
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

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageAddDatabaseAvailabilityGroupServer(this.m_ServerName, this.m_stampGroupName);
			}
		}

		public AddStampGroupServer()
		{
			this.m_serversInStampGroup = new List<Server>(8);
			this.m_output = new HaTaskOutputHelper("add-stampgroupserver", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.m_output.AppendLogMessage("add-stampgroupserver started", new object[0]);
		}

		protected override IConfigDataProvider CreateSession()
		{
			return DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(base.DomainController, false, ConsistencyMode.FullyConsistent, base.SessionSettings, 104, "CreateSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\SystemConfigurationTasks\\Cluster\\AddStampGroupServer.cs");
		}

		private void ResolveParameters()
		{
			this.m_output.WriteProgressSimple(Strings.DagTaskValidatingParameters);
			this.m_stampGroup = StampGroupTaskHelper.StampGroupIdParameterToStampGroup(this.Identity, this.ConfigurationSession);
			this.m_stampGroupName = this.m_stampGroup.Name;
			this.m_Server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			this.m_ServerName = this.m_Server.Name;
			this.m_output.AppendLogMessage("Server: value passed in = {0}, server.Name = {1}", new object[]
			{
				this.Server,
				this.m_ServerName
			});
			if (this.m_Server.MajorVersion != Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.CurrentExchangeMajorVersion)
			{
				this.m_output.WriteErrorSimple(new DagTaskErrorServerWrongVersion(this.m_Server.Name));
			}
		}

		private void CheckServerStampGroupAdSettings()
		{
			if (this.m_stampGroup.Servers.Count >= 512)
			{
				this.m_output.WriteErrorSimple(new DagTaskErrorTooManyServers(this.m_stampGroupName, 512));
			}
			this.StampGroupTrace("Stamp group {0} has {1} servers:", new object[]
			{
				this.m_stampGroupName,
				this.m_stampGroup.Servers.Count
			});
			foreach (ADObjectId identity in this.m_stampGroup.Servers)
			{
				Server server = (Server)base.DataSession.Read<Server>(identity);
				this.StampGroupTrace("Stamp group {0} contains server {1}.", new object[]
				{
					this.m_stampGroupName,
					server.Name
				});
				this.m_serversInStampGroup.Add(server);
			}
		}

		private void LogCommandLineParameters()
		{
			string[] parametersToLog = new string[]
			{
				"Identity",
				"Server",
				"WhatIf"
			};
			DagTaskHelper.LogCommandLineParameters(this.m_output, base.MyInvocation.Line, parametersToLog, base.Fields);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.LogCommandLineParameters();
			this.ResolveParameters();
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_Server, true, new DataAccessTask<StampGroup>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			StampGroupTaskHelper.CheckServerDoesNotBelongToDifferentStampGroup(new Task.TaskErrorLoggingDelegate(this.m_output.WriteError), base.DataSession, this.m_Server, this.m_stampGroupName);
			this.CheckServerStampGroupAdSettings();
			base.InternalValidate();
			this.StampGroupTrace("InternalValidate() done.");
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.m_output.WriteProgressSimple(ReplayStrings.DagTaskAddingServerToDag(this.m_ServerName, this.m_stampGroupName));
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatingAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			base.InternalProcessRecord();
			this.UpdateAdSettings();
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatedAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			this.m_output.WriteProgressSimple(Strings.DagTaskAddedServerToDag(this.m_ServerName, this.m_stampGroupName));
			TaskLogger.LogExit();
		}

		protected override void InternalEndProcessing()
		{
			TaskLogger.LogEnter();
			if (this.m_output != null)
			{
				this.m_output.WriteProgress(Strings.ProgressStatusCompleted, Strings.DagTaskDone, 100);
				this.m_output.CloseTempLogFile();
			}
			TaskLogger.LogExit();
		}

		private void UpdateAdSettings()
		{
			this.m_Server.DatabaseAvailabilityGroup = (ADObjectId)this.m_stampGroup.Identity;
			base.DataSession.Save(this.m_Server);
			ExTraceGlobals.ClusterTracer.TraceDebug<string, string>((long)this.GetHashCode(), "PrepareDataObject() called on stampgroup={0} and server={1}.", this.m_stampGroupName, this.m_ServerName);
		}

		private void StampGroupTrace(string format, params object[] args)
		{
			this.m_output.AppendLogMessage(format, args);
		}

		private void StampGroupTrace(string message)
		{
			this.m_output.AppendLogMessage(message, new object[0]);
		}

		private const int MaxServersInStampGroup = 512;

		private StampGroup m_stampGroup;

		private string m_stampGroupName;

		private HaTaskOutputHelper m_output;

		private Server m_Server;

		private List<Server> m_serversInStampGroup;

		private string m_ServerName;
	}
}
