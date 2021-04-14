using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Monitoring;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Update", "MailboxDatabaseCopy", SupportsShouldProcess = true, DefaultParameterSetName = "Identity", ConfirmImpact = ConfirmImpact.High)]
	public sealed class UpdateDatabaseCopy : DatabaseCopyActionTask
	{
		private bool IsServerLevel { get; set; }

		[Parameter(Mandatory = true, ParameterSetName = "CancelSeed", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		[Parameter(Mandatory = true, ParameterSetName = "Identity", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public override DatabaseCopyIdParameter Identity
		{
			get
			{
				return (DatabaseCopyIdParameter)base.Fields["Identity"];
			}
			set
			{
				base.Fields["Identity"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "ExplicitServer", ValueFromPipeline = true, ValueFromPipelineByPropertyName = true)]
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

		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public int MaximumSeedsInParallel
		{
			get
			{
				return (int)(base.Fields["MaximumSeedsInParallel"] ?? 10);
			}
			set
			{
				base.Fields["MaximumSeedsInParallel"] = value;
			}
		}

		[Parameter(Mandatory = true, ParameterSetName = "CancelSeed")]
		public SwitchParameter CancelSeed
		{
			get
			{
				return (SwitchParameter)(base.Fields["CancelSeed"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CancelSeed"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter BeginSeed
		{
			get
			{
				return (SwitchParameter)(base.Fields["BeginSeed"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["BeginSeed"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public SwitchParameter DatabaseOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["DatabaseOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DatabaseOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public SwitchParameter CatalogOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["CatalogOnly"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["CatalogOnly"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter ManualResume
		{
			get
			{
				return (SwitchParameter)(base.Fields["ManualResume"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["ManualResume"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public SwitchParameter DeleteExistingFiles
		{
			get
			{
				return (SwitchParameter)(base.Fields["DeleteExistingFiles"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["DeleteExistingFiles"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public SwitchParameter SafeDeleteExistingFiles
		{
			get
			{
				return (SwitchParameter)(base.Fields["SafeDeleteExistingFiles"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["SafeDeleteExistingFiles"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public SwitchParameter Force
		{
			get
			{
				return (SwitchParameter)(base.Fields["Force"] ?? new SwitchParameter(false));
			}
			set
			{
				base.Fields["Force"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public DatabaseAvailabilityGroupNetworkIdParameter Network
		{
			get
			{
				return (DatabaseAvailabilityGroupNetworkIdParameter)base.Fields["Network"];
			}
			set
			{
				base.Fields["Network"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public UseDagDefaultOnOff NetworkCompressionOverride
		{
			get
			{
				return (UseDagDefaultOnOff)(base.Fields["NetworkCompressionOverride"] ?? UseDagDefaultOnOff.UseDagDefault);
			}
			set
			{
				base.Fields["NetworkCompressionOverride"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		[Parameter(Mandatory = false, ParameterSetName = "ExplicitServer")]
		public UseDagDefaultOnOff NetworkEncryptionOverride
		{
			get
			{
				return (UseDagDefaultOnOff)(base.Fields["NetworkEncryptionOverride"] ?? UseDagDefaultOnOff.UseDagDefault);
			}
			set
			{
				base.Fields["NetworkEncryptionOverride"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "Identity")]
		public ServerIdParameter SourceServer
		{
			get
			{
				return (ServerIdParameter)base.Fields["SourceServer"];
			}
			set
			{
				base.Fields["SourceServer"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				if (this.IsServerLevel)
				{
					return Strings.ConfirmationMessageUpdateDatabaseCopyServerBegin(this.m_server.Fqdn);
				}
				if (this.CancelSeed)
				{
					return Strings.ConfirmationMessageUpdateDatabaseCopyCancel(this.DataObject.Identity.ToString());
				}
				if (this.BeginSeed)
				{
					return Strings.ConfirmationMessageUpdateDatabaseCopyBegin(this.DataObject.Identity.ToString());
				}
				return Strings.ConfirmationMessageUpdateDatabaseCopy(this.DataObject.Identity.ToString());
			}
		}

		protected override bool IsKnownException(Exception ex)
		{
			return AmExceptionHelper.IsKnownClusterException(this, ex) || ex is SeederServerException || ex is TaskServerException || base.IsKnownException(ex);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.CheckParameterCombination();
			if (this.IsServerLevel)
			{
				this.InternalValidateServerMode();
			}
			else
			{
				this.InternalValidateDatabaseMode();
			}
			base.VerifyIsWithinScopes((IConfigurationSession)base.DataSession, this.m_server, true, new DataAccessTask<DatabaseCopy>.ADObjectOutOfScopeString(Strings.ErrorServerOutOfScope));
			TaskLogger.LogExit();
		}

		protected sealed override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			try
			{
				if (this.IsServerLevel)
				{
					this.InternalProcessServerMode();
				}
				else
				{
					this.InternalProcessDatabaseMode();
				}
			}
			finally
			{
				TaskLogger.LogExit();
			}
		}

		private void PerformDatabaseSeed()
		{
			ExTraceGlobals.CmdletsTracer.TraceDebug<ObjectId>((long)this.GetHashCode(), "UpdateDatabaseCopy: PerformDatabaseSeed: {0}", this.DataObject.Identity);
			using (TaskSeeder taskSeeder = this.ConstructSeeder())
			{
				taskSeeder.SeedDatabase();
			}
		}

		private void CheckParameterCombination()
		{
			if (this.Server != null)
			{
				this.IsServerLevel = true;
			}
			this.m_seedDatabase = !this.CatalogOnly;
			this.m_seedCiFiles = !this.DatabaseOnly;
			if (this.CatalogOnly && this.DatabaseOnly)
			{
				base.WriteError(new UpdateDbcCatalogOnlyAndDatabaseOnlyAreMutuallyExclusiveException(), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (this.DeleteExistingFiles && this.SafeDeleteExistingFiles)
			{
				base.WriteError(new UpdateDbcDeleteFilesInvalidParametersException(), ErrorCategory.InvalidArgument, this.Identity);
			}
		}

		private void InternalValidateDatabaseMode()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.m_database = this.DataObject.GetDatabase<Database>();
			this.m_dbCopyName = string.Format("{0}\\{1}", this.m_database.Name, this.m_server.Name);
			if (this.m_database.ReplicationType != ReplicationType.Remote)
			{
				base.WriteError(new InvalidRCROperationOnNonRcrDB(this.m_database.Name), ErrorCategory.InvalidOperation, this.Identity);
			}
			if (this.CancelSeed)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: CancelSeed called for database copy '{0}'", this.m_dbCopyName);
				base.WriteVerbose(Strings.SeederCancelCalled(this.m_dbCopyName));
			}
			if (this.SourceServer != null)
			{
				this.m_sourceServer = (Server)base.GetDataObject<Server>(this.SourceServer, base.DataSession, null, new LocalizedString?(Strings.ErrorServerNotFound(this.SourceServer.ToString())), new LocalizedString?(Strings.ErrorServerNotUnique(this.SourceServer.ToString())));
				if (!this.m_sourceServer.IsMailboxServer)
				{
					base.WriteError(new OperationOnlyOnMailboxServerException(this.m_sourceServer.Name), ErrorCategory.InvalidOperation, this.Identity);
				}
				if (this.m_sourceServer.MajorVersion != Microsoft.Exchange.Data.Directory.SystemConfiguration.Server.CurrentExchangeMajorVersion)
				{
					base.WriteError(new DagTaskErrorServerWrongVersion(this.m_sourceServer.Name), ErrorCategory.InvalidOperation, this.Identity);
				}
			}
			DatabaseAvailabilityGroup dagForDatabase = DagTaskHelper.GetDagForDatabase(this.m_database, base.DataSession, new Task.TaskErrorLoggingDelegate(base.WriteError));
			DagTaskHelper.PreventTaskWhenTPREnabled(dagForDatabase, this);
		}

		private void InternalValidateServerMode()
		{
			this.ResolveServerParameters();
			DatabaseTasksHelper.CheckServerObjectForCopyTask(this.Server, new Task.TaskErrorLoggingDelegate(base.WriteError), this.m_server);
			if (this.m_server.DatabaseAvailabilityGroup == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorSourceServerNotInDag(this.m_server.Name)), ErrorCategory.InvalidOperation, this.m_server);
			}
			DatabaseAvailabilityGroup databaseAvailabilityGroup = ((IConfigurationSession)base.DataSession).Read<DatabaseAvailabilityGroup>(this.m_server.DatabaseAvailabilityGroup);
			if (databaseAvailabilityGroup == null)
			{
				base.WriteError(new InconsistentADException(Strings.InconsistentServerNotInDag(this.m_server.Name, this.m_server.DatabaseAvailabilityGroup.ToString())), ErrorCategory.InvalidOperation, this.m_server);
			}
			DagTaskHelper.PreventTaskWhenTPREnabled(databaseAvailabilityGroup, this);
		}

		private void ResolveServerParameters()
		{
			this.m_server = (Server)base.GetDataObject<Server>(this.Server, base.DataSession, null, new LocalizedString?(Strings.ErrorMailboxServerNotFound(this.Server.ToString())), new LocalizedString?(Strings.ErrorMailboxServerNotUnique(this.Server.ToString())));
			if (this.m_server == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ServerNotFound(this.Server.ToString())), ErrorCategory.InvalidData, this.Server.ToString());
			}
		}

		private void InternalProcessServerMode()
		{
			ExTraceGlobals.CmdletsTracer.TraceFunction<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: enter InternalProcessServerMode: {0}", this.m_server.Name);
			base.WriteVerbose(Strings.SeederServerLevelBeginCalled(this.m_server.Name));
			try
			{
				using (SeederClient seederClient = SeederClient.Create(this.m_server, null, null))
				{
					seederClient.BeginServerLevelSeed(this.DeleteExistingFiles, this.SafeDeleteExistingFiles, this.MaximumSeedsInParallel, false, this.ManualResume, this.m_seedDatabase, this.m_seedCiFiles, UpdateDatabaseCopy.UseDagDefaultOnOffToNullableBool(this.NetworkCompressionOverride), UpdateDatabaseCopy.UseDagDefaultOnOffToNullableBool(this.NetworkEncryptionOverride), SeederRpcFlags.None);
				}
			}
			catch (FullServerSeedInProgressException ex)
			{
				base.WriteWarning(ex.Message);
			}
			catch (SeederServerException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidOperation, this.m_server);
			}
			catch (SeederServerTransientException exception2)
			{
				base.WriteError(exception2, ErrorCategory.InvalidOperation, this.m_server);
			}
			ExTraceGlobals.CmdletsTracer.TraceFunction<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: leave InternalProcessServerMode: {0}", this.m_server.Name);
		}

		private void InternalProcessDatabaseMode()
		{
			ExTraceGlobals.CmdletsTracer.TraceFunction<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: enter InternalProcessDatabaseMode: {0}", this.m_dbCopyName);
			if (this.CancelSeed)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: Attempting to cancel the seed for '{0}'", this.m_dbCopyName);
				using (TaskSeeder taskSeeder = this.ConstructSeeder())
				{
					taskSeeder.CancelSeed();
					goto IL_9E;
				}
			}
			if (this.BeginSeed)
			{
				ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: BeginSeed called for database copy '{0}' to asynchronously start the seed.", this.m_dbCopyName);
				base.WriteVerbose(Strings.SeederAsyncBeginCalled(this.m_dbCopyName));
			}
			this.PerformDatabaseSeed();
			IL_9E:
			ExTraceGlobals.CmdletsTracer.TraceFunction<string>((long)this.GetHashCode(), "UpdateDatabaseCopy: leave InternalProcessDatabaseMode: {0}", this.m_dbCopyName);
		}

		private TaskSeeder ConstructSeeder()
		{
			return new TaskSeeder(SeedingTask.UpdateDatabaseCopy, this.m_server, this.m_database, this.m_sourceServer, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskShouldContinueDelegate(base.ShouldContinue), () => base.Stopping)
			{
				BeginSeed = this.BeginSeed,
				AutoSuspend = false,
				NetworkId = this.Network,
				DeleteExistingFiles = this.DeleteExistingFiles,
				SafeDeleteExistingFiles = this.SafeDeleteExistingFiles,
				Force = this.Force,
				ManualResume = this.ManualResume,
				SeedCiFiles = this.m_seedCiFiles,
				SeedDatabaseFiles = this.m_seedDatabase,
				CompressOverride = UpdateDatabaseCopy.UseDagDefaultOnOffToNullableBool(this.NetworkCompressionOverride),
				EncryptOverride = UpdateDatabaseCopy.UseDagDefaultOnOffToNullableBool(this.NetworkEncryptionOverride)
			};
		}

		private static bool? UseDagDefaultOnOffToNullableBool(UseDagDefaultOnOff behaviour)
		{
			bool? result = null;
			switch (behaviour)
			{
			case UseDagDefaultOnOff.Off:
				result = new bool?(false);
				break;
			case UseDagDefaultOnOff.On:
				result = new bool?(true);
				break;
			}
			return result;
		}

		private const string CancelSeedParamSetName = "CancelSeed";

		private const string ServerParamSetName = "ExplicitServer";

		private const int DefaultMaximumSeedsInParallel = 10;

		private bool m_seedDatabase;

		private bool m_seedCiFiles;

		private Database m_database;

		private Server m_sourceServer;

		private string m_dbCopyName;
	}
}
