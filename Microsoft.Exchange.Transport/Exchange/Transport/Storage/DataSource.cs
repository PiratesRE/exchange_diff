using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Transport;
using Microsoft.Isam.Esent;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Isam.Esent.Interop.Windows8;
using Microsoft.Office.Datacenter.ActiveMonitoring;

namespace Microsoft.Exchange.Transport.Storage
{
	internal class DataSource
	{
		public DataSource(string instanceName, string instancePath, string databaseName, int connectionLimit, string performanceCounter, string logFilePath, DatabaseAutoRecovery databaseAutoRecovery)
		{
			this.instanceName = instanceName;
			this.instancePath = instancePath;
			this.databasePath = Path.Combine(instancePath, databaseName);
			this.connectionLimitPoint = connectionLimit;
			this.databaseAutoRecovery = databaseAutoRecovery;
			if (!string.IsNullOrEmpty(logFilePath))
			{
				logFilePath = logFilePath.Trim();
			}
			if (!string.IsNullOrEmpty(logFilePath) && !logFilePath.EndsWith("\\"))
			{
				logFilePath += "\\";
			}
			this.logFilePath = logFilePath;
			if (string.IsNullOrEmpty(performanceCounter))
			{
				performanceCounter = "other";
			}
			this.perfCounters = DatabasePerfCounters.GetInstance(performanceCounter);
			this.references = 1;
		}

		public static ExEventLog EventLogger
		{
			get
			{
				return DataSource.eventLogger;
			}
		}

		public static uint ConfigMaxCacheSizeInPages
		{
			get
			{
				return DataSource.configMaxCacheSizeInPages;
			}
		}

		public static uint ConfigMinCacheSizeInPages
		{
			get
			{
				return DataSource.configMinCacheSizeInPages;
			}
		}

		public static uint CurrentMaxCacheSizeInPages
		{
			get
			{
				return DataSource.currentMaxCacheSizeInPages;
			}
		}

		public string DatabasePath
		{
			get
			{
				return this.databasePath;
			}
		}

		public string InstanceName
		{
			get
			{
				return this.instanceName;
			}
		}

		public string LogFilePath
		{
			get
			{
				return this.logFilePath;
			}
		}

		public bool NewDatabase
		{
			get
			{
				return this.newDatabase;
			}
		}

		public bool CleanupRequestInProgress
		{
			get
			{
				return this.cleanupRequestInProgress;
			}
			set
			{
				this.cleanupRequestInProgress = value;
			}
		}

		public bool IsAboveLimit
		{
			get
			{
				return this.currentConnectionCount > this.connectionLimitPoint;
			}
		}

		public int ConnectionLimitPoint
		{
			get
			{
				return this.connectionLimitPoint;
			}
		}

		public uint LogFileSize
		{
			get
			{
				return this.logFileSize * 1024U;
			}
			set
			{
				this.logFileSize = value / 1024U;
			}
		}

		public uint LogBuffers
		{
			get
			{
				return this.logBuffers * 512U;
			}
			set
			{
				this.logBuffers = value / 512U;
			}
		}

		public uint MaxBackgroundCleanupTasks
		{
			get
			{
				return this.maxBackgroundCleanupTasks;
			}
			set
			{
				this.maxBackgroundCleanupTasks = value;
			}
		}

		public DatabasePerfCountersInstance PerfCounters
		{
			get
			{
				return this.perfCounters;
			}
		}

		public uint ExtensionSize
		{
			get
			{
				return this.extensionSize;
			}
			set
			{
				this.extensionSize = value;
			}
		}

		public static bool HandleIsamException(EsentErrorException ex, DataSource source)
		{
			bool result;
			lock (DataSource.exceptionHandlerLock)
			{
				ExTraceGlobals.ExpoTracer.TraceError<EsentErrorException>(0L, "Error occurred during Jet operation : {0}", ex);
				EsentOperationException ex2 = ex as EsentOperationException;
				if (ex2 != null)
				{
					result = DataSource.HandleIsamOperationException(ex2, source);
				}
				else
				{
					EsentDataException ex3 = ex as EsentDataException;
					if (ex3 != null)
					{
						result = DataSource.HandleIsamDataException(ex3, source);
					}
					else
					{
						EsentApiException ex4 = ex as EsentApiException;
						if (ex4 != null)
						{
							result = DataSource.HandleIsamApiException(ex4, source);
						}
						else
						{
							result = false;
						}
					}
				}
			}
			return result;
		}

		public ulong GetAvailableFreeSpace()
		{
			if (Monitor.TryEnter(this.syncRoot))
			{
				try
				{
					if (!this.closed)
					{
						return this.GetAvailableFreeSpaceHelper();
					}
				}
				finally
				{
					Monitor.Exit(this.syncRoot);
				}
			}
			return this.lastAvailableFreeSpace;
		}

		public DataConnection DemandNewConnection()
		{
			return this.NewConnection(true);
		}

		public DataConnection TryNewConnection()
		{
			return this.NewConnection(false);
		}

		public void OpenDatabase()
		{
			if (!this.closed)
			{
				throw new InvalidOperationException(Strings.DatabaseOpen);
			}
			DataSource.InitGlobal();
			this.InitInstance();
			try
			{
				if (File.Exists(this.DatabasePath))
				{
					this.newDatabase = false;
					Api.JetAttachDatabase(this.baseSession, this.DatabasePath, AttachDatabaseGrbit.None);
					Api.JetOpenDatabase(this.baseSession, this.DatabasePath, null, out this.baseDatabase, OpenDatabaseGrbit.None);
				}
				else
				{
					this.newDatabase = true;
					Api.JetCreateDatabase(this.baseSession, this.DatabasePath, null, out this.baseDatabase, CreateDatabaseGrbit.None);
				}
				this.closed = false;
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this))
				{
					throw;
				}
			}
		}

		public void CloseDatabase(bool force = false)
		{
			if (this.closed)
			{
				return;
			}
			lock (this.cleanupThreadLock)
			{
				if (!force && Interlocked.Decrement(ref this.references) != 0)
				{
					throw new InvalidOperationException(Strings.DatabaseStillInUse);
				}
				lock (this.syncRoot)
				{
					this.StopBackgroundDefrag();
					try
					{
						Api.JetCloseDatabase(this.baseSession, this.baseDatabase, CloseDatabaseGrbit.None);
						Api.JetEndSession(this.baseSession, EndSessionGrbit.None);
						Api.JetTerm(this.instance);
						this.durableCommitCallback.End();
						this.closed = true;
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, this))
						{
							throw;
						}
					}
				}
			}
		}

		public void StartBackgroundDefrag(int secondsToRun, JET_CALLBACK endOnlineDefragCallback)
		{
			int num = 1;
			lock (this.syncRoot)
			{
				if (!this.closed)
				{
					try
					{
						Api.JetDefragment2(this.baseSession, this.baseDatabase, null, ref num, ref secondsToRun, endOnlineDefragCallback, DefragGrbit.BatchStart);
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, this))
						{
							throw;
						}
					}
				}
			}
		}

		public void RegisterAsyncCommitCallback(JET_COMMIT_ID commitId, Action callback)
		{
			this.lastCommitLock.EnterReadLock();
			if (commitId <= this.lastDurableCommitId)
			{
				this.lastCommitLock.ExitReadLock();
				callback();
				return;
			}
			this.pendingCommits.TryAdd(commitId, callback);
			this.lastCommitLock.ExitReadLock();
		}

		public string TryForceFlush()
		{
			try
			{
				lock (this.syncRoot)
				{
					Api.JetCommitTransaction(this.baseSession, CommitTransactionGrbit.WaitLastLevel0Commit);
				}
			}
			catch (EsentException ex)
			{
				return ex.ToString();
			}
			return null;
		}

		public long GetCurrentVersionBuckets()
		{
			lock (this.syncRoot)
			{
				if (!this.closed)
				{
					try
					{
						long result = 0L;
						UnpublishedApi.JetGetResourceParam(this.instance, JET_resoper.CurrentUse, JET_resid.VERBUCKET, out result);
						return result;
					}
					catch (EsentErrorException ex)
					{
						if (!DataSource.HandleIsamException(ex, this))
						{
							throw;
						}
					}
				}
			}
			return 0L;
		}

		internal static void InitGlobal()
		{
			if (DataSource.globalInitDone)
			{
				return;
			}
			lock (DataSource.globalInitLock)
			{
				if (!DataSource.globalInitDone)
				{
					DataSource.pageSize = (uint)TransportAppConfig.JetDatabaseConfig.PageSize.ToBytes();
					TransportAppConfig.JetDatabaseConfig jetDatabase = Components.TransportAppConfig.JetDatabase;
					DataSource.SetGlobalSystemParameter((JET_param)129, 1);
					string value = "reg:HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\Ese";
					try
					{
						DataSource.SetGlobalSystemParameter((JET_param)189, value);
					}
					catch (EsentErrorException)
					{
					}
					DataSource.SetGlobalSystemParameter(JET_param.DatabasePageSize, DataSource.pageSize);
					DataSource.configMinCacheSizeInPages = (uint)((long)((int)jetDatabase.MinCacheSize.ToBytes()) / (long)((ulong)DataSource.pageSize));
					DataSource.SetGlobalSystemParameter(JET_param.CacheSizeMin, DataSource.configMinCacheSizeInPages);
					DataSource.configMaxCacheSizeInPages = (uint)((long)((int)jetDatabase.MaxCacheSize.ToBytes()) / (long)((ulong)DataSource.pageSize));
					DataSource.currentMaxCacheSizeInPages = 0U;
					DataSource.SetCurrentMaxCacheSize(DataSource.configMaxCacheSizeInPages);
					DataSource.SetGlobalSystemParameter(JET_param.CheckpointDepthMax, (int)jetDatabase.CheckpointDepthMax.ToBytes());
					DataSource.globalInitDone = true;
				}
			}
		}

		internal int AddRef()
		{
			return Interlocked.Increment(ref this.references);
		}

		internal int Release()
		{
			int num = Interlocked.Decrement(ref this.references);
			if (num == 0)
			{
				this.CloseDatabase(true);
			}
			return num;
		}

		internal void TrackTryConnectionClose()
		{
			if (this.closed)
			{
				throw new InvalidOperationException(Strings.DatabaseClosed);
			}
			Interlocked.Decrement(ref this.currentConnectionCount);
			this.perfCounters.CurrentConnections.Decrement();
		}

		public Transaction BeginNewTransaction()
		{
			DataConnection dataConnection = this.DemandNewConnection();
			Transaction result = dataConnection.BeginTransaction();
			dataConnection.Release();
			return result;
		}

		internal void OnDataCleanup(object obj)
		{
			if (!Monitor.TryEnter(this.cleanupThreadLock))
			{
				return;
			}
			DataConnection dataConnection = null;
			try
			{
				if (!this.closed)
				{
					dataConnection = this.DemandNewConnection();
					try
					{
						Api.JetIdle(dataConnection.Session, IdleGrbit.None);
					}
					catch (EsentErrorException)
					{
					}
				}
				if (!this.closed)
				{
					try
					{
						Api.JetIdle(dataConnection.Session, IdleGrbit.None);
					}
					catch (EsentErrorException)
					{
					}
				}
			}
			finally
			{
				if (dataConnection != null)
				{
					dataConnection.Release();
				}
				Monitor.Exit(this.cleanupThreadLock);
				this.CleanupRequestInProgress = false;
			}
		}

		internal void HandleSchemaException(SchemaException schemaException)
		{
			if (!this.TakeErrorAction(DataSource.DatabaseErrorAction.DeemPermanent, DataSource.ProcessErrorAction.Restart, schemaException))
			{
				throw schemaException;
			}
		}

		internal bool IsDatabaseDriveAccessible()
		{
			DriveInfo driveInfo = new DriveInfo(Path.GetFullPath(this.instancePath));
			return driveInfo.IsReady;
		}

		private static void SetCurrentMaxCacheSize(uint maxCacheSizeInPages)
		{
			if (maxCacheSizeInPages < DataSource.configMinCacheSizeInPages || maxCacheSizeInPages > DataSource.configMaxCacheSizeInPages)
			{
				throw new ArgumentOutOfRangeException("maxCacheSizeInPages");
			}
			if (DataSource.currentMaxCacheSizeInPages != maxCacheSizeInPages)
			{
				TransportAppConfig.JetDatabaseConfig jetDatabase = Components.TransportAppConfig.JetDatabase;
				DataSource.currentMaxCacheSizeInPages = maxCacheSizeInPages;
				DataSource.SetGlobalSystemParameter(JET_param.CacheSizeMax, (int)DataSource.currentMaxCacheSizeInPages);
				uint value = DataSource.currentMaxCacheSizeInPages * jetDatabase.StartFlushThreshold / 100U;
				DataSource.SetGlobalSystemParameter(JET_param.StartFlushThreshold, value);
				value = DataSource.currentMaxCacheSizeInPages * jetDatabase.StopFlushThreshold / 100U;
				DataSource.SetGlobalSystemParameter(JET_param.StopFlushThreshold, value);
			}
		}

		private static bool HandleInitIsamException(EsentErrorException ex, JET_INSTANCE instance, string instanceName, DataSource source)
		{
			lock (DataSource.exceptionHandlerLock)
			{
				ExTraceGlobals.ExpoTracer.TraceError<EsentErrorException>(0L, "Error occurred during Jet initialization operation : {0}", ex);
				if (ex is EsentFileAccessDeniedException)
				{
					Api.JetTerm(instance);
					return false;
				}
				if (ex is EsentInstanceNameInUseException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetInstanceNameInUse, null, new object[]
					{
						instanceName,
						ex
					});
					return source.TakeErrorAction(DataSource.DatabaseErrorAction.DeemTransient, DataSource.ProcessErrorAction.Restart, ex);
				}
			}
			return DataSource.HandleIsamException(ex, source);
		}

		private static bool HandleIsamOperationException(EsentOperationException ex, DataSource source)
		{
			DataSource.DatabaseErrorAction databaseErrorAction;
			DataSource.ProcessErrorAction processErrorAction;
			if (ex is EsentResourceException)
			{
				if (ex is EsentDiskException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetOutOfSpaceError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					string notificationReason = string.Format("{0}: An operation has encountered a fatal error. There wasn't enough disk space to complete the operation. The exception is {1}.", source.InstanceName, ex);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Error, false);
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
					processErrorAction = DataSource.ProcessErrorAction.Stop;
				}
				else if (ex is EsentOutOfMemoryException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetInitInstanceOutOfMemory, null, new object[]
					{
						source.InstanceName,
						ex
					});
					databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
					processErrorAction = DataSource.ProcessErrorAction.Restart;
				}
				else if (ex is EsentQuotaException)
				{
					if (ex is EsentVersionStoreOutOfMemoryException)
					{
						DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetVersionStoreOutOfMemoryError, null, new object[]
						{
							source.InstanceName,
							ex
						});
						databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
						processErrorAction = DataSource.ProcessErrorAction.Rethrow;
					}
					else
					{
						DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetQuotaExceededError, null, new object[]
						{
							source.InstanceName,
							ex
						});
						databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
						processErrorAction = DataSource.ProcessErrorAction.Restart;
					}
				}
				else
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetInsufficientResourcesError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
					processErrorAction = DataSource.ProcessErrorAction.Restart;
				}
			}
			else if (ex is EsentIOException)
			{
				if (ex is EsentDiskIOException && !source.IsDatabaseDriveAccessible())
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseDriveIsNotAccessible, null, new object[]
					{
						source.InstanceName,
						ex
					});
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				}
				else
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetIOError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
				}
				processErrorAction = DataSource.ProcessErrorAction.Restart;
			}
			else
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetOperationError, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
				processErrorAction = DataSource.ProcessErrorAction.Restart;
			}
			return source.TakeErrorAction(databaseErrorAction, processErrorAction, ex);
		}

		private static bool HandleIsamDataException(EsentDataException ex, DataSource source)
		{
			DataSource.DatabaseErrorAction databaseErrorAction;
			DataSource.ProcessErrorAction processErrorAction;
			if (ex is EsentFragmentationException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetFragmentationError, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
				processErrorAction = DataSource.ProcessErrorAction.Restart;
			}
			else
			{
				if (ex is EsentInconsistentException)
				{
					ExTraceGlobals.ExpoTracer.TraceError<EsentDataException>(0L, "EsentInconsistentException caught : {0}", ex);
				}
				if (ex is EsentDatabaseDirtyShutdownException || ex is EsentMissingLogFileException || ex is EsentMissingPreviousLogFileException || ex is EsentRequiredLogFilesMissingException || ex is EsentBadLogVersionException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetLogFileError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					string notificationReason = string.Format("{0}: The database could not be opened because a log file is missing or corrupted. Manual database recovery or repair may be required. The exception is {1}.", source.InstanceName, ex);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Error, false);
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemPermanent;
					processErrorAction = DataSource.ProcessErrorAction.Restart;
				}
				else if (ex is EsentBadCheckpointSignatureException || ex is EsentCheckpointFileNotFoundException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetCheckpointFileError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					string notificationReason2 = string.Format("{0}: The database could not be opened because the checkpoint file (.chk) is missing or corrupted. The exception is {1}.", source.InstanceName, ex);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason2, ResultSeverityLevel.Error, false);
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemPermanent;
					processErrorAction = DataSource.ProcessErrorAction.Restart;
				}
				else if (ex is EsentAttachedDatabaseMismatchException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetMismatchError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					string notificationReason3 = string.Format("{0}: The database could not be opened because the database file does not match the log files. The exception is {1}.", source.InstanceName, ex);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason3, ResultSeverityLevel.Error, false);
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
					processErrorAction = DataSource.ProcessErrorAction.Stop;
				}
				else if (ex is EsentDatabaseLogSetMismatchException)
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetDatabaseLogSetMismatch, null, new object[]
					{
						source.InstanceName,
						ex
					});
					databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
					processErrorAction = DataSource.ProcessErrorAction.Stop;
				}
				else
				{
					DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetCorruptionError, null, new object[]
					{
						source.InstanceName,
						ex
					});
					string notificationReason4 = string.Format("{0}: An operation has encountered a fatal error. The database may be corrupted. Exception details: {1}", source.InstanceName, ex);
					EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason4, ResultSeverityLevel.Error, false);
					databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
					processErrorAction = DataSource.ProcessErrorAction.Restart;
				}
			}
			return source.TakeErrorAction(databaseErrorAction, processErrorAction, ex);
		}

		private static bool HandleIsamApiException(EsentApiException ex, DataSource source)
		{
			DataSource.DatabaseErrorAction databaseErrorAction;
			DataSource.ProcessErrorAction processErrorAction;
			if (ex is EsentInvalidPathException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetPathError, null, new object[]
				{
					source.InstanceName,
					ex
				});
				string notificationReason = string.Format("{0}: The database could not be opened because the log file path that was supplied is invalid. The Microsoft Exchange Transport service is shutting down. The exception is {1}.", source.InstanceName, ex);
				EventNotificationItem.Publish(ExchangeComponent.Transport.Name, "TransportServiceStartError", null, notificationReason, ResultSeverityLevel.Error, false);
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Stop;
			}
			else if (ex is EsentDatabaseNotFoundException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetDatabaseNotFound, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Stop;
			}
			else if (ex is EsentObjectNotFoundException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetTableNotFound, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
				processErrorAction = DataSource.ProcessErrorAction.Rethrow;
			}
			else if (ex is EsentFileNotFoundException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetFileNotFound, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Stop;
			}
			else if (ex is EsentDatabaseFileReadOnlyException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_JetFileReadOnly, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Stop;
			}
			else if (ex is EsentColumnTooBigException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_ColumnTooBigException, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Rethrow;
			}
			else if (ex is EsentTableLockedException)
			{
				DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_TableLockedException, null, new object[]
				{
					source.InstanceName,
					ex
				});
				databaseErrorAction = DataSource.DatabaseErrorAction.DeemTransient;
				processErrorAction = DataSource.ProcessErrorAction.Rethrow;
			}
			else
			{
				databaseErrorAction = DataSource.DatabaseErrorAction.SuspectTransient;
				processErrorAction = DataSource.ProcessErrorAction.Rethrow;
			}
			return source.TakeErrorAction(databaseErrorAction, processErrorAction, ex);
		}

		private static void SetGlobalSystemParameter(JET_param parameter, int value)
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, parameter, value, null);
		}

		private static void SetGlobalSystemParameter(JET_param parameter, uint value)
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, parameter, (int)value, null);
		}

		private static void SetGlobalSystemParameter(JET_param parameter, string value)
		{
			Api.JetSetSystemParameter(JET_INSTANCE.Nil, JET_SESID.Nil, parameter, 0, value);
		}

		private bool TakeErrorAction(DataSource.DatabaseErrorAction databaseErrorAction, DataSource.ProcessErrorAction processErrorAction, Exception exception)
		{
			bool flag = false;
			DataSource.EventLogger.LogEvent(TransportEventLogConstants.Tuple_DatabaseErrorDetected, null, new object[]
			{
				this.instanceName,
				databaseErrorAction,
				processErrorAction,
				exception
			});
			if (this.databaseAutoRecovery != null)
			{
				switch (databaseErrorAction)
				{
				case DataSource.DatabaseErrorAction.DeemTransient:
					flag = true;
					break;
				case DataSource.DatabaseErrorAction.SuspectTransient:
					flag = this.databaseAutoRecovery.IncrementDatabaseCorruptionCount();
					break;
				case DataSource.DatabaseErrorAction.DeemPermanent:
					flag = this.databaseAutoRecovery.SetDatabaseCorruptionFlag();
					break;
				}
			}
			switch (processErrorAction)
			{
			case DataSource.ProcessErrorAction.Rethrow:
				return false;
			case DataSource.ProcessErrorAction.Restart:
				Components.StopService(Strings.JetOperationFailure, flag || databaseErrorAction != DataSource.DatabaseErrorAction.DeemPermanent, false, false);
				return true;
			case DataSource.ProcessErrorAction.Stop:
				Components.StopService(Strings.JetOperationFailure, false, false, false);
				return true;
			default:
				return false;
			}
		}

		private JET_err FlushCallback(JET_INSTANCE firingInstance, JET_COMMIT_ID commitId, DurableCommitCallbackGrbit grbit)
		{
			this.lastCommitLock.EnterUpgradeableReadLock();
			if (commitId > this.lastDurableCommitId)
			{
				this.lastCommitLock.EnterWriteLock();
				this.lastDurableCommitId = commitId;
				this.lastCommitLock.ExitWriteLock();
				this.lastCommitLock.ExitUpgradeableReadLock();
				ThreadPool.QueueUserWorkItem(delegate(object param0)
				{
					this.DispatchPendingCommits();
				});
			}
			else
			{
				this.lastCommitLock.ExitUpgradeableReadLock();
			}
			Transaction.PerfCounters.TransactionDurableCallbackCount.Increment();
			return JET_err.Success;
		}

		private void DispatchPendingCommits()
		{
			Parallel.ForEach<JET_COMMIT_ID>(from transactionId in this.pendingCommits.Keys
			where transactionId <= this.lastDurableCommitId
			select transactionId, delegate(JET_COMMIT_ID transactionId)
			{
				Action action;
				if (this.pendingCommits.TryRemove(transactionId, out action))
				{
					action();
				}
			});
		}

		private ulong GetAvailableFreeSpaceHelper()
		{
			try
			{
				int num;
				Api.JetGetDatabaseInfo(this.baseSession, this.baseDatabase, out num, JET_DbInfo.PageSize);
				ulong num2 = (ulong)((long)num);
				int num3;
				Api.JetGetDatabaseInfo(this.baseSession, this.baseDatabase, out num3, JET_DbInfo.SpaceAvailable);
				ulong num4 = (ulong)((long)num3);
				this.lastAvailableFreeSpace = num2 * num4;
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this))
				{
					throw;
				}
			}
			return this.lastAvailableFreeSpace;
		}

		private void StopBackgroundDefrag()
		{
			int num = 0;
			int num2 = 0;
			try
			{
				lock (this.syncRoot)
				{
					Api.JetDefragment(this.baseSession, this.baseDatabase, null, ref num, ref num2, DefragGrbit.BatchStop);
				}
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleIsamException(ex, this))
				{
					throw;
				}
			}
		}

		private void SetSystemParameter(JET_param parameter, string value)
		{
			Api.JetSetSystemParameter(this.instance, JET_SESID.Nil, parameter, 0, value);
		}

		private void SetSystemParameter(JET_param parameter, int value)
		{
			Api.JetSetSystemParameter(this.instance, JET_SESID.Nil, parameter, value, null);
		}

		private void SetSystemParameter(JET_param parameter, uint value)
		{
			Api.JetSetSystemParameter(this.instance, JET_SESID.Nil, parameter, (int)value, null);
		}

		private void InitInstance()
		{
			try
			{
				Api.JetCreateInstance(out this.instance, this.instanceName);
				this.SetSystemParameter(JET_param.MaxSessions, 1000);
				this.SetSystemParameter(JET_param.MaxOpenTables, 3360);
				this.SetSystemParameter(JET_param.MaxCursors, 5000);
				this.SetSystemParameter(JET_param.VersionStoreTaskQueueMax, this.maxBackgroundCleanupTasks);
				this.SetSystemParameter(JET_param.MaxVerPages, 8000);
				this.SetSystemParameter(JET_param.PreferredVerPages, 6000);
				this.SetSystemParameter(JET_param.CreatePathIfNotExist, 1);
				this.SetSystemParameter(JET_param.LogFileSize, this.logFileSize);
				this.SetSystemParameter((JET_param)154, 5242880U / this.logFileSize);
				this.SetSystemParameter(JET_param.CircularLog, 1);
				this.SetSystemParameter(JET_param.CleanupMismatchedLogFiles, 1);
				this.SetSystemParameter(JET_param.LogBuffers, this.logBuffers);
				this.SetSystemParameter(JET_param.SystemPath, this.instancePath);
				this.SetSystemParameter(JET_param.LogFilePath, this.logFilePath);
				this.SetSystemParameter(JET_param.TempPath, this.logFilePath);
				this.SetSystemParameter(JET_param.BaseName, "trn");
				this.SetSystemParameter(JET_param.Recovery, "on");
				this.SetSystemParameter((JET_param)184, 0);
				this.SetSystemParameter(JET_param.DbExtensionSize, this.extensionSize / DataSource.pageSize);
				this.durableCommitCallback = new DurableCommitCallback(this.instance, new JET_PFNDURABLECOMMITCALLBACK(this.FlushCallback));
				Api.JetInit(ref this.instance);
				Api.JetBeginSession(this.instance, out this.baseSession, null, null);
			}
			catch (EsentErrorException ex)
			{
				if (!DataSource.HandleInitIsamException(ex, this.instance, this.instanceName, this))
				{
					throw;
				}
			}
		}

		private DataConnection NewConnection(bool demand)
		{
			if (this.closed)
			{
				throw new InvalidOperationException(Strings.DatabaseClosed);
			}
			if (!demand)
			{
				int num = Interlocked.Increment(ref this.currentConnectionCount);
				if (num > this.connectionLimitPoint)
				{
					Interlocked.Decrement(ref this.currentConnectionCount);
					this.perfCounters.RejectedConnections.Increment();
					return null;
				}
			}
			this.perfCounters.CurrentConnections.Increment();
			return DataConnection.Open(this.instance, this);
		}

		private const string TransportEseRegistryPath = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\ExchangeServer\\v15\\Transport\\Ese";

		public const long MaxVersionBuckets = 8000L;

		private const long PreferredVersionBuckets = 6000L;

		private const int KB = 1024;

		private const int MB = 1048576;

		private const int GB = 1073741824;

		private static object globalInitLock = new object();

		private static bool globalInitDone;

		private static uint configMaxCacheSizeInPages;

		private static uint configMinCacheSizeInPages;

		private static uint currentMaxCacheSizeInPages;

		private static object exceptionHandlerLock = new object();

		private static ExEventLog eventLogger = new ExEventLog(ExTraceGlobals.ExpoTracer.Category, TransportEventLog.GetEventSource());

		private static uint pageSize = 32768U;

		private readonly string instanceName;

		private readonly string instancePath;

		private readonly string databasePath;

		private readonly string logFilePath;

		private readonly int connectionLimitPoint;

		private readonly ConcurrentDictionary<JET_COMMIT_ID, Action> pendingCommits = new ConcurrentDictionary<JET_COMMIT_ID, Action>();

		private readonly ReaderWriterLockSlim lastCommitLock = new ReaderWriterLockSlim();

		private volatile JET_COMMIT_ID lastDurableCommitId;

		private DurableCommitCallback durableCommitCallback;

		private object cleanupThreadLock = new object();

		private ulong lastAvailableFreeSpace;

		private int currentConnectionCount;

		private uint logFileSize = 5120U;

		private uint logBuffers = 1024U;

		private uint maxBackgroundCleanupTasks = 32U;

		private JET_INSTANCE instance;

		private JET_SESID baseSession;

		private JET_DBID baseDatabase;

		private bool newDatabase;

		private DatabasePerfCountersInstance perfCounters;

		private volatile bool closed = true;

		private int references;

		private object syncRoot = new object();

		private DatabaseAutoRecovery databaseAutoRecovery;

		private bool cleanupRequestInProgress;

		private uint extensionSize = 10485760U;

		private enum ProcessErrorAction
		{
			Rethrow,
			Restart,
			Stop
		}

		private enum DatabaseErrorAction
		{
			DeemTransient,
			SuspectTransient,
			DeemPermanent
		}
	}
}
