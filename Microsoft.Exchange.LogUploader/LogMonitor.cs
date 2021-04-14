using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Exchange.LogUploaderProxy;

namespace Microsoft.Exchange.LogUploader
{
	internal class LogMonitor<T> : ILogManager where T : LogDataBatch
	{
		public LogMonitor(ConfigInstance config, string logDir, string logPrefixFilter, ILogMonitorHelper<T> logMonitorHelper, string instanceName = null, string wmkFileDiretory = null)
		{
			ArgumentValidator.ThrowIfNull("config", config);
			ArgumentValidator.ThrowIfNull("logDir", logDir);
			ArgumentValidator.ThrowIfNull("logMonitorHelper", logMonitorHelper);
			ArgumentValidator.ThrowIfNullOrEmpty("logPrefix", logPrefixFilter);
			this.logPrefixToBeMonitored = logPrefixFilter;
			this.config = config;
			this.logDirectory = logDir;
			this.logMonitorHelper = logMonitorHelper;
			this.logsJustFinishedParsing = new ConcurrentDictionary<string, LogFileInfo>(StringComparer.InvariantCultureIgnoreCase);
			this.watermarkFileHelper = new WatermarkFileHelper(this.logDirectory, wmkFileDiretory);
			this.instance = (string.IsNullOrWhiteSpace(instanceName) ? this.logPrefixToBeMonitored : instanceName);
			this.batchQueue = new ThreadSafeQueue<T>(this.config.QueueCapacity);
			this.knownLogNameToLogFileMap = new ConcurrentDictionary<string, ILogFileInfo>(StringComparer.InvariantCultureIgnoreCase);
			this.logsNeedProcessing = new List<LogFileInfo>();
			this.previousLogDirectories = new HashSet<string>();
			this.reprocessingActiveFileWaitTime = Tools.RandomizeTimeSpan(this.config.WaitTimeToReprocessActiveFile, this.config.WaitTimeToReprocessActiveFileRandomRange);
			this.instanceInstantiateTime = DateTime.UtcNow;
			this.staleLogs = new List<ILogFileInfo>();
			this.veryStaleLogs = new List<ILogFileInfo>();
			this.workerThreads = new List<Thread>();
			this.maxNumberOfWriterThreads = config.MaxNumOfWriters;
			this.maxNumberOfReaderThreads = config.MaxNumOfReaders;
			this.perfCounterInstance = PerfCountersInstanceCache.GetInstance(this.instance);
			this.perfCounterInstance.TotalIncompleteLogs.RawValue = 0L;
			this.perfCounterInstance.BatchQueueLength.RawValue = 0L;
			this.perfCounterInstance.InputBufferBatchCounts.RawValue = 0L;
			this.perfCounterInstance.InputBufferBackfilledLines.RawValue = 0L;
			this.perfCounterInstance.TotalLogLinesProcessed.RawValue = 0L;
			if (Tools.IsRawProcessingType<T>())
			{
				this.perfCounterInstance.RawIncompleteBytes.RawValue = 0L;
				this.perfCounterInstance.RawTotalLogBytes.RawValue = 0L;
				this.perfCounterInstance.RawWrittenBytes.RawValue = 0L;
				this.perfCounterInstance.RawReaderParsedBytes.RawValue = 0L;
				return;
			}
			this.perfCounterInstance.IncompleteBytes.RawValue = 0L;
			this.perfCounterInstance.TotalLogBytes.RawValue = 0L;
			this.perfCounterInstance.TotalLogBytesProcessed.RawValue = 0L;
			this.perfCounterInstance.ReaderParsedBytes.RawValue = 0L;
		}

		public int MaxNumberOfReaders
		{
			get
			{
				return this.maxNumberOfReaderThreads;
			}
			protected set
			{
				this.maxNumberOfReaderThreads = value;
			}
		}

		public int MaxNumberOfWriters
		{
			get
			{
				return this.maxNumberOfWriterThreads;
			}
			protected set
			{
				this.maxNumberOfWriterThreads = value;
			}
		}

		public TimeSpan LogDirCheckInterval
		{
			get
			{
				return this.config.LogDirCheckInterval;
			}
		}

		public ThreadSafeQueue<T> BatchQueue
		{
			get
			{
				return this.batchQueue;
			}
		}

		public int NumberOfWorkerThreads
		{
			get
			{
				return this.workerThreads.Count;
			}
		}

		public string Instance
		{
			get
			{
				return this.instance;
			}
		}

		public string LogPrefixToBeMonitored
		{
			get
			{
				return this.logPrefixToBeMonitored;
			}
		}

		public string LogDirectory
		{
			get
			{
				return this.logDirectory;
			}
		}

		public int StaleLogCount
		{
			get
			{
				return this.staleLogs.Count;
			}
		}

		public int VeryStaleLogCount
		{
			get
			{
				return this.veryStaleLogs.Count;
			}
		}

		public DatabaseWriter<T>[] Writers
		{
			get
			{
				return this.writers;
			}
			internal set
			{
				this.writers = value;
			}
		}

		public ConfigInstance Config
		{
			get
			{
				return this.config;
			}
			internal set
			{
				this.config = value;
			}
		}

		internal IEnumerable<LogFileInfo> LogsNeedProcessing
		{
			get
			{
				return this.logsNeedProcessing;
			}
		}

		internal ConcurrentDictionary<string, LogFileInfo> UnitTest_Get_logsJustFinishedParsing
		{
			get
			{
				return this.logsJustFinishedParsing;
			}
		}

		internal int CheckDirectoryCount
		{
			get
			{
				return this.checkDirectoryCount;
			}
		}

		internal List<ILogFileInfo> StaleLogs
		{
			get
			{
				return this.staleLogs;
			}
		}

		internal List<ILogFileInfo> VeryStaleLogs
		{
			get
			{
				return this.veryStaleLogs;
			}
		}

		internal IWatermarkFileHelper WatermarkFileHelper
		{
			get
			{
				return this.watermarkFileHelper;
			}
		}

		internal string UnitTestCatchedReaderWriterException
		{
			get
			{
				return this.unitTestCatchedExceptionMessage;
			}
		}

		protected ConcurrentDictionary<string, ILogFileInfo> KnownLogNameToLogFileMap
		{
			get
			{
				return this.knownLogNameToLogFileMap;
			}
		}

		public virtual void Start()
		{
			int num = this.maxNumberOfReaderThreads + this.maxNumberOfWriterThreads;
			this.stopWaitHandles = new ManualResetEvent[num];
			this.stopTokenSource = new CancellationTokenSource();
			CancellationToken token = this.stopTokenSource.Token;
			this.logMonitorHelper.Initialize(token);
			this.needToRetryStart = !this.RetryableStart();
			this.checkDirectoryTimer = new Timer(new TimerCallback(this.CheckDirectory), null, new TimeSpan(0L), this.config.LogDirCheckInterval);
		}

		public virtual void Stop()
		{
			if (this.checkDirectoryTimer != null)
			{
				this.checkDirectoryTimer.Dispose();
				this.checkDirectoryTimer = null;
			}
			if (this.stopTokenSource != null)
			{
				ExTraceGlobals.LogMonitorTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "LogMonitor of type {0} sent stop requests to readers and writers.", this.instance);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorRequestedStop, this.instance, new object[]
				{
					this.instance
				});
				ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogMonitorRequestedStop, "", this.instance, "");
				this.stopTokenSource.Cancel();
				if (this.stopWaitHandles != null)
				{
					ManualResetEvent[] array = (from h in this.stopWaitHandles
					where h != null
					select h).ToArray<ManualResetEvent>();
					if (array.Length == 0 || WaitHandle.WaitAll(array, this.config.ServiceStopWaitTime))
					{
						ExTraceGlobals.LogMonitorTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "No worker threads of log type {0} is running.", this.instance);
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorAllStopped, this.instance, new object[]
						{
							this.instance
						});
						ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogMonitorAllStopped, "", this.instance, "");
					}
					else
					{
						ExTraceGlobals.LogMonitorTracer.TraceInformation<string>(0, (long)this.GetHashCode(), "Timeout waiting for all readers and writers of log type {0} to stop. Call thread's abort anyway.", this.instance);
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorStopTimedOut, this.instance, new object[]
						{
							this.instance
						});
						ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogMonitorStopTimedOut, "", this.instance, "");
						foreach (Thread thread in this.workerThreads)
						{
							if (thread.IsAlive)
							{
								thread.Abort();
							}
						}
						Thread.Sleep(1000);
					}
				}
			}
			else
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorAllStopped, null, new object[]
				{
					this.instance
				});
				ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogMonitorAllStopped, "", this.instance, "");
			}
			this.workerThreads.Clear();
			if (this.batchQueue != null)
			{
				this.batchQueue.Close();
			}
			this.DisposeAllWatermarkFileObjects();
		}

		public virtual LogFileInfo GetLogForReaderToProcess()
		{
			if (this.checkDirectoryCount == 0)
			{
				return null;
			}
			LogFileInfo logFileInfo = null;
			bool flag = false;
			do
			{
				logFileInfo = null;
				flag = false;
				lock (this.logsNeedProcessingSyncObject)
				{
					if (this.logsNeedProcessing.Count > 0)
					{
						logFileInfo = this.logsNeedProcessing[0];
						this.logsNeedProcessing.RemoveAt(0);
					}
				}
				if (logFileInfo != null)
				{
					try
					{
						if (logFileInfo.Size == 0L)
						{
							flag = true;
						}
					}
					catch (IOException)
					{
						flag = true;
					}
					if (flag)
					{
						this.logsJustFinishedParsing.TryAdd(logFileInfo.FileName, logFileInfo);
					}
				}
			}
			while (flag);
			return logFileInfo;
		}

		public IWatermarkFile FindWatermarkFileObject(string logFileName)
		{
			ArgumentValidator.ThrowIfNullOrEmpty("logFileName", logFileName);
			string fileName = Path.GetFileName(logFileName);
			ILogFileInfo logFileInfo;
			this.knownLogNameToLogFileMap.TryGetValue(fileName, out logFileInfo);
			if (logFileInfo != null)
			{
				return logFileInfo.WatermarkFileObj;
			}
			return null;
		}

		public virtual void ReaderCompletedProcessingLog(LogFileInfo log)
		{
			if (log == null)
			{
				return;
			}
			this.logsJustFinishedParsing.TryAdd(log.FileName, log);
		}

		internal virtual void AddLogToNeedProcessing(IEnumerable<LogFileInfo> logFilesList)
		{
			lock (this.logsNeedProcessingSyncObject)
			{
				foreach (LogFileInfo item in logFilesList)
				{
					this.logsNeedProcessing.Add(item);
				}
				this.logsNeedProcessing.Sort((LogFileInfo log1, LogFileInfo log2) => DateTime.Compare(log2.CreationTimeUtc, log1.CreationTimeUtc));
			}
		}

		internal void CheckDirectory(object state)
		{
			if (Interlocked.CompareExchange(ref this.checkDirectoryDone, 0, 1) == 1)
			{
				try
				{
					if (this.logDirectory == null)
					{
						return;
					}
					if (this.needToRetryStart && (this.needToRetryStart = !this.RetryableStart()))
					{
						return;
					}
					if (!Directory.Exists(this.logDirectory))
					{
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_NonexistentLogDirectory, this.logDirectory, new object[]
						{
							this.logDirectory
						});
						return;
					}
					IEnumerable<FileInfo> source = new DirectoryInfo(this.logDirectory).EnumerateFiles(this.logPrefixToBeMonitored + "*.log");
					IEnumerable<FileInfo> enumerable = from f in source
					where f.Length < 2147483647L && this.logMonitorHelper.ShouldProcessLogFile(this.logPrefixToBeMonitored, f.Name) && !this.HasDoneFile(f.Name)
					select f;
					this.DumpPendingProcessLogFilesInfo(enumerable);
					this.AddNewLogsToProcess(enumerable);
					this.CheckBackLogs();
					this.CheckLogsAndMarkComplete();
					this.CheckLogsDueToReprocess();
					this.DeleteResidueFilesForRetiredLogs();
				}
				catch (Exception ex)
				{
					string message = string.Format("Caught an Exception when checking directory {0}. Exception: {1}", this.logDirectory, ex);
					ExTraceGlobals.LogMonitorTracer.TraceError((long)this.GetHashCode(), message);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_CheckDirectoryCaughtException, this.logDirectory, new object[]
					{
						this.logDirectory,
						ex
					});
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221228483U, ex.Message, this.instance, this.logDirectory);
					if (!ex.Message.Contains("Insufficient system resources exist to complete the requested service"))
					{
						throw;
					}
				}
				finally
				{
					this.checkDirectoryCount++;
					this.checkDirectoryDone = 1;
				}
				this.UpdateIncompleteLogsPerfCounter();
				if (Tools.IsRawProcessingType<T>())
				{
					this.perfCounterInstance.RawIncompleteBytes.RawValue = this.unprocessedBytes;
					this.perfCounterInstance.RawTotalLogBytes.RawValue = this.totalLogBytes;
				}
				else
				{
					this.perfCounterInstance.IncompleteBytes.RawValue = this.unprocessedBytes;
					this.perfCounterInstance.TotalLogBytes.RawValue = this.totalLogBytes;
				}
				this.perfCounterInstance.ThreadSafeQueueConsumerSemaphoreCount.RawValue = (long)this.batchQueue.ConsumerSemaphoreCount;
				this.perfCounterInstance.ThreadSafeQueueProducerSemaphoreCount.RawValue = (long)this.batchQueue.ProducerSemaphoreCount;
			}
		}

		internal virtual void AddNewLogsToProcess(IEnumerable<FileInfo> fileInfoList)
		{
			this.ResetStaleLogCollection();
			int num = 0;
			int num2 = 0;
			IOrderedEnumerable<FileInfo> orderedEnumerable = from f in fileInfoList
			orderby f.CreationTimeUtc descending
			select f;
			bool flag = true;
			bool flag2 = false;
			List<LogFileInfo> list = new List<LogFileInfo>();
			DateTime t = DateTime.UtcNow - this.Config.ActiveLogFileIdleTimeout;
			foreach (FileInfo fileInfo in orderedEnumerable)
			{
				try
				{
					bool isActive = flag || (this.Config.EnableMultipleWriters && fileInfo.LastWriteTimeUtc > t);
					ILogFileInfo logFileInfo;
					LogFileInfo logFileInfo2;
					if (!this.knownLogNameToLogFileMap.TryGetValue(fileInfo.Name, out logFileInfo))
					{
						logFileInfo2 = new LogFileInfo(fileInfo.Name, isActive, this.instance, this.watermarkFileHelper);
						this.knownLogNameToLogFileMap.TryAdd(fileInfo.Name, logFileInfo2);
						list.Add(logFileInfo2);
						num++;
					}
					else
					{
						logFileInfo2 = (LogFileInfo)logFileInfo;
						logFileInfo2.IsActive = isActive;
					}
					if (!flag2)
					{
						this.CalculateIncompleteBytes(logFileInfo2, out flag2);
					}
					this.UpdateStaleLogs(logFileInfo2);
					if (logFileInfo2.Status == ProcessingStatus.NeedProcessing)
					{
						num2++;
					}
				}
				catch (FailedToInstantiateLogFileInfoException ex)
				{
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_FailedToInstantiateLogFileInfo, fileInfo.Name, new object[]
					{
						ex.Message
					});
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)2147486660U, ex.Message, this.instance, fileInfo.Name);
				}
				flag = false;
			}
			if (list.Count > 0)
			{
				this.AddLogToNeedProcessing(list);
			}
			this.perfCounterInstance.TotalIncomingLogs.IncrementBy((long)num);
			this.perfCounterInstance.NumberOfIncomingLogs.RawValue = (long)num;
			this.perfCounterInstance.LogsNeverProcessedBefore.RawValue = (long)num2;
		}

		internal virtual void CheckLogsAndMarkComplete()
		{
			List<LogFileInfo> list = new List<LogFileInfo>();
			List<LogFileInfo> list2 = new List<LogFileInfo>();
			foreach (LogFileInfo logFileInfo in this.logsJustFinishedParsing.Values)
			{
				if (!logFileInfo.FileExists)
				{
					list2.Add(logFileInfo);
				}
				else if (!logFileInfo.IsActive && logFileInfo.WatermarkFileObj.IsLogCompleted())
				{
					list.Add(logFileInfo);
				}
			}
			this.RemoveLogFileInfoObjects(list2);
			this.ReportMissingLogs(list2);
			this.ReportCompleteLogs(list);
			this.RemoveLogFileInfoObjects(list);
		}

		internal virtual bool ShouldSuppressBackLogAlert()
		{
			return DateTime.UtcNow.Subtract(this.instanceInstantiateTime) < AppConfigReader.MaxWaitTimeBeforeAlertOnBackLog;
		}

		internal bool IsFileProcessedBefore(ILogFileInfo logFileInfo)
		{
			string watermarkFileFullName = logFileInfo.WatermarkFileObj.WatermarkFileFullName;
			return File.Exists(watermarkFileFullName) || (logFileInfo.Status != ProcessingStatus.NeedProcessing && logFileInfo.Status != ProcessingStatus.Unknown);
		}

		internal void DeleteResidueFilesForRetiredLogs()
		{
			string filter = string.Format("{0}*.{1}", this.logPrefixToBeMonitored, "wmk");
			string filter2 = string.Format("{0}*.{1}", this.logPrefixToBeMonitored, "done");
			this.ClearnupWmkDoneFiles(this.watermarkFileHelper.WatermarkFileDirectory, filter);
			this.ClearnupWmkDoneFiles(this.watermarkFileHelper.WatermarkFileDirectory, filter2);
			if (this.previousLogDirectories.Count > 0)
			{
				foreach (string dir in this.previousLogDirectories)
				{
					this.ClearnupWmkDoneFiles(dir, filter);
					this.ClearnupWmkDoneFiles(dir, filter2);
				}
			}
		}

		internal void ClearnupWmkDoneFiles(string dir, string filter)
		{
			foreach (string text in Directory.EnumerateFiles(dir, filter))
			{
				try
				{
					string path = this.watermarkFileHelper.DeduceLogFullFileNameFromDoneOrWatermarkFileName(text);
					if (!File.Exists(path))
					{
						if (this.knownLogNameToLogFileMap.ContainsKey(Path.GetFileName(path)))
						{
							ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.DeletingFile, "Skipped for it's still processed by this logMonitor", this.instance, text);
						}
						else
						{
							ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.DeletingFile, "Delete Done or watermark file", this.instance, text);
							File.Delete(text);
						}
					}
				}
				catch (IOException ex)
				{
					string text2 = string.Format("Failed to clean up {0}. Exception: {1}.", text, ex.Message);
					ExTraceGlobals.LogMonitorTracer.TraceError((long)this.GetHashCode(), text2);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorWatermarkCleanupFailed, text, new object[]
					{
						text,
						ex.Message
					});
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221228476U, text2, "", text);
				}
				catch (UnauthorizedAccessException ex2)
				{
					string text3 = string.Format("Failed to clean up {0}. Exception: {1}.", text, ex2);
					ExTraceGlobals.LogMonitorTracer.TraceError((long)this.GetHashCode(), text3);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorWatermarkCleanupFailed, text, new object[]
					{
						text,
						ex2
					});
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221228476U, text3, "", text);
				}
			}
		}

		internal void StartReaderThreads(CancellationToken stopToken)
		{
			ExTraceGlobals.LogMonitorTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Starting {0} reader threads for logPrefix {1}", this.config.MaxNumOfReaders, this.instance);
			this.readers = new LogReader<T>[this.config.MaxNumOfReaders];
			for (int i = 0; i < this.maxNumberOfReaderThreads; i++)
			{
				this.readers[i] = new LogReader<T>(this.batchQueue, i, this, this.config, this.logPrefixToBeMonitored, this.logMonitorHelper, this.instance);
				this.stopWaitHandles[i] = new ManualResetEvent(false);
				Thread thread = new Thread(new ParameterizedThreadStart(this.DoWorkAction(new Action<object>(this.readers[i], ldftn(DoWork))).Invoke));
				thread.Priority = this.config.ReaderPrioritySetting;
				thread.Start(new CancellationContext(stopToken, this.stopWaitHandles[i]));
				this.workerThreads.Add(thread);
			}
		}

		internal void StartWriterThreads(CancellationToken stopToken)
		{
			ExTraceGlobals.LogMonitorTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Starting {0} writer threads for logPrefix {1}", this.maxNumberOfWriterThreads, this.instance);
			for (int i = 0; i < this.maxNumberOfWriterThreads; i++)
			{
				this.stopWaitHandles[i + this.maxNumberOfReaderThreads] = new ManualResetEvent(false);
				Thread thread = new Thread(new ParameterizedThreadStart(this.DoWorkAction(new Action<object>(this.writers[i], ldftn(DoWork))).Invoke));
				thread.Priority = this.config.WriterPrioritySetting;
				thread.Start(new CancellationContext(stopToken, this.stopWaitHandles[i + this.maxNumberOfReaderThreads]));
				this.workerThreads.Add(thread);
			}
		}

		internal virtual void CreateWriters()
		{
			if (this.writers == null)
			{
				this.writers = new DatabaseWriter<T>[this.maxNumberOfWriterThreads];
			}
			for (int i = 0; i < this.maxNumberOfWriterThreads; i++)
			{
				if (this.writers[i] == null)
				{
					this.writers[i] = this.logMonitorHelper.CreateDBWriter(this.batchQueue, i, this.config, this.instance);
					this.writers[i].SetLogMonitorInterface(this);
				}
			}
		}

		internal long UpdateIncompleteLogsPerfCounter()
		{
			long num = 0L;
			this.unprocessedBytes = 0L;
			foreach (string key in this.knownLogNameToLogFileMap.Keys)
			{
				ILogFileInfo logFileInfo;
				if (this.knownLogNameToLogFileMap.TryGetValue(key, out logFileInfo))
				{
					if (logFileInfo.IsActive)
					{
						if (!logFileInfo.WatermarkFileObj.IsLogCompleted())
						{
							this.unprocessedBytes += Math.Max(0L, logFileInfo.Size - logFileInfo.WatermarkFileObj.ProcessedSize);
							num += 1L;
						}
					}
					else if (logFileInfo.Status != ProcessingStatus.CompletedProcessing)
					{
						this.unprocessedBytes += Math.Max(0L, logFileInfo.Size - logFileInfo.WatermarkFileObj.ProcessedSize);
						num += 1L;
					}
				}
			}
			long incrementValue = num - this.perfCounterInstance.TotalIncompleteLogs.RawValue;
			this.perfCounterInstance.TotalIncompleteLogs.IncrementBy(incrementValue);
			return num;
		}

		internal void CheckLogsDueToReprocess()
		{
			List<LogFileInfo> list = new List<LogFileInfo>();
			foreach (LogFileInfo logFileInfo in this.logsJustFinishedParsing.Values)
			{
				if (this.DueForReprocess(logFileInfo))
				{
					list.Add(logFileInfo);
				}
			}
			foreach (LogFileInfo logFileInfo2 in list)
			{
				LogFileInfo logFileInfo3;
				this.logsJustFinishedParsing.TryRemove(logFileInfo2.FileName, out logFileInfo3);
			}
			this.AddLogToNeedProcessing(list);
		}

		internal bool DueForReprocess(LogFileInfo log)
		{
			bool flag = log.WatermarkFileObj.ReaderHasBytesToParse();
			if (flag)
			{
				bool flag2 = DateTime.UtcNow.Subtract(log.LastProcessedTime).TotalMilliseconds > (double)this.reprocessingActiveFileWaitTime;
				if (flag2)
				{
					this.reprocessingActiveFileWaitTime = Tools.RandomizeTimeSpan(this.config.WaitTimeToReprocessActiveFile, this.config.WaitTimeToReprocessActiveFileRandomRange);
				}
				return flag2;
			}
			return false;
		}

		internal void UnitTestSetCatchReaderWriterExceptionFlag()
		{
			this.catchReaderWriterExceptionsForUnitTest = true;
		}

		protected void ResetStaleLogCollection()
		{
			this.staleLogs = new List<ILogFileInfo>();
			this.veryStaleLogs = new List<ILogFileInfo>();
		}

		protected void CalculateIncompleteBytes(LogFileInfo logFile, out bool skipRestOfFilesInTheList)
		{
			skipRestOfFilesInTheList = false;
			if (this.checkDirectoryCount == 0)
			{
				long num = logFile.AddedLogSize();
				this.totalLogBytes += Math.Max(0L, num - logFile.WatermarkFileObj.ProcessedSize);
				return;
			}
			long num2 = logFile.AddedLogSize();
			this.totalLogBytes += num2;
			skipRestOfFilesInTheList = (!logFile.IsActive && num2 == 0L);
		}

		protected void UpdateStaleLogs(ILogFileInfo logFileInfo)
		{
			double totalHours = DateTime.UtcNow.Subtract(logFileInfo.LastWriteTimeUtc).TotalHours;
			if (!this.IsFileProcessedBefore(logFileInfo) && !LogMonitor<T>.IsEmptyLog(logFileInfo) && totalHours >= this.config.BacklogAlertNonUrgentThreshold.TotalHours)
			{
				this.staleLogs.Add(logFileInfo);
				if (totalHours >= this.config.BacklogAlertUrgentThreshold.TotalHours)
				{
					this.veryStaleLogs.Add(logFileInfo);
				}
			}
		}

		protected void CheckBackLogs()
		{
			if (this.veryStaleLogs.Count == 0)
			{
				Interlocked.CompareExchange(ref this.veryStaleLogReportedBefore, 0, 1);
			}
			if (this.staleLogs.Count == 0)
			{
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorDetectNoStaleLog, null, new object[]
				{
					this.instance
				});
				Interlocked.CompareExchange(ref this.staleLogReportedBefore, 0, 1);
				return;
			}
			this.HandleBackLogAlert();
		}

		protected bool HasDoneFile(string logFileName)
		{
			return File.Exists(this.watermarkFileHelper.DeduceDoneFileFullNameFromLogFileName(logFileName));
		}

		private static void RaiseAlertIfHealthStateChange(ref int state, string monitorName, string error)
		{
			if (Interlocked.CompareExchange(ref state, 1, 0) == 0)
			{
				EventNotificationItem.Publish(ExchangeComponent.Name, monitorName, null, error, ResultSeverityLevel.Error, false);
			}
		}

		private static bool IsEmptyLog(ILogFileInfo logFileInfo)
		{
			return logFileInfo.Size == 0L;
		}

		private void ReportMissingLogs(List<LogFileInfo> missingLogs)
		{
			if (missingLogs.Count == 0)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("The following logs are not found while being processed:");
			foreach (LogFileInfo logFileInfo in missingLogs)
			{
				ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221231489U, "CheckLogsAndMarkComplete", this.instance, logFileInfo.FullFileName);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_FileDeleted, logFileInfo.FullFileName, new object[]
				{
					logFileInfo.FullFileName
				});
				stringBuilder.AppendLine(logFileInfo.FullFileName);
			}
			EventNotificationItem.Publish(ExchangeComponent.Name, "LogFileNotFoundError", null, stringBuilder.ToString(), ResultSeverityLevel.Error, false);
		}

		private void ReportCompleteLogs(List<LogFileInfo> completeLogs)
		{
			foreach (LogFileInfo logFileInfo in completeLogs)
			{
				this.perfCounterInstance.TotalCompletedLogs.Increment();
				string message = string.Format("The processing of log {0} is completed.", logFileInfo.FullFileName);
				ExTraceGlobals.LogMonitorTracer.TraceDebug((long)this.GetHashCode(), message);
				EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorLogCompleted, logFileInfo.FullFileName, new object[]
				{
					logFileInfo.FullFileName
				});
				ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogMonitorLogCompleted, "", "", logFileInfo.FullFileName);
			}
		}

		private void RemoveLogFileInfoObjects(IEnumerable<LogFileInfo> toBeRemovedList)
		{
			foreach (LogFileInfo logFileInfo in toBeRemovedList)
			{
				LogFileInfo logFileInfo2;
				this.logsJustFinishedParsing.TryRemove(logFileInfo.FileName, out logFileInfo2);
				logFileInfo.Status = ProcessingStatus.CompletedProcessing;
				logFileInfo.WatermarkFileObj.Dispose();
				if (logFileInfo.FileExists)
				{
					logFileInfo.WatermarkFileObj.CreateDoneFile();
				}
				ILogFileInfo logFileInfo3;
				if (this.knownLogNameToLogFileMap.TryRemove(logFileInfo.FileName, out logFileInfo3))
				{
					string text = string.Format("log file {0} is removed from KnownLogNameToLogFileMap.", logFileInfo.FileName);
					ExTraceGlobals.LogMonitorTracer.TraceDebug((long)this.GetHashCode(), text);
					ServiceLogger.LogInfo(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.LogFileDeletedFromKnownLogNameToLogFileMap, text, "", logFileInfo.FileName);
				}
				else
				{
					string.Format("The log {0} disappeared from KnownLogNameToLogFileMap. This indicates a bug somewhere", logFileInfo.FullFileName);
					EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogDisappearedFromKnownLogNameToLogFileMap, logFileInfo.FullFileName, new object[]
					{
						logFileInfo.FullFileName
					});
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)2147748808U, "", "", logFileInfo.FullFileName);
				}
			}
		}

		private void ReportBacklogCondition(int staleLogCount, int veryStaleLogCount, string logDir, List<ILogFileInfo> logList, bool isUrgent)
		{
			int num = Math.Min(logList.Count, 10);
			IEnumerable<ILogFileInfo> enumerable = logList.Take(num);
			StringBuilder stringBuilder = new StringBuilder();
			string value = (num > 1) ? string.Format("The first {0} logs are:", num) : "Here are the detailed info:";
			stringBuilder.AppendLine(value);
			foreach (ILogFileInfo logFileInfo in enumerable)
			{
				string text = this.BuildWatermarkFileInfo(logFileInfo);
				string text2 = "unknown";
				try
				{
					FileInfo fileInfo = new FileInfo(logFileInfo.FullFileName);
					if (fileInfo.Exists)
					{
						text2 = string.Format("{0} bytes", fileInfo.Length);
					}
				}
				catch (Exception ex)
				{
					if (RetryHelper.IsSystemFatal(ex))
					{
						throw;
					}
				}
				stringBuilder.AppendLine(string.Format("The log file {0} is {1}, its size is {2}, created on {3}, last modified on {4}, {5}", new object[]
				{
					logFileInfo.FullFileName,
					logFileInfo.IsActive ? "Active" : "Inactive",
					text2,
					logFileInfo.CreationTimeUtc,
					logFileInfo.LastWriteTimeUtc,
					text
				}));
			}
			string text3 = string.Format("There are {0} logs in directory {1} that haven't been processed for {2} hours. {3} of them are over {4} hours.\n{5}", new object[]
			{
				staleLogCount,
				logDir,
				this.config.BacklogAlertNonUrgentThreshold.TotalHours,
				veryStaleLogCount,
				this.config.BacklogAlertUrgentThreshold.TotalHours,
				stringBuilder.ToString()
			});
			if (isUrgent)
			{
				LogMonitor<T>.RaiseAlertIfHealthStateChange(ref this.veryStaleLogReportedBefore, "SeriousBacklogBuiltUp", text3);
			}
			else
			{
				LogMonitor<T>.RaiseAlertIfHealthStateChange(ref this.staleLogReportedBefore, "BacklogBuiltUp", text3);
			}
			EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_LogMonitorDetectLogProcessingFallsBehind, this.instance, new object[]
			{
				staleLogCount,
				logDir,
				this.config.BacklogAlertNonUrgentThreshold.TotalHours,
				veryStaleLogCount,
				this.config.BacklogAlertUrgentThreshold.TotalHours,
				stringBuilder.ToString()
			});
			ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221228478U, text3, this.instance, this.logDirectory);
		}

		private void HandleBackLogAlert()
		{
			if (this.ShouldSuppressBackLogAlert())
			{
				return;
			}
			if (this.veryStaleLogs.Count > 0)
			{
				this.ReportBacklogCondition(this.staleLogs.Count, this.veryStaleLogs.Count, this.logDirectory, this.veryStaleLogs, true);
				return;
			}
			if (this.staleLogs.Count > 0)
			{
				this.ReportBacklogCondition(this.staleLogs.Count, this.veryStaleLogs.Count, this.logDirectory, this.staleLogs, false);
			}
		}

		private void DumpPendingProcessLogFilesInfo(IEnumerable<FileInfo> pendingProcessLogFiles)
		{
			if (ServiceLogger.ServiceLogLevel > ServiceLogger.LogLevel.Debug)
			{
				return;
			}
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FileInfo fileInfo in pendingProcessLogFiles)
			{
				stringBuilder.AppendLine(string.Format("file={0},created={1},modified={2}", fileInfo.FullName, fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc));
			}
			if (stringBuilder.Length > 0)
			{
				ServiceLogger.LogDebug(ServiceLogger.Component.LogMonitor, LogUploaderEventLogConstants.Message.PendingProcessLogFilesInfo, stringBuilder.ToString(), this.instance, this.logDirectory);
			}
		}

		private string BuildWatermarkFileInfo(ILogFileInfo logFileInfo)
		{
			string watermarkFileFullName = logFileInfo.WatermarkFileObj.WatermarkFileFullName;
			string result = string.Empty;
			try
			{
				FileInfo fileInfo = new FileInfo(watermarkFileFullName);
				if (fileInfo.Exists)
				{
					result = string.Format("the watermark file={0},created={1},modified={2}", watermarkFileFullName, fileInfo.CreationTimeUtc, fileInfo.LastWriteTimeUtc);
				}
				else
				{
					result = string.Format("the watermark file {0} doesn't exist", watermarkFileFullName);
				}
			}
			catch (Exception ex)
			{
				if (RetryHelper.IsSystemFatal(ex))
				{
					throw;
				}
				result = string.Format("the watermark file {0} exists but can't retrive its info because of exception: {1}", watermarkFileFullName, ex.Message);
			}
			return result;
		}

		private bool RetryableStart()
		{
			try
			{
				this.CreateWriters();
			}
			catch (ConfigurationErrorsException ex)
			{
				if (ex.Message.Contains("Log Path is not set in Registry"))
				{
					int num = Math.Max(1, 300 / (int)this.config.LogDirCheckInterval.TotalSeconds);
					if (this.checkDirectoryCount / num < 1)
					{
						EventLogger.Logger.LogEvent(LogUploaderEventLogConstants.Tuple_FailedToGetLogPath, this.instance, new object[]
						{
							this.instance,
							ex.Message
						});
						ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221488641U, ex.Message, this.instance, this.logDirectory);
						return false;
					}
				}
				throw;
			}
			this.StartWriterThreads(this.stopTokenSource.Token);
			this.StartReaderThreads(this.stopTokenSource.Token);
			return true;
		}

		private void DisposeAllWatermarkFileObjects()
		{
			foreach (ILogFileInfo logFileInfo in this.knownLogNameToLogFileMap.Values)
			{
				LogFileInfo logFileInfo2 = (LogFileInfo)logFileInfo;
				logFileInfo2.WatermarkFileObj.Dispose();
			}
			this.knownLogNameToLogFileMap.Clear();
		}

		private Action<object> DoWorkAction(Action<object> realDoWorkMethod)
		{
			if (!this.catchReaderWriterExceptionsForUnitTest)
			{
				return realDoWorkMethod;
			}
			return delegate(object stateObj)
			{
				try
				{
					realDoWorkMethod(stateObj);
				}
				catch (Exception ex)
				{
					ServiceLogger.LogError(ServiceLogger.Component.LogMonitor, (LogUploaderEventLogConstants.Message)3221226485U, ex.ToString(), "", "");
					EventNotificationItem.Publish(ExchangeComponent.Name, "ServiceStartUnknownException", null, ex.ToString(), ResultSeverityLevel.Error, false);
					LogMonitor<T> <>4__this = this;
					<>4__this.unitTestCatchedExceptionMessage += ex.ToString();
				}
			};
		}

		private const int MaxNumberOfStaleLogsToReport = 10;

		private readonly string instance;

		private readonly string logPrefixToBeMonitored;

		private readonly string logDirectory;

		private readonly ThreadSafeQueue<T> batchQueue;

		private readonly DateTime instanceInstantiateTime;

		private HashSet<string> previousLogDirectories;

		private Timer checkDirectoryTimer;

		private ConcurrentDictionary<string, ILogFileInfo> knownLogNameToLogFileMap;

		private List<LogFileInfo> logsNeedProcessing;

		private object logsNeedProcessingSyncObject = new object();

		private ConcurrentDictionary<string, LogFileInfo> logsJustFinishedParsing;

		private DatabaseWriter<T>[] writers;

		private LogReader<T>[] readers;

		private int checkDirectoryDone = 1;

		private CancellationTokenSource stopTokenSource;

		private ManualResetEvent[] stopWaitHandles;

		private List<Thread> workerThreads;

		private int staleLogReportedBefore;

		private int veryStaleLogReportedBefore;

		private ConfigInstance config;

		private int checkDirectoryCount;

		private ILogMonitorHelper<T> logMonitorHelper;

		private int reprocessingActiveFileWaitTime;

		private List<ILogFileInfo> staleLogs;

		private List<ILogFileInfo> veryStaleLogs;

		private long totalLogBytes;

		private long unprocessedBytes;

		private ILogUploaderPerformanceCounters perfCounterInstance;

		private bool needToRetryStart;

		private IWatermarkFileHelper watermarkFileHelper;

		private int maxNumberOfReaderThreads;

		private int maxNumberOfWriterThreads;

		private bool catchReaderWriterExceptionsForUnitTest;

		private string unitTestCatchedExceptionMessage;
	}
}
