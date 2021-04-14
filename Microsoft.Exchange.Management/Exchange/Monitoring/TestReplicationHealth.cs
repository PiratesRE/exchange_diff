using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management.Automation;
using System.Security;
using System.Text;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	[Cmdlet("Test", "ReplicationHealth", SupportsShouldProcess = true)]
	[ClassAccessLevel(AccessLevel.Consumer)]
	public sealed class TestReplicationHealth : SystemConfigurationObjectActionTask<ServerIdParameter, Server>
	{
		[ValidateNotNullOrEmpty]
		[Alias(new string[]
		{
			"Server"
		})]
		[Parameter(Mandatory = false, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override ServerIdParameter Identity
		{
			get
			{
				return base.Identity;
			}
			set
			{
				base.Identity = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter OutputObjects
		{
			get
			{
				return (SwitchParameter)(base.Fields["OutputObjects"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["OutputObjects"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool MonitoringContext
		{
			get
			{
				return (bool)(base.Fields["MonitoringContext"] ?? false);
			}
			set
			{
				base.Fields["MonitoringContext"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public uint TransientEventSuppressionWindow
		{
			get
			{
				object obj;
				if ((obj = base.Fields["TransientEventSuppressionWindow"]) == null)
				{
					obj = (this.MonitoringContext ? 3U : 0U);
				}
				return (uint)obj;
			}
			set
			{
				base.Fields["TransientEventSuppressionWindow"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		[ValidateRange(1, 2147483647)]
		public int ActiveDirectoryTimeout
		{
			get
			{
				return (int)(base.Fields["ActiveDirectoryTimeout"] ?? 15);
			}
			set
			{
				base.Fields["ActiveDirectoryTimeout"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageTestReplicationHealth(this.m_serverName);
			}
		}

		protected override bool IsKnownException(Exception exception)
		{
			return base.IsKnownException(exception) || DataAccessHelper.IsDataAccessKnownException(exception) || MonitoringHelper.IsKnownExceptionForMonitoring(exception) || AmExceptionHelper.IsKnownClusterException(this, exception) || exception is ReplicationCheckException;
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.ResetPrivateState();
			ReplicationCheckGlobals.ResetState();
		}

		protected override void InternalBeginProcessing()
		{
			this.m_monitoringData = new MonitoringData();
			this.m_eventManager = new ReplicationEventManager();
			this.m_replayConfigs = new List<ReplayConfiguration>();
			base.InternalBeginProcessing();
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			try
			{
				this.ConfigurationSession.ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)this.ActiveDirectoryTimeout));
				((IDirectorySession)base.DataSession).ServerTimeout = new TimeSpan?(TimeSpan.FromSeconds((double)this.ActiveDirectoryTimeout));
				if (this.Identity == null)
				{
					this.m_serverName = Environment.MachineName;
					this.Identity = ServerIdParameter.Parse(this.m_serverName);
				}
				base.InternalValidate();
				if (base.HasErrors)
				{
					TaskLogger.LogExit();
				}
				else
				{
					ADServerWrapper server = ADObjectWrapperFactory.CreateWrapper(this.DataObject);
					ReplicationCheckGlobals.Server = server;
					this.m_serverName = this.DataObject.Name;
					ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "serverName is '{0}'.", this.m_serverName ?? "null");
					this.m_isLocal = SharedHelper.StringIEquals(this.m_serverName, Environment.MachineName);
					if (!this.m_isLocal && this.MonitoringContext)
					{
						this.WriteErrorAndMonitoringEvent(new CannotRunMonitoringTaskRemotelyException(this.m_serverName), ErrorCategory.InvalidOperation, this.Identity, 10011, "MSExchange Monitoring ReplicationHealth");
					}
					ReplicationCheckGlobals.RunningInMonitoringContext = this.MonitoringContext;
					if (this.m_isLocal && !this.CheckLocalServerRegistryRoles())
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "Local server does not have Exchange 2009 Mailbox Role in the registry.");
					}
					else
					{
						this.CheckServerObject();
						if (this.DataObject.DatabaseAvailabilityGroup != null)
						{
							this.m_serverConfigBitfield |= ServerConfig.DagMember;
							this.m_dag = this.ConfigurationSession.Read<DatabaseAvailabilityGroup>(this.DataObject.DatabaseAvailabilityGroup);
							if (this.m_dag.StoppedMailboxServers.Contains(new AmServerName(this.m_serverName).Fqdn))
							{
								this.m_serverConfigBitfield |= ServerConfig.Stopped;
							}
						}
						else
						{
							ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "{0} is a Standalone non-DAG Mailbox server.", this.DataObject.Name);
						}
						try
						{
							this.BuildReplayConfigurations(this.m_dag, server);
						}
						catch (ClusterException exception)
						{
							this.WriteErrorAndMonitoringEvent(exception, ErrorCategory.InvalidOperation, null, 10003, "MSExchange Monitoring ReplicationHealth");
							return;
						}
						catch (TransientException exception2)
						{
							this.WriteErrorAndMonitoringEvent(exception2, ErrorCategory.InvalidOperation, null, 10003, "MSExchange Monitoring ReplicationHealth");
							return;
						}
						catch (DataSourceOperationException exception3)
						{
							this.WriteErrorAndMonitoringEvent(exception3, ErrorCategory.InvalidOperation, null, 10003, "MSExchange Monitoring ReplicationHealth");
							return;
						}
						catch (DataValidationException exception4)
						{
							this.WriteErrorAndMonitoringEvent(exception4, ErrorCategory.InvalidData, null, 10003, "MSExchange Monitoring ReplicationHealth");
							return;
						}
						ReplicationCheckGlobals.ServerConfiguration = this.m_serverConfigBitfield;
						if (this.DataObject != null)
						{
							this.m_useReplayRpc = ReplayRpcVersionControl.IsGetCopyStatusEx2RpcSupported(this.DataObject.AdminDisplayVersion);
						}
						this.CheckIfTaskCanRun();
					}
				}
			}
			finally
			{
				if (base.HasErrors)
				{
					if (this.MonitoringContext)
					{
						this.WriteMonitoringData();
					}
					ReplicationCheckGlobals.ResetState();
				}
				TaskLogger.LogExit();
			}
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			ReplicationCheckGlobals.WriteVerboseDelegate = new Task.TaskVerboseLoggingDelegate(base.WriteVerbose);
			try
			{
				base.WriteVerbose(Strings.StartingToRunChecks(this.m_serverName));
				this.RunChecks();
				if (this.MonitoringContext && !this.m_eventManager.HasMomEvents())
				{
					base.WriteVerbose(Strings.NoMonitoringErrorsInTestReplicationHealth(this.m_serverName));
					this.m_eventManager.LogEvent(10000, Strings.NoMonitoringErrorsInTestReplicationHealth(this.m_serverName));
				}
			}
			finally
			{
				if (this.MonitoringContext)
				{
					this.WriteMonitoringData();
				}
				ReplicationCheckGlobals.ResetState();
				TaskLogger.LogExit();
			}
		}

		private static bool AreConfigBitsSet(ServerConfig configuration, ServerConfig comparisonBits)
		{
			if (comparisonBits == ServerConfig.Unknown)
			{
				throw new ArgumentException("comparisonBits cannot be Unknown.", "comparisonBits");
			}
			return (configuration & comparisonBits) == comparisonBits;
		}

		internal static LocalizedString GetMachineConfigurationString(ServerConfig machineConfig)
		{
			StringBuilder stringBuilder = new StringBuilder(4);
			string serverName = string.Empty;
			if (ReplicationCheckGlobals.Server != null)
			{
				serverName = ReplicationCheckGlobals.Server.Name;
			}
			if (TestReplicationHealth.AreConfigBitsSet(machineConfig, ServerConfig.DagMemberNoDatabases))
			{
				stringBuilder.AppendFormat(TestReplicationHealth.SpaceConcatFormatString, Strings.DagMemberNoDatabasesString(serverName));
			}
			else if (TestReplicationHealth.AreConfigBitsSet(machineConfig, ServerConfig.DagMember))
			{
				stringBuilder.AppendFormat(TestReplicationHealth.SpaceConcatFormatString, Strings.DagMemberString(serverName));
			}
			else
			{
				stringBuilder.AppendFormat(TestReplicationHealth.SpaceConcatFormatString, Strings.StandaloneMailboxString(serverName));
			}
			if (TestReplicationHealth.AreConfigBitsSet(machineConfig, ServerConfig.RcrSource) || TestReplicationHealth.AreConfigBitsSet(machineConfig, ServerConfig.RcrTarget))
			{
				stringBuilder.AppendFormat(TestReplicationHealth.SpaceConcatFormatString, Strings.RcrConfigString(serverName));
			}
			LocalizedString result = new LocalizedString(stringBuilder.ToString());
			return result;
		}

		private bool AreConfigBitsSet(ServerConfig configBits)
		{
			return TestReplicationHealth.AreConfigBitsSet(this.m_serverConfigBitfield, configBits);
		}

		private void BuildReplayConfigurations(DatabaseAvailabilityGroup dag, IADServer server)
		{
			IADDatabaseAvailabilityGroup dag2 = null;
			if (dag != null)
			{
				dag2 = ADObjectWrapperFactory.CreateWrapper(dag);
			}
			List<ReplayConfiguration> list;
			List<ReplayConfiguration> list2;
			ReplayConfigurationHelper.TaskConstructAllDatabaseConfigurations(dag2, server, out list, out list2);
			if (list != null && list.Count > 0)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "BuildReplayConfigurations(): Found RCR Source Configurations.");
				this.m_serverConfigBitfield |= ServerConfig.RcrSource;
				this.m_replayConfigs.AddRange(list);
			}
			if (list2 != null && list2.Count > 0)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "BuildReplayConfigurations(): Found RCR Target Configurations.");
				this.m_serverConfigBitfield |= ServerConfig.RcrTarget;
				this.m_replayConfigs.AddRange(list2);
			}
			if (this.AreConfigBitsSet(ServerConfig.DagMember) && !this.AreConfigBitsSet(ServerConfig.RcrSource) && !this.AreConfigBitsSet(ServerConfig.RcrTarget))
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug((long)this.GetHashCode(), "BuildReplayConfigurations(): Server is a DAG member but has no Database copies.");
				this.m_serverConfigBitfield |= ServerConfig.DagMemberNoDatabases;
			}
			ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "BuildReplayConfigurations(): The following bits are set on localConfigBitfield: {0}", this.m_serverConfigBitfield.ToString());
		}

		private void CheckIfTaskCanRun()
		{
			base.WriteVerbose(TestReplicationHealth.GetMachineConfigurationString(this.m_serverConfigBitfield));
			if (this.AreConfigBitsSet(ServerConfig.Stopped))
			{
				this.WriteWarning(Strings.DagMemberStopped(this.m_serverName));
			}
			if (this.AreConfigBitsSet(ServerConfig.DagMemberNoDatabases))
			{
				this.WriteWarning(Strings.DagMemberNoDatabases(this.m_serverName));
			}
			if (this.IsServerStandaloneWithNoReplicas())
			{
				this.WriteWarning(Strings.StandaloneMailboxNoReplication(this.m_serverName));
			}
		}

		private bool IsServerStandaloneWithNoReplicas()
		{
			return this.m_dag == null && this.m_serverConfigBitfield == ServerConfig.Unknown;
		}

		private void RunChecks()
		{
			TaskLogger.LogEnter();
			try
			{
				DatabaseHealthValidationRunner validationRunner = new DatabaseHealthValidationRunner(this.m_serverName);
				if (this.AreConfigBitsSet(ServerConfig.DagMember))
				{
					using (DagMemberMultiChecks dagMemberMultiChecks = new DagMemberMultiChecks(this.m_serverName, this.m_eventManager, "MSExchange Monitoring ReplicationHealth", this.TransientEventSuppressionWindow, ADObjectWrapperFactory.CreateWrapper(this.m_dag)))
					{
						this.RunMultiChecks(dagMemberMultiChecks);
					}
				}
				if (this.m_useReplayRpc)
				{
					ReplicationCheckGlobals.UsingReplayRpc = true;
				}
				if (this.AreConfigBitsSet(ServerConfig.RcrSource))
				{
					using (RcrSourceMultiChecks rcrSourceMultiChecks = new RcrSourceMultiChecks(this.m_serverName, this.m_eventManager, "MSExchange Monitoring ReplicationHealth", validationRunner, this.m_replayConfigs, ReplicationCheckGlobals.CopyStatusResults, this.TransientEventSuppressionWindow))
					{
						this.RunMultiChecks(rcrSourceMultiChecks);
					}
				}
				if (this.AreConfigBitsSet(ServerConfig.RcrTarget) && this.m_dag.ThirdPartyReplication != ThirdPartyReplicationMode.Enabled)
				{
					using (TargetCopyMultiChecks targetCopyMultiChecks = new TargetCopyMultiChecks(this.m_serverName, this.m_eventManager, "MSExchange Monitoring ReplicationHealth", validationRunner, this.m_replayConfigs, ReplicationCheckGlobals.CopyStatusResults, this.TransientEventSuppressionWindow))
					{
						this.RunMultiChecks(targetCopyMultiChecks);
					}
				}
				if (this.IsServerStandaloneWithNoReplicas())
				{
					using (StandaloneMultiChecks standaloneMultiChecks = new StandaloneMultiChecks(this.m_serverName, this.m_eventManager, "MSExchange Monitoring ReplicationHealth", this.TransientEventSuppressionWindow))
					{
						this.RunMultiChecks(standaloneMultiChecks);
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void RunMultiChecks(MultiReplicationCheck multiChecks)
		{
			multiChecks.Run();
			this.WriteCheckResults(multiChecks);
			this.LogEventsInMonitoringContext(multiChecks);
		}

		private void WriteErrorAndMonitoringEvent(Exception exception, ErrorCategory errorCategory, object target, int eventId, string eventSource)
		{
			this.m_monitoringData.Events.Add(new MonitoringEvent(eventSource, eventId, EventTypeEnumeration.Error, exception.Message));
			base.WriteError(exception, errorCategory, target);
		}

		private void WriteMonitoringData()
		{
			this.m_eventManager.WriteMonitoringEvents(this.m_monitoringData, "MSExchange Monitoring ReplicationHealth");
			base.WriteObject(this.m_monitoringData);
		}

		private void WriteCheckResults(MultiReplicationCheck multiChecks)
		{
			if (this.OutputObjects.ToBool())
			{
				using (List<ReplicationCheckOutputObject>.Enumerator enumerator = multiChecks.GetAllOutputObjects().GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						ReplicationCheckOutputObject sendToPipeline = enumerator.Current;
						base.WriteObject(sendToPipeline);
					}
					return;
				}
			}
			foreach (ReplicationCheckOutcome sendToPipeline2 in multiChecks.GetAllOutcomes())
			{
				base.WriteObject(sendToPipeline2);
			}
		}

		private void LogEventsInMonitoringContext(MultiReplicationCheck multiChecks)
		{
			if (this.MonitoringContext)
			{
				multiChecks.LogEvents();
			}
		}

		private string[] GetServerName()
		{
			string[] array = new string[]
			{
				this.m_serverName
			};
			ExTraceGlobals.HealthChecksTracer.TraceDebug<string>((long)this.GetHashCode(), "GetServerName(): Returning the following servers to use for the RPC: {0}", string.Join(",", array));
			return array;
		}

		private bool CheckLocalServerRegistryRoles()
		{
			bool result;
			try
			{
				base.WriteVerbose(Strings.ReadingE14ServerRoles(this.m_serverName));
				this.m_serverRolesBitfield = MpServerRoles.GetLocalE12ServerRolesFromRegistry();
				if ((this.m_serverRolesBitfield & ServerRole.Mailbox) != ServerRole.Mailbox)
				{
					this.WriteErrorAndMonitoringEvent(new NoMailboxRoleInstalledException(this.m_serverName), ErrorCategory.NotInstalled, null, 10002, "MSExchange Monitoring ReplicationHealth");
					result = false;
				}
				else
				{
					result = true;
				}
			}
			catch (UnauthorizedAccessException ex)
			{
				CannotReadRolesFromRegistryException exception = new CannotReadRolesFromRegistryException(ex.Message);
				this.WriteErrorAndMonitoringEvent(exception, ErrorCategory.PermissionDenied, null, 10001, "MSExchange Monitoring ReplicationHealth");
				result = false;
			}
			catch (SecurityException ex2)
			{
				CannotReadRolesFromRegistryException exception2 = new CannotReadRolesFromRegistryException(ex2.Message);
				this.WriteErrorAndMonitoringEvent(exception2, ErrorCategory.PermissionDenied, null, 10001, "MSExchange Monitoring ReplicationHealth");
				result = false;
			}
			return result;
		}

		private void CheckServerObject()
		{
			if (!this.DataObject.IsMailboxServer)
			{
				this.WriteErrorAndMonitoringEvent(this.DataObject.GetServerRoleError(ServerRole.Mailbox), ErrorCategory.InvalidOperation, this.Identity, 10003, "MSExchange Monitoring ReplicationHealth");
				return;
			}
			if (!this.DataObject.IsE14OrLater)
			{
				this.WriteErrorAndMonitoringEvent(new ServerConfigurationException(this.m_serverName, Strings.ErrorServerNotE14OrLater(this.m_serverName)), ErrorCategory.InvalidOperation, this.Identity, 10003, "MSExchange Monitoring ReplicationHealth");
			}
		}

		private void ResetPrivateState()
		{
			this.m_monitoringData = new MonitoringData();
			this.m_eventManager = new ReplicationEventManager();
			this.m_replayConfigs = new List<ReplayConfiguration>();
			this.m_dag = null;
			this.m_useReplayRpc = false;
			this.m_serverName = null;
			this.m_serverRolesBitfield = ServerRole.None;
			this.m_isLocal = false;
			this.m_serverConfigBitfield = ServerConfig.Unknown;
		}

		[Conditional("DEBUG")]
		private void AssertConfigurationIsValid()
		{
		}

		[Conditional("DEBUG")]
		private void AssertBitsNotSetOnServerType(ServerConfig configBits, ServerConfig serverConfig)
		{
		}

		[Conditional("DEBUG")]
		private void AssertBitsNotSetOnStandalone(ServerConfig configBits)
		{
		}

		private const string CmdletNoun = "ReplicationHealth";

		private const string CmdletMonitoringEventSource = "MSExchange Monitoring ReplicationHealth";

		private const int DefaultADOperationsTimeoutInSeconds = 15;

		private MonitoringData m_monitoringData;

		private ReplicationEventManager m_eventManager;

		private string m_serverName;

		private DatabaseAvailabilityGroup m_dag;

		private ServerRole m_serverRolesBitfield;

		private List<ReplayConfiguration> m_replayConfigs;

		private bool m_useReplayRpc;

		private ServerConfig m_serverConfigBitfield;

		private bool m_isLocal;

		private static string SpaceConcatFormatString = "{0} ";
	}
}
