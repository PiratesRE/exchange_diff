using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.ActiveManager;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Move", "ActiveMailboxDatabase", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.Medium)]
	public sealed class MoveActiveMailboxDatabase : SystemConfigurationObjectActionTask<DatabaseIdParameter, Database>
	{
		private MoveActiveMailboxDatabase.CommandTypes CommandType { get; set; }

		private bool TargetServerSpecified { get; set; }

		private bool IsMoveEx2RpcSupported { get; set; }

		[ValidateNotNullOrEmpty]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 1, ParameterSetName = "Server")]
		[Parameter(Mandatory = false, ValueFromPipeline = true, Position = 1, ParameterSetName = "Identity")]
		public MailboxServerIdParameter ActivateOnServer
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["ActivateOnServer"];
			}
			set
			{
				base.Fields["ActivateOnServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public DatabaseMountDialOverride MountDialOverride
		{
			get
			{
				return (DatabaseMountDialOverride)(base.Fields["MountDialOverride"] ?? DatabaseMountDialOverride.Lossless);
			}
			set
			{
				base.Fields["MountDialOverride"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipActiveCopyChecks
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipActiveCopyChecks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipActiveCopyChecks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipClientExperienceChecks
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipClientExperienceChecks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipClientExperienceChecks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipHealthChecks
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipHealthChecks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipHealthChecks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipLagChecks
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipLagChecks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipLagChecks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter SkipMaximumActiveDatabasesChecks
		{
			get
			{
				return (SwitchParameter)(base.Fields["SkipMaximumActiveDatabasesChecks"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SkipMaximumActiveDatabasesChecks"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter TerminateOnWarning
		{
			get
			{
				return (SwitchParameter)(base.Fields["TerminateOnWarning"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["TerminateOnWarning"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "Server", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public MailboxServerIdParameter Server
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["Server"];
			}
			set
			{
				base.Fields["Server"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ActivatePreferred", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public MailboxServerIdParameter ActivatePreferredOnServer
		{
			get
			{
				return (MailboxServerIdParameter)base.Fields["ActivatePreferredOnServer"];
			}
			set
			{
				base.Fields["ActivatePreferredOnServer"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public string MoveComment
		{
			get
			{
				return (string)base.Fields["MoveComment"];
			}
			set
			{
				base.Fields["MoveComment"] = value;
			}
		}

		internal bool IsSkipChecksRequested
		{
			get
			{
				return this.SkipClientExperienceChecks || this.SkipHealthChecks || this.SkipLagChecks || this.SkipActiveCopyChecks || this.SkipMaximumActiveDatabasesChecks;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
				{
					return Strings.ConfirmationMessageMoveActiveMailboxDatabasePrefToServer(this.m_targetServer.Fqdn);
				}
				if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
				{
					if (this.TargetServerSpecified)
					{
						return Strings.ConfirmationMessageMoveMailboxDatabaseMasterSourceServer(this.m_sourceServer.Fqdn, this.m_targetServer.Fqdn);
					}
					return Strings.ConfirmationMessageMoveMailboxDatabaseMasterAnyServerSourceServer(this.m_sourceServer.Fqdn);
				}
				else
				{
					if (this.TargetServerSpecified)
					{
						return Strings.ConfirmationMessageMoveMailboxDatabaseMaster(this.DataObject.Name, this.m_startingServer.Fqdn, this.m_targetServer.Fqdn);
					}
					return Strings.ConfirmationMessageMoveMailboxDatabaseMasterAnyServer(this.DataObject.Name, this.m_startingServer.Fqdn);
				}
			}
		}

		protected override bool IsKnownException(Exception e)
		{
			return AmExceptionHelper.IsKnownClusterException(this, e) || SystemConfigurationTasksHelper.IsKnownMapiDotNETException(e) || e is AmServerException || e is ObjectNotFoundException || base.IsKnownException(e);
		}

		protected override void InternalStateReset()
		{
			base.InternalStateReset();
			this.m_dag = null;
			this.m_database = null;
			this.m_targetServer = null;
			this.m_targetAmServer = null;
			this.m_sourceServer = null;
			this.m_sourceAmServer = null;
			this.m_startingServer = null;
			this.m_lastServerContacted = null;
			this.m_databaseTable.Clear();
		}

		private void InternalValidateServerMode()
		{
			this.ResolveParameters();
			Server server;
			MailboxServerIdParameter serverIdParam;
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
			{
				server = this.m_sourceServer;
				serverIdParam = this.Server;
			}
			else
			{
				server = this.m_targetServer;
				serverIdParam = this.ActivatePreferredOnServer;
			}
			DatabaseTasksHelper.CheckServerObjectForCopyTask(serverIdParam, new Task.TaskErrorLoggingDelegate(base.WriteError), server);
			if (server.DatabaseAvailabilityGroup == null)
			{
				this.m_output.WriteError(new InvalidOperationException(Strings.ErrorSourceServerNotInDag(server.Name)), ErrorCategory.InvalidOperation, server.Identity);
			}
			this.m_dag = ((IConfigurationSession)base.DataSession).Read<DatabaseAvailabilityGroup>(server.DatabaseAvailabilityGroup);
			if (this.m_dag == null)
			{
				this.m_output.WriteError(new InconsistentADException(Strings.InconsistentServerNotInDag(server.Name, server.DatabaseAvailabilityGroup.ToString())), ErrorCategory.InvalidOperation, server.Identity);
			}
			if (this.m_dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				this.m_output.WriteError(new InvalidTPRTaskException(base.MyInvocation.MyCommand.Name), ErrorCategory.InvalidOperation, server.Identity);
			}
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, server, true, new DataAccessTask<Database>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
			{
				Database[] databases = null;
				this.PreventMoveOfActiveSeedingSource(this.m_sourceAmServer, databases);
				if (this.TargetServerSpecified)
				{
					this.PerformTargetServerValidation();
				}
			}
		}

		private void InternalValidateDatabaseMode()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.m_database = this.DataObject;
			this.ResolveParameters();
			this.CheckDatabaseObject();
			if (this.m_database.MasterType != MasterType.DatabaseAvailabilityGroup)
			{
				this.m_output.WriteError(new InvalidOperationException(Strings.ErrorSingleDbCopyMove(this.m_database.Name, this.m_database.ServerName)), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			this.m_dag = DagTaskHelper.ReadDag(this.m_database.MasterServerOrAvailabilityGroup, base.DataSession);
			if (this.m_dag == null)
			{
				this.m_output.WriteError(new InconsistentADException(Strings.InconsistentADDbMasterServerNotADag(this.m_database.Name, this.m_database.MasterServerOrAvailabilityGroup.ToString())), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			if (this.m_dag.ThirdPartyReplication == ThirdPartyReplicationMode.Enabled)
			{
				this.m_output.WriteError(new InvalidTPRTaskException(base.MyInvocation.MyCommand.Name), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			this.m_startingServer = this.GetCurrentActiveServer(this.m_database.Guid);
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, DatabaseTasksHelper.GetServerObject(new ServerIdParameter(Fqdn.Parse(this.m_startingServer.Fqdn)), (IConfigurationSession)base.DataSession, this.RootId, new DataAccessHelper.GetDataObjectDelegate(base.GetDataObject<Server>)), true, new DataAccessTask<Database>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			this.PreventMoveOfActiveSeedingSource(this.m_startingServer, new Database[]
			{
				this.m_database
			});
			if (this.TargetServerSpecified)
			{
				this.PerformTargetServerValidation();
			}
		}

		private void PreventMoveOfActiveSeedingSource(AmServerName server, Database[] databases)
		{
			if (this.SkipActiveCopyChecks)
			{
				TaskLogger.Trace("PreventMoveOfActiveSeedingSource: Check skipped because -SkipActiveCopyChecks flag is specified.", new object[0]);
				return;
			}
			Exception ex = null;
			RpcDatabaseCopyStatus2[] array = null;
			try
			{
				Guid[] array2 = null;
				if (databases != null && databases.Length > 0)
				{
					array2 = new Guid[databases.Length];
					for (int i = 0; i < databases.Length; i++)
					{
						array2[i] = databases[i].Guid;
					}
				}
				array = ReplayRpcClientHelper.GetCopyStatus(server.Fqdn, RpcGetDatabaseCopyStatusFlags2.ReadThrough, array2);
			}
			catch (TaskServerTransientException ex2)
			{
				ex = ex2;
			}
			catch (TaskServerException ex3)
			{
				ex = ex3;
			}
			if (ex != null)
			{
				TaskLogger.Trace("PreventMoveOfActiveSeedingSource: Check failed because of exception: {2}", new object[]
				{
					ex
				});
				base.WriteError(new ErrorMoveUnableToGetCopyStatusException(server.NetbiosName, ex.Message), ErrorCategory.InvalidOperation, server.Fqdn);
			}
			foreach (RpcDatabaseCopyStatus2 rpcDatabaseCopyStatus in array)
			{
				if (SharedHelper.StringIEquals(rpcDatabaseCopyStatus.ActiveDatabaseCopy, server.NetbiosName) && rpcDatabaseCopyStatus.SeedingSource)
				{
					if (!rpcDatabaseCopyStatus.SeedingSourceForDB)
					{
						if (rpcDatabaseCopyStatus.SeedingSourceForDB || rpcDatabaseCopyStatus.SeedingSourceForCI)
						{
							goto IL_1B6;
						}
					}
					try
					{
						IADDatabase iaddatabase = AmHelper.FindDatabaseByGuid(rpcDatabaseCopyStatus.DBGuid);
						TaskLogger.Trace("PreventMoveOfActiveSeedingSource: Check failed for {0}\\{1} because it is seeding source.", new object[]
						{
							iaddatabase.Name,
							rpcDatabaseCopyStatus.MailboxServer
						});
						base.WriteError(new ErrorMoveActiveCopyIsSeedingSourceException(iaddatabase.Name, rpcDatabaseCopyStatus.MailboxServer), ErrorCategory.InvalidOperation, iaddatabase.Identity);
					}
					catch (AmDatabaseNotFoundException ex4)
					{
						TaskLogger.Trace("PreventMoveOfActiveSeedingSource: Check failed for '{0}' because it is not found in AD. Error: {1}", new object[]
						{
							rpcDatabaseCopyStatus.DBGuid,
							ex4.Message
						});
						base.WriteError(new ErrorMoveActiveCopyNotFoundException(rpcDatabaseCopyStatus.DBGuid, ex4.Message), ErrorCategory.InvalidOperation, rpcDatabaseCopyStatus.DBGuid);
					}
				}
				IL_1B6:;
			}
		}

		protected override void InternalValidate()
		{
			this.m_output = new HaTaskOutputHelper("Move-MailboxDatabaseMaster", new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskProgressLoggingDelegate(base.WriteProgress), this.GetHashCode());
			try
			{
				this.CheckParameterCombination();
				if (this.CommandType != MoveActiveMailboxDatabase.CommandTypes.MoveSingleDatabase)
				{
					this.InternalValidateServerMode();
				}
				else
				{
					this.InternalValidateDatabaseMode();
				}
				if (this.DataObject != null)
				{
					MapiTaskHelper.VerifyDatabaseIsWithinScope(base.SessionSettings, this.DataObject, new Task.ErrorLoggerDelegate(base.WriteError));
				}
				if (this.TargetServerSpecified && this.m_targetServer.DatabaseCopyActivationDisabledAndMoveNow)
				{
					this.m_output.WriteWarning(Strings.ConfirmationMessageMoveMailboxDatabaseMasterToDisabledServer(this.m_targetServer.Fqdn));
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void MoveSingleDatabase()
		{
			AmDatabaseMoveResult result = null;
			try
			{
				int mountDialOverride = (int)this.MountDialOverride;
				bool tryOtherHealthyServers = !this.TargetServerSpecified;
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Move);
				AmRpcClientHelper.MoveDatabaseEx(ADObjectWrapperFactory.CreateWrapper(this.m_database), 0, 16, mountDialOverride, null, this.TargetServerSpecified ? this.m_targetServer.Fqdn : null, tryOtherHealthyServers, (int)this.GetSkipFlags(), actionCode, this.MoveComment, out this.m_lastServerContacted, ref result);
				this.m_output.WriteProgress(Strings.ProgressStatusCompleted, this.GetRpcDoneProgressString(), 100);
				this.WriteMoveResult(this.DataObject, this.m_startingServer, result, null);
			}
			catch (AmReplayServiceDownException ex)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveSingleDatabase raised exception while moving database: {0}", new object[]
				{
					ex
				});
				this.WriteMoveResult(this.DataObject, this.m_startingServer, result, ex);
				this.m_output.WriteError(new InvalidOperationException(Strings.ErrorActiveManagerIsNotReachable(ex.ServerName, ex.RpcErrorMessage)), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			catch (AmServerException ex2)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveSingleDatabase raised exception while moving database: {0}", new object[]
				{
					ex2
				});
				this.WriteMoveResult(this.DataObject, this.m_startingServer, result, ex2);
				this.m_output.WriteError(ex2, ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
			catch (AmServerTransientException ex3)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveSingleDatabase raised exception while moving database: {0}", new object[]
				{
					ex3
				});
				this.WriteMoveResult(this.DataObject, this.m_startingServer, result, ex3);
				this.m_output.WriteError(ex3, ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
		}

		private void MoveAllDatabasesFromSourceServer()
		{
			try
			{
				this.PopulateDatabaseCache();
				int mountDialOverride = (int)this.MountDialOverride;
				bool tryOtherHealthyServers = !this.TargetServerSpecified;
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Move);
				List<AmDatabaseMoveResult> moveResults = AmRpcClientHelper.ServerMoveAllDatabases(this.m_sourceServer.Fqdn, this.TargetServerSpecified ? this.m_targetServer.Fqdn : null, 0, 16, mountDialOverride, tryOtherHealthyServers, (int)this.GetSkipFlags(), (int)actionCode, this.MoveComment, out this.m_lastServerContacted);
				this.m_output.WriteProgress(Strings.ProgressStatusCompleted, this.GetRpcDoneProgressString(), 100);
				this.WriteAllMoveResults(moveResults);
			}
			catch (AmReplayServiceDownException ex)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveAllDatabasesFromSourceServer raised exception while moving databases: {0}", new object[]
				{
					ex
				});
				this.m_output.WriteError(new InvalidOperationException(Strings.ErrorActiveManagerIsNotReachable(ex.ServerName, ex.RpcErrorMessage)), ErrorCategory.InvalidOperation, this.m_sourceServer.Identity);
			}
			catch (AmServerException ex2)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveAllDatabasesFromSourceServer raised exception while moving databases: {0}", new object[]
				{
					ex2
				});
				this.m_output.WriteError(ex2, ErrorCategory.InvalidOperation, this.m_sourceServer.Identity);
			}
			catch (AmServerTransientException ex3)
			{
				TaskLogger.Trace("MoveMdbMaster.MoveAllDatabasesFromSourceServer raised exception while moving databases: {0}", new object[]
				{
					ex3
				});
				this.m_output.WriteError(ex3, ErrorCategory.InvalidOperation, this.m_sourceServer.Identity);
			}
		}

		private void MoveDatabasesBackToServer()
		{
			Exception ex = null;
			try
			{
				int mountDialOverride = (int)this.MountDialOverride;
				AmDbActionCode actionCode = new AmDbActionCode(AmDbActionInitiator.Admin, AmDbActionReason.Cmdlet, AmDbActionCategory.Move);
				List<AmDatabaseMoveResult> moveResults = AmRpcClientHelper.ServerMoveAllDatabases(null, this.m_targetServer.Fqdn, 0, 16, mountDialOverride, false, (int)this.GetSkipFlags(), (int)actionCode, this.MoveComment, out this.m_lastServerContacted);
				this.m_output.WriteProgress(Strings.ProgressStatusCompleted, this.GetRpcDoneProgressString(), 100);
				this.WriteMoveBackResults(moveResults);
			}
			catch (AmReplayServiceDownException ex2)
			{
				ex = new InvalidOperationException(Strings.ErrorActiveManagerIsNotReachable(ex2.ServerName, ex2.RpcErrorMessage));
			}
			catch (AmServerException ex3)
			{
				ex = ex3;
			}
			catch (AmServerTransientException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				TaskLogger.Trace("MoveActiveMdb.MoveDatabasesBackToServer raised exception while moving databases: {0}", new object[]
				{
					ex
				});
				this.m_output.WriteError(ex, ErrorCategory.InvalidOperation, this.m_targetServer.Identity);
			}
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				TaskLogger.LogEnter();
				if (!this.IsSkipChecksRequested || base.UserSpecifiedParameters.Contains("Confirm") || base.ShouldContinue(this.ConfirmationMessage))
				{
					this.m_output.WriteProgress(Strings.ProgressStatusInProgress, this.GetStartRpcProgressString(), 77);
					if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
					{
						this.MoveAllDatabasesFromSourceServer();
					}
					else if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
					{
						this.MoveDatabasesBackToServer();
					}
					else
					{
						this.MoveSingleDatabase();
					}
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private AmBcsSkipFlags GetSkipFlags()
		{
			AmBcsSkipFlags amBcsSkipFlags = AmBcsSkipFlags.None;
			if (this.SkipClientExperienceChecks)
			{
				amBcsSkipFlags |= AmBcsSkipFlags.SkipClientExperienceChecks;
			}
			if (this.SkipHealthChecks)
			{
				amBcsSkipFlags |= AmBcsSkipFlags.SkipHealthChecks;
			}
			if (this.SkipLagChecks)
			{
				amBcsSkipFlags |= AmBcsSkipFlags.SkipLagChecks;
			}
			if (this.SkipMaximumActiveDatabasesChecks)
			{
				amBcsSkipFlags |= AmBcsSkipFlags.SkipMaximumActiveDatabasesChecks;
			}
			if (this.SkipActiveCopyChecks)
			{
				amBcsSkipFlags |= AmBcsSkipFlags.SkipActiveCopyChecks;
			}
			return amBcsSkipFlags;
		}

		private void CheckDatabaseObject()
		{
			DatabaseTasksHelper.CheckDatabaseForCopyTask(this.m_database, new Task.TaskErrorLoggingDelegate(base.WriteError), Strings.ErrorSingleDbCopyMove(this.m_database.Name, this.m_database.ServerName));
			if (this.m_database.Recovery)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorInvalidOperationOnRecoveryMailboxDatabase(this.m_database.Name)), ErrorCategory.InvalidOperation, this.m_database.Identity);
			}
		}

		private void ResolveParameters()
		{
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
			{
				this.m_sourceServer = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(this.Server.ToString())));
				if (this.m_sourceServer == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ServerNotFound(this.Server.ToString())), ErrorCategory.InvalidData, this.Server.ToString());
				}
				this.m_sourceAmServer = new AmServerName(this.m_sourceServer);
			}
			MailboxServerIdParameter mailboxServerIdParameter = null;
			if (this.TargetServerSpecified)
			{
				mailboxServerIdParameter = this.ActivateOnServer;
			}
			else if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
			{
				mailboxServerIdParameter = this.ActivatePreferredOnServer;
			}
			if (mailboxServerIdParameter != null)
			{
				this.m_targetServer = (Server)base.GetDataObject<Server>(mailboxServerIdParameter, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxServerNotFound(mailboxServerIdParameter.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(mailboxServerIdParameter.ToString())));
				if (this.m_targetServer == null)
				{
					base.WriteError(new InvalidOperationException(Strings.ServerNotFound(mailboxServerIdParameter.ToString())), ErrorCategory.InvalidData, mailboxServerIdParameter);
				}
				this.m_targetAmServer = new AmServerName(this.m_targetServer);
			}
		}

		private void CheckParameterCombination()
		{
			if (this.ActivateOnServer != null)
			{
				this.TargetServerSpecified = true;
			}
			if (this.Server != null)
			{
				this.CommandType = MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer;
			}
			else if (this.ActivatePreferredOnServer != null)
			{
				this.CommandType = MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer;
			}
			else
			{
				this.CommandType = MoveActiveMailboxDatabase.CommandTypes.MoveSingleDatabase;
			}
			if (this.CommandType != MoveActiveMailboxDatabase.CommandTypes.MoveSingleDatabase && this.MountDialOverride == DatabaseMountDialOverride.BestEffort)
			{
				base.WriteError(new BestEffortNotAllowedInServerModeException(), ErrorCategory.InvalidArgument, this.Server);
			}
		}

		private void PerformTargetServerValidation()
		{
			DatabaseTasksHelper.CheckServerObjectForCopyTask(this.ActivateOnServer, new Task.TaskErrorLoggingDelegate(base.WriteError), this.m_targetServer);
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveSingleDatabase)
			{
				DatabaseCopy databaseCopy;
				DatabaseCopy[] array;
				DatabaseTasksHelper.CheckDatabaseCopyForCopyTask(this.m_database, new Task.TaskErrorLoggingDelegate(this.m_output.WriteError), this.m_targetServer, out databaseCopy, out array);
			}
			if (this.m_dag == null)
			{
				this.m_dag = ((IConfigurationSession)base.DataSession).Read<DatabaseAvailabilityGroup>(this.m_targetServer.DatabaseAvailabilityGroup);
			}
			IADDatabaseAvailabilityGroup dag = ADObjectWrapperFactory.CreateWrapper(this.m_dag);
			if (AmBestCopySelectionHelper.IsServerInDacAndStopped(dag, new AmServerName(this.m_targetServer.Name)))
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorServerDacedAndNotStarted(this.m_dag.Name, this.m_targetServer.Name)), ErrorCategory.InvalidOperation, null);
			}
			DatabaseTasksHelper.CheckReplayServiceRunningOnNode(this.m_targetAmServer, new Task.TaskErrorLoggingDelegate(base.WriteError));
			DagTaskHelper.CheckStoreIsRunning(new Task.TaskErrorLoggingDelegate(base.WriteError), this.m_targetAmServer);
			if (this.MountDialOverride != DatabaseMountDialOverride.Lossless)
			{
				base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_targetServer, true, new DataAccessTask<Database>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			}
		}

		private LocalizedString GetStartRpcProgressString()
		{
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
			{
				return Strings.ConfirmationMessageMoveActiveMailboxDatabasePrefToServer(this.m_targetServer.Fqdn);
			}
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
			{
				if (this.TargetServerSpecified)
				{
					return Strings.AmMoveRpcRequestedSourceServer(this.m_sourceServer.Name, this.m_targetServer.Name);
				}
				return Strings.AmMoveRpcRequestedAnyServerSourceServer(this.m_sourceServer.Name);
			}
			else
			{
				if (this.TargetServerSpecified)
				{
					return Strings.AmMoveRpcRequested(this.m_database.Name, this.m_targetServer.Name);
				}
				return Strings.AmMoveRpcRequestedAnyServer(this.m_database.Name);
			}
		}

		private LocalizedString GetRpcDoneProgressString()
		{
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
			{
				return Strings.ConfirmationMessageMoveActiveMailboxDatabasePrefToServer(this.m_targetServer.Fqdn);
			}
			if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
			{
				if (this.TargetServerSpecified)
				{
					return Strings.AmMoveRpcCompletedSourceServer(this.m_sourceServer.Name, this.m_targetServer.Name);
				}
				return Strings.AmMoveRpcCompletedAnyServerSourceServer(this.m_sourceServer.Name);
			}
			else
			{
				if (this.TargetServerSpecified)
				{
					return Strings.AmMoveRpcCompleted(this.m_database.Name, this.m_targetServer.Name);
				}
				return Strings.AmMoveRpcCompletedAnyServer(this.m_database.Name);
			}
		}

		private void WriteWarningOrError(LocalizedString message)
		{
			if (this.TerminateOnWarning)
			{
				object identity;
				if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveBackToServer)
				{
					identity = this.m_targetServer.Identity;
				}
				else if (this.CommandType == MoveActiveMailboxDatabase.CommandTypes.MoveAwayFromServer)
				{
					identity = this.m_sourceServer.Identity;
				}
				else
				{
					identity = this.m_database.Identity;
				}
				this.WriteError(new InvalidOperationException(message), ErrorCategory.InvalidOperation, identity, false);
				return;
			}
			this.WriteWarning(message);
		}

		private void PopulateDatabaseCache()
		{
			if (this.m_databaseTable == null || this.m_databaseTable.Count == 0)
			{
				Database[] databases = this.m_sourceServer.GetDatabases();
				Dictionary<Guid, DatabaseLocationInfo> dbInfoTable = new Dictionary<Guid, DatabaseLocationInfo>(databases.Length);
				ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
				foreach (Database database in databases)
				{
					DatabaseLocationInfo serverForDatabase = noncachingActiveManagerInstance.GetServerForDatabase(database.Guid);
					dbInfoTable[database.Guid] = serverForDatabase;
				}
				IEnumerable<Database> source = from db in databases
				where this.IsDatabaseActiveOnSourceServer(db, dbInfoTable)
				select db;
				this.m_databaseTable = source.ToDictionary((Database db) => db.Guid);
			}
		}

		private bool IsDatabaseActiveOnSourceServer(Database db, Dictionary<Guid, DatabaseLocationInfo> dbInfoTable)
		{
			AmServerName amServerName = new AmServerName(dbInfoTable[db.Guid].ServerFqdn);
			return amServerName.Equals(this.m_sourceAmServer);
		}

		private void WriteAllMoveResults(List<AmDatabaseMoveResult> moveResults)
		{
			if (moveResults == null || moveResults.Count == 0)
			{
				this.WriteWarningOrError(Strings.WarningNoActiveDatabasesFoundOnServer(this.m_sourceServer.Fqdn));
				return;
			}
			IEnumerable<AmDatabaseMoveResult> rpcMoveResults = from moveResult in moveResults
			where this.m_databaseTable.ContainsKey(moveResult.DbGuid)
			select moveResult;
			IEnumerable<DatabaseMoveResult> convertedMoveResults = this.GetConvertedMoveResults(this.m_sourceAmServer, rpcMoveResults, null);
			foreach (DatabaseMoveResult sendToPipeline in convertedMoveResults)
			{
				base.WriteObject(sendToPipeline);
			}
			this.WriteErrorsForUnmovedDatabases(moveResults);
			foreach (DatabaseMoveResult databaseMoveResult in convertedMoveResults)
			{
				if (databaseMoveResult.Status == MoveStatus.Failed)
				{
					this.WriteError(databaseMoveResult.Exception, ErrorCategory.InvalidOperation, this.m_databaseTable[databaseMoveResult.Guid].Identity, false);
				}
			}
			foreach (DatabaseMoveResult databaseMoveResult2 in convertedMoveResults)
			{
				if (databaseMoveResult2.Status == MoveStatus.Warning)
				{
					this.WriteWarningOrError(new LocalizedString(databaseMoveResult2.ErrorMessage));
				}
			}
		}

		private void WriteMoveBackResults(List<AmDatabaseMoveResult> moveResults)
		{
			if (moveResults == null || moveResults.Count == 0)
			{
				this.WriteWarningOrError(Strings.WarningNoDatabasesWereMovedBackToServer(this.m_targetServer.Fqdn));
				return;
			}
			List<DatabaseMoveResult> list = new List<DatabaseMoveResult>(moveResults.Count);
			foreach (AmDatabaseMoveResult result in moveResults)
			{
				list.Add(this.PrepareResultEntryForMoveBack(result));
			}
			foreach (DatabaseMoveResult sendToPipeline in list)
			{
				base.WriteObject(sendToPipeline);
			}
			foreach (DatabaseMoveResult databaseMoveResult in list)
			{
				if (databaseMoveResult.Status == MoveStatus.Failed)
				{
					this.WriteError(databaseMoveResult.Exception, ErrorCategory.InvalidOperation, databaseMoveResult.Identity, false);
				}
			}
			foreach (DatabaseMoveResult databaseMoveResult2 in list)
			{
				if (databaseMoveResult2.Status == MoveStatus.Warning)
				{
					this.WriteWarningOrError(new LocalizedString(databaseMoveResult2.ErrorMessage));
				}
			}
		}

		private void WriteErrorsForUnmovedDatabases(List<AmDatabaseMoveResult> rpcMoveResults)
		{
			Dictionary<Guid, AmDatabaseMoveResult> rpcMoveResultsTable = rpcMoveResults.ToDictionary((AmDatabaseMoveResult result) => result.DbGuid);
			IEnumerable<KeyValuePair<Guid, Database>> enumerable = from kvp in this.m_databaseTable
			where !rpcMoveResultsTable.ContainsKey(kvp.Key)
			select kvp;
			foreach (KeyValuePair<Guid, Database> keyValuePair in enumerable)
			{
				this.WriteError(new DatabaseNotMovedInServerModeException(keyValuePair.Value.Name, this.m_sourceServer.Fqdn), ErrorCategory.InvalidOperation, keyValuePair.Value.Identity, false);
			}
		}

		private IEnumerable<DatabaseMoveResult> GetConvertedMoveResults(AmServerName startingServer, IEnumerable<AmDatabaseMoveResult> rpcMoveResults, Exception ex)
		{
			foreach (AmDatabaseMoveResult rpcMoveResult in rpcMoveResults)
			{
				yield return this.ConvertToDatabaseMoveResult(this.m_databaseTable[rpcMoveResult.DbGuid], startingServer, rpcMoveResult, ex);
			}
			yield break;
		}

		private void WriteMoveResult(Database db, AmServerName startingServer, AmDatabaseMoveResult result, Exception ex)
		{
			if (result != null)
			{
				DatabaseMoveResult databaseMoveResult = this.ConvertToDatabaseMoveResult(db, startingServer, result, ex);
				base.WriteObject(databaseMoveResult);
				if (databaseMoveResult.Status == MoveStatus.Warning)
				{
					this.WriteWarningOrError(new LocalizedString(databaseMoveResult.ErrorMessage));
				}
			}
		}

		private DatabaseMoveResult ConvertToDatabaseMoveResult(Database db, AmServerName startingServer, AmDatabaseMoveResult result, Exception ex)
		{
			DatabaseMoveResult result2 = null;
			if (result != null)
			{
				if (result.IsLegacy)
				{
					result2 = this.PrepareResultEntryLegacy(db, startingServer, result, ex);
				}
				else
				{
					result2 = this.PrepareResultEntry(db, result, ex);
				}
			}
			return result2;
		}

		private DatabaseMoveResult PrepareResultEntryLegacy(Database db, AmServerName startingServer, AmDatabaseMoveResult result, Exception ex)
		{
			DatabaseMoveResult databaseMoveResult = new DatabaseMoveResult();
			bool flag = ex == null;
			databaseMoveResult.Guid = db.Guid;
			databaseMoveResult.Identity = db.Id;
			databaseMoveResult.ActiveServerAtStart = startingServer.NetbiosName;
			databaseMoveResult.ActiveServerAtEnd = this.GetCurrentActiveServer(db.Guid).NetbiosName;
			if (flag)
			{
				databaseMoveResult.Status = MoveStatus.Succeeded;
			}
			else
			{
				databaseMoveResult.Status = MoveStatus.Failed;
				databaseMoveResult.Exception = ex;
				databaseMoveResult.ErrorMessage = ex.Message;
			}
			return databaseMoveResult;
		}

		private string GetSafeShortServerName(string possibleServerFqdn, Database db)
		{
			if (!string.IsNullOrEmpty(possibleServerFqdn))
			{
				return new AmServerName(possibleServerFqdn).NetbiosName;
			}
			return this.GetCurrentActiveServer(db.Guid).NetbiosName;
		}

		private DatabaseMoveResult PrepareResultEntry(Database db, AmDatabaseMoveResult result, Exception exception)
		{
			DatabaseMoveResult databaseMoveResult = new DatabaseMoveResult();
			databaseMoveResult.Guid = db.Guid;
			databaseMoveResult.Identity = db.Id;
			databaseMoveResult.ActiveServerAtStart = this.GetSafeShortServerName(result.FromServerFqdn, db);
			databaseMoveResult.ActiveServerAtEnd = this.GetSafeShortServerName(result.FinalActiveServerFqdn, db);
			databaseMoveResult.Status = MoveActiveMailboxDatabase.ConvertToMoveStatus(result.MoveStatus);
			databaseMoveResult.MountStatusAtMoveStart = MoveActiveMailboxDatabase.ConvertToMountStatus(result.MountStatusAtStart);
			databaseMoveResult.MountStatusAtMoveEnd = MoveActiveMailboxDatabase.ConvertToMountStatus(result.MountStatusAtEnd);
			if (result.AttemptedServerSubStatuses != null)
			{
				AmDbRpcOperationSubStatus amDbRpcOperationSubStatus = result.AttemptedServerSubStatuses.LastOrDefault<AmDbRpcOperationSubStatus>();
				if (amDbRpcOperationSubStatus != null)
				{
					databaseMoveResult.NumberOfLogsLost = MoveActiveMailboxDatabase.ToNullableLogGeneration(amDbRpcOperationSubStatus.AcllStatus.NumberOfLogsLost);
					databaseMoveResult.RecoveryPointObjective = DumpsterStatisticsEntry.ToNullableLocalDateTime(amDbRpcOperationSubStatus.AcllStatus.LastInspectedLogTime);
				}
			}
			Exception ex = exception ?? this.GetExceptionFromMoveResult(result);
			if (ex != null)
			{
				databaseMoveResult.Exception = ex;
				databaseMoveResult.ErrorMessage = ex.Message;
			}
			return databaseMoveResult;
		}

		private DatabaseMoveResult PrepareResultEntryForMoveBack(AmDatabaseMoveResult result)
		{
			DatabaseMoveResult databaseMoveResult = new DatabaseMoveResult();
			databaseMoveResult.Guid = result.DbGuid;
			ConfigObjectId identity = new ConfigObjectId(result.DbName);
			databaseMoveResult.Identity = identity;
			databaseMoveResult.ActiveServerAtStart = result.FromServerFqdn;
			databaseMoveResult.ActiveServerAtEnd = result.FinalActiveServerFqdn;
			databaseMoveResult.Status = MoveActiveMailboxDatabase.ConvertToMoveStatus(result.MoveStatus);
			databaseMoveResult.MountStatusAtMoveStart = MoveActiveMailboxDatabase.ConvertToMountStatus(result.MountStatusAtStart);
			databaseMoveResult.MountStatusAtMoveEnd = MoveActiveMailboxDatabase.ConvertToMountStatus(result.MountStatusAtEnd);
			if (result.AttemptedServerSubStatuses != null)
			{
				AmDbRpcOperationSubStatus amDbRpcOperationSubStatus = result.AttemptedServerSubStatuses.LastOrDefault<AmDbRpcOperationSubStatus>();
				if (amDbRpcOperationSubStatus != null)
				{
					databaseMoveResult.NumberOfLogsLost = MoveActiveMailboxDatabase.ToNullableLogGeneration(amDbRpcOperationSubStatus.AcllStatus.NumberOfLogsLost);
					databaseMoveResult.RecoveryPointObjective = DumpsterStatisticsEntry.ToNullableLocalDateTime(amDbRpcOperationSubStatus.AcllStatus.LastInspectedLogTime);
				}
			}
			Exception exceptionFromMoveResult = this.GetExceptionFromMoveResult(result);
			if (exceptionFromMoveResult != null)
			{
				databaseMoveResult.Exception = exceptionFromMoveResult;
				databaseMoveResult.ErrorMessage = exceptionFromMoveResult.Message;
			}
			return databaseMoveResult;
		}

		private Exception GetExceptionFromMoveResult(AmDatabaseMoveResult result)
		{
			Exception result2 = null;
			try
			{
				AmRpcExceptionWrapper.Instance.ClientRethrowIfFailed(result.DbName, this.m_lastServerContacted, result.ErrorInfo);
			}
			catch (AmServerException ex)
			{
				result2 = ex;
			}
			catch (AmServerTransientException ex2)
			{
				result2 = ex2;
			}
			return result2;
		}

		private AmServerName GetCurrentActiveServer(Guid dbGuid)
		{
			ActiveManager noncachingActiveManagerInstance = ActiveManager.GetNoncachingActiveManagerInstance();
			DatabaseLocationInfo serverForDatabase = noncachingActiveManagerInstance.GetServerForDatabase(dbGuid);
			return new AmServerName(serverForDatabase.ServerFqdn);
		}

		private static MoveStatus ConvertToMoveStatus(AmDbMoveStatus rpcMoveStatus)
		{
			switch (rpcMoveStatus)
			{
			case AmDbMoveStatus.Unknown:
				return MoveStatus.Unknown;
			case AmDbMoveStatus.Succeeded:
				return MoveStatus.Succeeded;
			case AmDbMoveStatus.Warning:
				return MoveStatus.Warning;
			case AmDbMoveStatus.Failed:
				return MoveStatus.Failed;
			case AmDbMoveStatus.Skipped:
				return MoveStatus.Skipped;
			default:
				return MoveStatus.Unknown;
			}
		}

		private static MountStatus ConvertToMountStatus(AmDbMountStatus rpcMountStatus)
		{
			switch (rpcMountStatus)
			{
			case AmDbMountStatus.Unknown:
				return MountStatus.Unknown;
			case AmDbMountStatus.Mounted:
				return MountStatus.Mounted;
			case AmDbMountStatus.Dismounted:
				return MountStatus.Dismounted;
			case AmDbMountStatus.Mounting:
				return MountStatus.Mounting;
			case AmDbMountStatus.Dismounting:
				return MountStatus.Dismounting;
			default:
				return MountStatus.Unknown;
			}
		}

		private static long? ToNullableLogGeneration(long logGenNumber)
		{
			if (logGenNumber == -1L)
			{
				return null;
			}
			return new long?(logGenNumber);
		}

		private const string ActivateOnServerParamName = "ActivateOnServer";

		private const string MountDialOverrideParamName = "MountDialOverride";

		private const string ServerParamName = "Server";

		private const string ServerParameterSetName = "Server";

		private const string ActivatePreferredParameterSetName = "ActivatePreferred";

		private const string MoveCommentName = "MoveComment";

		private Database m_database;

		private Server m_targetServer;

		private AmServerName m_targetAmServer;

		private AmServerName m_startingServer;

		private DatabaseAvailabilityGroup m_dag;

		private string m_lastServerContacted;

		private Server m_sourceServer;

		private AmServerName m_sourceAmServer;

		private Dictionary<Guid, Database> m_databaseTable = new Dictionary<Guid, Database>(25);

		private HaTaskOutputHelper m_output;

		private enum CommandTypes
		{
			Unknown,
			MoveSingleDatabase,
			MoveAwayFromServer,
			MoveBackToServer
		}
	}
}
