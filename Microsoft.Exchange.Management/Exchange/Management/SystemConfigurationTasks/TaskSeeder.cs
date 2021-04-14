using System;
using System.Management.Automation;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class TaskSeeder : IDisposable
	{
		public bool Force { get; set; }

		public bool DeleteExistingFiles { get; set; }

		public bool SafeDeleteExistingFiles { get; set; }

		public bool AutoSuspend { get; set; }

		public bool ManualResume { get; set; }

		public bool SeedDatabaseFiles { get; set; }

		public bool SeedCiFiles { get; set; }

		public DatabaseAvailabilityGroupNetworkIdParameter NetworkId { get; set; }

		public bool? CompressOverride { get; set; }

		public bool? EncryptOverride { get; set; }

		public bool BeginSeed { get; set; }

		public TaskSeeder(SeedingTask seedingTask, Server server, Database database, Server sourceServer, Task.TaskVerboseLoggingDelegate writeVerbose, Task.TaskWarningLoggingDelegate writeWarning, Task.TaskErrorLoggingDelegate writeError, Task.TaskProgressLoggingDelegate writeProgress, Task.TaskShouldContinueDelegate shouldContinue, TaskSeeder.TaskIsStoppingDelegate stopping)
		{
			this.m_seedingTask = seedingTask;
			this.m_server = server;
			this.m_dbGuid = database.Guid;
			this.m_dbName = database.Name;
			this.m_isPublicDB = database.IsPublicFolderDatabase;
			this.m_sourceServer = sourceServer;
			this.m_writeVerbose = writeVerbose;
			this.m_writeWarning = writeWarning;
			this.m_writeError = writeError;
			this.m_writeProgress = writeProgress;
			this.m_shouldContinue = shouldContinue;
			this.m_stopping = stopping;
			this.m_taskName = this.GetTaskNameFromSeedingEnum(this.m_seedingTask);
			this.InitializeDefaultParameters();
			this.m_seeder = SeederClient.Create(this.m_server, this.m_dbName, this.m_sourceServer);
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public void Dispose(bool disposing)
		{
			if (!this.m_fDisposed)
			{
				if (disposing && this.m_seeder != null)
				{
					this.m_seeder.Dispose();
					this.m_seeder = null;
				}
				this.m_fDisposed = true;
			}
		}

		public void SeedDatabase()
		{
			bool flag = true;
			try
			{
				bool flag2 = this.TryPrepareDbSeedAndBegin(false);
				if (flag2)
				{
					this.PerformSeedCleanup();
					flag = !this.TryPrepareDbSeedAndBegin(true);
				}
				flag = (!this.BeginSeed && flag);
				if (flag)
				{
					SeedProgressReporter seedProgressReporter = new SeedProgressReporter(this.m_dbGuid, this.m_dbName, this.m_seeder, this.m_writeError, this.m_writeWarning, new SeedProgressReporter.ProgressReportDelegate(this.ProgressReportDatabaseSeeding), new SeedProgressReporter.ProgressCompletedDelegate(this.ProgressComplete), new SeedProgressReporter.ProgressFailedDelegate(this.ProgressFailed));
					seedProgressReporter.Start();
					try
					{
						seedProgressReporter.MonitorProgress();
					}
					catch (PipelineStoppedException arg)
					{
						ExTraceGlobals.CmdletsTracer.TraceDebug<string, PipelineStoppedException>((long)this.GetHashCode(), "{0} task caught PipelineStoppedException: {1}.", this.m_taskName, arg);
					}
					finally
					{
						seedProgressReporter.Stop();
					}
				}
			}
			finally
			{
				if (this.m_seedingTask == SeedingTask.UpdateDatabaseCopy && this.ManualResume && flag)
				{
					ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: skipped to AutoResume {1} because -ManualResume is specified or it failed to seed.", this.m_taskName, this.m_dbName);
					if (!this.m_stopping())
					{
						this.m_writeWarning(Strings.WarningForStillSuspended(this.m_dbName, this.m_server.Name));
					}
				}
			}
		}

		public void CancelSeed()
		{
			ExTraceGlobals.CmdletsTracer.TraceDebug<string>((long)this.GetHashCode(), "{0}: CancelSeed() called", this.m_taskName);
			this.m_writeVerbose(Strings.SeederCancelDbSeedRpcBegin(this.m_taskName, this.m_server.Name, this.m_dbName));
			try
			{
				this.m_seeder.CancelDbSeed(this.m_dbGuid);
			}
			catch (SeederServerTransientException ex)
			{
				this.m_writeError(new SeederCancelDbSeedRpcFailedException(this.m_dbName, this.m_server.Name, ex.Message, ex), ErrorCategory.InvalidOperation, null);
			}
			catch (SeederServerException ex2)
			{
				this.m_writeError(new SeederCancelDbSeedRpcFailedException(this.m_dbName, this.m_server.Name, ex2.Message, ex2), ErrorCategory.InvalidOperation, null);
			}
		}

		private bool TryPrepareDbSeedAndBegin(bool fFailOnError)
		{
			bool result = false;
			try
			{
				this.m_writeVerbose(Strings.SeederPrepareDbSeedRpcBegin(this.m_taskName, this.m_server.Name));
				string text = string.Empty;
				if (this.NetworkId != null)
				{
					if (this.NetworkId.NetName != null)
					{
						text = this.NetworkId.NetName;
					}
					else
					{
						text = this.NetworkId.ToString();
					}
					ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "Seeding over '{0}'. Full name provided was '{1}'", text, this.NetworkId.ToString());
				}
				this.m_seeder.PrepareDbSeedAndBegin(this.m_dbGuid, this.DeleteExistingFiles, this.SafeDeleteExistingFiles, this.AutoSuspend, this.ManualResume, this.SeedDatabaseFiles, this.SeedCiFiles, text, null, (this.m_sourceServer == null) ? string.Empty : this.m_sourceServer.Fqdn, this.CompressOverride, this.EncryptOverride, SeederRpcFlags.None);
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: succeeded to begin seeding {1}", this.m_taskName, this.m_dbName);
			}
			catch (SeederInstanceAlreadyInProgressException ex)
			{
				result = this.PromptOrWriteError(ex, fFailOnError, Strings.SeederAlreadyInProgressPrompt(this.m_dbName, this.m_server.Name, ex.SourceMachine));
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: found another seed instance in progress for {1}. Reporting on that instance now.", this.m_taskName, this.m_dbName);
			}
			catch (SeederInstanceAlreadyCompletedException ex2)
			{
				result = this.PromptOrWriteError(ex2, fFailOnError, Strings.SeederAlreadyCompletedPrompt(this.m_dbName, this.m_server.Name, ex2.SourceMachine));
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: found another completed seed instance for {1}. Now attempting to cleanup that instance.", this.m_taskName, this.m_dbName);
			}
			catch (SeederInstanceAlreadyFailedException ex3)
			{
				result = this.PromptOrWriteError(ex3, fFailOnError, Strings.SeederAlreadyFailedPrompt(this.m_dbName, this.m_server.Name, ex3.SourceMachine));
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: found another failed seed instance for {1}. Now attempting to cleanup that instance.", this.m_taskName, this.m_dbName);
			}
			catch (SeederInstanceAlreadyCancelledException ex4)
			{
				result = this.PromptOrWriteError(ex4, fFailOnError, Strings.SeederAlreadyCancelledPrompt(this.m_dbName, this.m_server.Name, ex4.SourceMachine));
				ExTraceGlobals.CmdletsTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: found another cancelled seed instance for {1}. Now attempting to cleanup that instance.", this.m_taskName, this.m_dbName);
			}
			catch (SeederServerException ex5)
			{
				this.ProgressFailed();
				ExTraceGlobals.CmdletsTracer.TraceError<string, string, SeederServerException>((long)this.GetHashCode(), "{0}: failed to begin seeding {1}, reason: {2}", this.m_taskName, this.m_dbName, ex5);
				this.m_writeError(ex5, ErrorCategory.InvalidOperation, this.m_dbName);
			}
			catch (SeederServerTransientException ex6)
			{
				this.ProgressFailed();
				ExTraceGlobals.CmdletsTracer.TraceError<string, string, SeederServerTransientException>((long)this.GetHashCode(), "{0}: failed to begin seeding {1}, reason: {2}", this.m_taskName, this.m_dbName, ex6);
				this.m_writeError(ex6, ErrorCategory.InvalidOperation, this.m_dbName);
			}
			return result;
		}

		private bool PromptOrWriteError(SeederServerException ex, bool fFailOnError, LocalizedString promptMessage)
		{
			bool result = true;
			ExTraceGlobals.CmdletsTracer.TraceError<string, string, SeederServerException>((long)this.GetHashCode(), "{0}: failed to begin seeding {1}, reason: {2}", this.m_taskName, this.m_dbName, ex);
			if (fFailOnError || (!this.Force && !this.m_shouldContinue(promptMessage)))
			{
				if (fFailOnError)
				{
					result = false;
				}
				this.m_writeError(ex, ErrorCategory.InvalidOperation, null);
				return result;
			}
			if (ex is SeederInstanceAlreadyInProgressException)
			{
				result = false;
			}
			return result;
		}

		private void ProgressFailed()
		{
			if (this.m_progressString != null)
			{
				this.m_writeProgress(Strings.ProgressStatusFailed, new LocalizedString(this.m_progressString), 100);
			}
		}

		private void ProgressComplete()
		{
			if (this.m_progressString != null)
			{
				this.m_writeProgress(Strings.ProgressStatusCompleted, new LocalizedString(this.m_progressString), 100);
			}
		}

		private void ProgressReportDatabaseSeeding(string edbName, string addressForData, int percent, long bytesRead, long bytesWritten, long bytesRemaining, string databaseName)
		{
			if (!string.IsNullOrEmpty(addressForData))
			{
				this.m_progressString = Strings.SeederProgressMsgOverSpecifiedNetwork(edbName, this.m_server.Name, addressForData, bytesRead / 1024L, bytesWritten / 1024L, bytesRemaining / 1024L, databaseName);
			}
			else
			{
				this.m_progressString = Strings.SeederProgressMsgNoNetwork(edbName, this.m_server.Name, bytesRead / 1024L, bytesWritten / 1024L, bytesRemaining / 1024L, databaseName);
			}
			this.m_writeProgress(Strings.ProgressStatusInProgress, new LocalizedString(this.m_progressString), percent);
		}

		private void PerformSeedCleanup()
		{
			if (this.m_seeder != null)
			{
				try
				{
					this.m_writeVerbose(Strings.SeederEndDbSeedRpcBegin(this.m_taskName, this.m_server.Name, this.m_dbName));
					this.m_seeder.EndDbSeed(this.m_dbGuid);
				}
				catch (SeederServerTransientException ex)
				{
					ExTraceGlobals.CmdletsTracer.TraceError<string, SeederServerTransientException>((long)this.GetHashCode(), "{0}: TrySeedCleanup: Exception caught during Cleanup RPC: {1}", this.m_taskName, ex);
					this.m_writeError(new SeederEndDbSeedRpcFailedException(this.m_dbName, this.m_server.Name, ex.Message, ex), ErrorCategory.InvalidOperation, null);
				}
				catch (SeederServerException ex2)
				{
					ExTraceGlobals.CmdletsTracer.TraceError<string, SeederServerException>((long)this.GetHashCode(), "{0}: TrySeedCleanup: Exception caught during Cleanup RPC: {1}", this.m_taskName, ex2);
					this.m_writeError(new SeederEndDbSeedRpcFailedException(this.m_dbName, this.m_server.Name, ex2.Message, ex2), ErrorCategory.InvalidOperation, null);
				}
			}
		}

		private string GetTaskNameFromSeedingEnum(SeedingTask seedingTask)
		{
			string result = string.Empty;
			switch (seedingTask)
			{
			case SeedingTask.AddMailboxDatabaseCopy:
				result = "Add-MailboxDatabaseCopy";
				break;
			case SeedingTask.UpdateDatabaseCopy:
				result = "Update-DatabaseCopy";
				break;
			default:
				DiagCore.RetailAssert(false, "Unhandled case for SeedingTask '{0}'", new object[]
				{
					seedingTask
				});
				break;
			}
			return result;
		}

		private void InitializeDefaultParameters()
		{
			switch (this.m_seedingTask)
			{
			case SeedingTask.AddMailboxDatabaseCopy:
				this.DeleteExistingFiles = true;
				this.SafeDeleteExistingFiles = false;
				this.AutoSuspend = true;
				this.ManualResume = false;
				this.SeedDatabaseFiles = true;
				this.SeedCiFiles = !this.m_isPublicDB;
				this.Force = true;
				return;
			case SeedingTask.UpdateDatabaseCopy:
				this.AutoSuspend = false;
				return;
			default:
				return;
			}
		}

		private readonly string m_taskName;

		private SeedingTask m_seedingTask;

		private Server m_server;

		private readonly Guid m_dbGuid;

		private readonly string m_dbName;

		private Server m_sourceServer;

		private Task.TaskVerboseLoggingDelegate m_writeVerbose;

		private Task.TaskWarningLoggingDelegate m_writeWarning;

		private Task.TaskErrorLoggingDelegate m_writeError;

		private Task.TaskProgressLoggingDelegate m_writeProgress;

		private Task.TaskShouldContinueDelegate m_shouldContinue;

		private TaskSeeder.TaskIsStoppingDelegate m_stopping;

		private readonly bool m_isPublicDB;

		private string m_progressString;

		private bool m_fDisposed;

		private SeederClient m_seeder;

		public delegate bool TaskIsStoppingDelegate();
	}
}
