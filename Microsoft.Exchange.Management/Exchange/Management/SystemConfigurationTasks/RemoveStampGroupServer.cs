using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "StampGroupServer", ConfirmImpact = ConfirmImpact.High, SupportsShouldProcess = true)]
	public sealed class RemoveStampGroupServer : SystemConfigurationObjectActionTask<StampGroupIdParameter, StampGroup>, IDisposable
	{
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipelineByPropertyName = true, ValueFromPipeline = true, Position = 1)]
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

		[Parameter(Mandatory = false)]
		public SwitchParameter ConfigurationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ConfigurationOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ConfigurationOnly"] = value;
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return AmExceptionHelper.IsKnownClusterException(this, e) || base.IsKnownException(e);
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveDatabaseAvailabilityGroupServer(this.m_ServerName, this.m_stampGroupName);
			}
		}

		public RemoveStampGroupServer()
		{
			this.m_output = new HaTaskOutputHelper("remove-stampgroupserver", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			this.m_output.CreateTempLogFile();
			this.m_output.AppendLogMessage("remove-stampgroupserver started", new object[0]);
		}

		private void ResolveParameters()
		{
			this.m_output.WriteProgressSimple(Strings.DagTaskValidatingParameters);
			this.m_stampGroup = StampGroupTaskHelper.StampGroupIdParameterToStampGroup(this.Identity, this.ConfigurationSession);
			this.m_stampGroupName = this.m_stampGroup.Name;
			this.m_Server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.Server.ToString())));
			this.m_ServerName = this.m_Server.Name;
			this.m_configurationOnly = this.ConfigurationOnly;
			DagTaskHelper.LogMachineIpAddresses(this.m_output, this.m_stampGroupName);
		}

		private void LogCommandLineParameters()
		{
			string[] parametersToLog = new string[]
			{
				"Identity",
				"Server",
				"ConfigurationOnly",
				"WhatIf"
			};
			DagTaskHelper.LogCommandLineParameters(this.m_output, base.MyInvocation.Line, parametersToLog, base.Fields);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			if (this.m_serversInDag != null)
			{
				this.m_serversInDag.Clear();
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.LogCommandLineParameters();
			this.ResolveParameters();
			base.InternalValidate();
			TaskLogger.LogExit();
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			this.m_output.WriteProgressSimple(Strings.DagTaskRemovingServerFromDag(this.m_ServerName, this.m_stampGroupName));
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatingAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			base.InternalProcessRecord();
			this.UpdateAdSettings();
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatedAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			this.m_output.WriteProgressSimple(Strings.DagTaskRemovedServerFromDag(this.m_ServerName, this.m_stampGroupName));
			if (!this.m_configurationOnly)
			{
				this.m_output.WriteProgressSimple(Strings.DagTaskSleepAfterNodeRemoval(60, this.m_stampGroupName, this.m_ServerName));
				Thread.Sleep(60000);
			}
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
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatingAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			this.m_Server.DatabaseAvailabilityGroup = null;
			base.DataSession.Save(this.m_Server);
			this.m_output.WriteProgressSimple(Strings.DagTaskUpdatedAdDagMembership(this.m_ServerName, this.m_stampGroupName));
			DagTaskHelper.RevertDagServersDatabasesToStandalone(this.ConfigurationSession, this.m_output, this.m_Server);
		}

		private void DagTrace(string format, params object[] args)
		{
			this.m_output.AppendLogMessage(format, args);
		}

		private StampGroup m_stampGroup;

		private string m_stampGroupName;

		private bool m_configurationOnly;

		private Server m_Server;

		private string m_ServerName;

		private List<Server> m_serversInDag = new List<Server>(8);

		private HaTaskOutputHelper m_output;
	}
}
