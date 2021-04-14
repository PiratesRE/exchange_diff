using System;
using System.IO;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	public abstract class AddDatabaseCopyTaskBase<TDataObject> : DatabaseActionTaskBase<TDataObject> where TDataObject : Database, new()
	{
		[Parameter(Mandatory = false)]
		public SwitchParameter SeedingPostponed
		{
			get
			{
				return (SwitchParameter)(base.Fields["SeedingPostponed"] ?? false);
			}
			set
			{
				base.Fields["SeedingPostponed"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SwitchParameter ConfigurationOnly
		{
			get
			{
				return (SwitchParameter)(base.Fields["ConfigurationOnly"] ?? false);
			}
			set
			{
				base.Fields["ConfigurationOnly"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan ReplayLagTime
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["ReplayLagTime"];
			}
			set
			{
				base.Fields["ReplayLagTime"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public EnhancedTimeSpan TruncationLagTime
		{
			get
			{
				return (EnhancedTimeSpan)base.Fields["TruncationLagTime"];
			}
			set
			{
				base.Fields["TruncationLagTime"] = value;
			}
		}

		protected override bool IsKnownException(Exception ex)
		{
			return ex is WmiException || ex is ArgumentException || AmExceptionHelper.IsKnownClusterException(this, ex) || ex is SeederServerException || ex is TaskServerException || base.IsKnownException(ex);
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				TaskLogger.LogExit();
				return;
			}
			TDataObject dataObject = this.DataObject;
			if (!dataObject.IsExchange2009OrLater)
			{
				TDataObject dataObject2 = this.DataObject;
				base.WriteError(new ErrorDatabaseWrongVersion(dataObject2.Name), ErrorCategory.InvalidOperation, this.Identity);
			}
			TDataObject dataObject3 = this.DataObject;
			if (dataObject3.EdbFilePath == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorEdbFilePathMissed(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TDataObject dataObject4 = this.DataObject;
			if (dataObject4.LogFolderPath == null)
			{
				base.WriteError(new InvalidOperationException(Strings.ErrorLogFolderPathMissed(this.Identity.ToString())), ErrorCategory.InvalidOperation, this.Identity);
			}
			TDataObject dataObject5 = this.DataObject;
			if (dataObject5.Recovery)
			{
				TDataObject dataObject6 = this.DataObject;
				Exception exception = new InvalidOperationException(Strings.ErrorInvalidOperationOnRecoveryMailboxDatabase(dataObject6.Name));
				ErrorCategory category = ErrorCategory.InvalidOperation;
				TDataObject dataObject7 = this.DataObject;
				base.WriteError(exception, category, dataObject7.Identity);
			}
			this.m_SeedingPostponedSpecified = (base.Fields["SeedingPostponed"] != null);
			if (!this.m_SeedingPostponedSpecified)
			{
				this.m_fSeeding = true;
			}
			else
			{
				this.m_fSeeding = !this.SeedingPostponed;
			}
			this.m_ConfigurationOnlySpecified = (base.Fields["ConfigurationOnly"] != null);
			if (!this.m_ConfigurationOnlySpecified)
			{
				this.m_fConfigOnly = false;
			}
			else
			{
				this.m_fConfigOnly = this.ConfigurationOnly;
				if (this.m_fConfigOnly)
				{
					this.m_fSeeding = false;
				}
			}
			if (base.Fields["ReplayLagTime"] != null)
			{
				this.WriteWarning(Strings.WarningReplayLagTimeMustBeLessThanSafetyNetHoldTime);
			}
			TaskLogger.LogExit();
		}

		protected void InitializeLagTimes(DatabaseCopy preExistingCopy)
		{
			if (base.Fields["ReplayLagTime"] == null)
			{
				if (preExistingCopy == null)
				{
					this.m_replayLagTime = EnhancedTimeSpan.Parse("00:00:00");
				}
				else
				{
					this.m_replayLagTime = preExistingCopy.ReplayLagTime;
				}
			}
			else
			{
				this.m_replayLagTime = this.ReplayLagTime;
			}
			if (base.Fields["TruncationLagTime"] != null)
			{
				this.m_truncationLagTime = this.TruncationLagTime;
				return;
			}
			if (preExistingCopy == null)
			{
				this.m_truncationLagTime = EnhancedTimeSpan.Parse("00:00:00");
				return;
			}
			this.m_truncationLagTime = preExistingCopy.TruncationLagTime;
		}

		protected void CreateTargetEdbDirectory()
		{
			TDataObject dataObject = this.DataObject;
			string pathName = dataObject.EdbFilePath.PathName;
			try
			{
				base.WriteVerbose(Strings.VerboseCheckFileExistenceCondition(this.m_server.Fqdn, pathName));
				bool flag = false;
				string directoryName = Path.GetDirectoryName(pathName);
				TDataObject dataObject2 = this.DataObject;
				if (string.Compare(dataObject2.LogFolderPath.PathName, directoryName, StringComparison.InvariantCultureIgnoreCase) != 0)
				{
					TDataObject dataObject3 = this.DataObject;
					if (string.Compare(dataObject3.SystemFolderPath.PathName, directoryName, StringComparison.InvariantCultureIgnoreCase) != 0)
					{
						goto IL_82;
					}
				}
				flag = true;
				IL_82:
				if (!flag)
				{
					SystemConfigurationTasksHelper.TryCreateDirectory(this.m_server.Fqdn, directoryName, Database_Directory.GetDomainWidePermissions(), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				}
			}
			catch (WmiException)
			{
				this.WriteWarning(Strings.FailedToGetCopyEdbFileStatus(this.m_server.Fqdn, pathName));
			}
		}

		protected void PerformSeedIfNecessary()
		{
			TDataObject dataObject = this.DataObject;
			IIdentityParameter id = new DatabaseIdParameter(dataObject.Id);
			IConfigDataProvider dataSession = base.DataSession;
			ObjectId rootId = this.RootId;
			TDataObject dataObject2 = this.DataObject;
			LocalizedString? notFoundError = new LocalizedString?(Strings.ErrorDatabaseNotFound(dataObject2.Name));
			TDataObject dataObject3 = this.DataObject;
			Database database = (Database)base.GetDataObject<Database>(id, dataSession, rootId, notFoundError, new LocalizedString?(Strings.ErrorDatabaseNotUnique(dataObject3.Name)));
			IADDatabaseAvailabilityGroup dag = null;
			if (this.m_server.DatabaseAvailabilityGroup != null)
			{
				DatabaseAvailabilityGroup dag2 = this.ConfigurationSession.Read<DatabaseAvailabilityGroup>(this.m_server.DatabaseAvailabilityGroup);
				dag = ADObjectWrapperFactory.CreateWrapper(dag2);
			}
			ReplayConfiguration config = RemoteReplayConfiguration.TaskGetReplayConfig(dag, ADObjectWrapperFactory.CreateWrapper(database), ADObjectWrapperFactory.CreateWrapper(this.m_server));
			if (this.m_fSeeding)
			{
				this.SeedDatabase(config);
			}
			this.SuspendDatabaseCopyIfNecessary(config);
		}

		internal void SuspendDatabaseCopyIfNecessary(ReplayConfiguration config)
		{
			string text = string.Empty;
			text = config.TargetMachine;
			if (!this.m_fConfigOnly && !WmiWrapper.IsFileExisting(text, config.DestinationEdbPath))
			{
				string fileName = string.Empty;
				fileName = Path.Combine(config.DestinationLogPath, EseHelper.MakeLogfileName(config.LogFilePrefix, "." + config.LogExtension, 1L));
				if (!WmiWrapper.IsFileExisting(SharedHelper.GetFqdnNameFromNode(config.SourceMachine), fileName))
				{
					try
					{
						this.WriteWarning(Strings.EnableDBCSuspendReplayNoDbComment(config.Name));
						ReplayRpcClientWrapper.RequestSuspend(text, config.IdentityGuid, Strings.EnableDBCSuspendReplayNoDbComment(config.Name));
						ReplayEventLogConstants.Tuple_DbSeedingRequired.LogEvent(null, new object[]
						{
							config.Name,
							text
						});
					}
					catch (TaskServerTransientException ex)
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug<TaskServerTransientException>((long)this.GetHashCode(), "SeedDatabase: Caught exception in RPC: {0}", ex);
						base.WriteError(new InvalidOperationException(Strings.SgcFailedToSuspendRpc(config.Name, ex.Message)), ErrorCategory.InvalidOperation, this.Identity);
					}
					catch (TaskServerException ex2)
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug<TaskServerException>((long)this.GetHashCode(), "SeedDatabase: Caught exception in RPC: {0}", ex2);
						if (!(ex2 is ReplayServiceSuspendWantedSetException))
						{
							if (ex2 is ReplayServiceSuspendRpcPartialSuccessCatalogFailedException)
							{
								base.WriteWarning(ex2.Message);
							}
							else
							{
								base.WriteError(new InvalidOperationException(Strings.SgcFailedToSuspendRpc(config.Name, ex2.Message)), ErrorCategory.InvalidOperation, this.Identity);
							}
						}
					}
				}
			}
		}

		internal void SeedDatabase(ReplayConfiguration config)
		{
			ReplayState replayState = config.ReplayState;
			if (config is RemoteReplayConfiguration)
			{
				string targetMachine = config.TargetMachine;
				try
				{
					string machineFqdn = targetMachine;
					string destinationLogPath = config.DestinationLogPath;
					string destinationEdbPath = config.DestinationEdbPath;
					TDataObject dataObject = this.DataObject;
					AddDatabaseCopyTaskBase<TDataObject>.CheckSeedingPath(machineFqdn, destinationLogPath, destinationEdbPath, dataObject.LogFilePrefix);
				}
				catch (SeedingPathWarningException ex)
				{
					if (this.m_SeedingPostponedSpecified)
					{
						base.WriteWarning(ex.Message);
					}
					return;
				}
				catch (SeedingPathErrorException exception)
				{
					base.WriteError(exception, ErrorCategory.InvalidOperation, this.Identity);
				}
				SystemConfigurationTasksHelper.TryCreateDirectory(this.m_server.Fqdn, config.DestinationLogPath, Database_Directory.GetDomainWidePermissions(), new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning));
				using (TaskSeeder taskSeeder = this.ConstructSeeder())
				{
					taskSeeder.SeedDatabase();
				}
				return;
			}
			throw new NotSupportedException(config.GetType() + " is not supported");
		}

		private TaskSeeder ConstructSeeder()
		{
			return new TaskSeeder(SeedingTask.AddMailboxDatabaseCopy, this.m_server, this.DataObject, null, new Task.TaskVerboseLoggingDelegate(base.WriteVerbose), new Task.TaskWarningLoggingDelegate(this.WriteWarning), new Task.TaskErrorLoggingDelegate(base.WriteError), new Task.TaskProgressLoggingDelegate(base.WriteProgress), new Task.TaskShouldContinueDelegate(base.ShouldContinue), () => base.Stopping);
		}

		public static bool CheckSeedingPath(string machineFqdn, string logFolderPath, string edbFilePath, string logPrefix)
		{
			LocalLongFullPath.Parse(logFolderPath);
			LocalLongFullPath.Parse(edbFilePath);
			string directoryName = Path.GetDirectoryName(edbFilePath);
			string[] array = new string[]
			{
				logFolderPath,
				directoryName
			};
			string[] array2 = new string[]
			{
				"LogFolderPath",
				"EdbFolderPath"
			};
			for (int i = 0; i < array.Length; i++)
			{
				if (WmiWrapper.IsFileExisting(machineFqdn, array[i]))
				{
					throw new SeedingPathErrorException(Strings.SeedingErrorDirectoryIsFile(array2[i], array[i]));
				}
			}
			string text = Path.Combine(logFolderPath, logPrefix + ".log");
			if (WmiWrapper.IsDirectoryExisting(machineFqdn, text))
			{
				throw new SeedingPathErrorException(Strings.SeedingErrorFileIsDirectory("CopyLogFile", text));
			}
			if (WmiWrapper.IsDirectoryExisting(machineFqdn, edbFilePath))
			{
				throw new SeedingPathErrorException(Strings.SeedingErrorFileIsDirectory("CopyEdbFilePath", edbFilePath));
			}
			if (WmiWrapper.IsFileExisting(machineFqdn, edbFilePath))
			{
				throw new SeedingPathWarningException(Strings.SeedingEdbFileExists(edbFilePath));
			}
			if (WmiWrapper.IsFileExisting(machineFqdn, text))
			{
				throw new SeedingPathWarningException(Strings.SeedingLogFileExists(text));
			}
			return true;
		}

		internal const string ReplayLagTimeName = "ReplayLagTime";

		internal const string TruncationLagTimeName = "TruncationLagTime";

		protected const string ParamNameSeedingPostponed = "SeedingPostponed";

		protected const string ParamNameConfigurationOnly = "ConfigurationOnly";

		protected const string DefaultReplayLagTimeStr = "00:00:00";

		protected const string DefaultTruncationLagTimeStr = "00:00:00";

		protected Server m_server;

		protected Database[] m_ownerServerDatabases;

		protected string m_progressMessage;

		protected string m_taskName;

		protected bool m_fConfigOnly;

		protected bool m_fSeeding;

		protected bool m_ConfigurationOnlySpecified;

		protected bool m_SeedingPostponedSpecified;

		protected EnhancedTimeSpan m_replayLagTime;

		protected EnhancedTimeSpan m_truncationLagTime;
	}
}
