using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data.HA;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.HA.FailureItem;
using Microsoft.Exchange.Rpc.Cluster;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Isam.Esent.Interop.Unpublished;
using Microsoft.Mapi;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class MonitoredDatabase : IIdentityGuid
	{
		public MonitoredDatabase(IReplayConfiguration config)
		{
			if (config == null)
			{
				throw new ArgumentNullException("config");
			}
			this.m_fPassive = (config.Type == ReplayConfigType.RemoteCopyTarget);
			this.m_sourceDir = (this.m_fPassive ? config.DestinationLogPath : config.SourceLogPath);
			this.m_logPrefix = config.LogFilePrefix;
			this.m_logSuffix = "." + config.LogExtension;
			this.m_config = config;
			this.m_activeLogCopyClients = new List<LogCopyServerContext>();
		}

		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoredDatabaseTracer;
			}
		}

		public IReplayConfiguration Config
		{
			get
			{
				return this.m_config;
			}
		}

		public string Identity
		{
			get
			{
				return this.m_config.Identity;
			}
		}

		public Guid DatabaseGuid
		{
			get
			{
				return this.m_config.IdentityGuid;
			}
		}

		public string DatabaseName
		{
			get
			{
				return this.m_config.DatabaseName;
			}
		}

		public string DatabaseCopyName
		{
			get
			{
				return this.m_config.DisplayName;
			}
		}

		public bool IsPassiveCopy
		{
			get
			{
				return this.m_fPassive;
			}
		}

		public long EndOfLogGeneration
		{
			get
			{
				return this.m_currentEndOfLog.Generation;
			}
		}

		public EndOfLog CurrentEndOfLog
		{
			get
			{
				return this.m_currentEndOfLog;
			}
		}

		public long StartOfLogGeneration
		{
			get
			{
				return this.m_minLogGen;
			}
		}

		public static bool StringIEquals(string s1, string s2)
		{
			return SharedHelper.StringIEquals(s1, s2);
		}

		public bool TestGranularReplicationExceptionDuringAcll { get; set; }

		public int TestGranularReplicationDeliveryDelay { get; set; }

		public static MonitoredDatabase FindMonitoredDatabase(string nodeName, Guid dbGuid)
		{
			MonitoredDatabase result = null;
			IFindComponent componentFinder = Dependencies.ComponentFinder;
			if (componentFinder != null)
			{
				result = componentFinder.FindMonitoredDatabase(nodeName, dbGuid);
			}
			return result;
		}

		public static void SetCopyProperty(Guid dbGuid, string propName, string propVal)
		{
			MonitoredDatabase monitoredDatabase = MonitoredDatabase.FindMonitoredDatabase(Environment.MachineName, dbGuid);
			if (monitoredDatabase == null)
			{
				throw new ArgumentException(string.Format("Monitored database '{0}' not active", dbGuid));
			}
			if (MonitoredDatabase.StringIEquals(propName, "TestGranularReplicationExceptionDuringAcll"))
			{
				bool testGranularReplicationExceptionDuringAcll;
				if (!bool.TryParse(propVal, out testGranularReplicationExceptionDuringAcll))
				{
					throw new ArgumentException("TestGranularReplicationExceptionDuringAcll must be a bool");
				}
				monitoredDatabase.TestGranularReplicationExceptionDuringAcll = testGranularReplicationExceptionDuringAcll;
				return;
			}
			else
			{
				if (!MonitoredDatabase.StringIEquals(propName, "TestGranularReplicationDeliveryDelay"))
				{
					throw new ArgumentException(string.Format("'{0}' is not recognized", propName));
				}
				int testGranularReplicationDeliveryDelay;
				if (!int.TryParse(propVal, out testGranularReplicationDeliveryDelay))
				{
					throw new ArgumentException("TestGranularReplicationDeliveryDelay must be an int");
				}
				monitoredDatabase.TestGranularReplicationDeliveryDelay = testGranularReplicationDeliveryDelay;
				return;
			}
		}

		public MonitoredDatabaseInitException StartMonitoring()
		{
			Exception ex = null;
			MonitoredDatabase.Tracer.TraceDebug<string, Guid>((long)this.GetHashCode(), "StartMonitoring: '{0}' {1}", this.DatabaseName, this.DatabaseGuid);
			try
			{
				if (!this.m_fPassive)
				{
					this.SetupWatcher();
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(2294689085U);
				TimeSpan invokeTimeout = TimeSpan.FromSeconds((double)RegistryParameters.StartupLogScanTimeoutInSec);
				InvokeWithTimeout.Invoke(delegate()
				{
					this.DetermineEndOfLog();
				}, invokeTimeout);
				if (!this.m_fPassive)
				{
					this.m_selfCheckEnabled = true;
				}
				MonitoredDatabase.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "Monitoring database {0}. EOL=0x{1:x}", this.m_config.DatabaseName, this.m_currentEndOfLog.Generation);
			}
			catch (TimeoutException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			catch (IOException ex4)
			{
				ex = ex4;
			}
			catch (UnauthorizedAccessException ex5)
			{
				ex = ex5;
			}
			catch (EsentErrorException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				string text = string.Format("0x{0:X}", ex.HResult);
				MonitoredDatabase.Tracer.TraceError<string, string, Exception>((long)this.GetHashCode(), "StartMonitoring database {0} failed with HRESULT {1} exception {2}", this.m_config.DatabaseName, text, ex);
				ReplayCrimsonEvents.MonitoredDBFailedToStart.LogPeriodic<Guid, string, string, string, string, string>(this.DatabaseName, DiagCore.DefaultEventSuppressionInterval, this.DatabaseGuid, this.DatabaseName, ex.Message, text, ex.ToString(), this.m_sourceDir);
				if (this.IsPassiveCopy)
				{
					ReplayEventLogConstants.Tuple_PassiveMonitoredDBFailedToStart.LogEvent(this.Identity, new object[]
					{
						this.DatabaseName,
						ex.ToString()
					});
				}
				else
				{
					FailureTag failureTag = FailureTag.MonitoredDatabaseFailed;
					IOException ex7 = ex as IOException;
					if (ex7 != null && DirectoryOperations.IsPathOnLockedVolume(this.m_sourceDir))
					{
						failureTag = FailureTag.LockedVolume;
					}
					ReplayEventLogConstants.Tuple_ActiveMonitoredDBFailedToStart.LogEvent(this.Identity, new object[]
					{
						this.DatabaseName,
						ex.ToString()
					});
					FailureItemPublisherHelper.PublishAction(failureTag, this.DatabaseGuid, this.DatabaseName);
				}
				this.StopMonitoring();
				return new MonitoredDatabaseInitException(this.DatabaseName, ex.Message, ex);
			}
			return null;
		}

		public void StopMonitoring()
		{
			ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<string>((long)this.GetHashCode(), "StopMonitoring() called for db '{0}'.", this.m_config.DatabaseName);
			lock (this)
			{
				if (!this.m_stopped)
				{
					this.m_stopped = true;
					this.m_selfCheckEnabled = false;
					this.m_fileWatcher.Dispose();
					this.IssueNotifications(MonitoredDatabase.NotifyPurpose.Terminating);
				}
			}
		}

		public void SelfCheck()
		{
			long num = 0L;
			string text = null;
			string fileName = null;
			long num2 = this.m_currentEndOfLog.Generation;
			try
			{
				while (this.m_selfCheckEnabled)
				{
					num2 += 1L;
					if (this.m_currentEndOfLog.Generation >= num2)
					{
						num2 = this.m_currentEndOfLog.Generation + 1L;
					}
					text = this.BuildLogFileName(num2);
					if (!File.Exists(text))
					{
						break;
					}
					num = num2;
					fileName = text;
				}
				if (!this.m_stopped && num > 0L)
				{
					FileInfo fileInfo = new FileInfo(fileName);
					ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long, string>((long)this.GetHashCode(), "New EOL (0x{0:X}) detected by SelfCheck on db {1}", num, this.DatabaseName);
					this.DetectNewEndOfLog(num, new DateTime?(fileInfo.LastWriteTimeUtc), true);
				}
				if (!this.m_stopped && ExDateTime.Now >= this.m_nextProbeMinGenDue)
				{
					this.m_nextProbeMinGenDue += this.m_probeMinGenInterval;
					this.m_minLogGen = this.DetermineStartOfLog();
				}
			}
			catch (IOException arg)
			{
				ExTraceGlobals.MonitoredDatabaseTracer.TraceError<string, IOException>((long)this.GetHashCode(), "SelfCheck: Failed to access {0}. Exception is {1}", text, arg);
			}
		}

		public string BuildLogFileName(long logNum)
		{
			return Path.Combine(this.m_sourceDir, EseHelper.MakeLogfileName(this.m_logPrefix, this.m_logSuffix, logNum));
		}

		public long DetermineStartOfLog()
		{
			DirectoryInfo di = new DirectoryInfo(this.m_sourceDir);
			long num = ShipControl.LowestGenerationInDirectory(di, this.m_logPrefix, this.m_logSuffix, false);
			ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long>((long)this.GetHashCode(), "DetermineStartOfLog 0x{0:x}", num);
			return num;
		}

		public void UpdateCurrentEndOfLog(long highestLogGen, bool canGobackInTime)
		{
			string fileName = this.BuildLogFileName(highestLogGen);
			FileInfo fileInfo = new FileInfo(fileName);
			ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long>((long)this.GetHashCode(), "UpdateCurrentEndOfLog 0x{0:x}", highestLogGen);
			this.TryUpdateEndOfLog(highestLogGen, new DateTime?(fileInfo.LastWriteTimeUtc), canGobackInTime);
		}

		public bool AddActiveLogCopyClient(LogCopyServerContext channel)
		{
			bool result = false;
			lock (this.m_activeLogCopyClients)
			{
				if (!this.m_stopped)
				{
					this.m_activeLogCopyClients.Add(channel);
					result = true;
				}
			}
			return result;
		}

		public void RemoveActiveLogCopyClient(LogCopyServerContext channel)
		{
			lock (this.m_activeLogCopyClients)
			{
				this.m_activeLogCopyClients.Remove(channel);
			}
		}

		public void CollectConnectionStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			TcpListener tcpListener = RemoteDataProvider.GetTcpListener();
			if (tcpListener == null)
			{
				ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<string>((long)this.GetHashCode(), "CollectConnectionStatus ( {0} ): tcpListener is null, so skipping collecting connection status.", this.DatabaseName);
				return;
			}
			NetworkChannel networkChannel = tcpListener.FindSeedingChannel(this);
			if (networkChannel != null)
			{
				ConnectionStatus obj = new ConnectionStatus(networkChannel.PartnerNodeName, networkChannel.NetworkName, null, ConnectionDirection.Outgoing, true);
				byte[] seedingNetwork = Serialization.ObjectToBytes(obj);
				copyStatus.SeedingNetwork = seedingNetwork;
			}
			List<ConnectionStatus> list = new List<ConnectionStatus>();
			lock (this.m_activeLogCopyClients)
			{
				foreach (LogCopyServerContext logCopyServerContext in this.m_activeLogCopyClients)
				{
					ConnectionStatus item = logCopyServerContext.CollectConnectionStatus();
					list.Add(item);
				}
			}
			ConnectionStatus[] obj2 = list.ToArray();
			byte[] outgoingConnections = Serialization.ObjectToBytes(obj2);
			copyStatus.OutgoingConnections = outgoingConnections;
		}

		public void SendE00Generation(NetworkChannel channel)
		{
			long logGeneration = 0L;
			string e00Filename = this.BuildLogFileName(0L);
			Exception e00Generation = this.GetE00Generation(out logGeneration, e00Filename);
			if (e00Generation != null)
			{
				channel.SendException(e00Generation);
				return;
			}
			new GetE00GenerationReply(channel)
			{
				LogGeneration = logGeneration
			}.Send();
		}

		public FileIOonSourceException GetE00Generation(out long e00Gen, string e00Filename)
		{
			Exception ex = null;
			FileIOonSourceException result = null;
			e00Gen = 0L;
			try
			{
				e00Gen = EseHelper.GetLogfileGeneration(e00Filename);
				ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long, string>((long)this.GetHashCode(), "E00 Gen is 0x{0:x} for {1}", e00Gen, e00Filename);
			}
			catch (EsentErrorException ex2)
			{
				ex = ex2;
			}
			catch (IOException ex3)
			{
				ex = ex3;
			}
			catch (UnauthorizedAccessException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				ExTraceGlobals.MonitoredDatabaseTracer.TraceError<string, Exception>((long)this.GetHashCode(), "Could not get generation of {0}: {1}", e00Filename, ex);
				result = new FileIOonSourceException(Environment.MachineName, e00Filename, ex.Message, ex);
			}
			return result;
		}

		public bool ProbeForMoreLogs(long startGen)
		{
			long num = startGen;
			bool flag = false;
			string text;
			for (;;)
			{
				text = this.BuildLogFileName(num);
				if (!File.Exists(text))
				{
					break;
				}
				flag = true;
				num += 1L;
			}
			if (flag)
			{
				FileInfo fileInfo = new FileInfo(text);
				long num2 = num - 1L;
				if (this.TryUpdateEndOfLog(num2, new DateTime?(fileInfo.LastWriteTimeUtc), false))
				{
					ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long, string>((long)this.GetHashCode(), "ProbeForMoreLogs finds end of log: 0x{0:X} for db '{1}'", num2, this.DatabaseName);
				}
			}
			return flag;
		}

		public void SyncWithE00(long e00Gen)
		{
			long num = e00Gen - 1L;
			string text = this.BuildLogFileName(num);
			if (File.Exists(text))
			{
				FileInfo fileInfo = new FileInfo(text);
				if (this.TryUpdateEndOfLog(num, new DateTime?(fileInfo.LastWriteTimeUtc), false))
				{
					ExTraceGlobals.MonitoredDatabaseTracer.TraceDebug<long, string>((long)this.GetHashCode(), "SyncWithE00 finds end of log: 0x{0:X} for db '{1}'", num, this.DatabaseName);
					return;
				}
			}
			else
			{
				new AcllFailedException(ReplayStrings.LogCopierE00MissingPrevious(e00Gen, text));
			}
		}

		internal string GetDatabaseFullPath()
		{
			if (this.m_fPassive)
			{
				return this.Config.DestinationEdbPath;
			}
			return this.Config.SourceEdbPath;
		}

		internal void SendLog(long logGen, NetworkChannel channel, SourceDatabasePerformanceCountersInstance perfCounters)
		{
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			string text = this.BuildLogFileName(logGen);
			Exception ex = null;
			try
			{
				using (SafeFileHandle safeFileHandle = LogCopy.OpenLogForRead(text))
				{
					try
					{
						JET_LOGINFOMISC jet_LOGINFOMISC;
						UnpublishedApi.JetGetLogFileInfo(text, out jet_LOGINFOMISC, JET_LogInfo.Misc2);
						if (logGen != 0L && (long)jet_LOGINFOMISC.ulGeneration != logGen)
						{
							FileCheckLogfileGenerationException ex2 = new FileCheckLogfileGenerationException(text, (long)jet_LOGINFOMISC.ulGeneration, logGen);
							CorruptLogDetectedException ex3 = new CorruptLogDetectedException(text, ex2.Message, ex2);
							throw new FileIOonSourceException(Environment.MachineName, text, ex3.Message, ex3);
						}
						JET_SIGNATURE? jet_SIGNATURE = null;
						lock (this.m_sigLock)
						{
							if (this.m_logfileSignature == null)
							{
								this.m_logfileSignature = new JET_SIGNATURE?(jet_LOGINFOMISC.signLog);
							}
							jet_SIGNATURE = this.m_logfileSignature;
						}
						if (!jet_LOGINFOMISC.signLog.Equals(jet_SIGNATURE))
						{
							FileCheckLogfileSignatureException ex4 = new FileCheckLogfileSignatureException(text, jet_LOGINFOMISC.signLog.ToString(), jet_SIGNATURE.Value.ToString());
							CorruptLogDetectedException ex5 = new CorruptLogDetectedException(text, ex4.Message, ex4);
							throw new FileIOonSourceException(Environment.MachineName, text, ex5.Message, ex5);
						}
					}
					catch (EsentLogFileCorruptException ex6)
					{
						CorruptLogDetectedException ex7 = new CorruptLogDetectedException(text, ex6.Message, ex6);
						throw new FileIOonSourceException(Environment.MachineName, text, ex7.Message, ex7);
					}
					EndOfLog currentEndOfLog = this.CurrentEndOfLog;
					CopyLogReply copyLogReply = new CopyLogReply(channel);
					copyLogReply.ThisLogGeneration = logGen;
					copyLogReply.EndOfLogGeneration = currentEndOfLog.Generation;
					copyLogReply.EndOfLogUtc = currentEndOfLog.Utc;
					CheckSummer summer = null;
					if (channel.ChecksumDataTransfer)
					{
						summer = new CheckSummer();
					}
					channel.SetupLogChecksummer(this.Config.LogFilePrefix);
					channel.SendLogFileTransferReply(copyLogReply, text, safeFileHandle, perfCounters, summer);
					MonitoredDatabase.Tracer.TracePerformance<long>((long)this.GetHashCode(), "CopyLogFile finished streaming after {0}ms.", replayStopwatch.ElapsedMilliseconds);
				}
			}
			catch (IOException ex8)
			{
				ex = ex8;
			}
			catch (UnauthorizedAccessException ex9)
			{
				ex = ex9;
			}
			catch (EsentErrorException ex10)
			{
				ex = ex10;
			}
			if (ex != null)
			{
				ExTraceGlobals.MonitoredDatabaseTracer.TraceError<string, Exception>((long)this.GetHashCode(), "SendLog({0}) failed: {1}", text, ex);
				throw new FileIOonSourceException(Environment.MachineName, text, ex.Message, ex);
			}
		}

		protected void SetupWatcher()
		{
			DirectoryOperations.ProbeDirectory(this.m_sourceDir);
			this.m_fileWatcher.Database = this;
			this.m_fileWatcher.Path = this.m_sourceDir;
			this.m_fileWatcher.Filter = this.m_logPrefix + '*' + this.m_logSuffix;
			this.m_fileWatcher.NotifyFilter = (NotifyFilters.FileName | NotifyFilters.LastWrite);
			this.m_fileWatcher.Created += MonitoredDatabase.DirectoryChangeEvent;
			this.m_fileWatcher.Renamed += new RenamedEventHandler(MonitoredDatabase.DirectoryChangeEvent);
			this.m_fileWatcher.EnableRaisingEvents = true;
		}

		protected bool GetGenerationNumberFromFilename(string filename, out long generation)
		{
			string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
			return long.TryParse(fileNameWithoutExtension.Substring(this.m_logPrefix.Length), NumberStyles.HexNumber, null, out generation);
		}

		internal Action TestCallbackForEndOfLogScan { get; set; }

		private void DetermineEndOfLog()
		{
			if (this.TestCallbackForEndOfLogScan != null)
			{
				this.TestCallbackForEndOfLogScan();
			}
			DirectoryInfo di = new DirectoryInfo(this.m_sourceDir);
			long highestLogGen = ShipControl.HighestGenerationInDirectory(di, this.m_logPrefix, this.m_logSuffix);
			this.UpdateCurrentEndOfLog(highestLogGen, true);
		}

		private static void DirectoryChangeEvent(object source, FileSystemEventArgs eventInfo)
		{
			MonitoredDatabase.FileWatcher fileWatcher = source as MonitoredDatabase.FileWatcher;
			MonitoredDatabase database = fileWatcher.Database;
			long num = 0L;
			if (database.GetGenerationNumberFromFilename(eventInfo.Name, out num))
			{
				MonitoredDatabase.Tracer.TraceDebug<long, string>((long)database.GetHashCode(), "DirectoryChangeEvent: Log 0x{0:X} exists for db '{1}'", num, database.DatabaseName);
				DateTime? writeTimeUtc = null;
				Exception ex = null;
				try
				{
					FileInfo fileInfo = new FileInfo(eventInfo.FullPath);
					if (!fileInfo.Exists)
					{
						throw new FileNotFoundException("FileNotFound");
					}
					writeTimeUtc = new DateTime?(fileInfo.LastWriteTimeUtc);
				}
				catch (IOException ex2)
				{
					ex = ex2;
				}
				catch (UnauthorizedAccessException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					MonitoredDatabase.Tracer.TraceError<string, string>((long)database.GetHashCode(), "DirectoryChangeEvent for {0} failed with exception {1}", eventInfo.FullPath, ex.Message);
					ReplayCrimsonEvents.MonitoredDBFileNotifyFailure.Log<string, string, string, string>(database.DatabaseName, Environment.MachineName, eventInfo.FullPath, ex.ToString());
				}
				database.DetectNewEndOfLog(num, writeTimeUtc, false);
			}
		}

		private void DetectNewEndOfLog(long logFileNumber, DateTime? writeTimeUtc, bool detectedBySelfCheck)
		{
			if (this.Config.Type == ReplayConfigType.RemoteCopySource)
			{
				ClusterBatchWriter.SetLastLog(this.Config.IdentityGuid, logFileNumber);
			}
			if (this.TryUpdateEndOfLog(logFileNumber, writeTimeUtc, false))
			{
				MonitoredDatabase.Tracer.TraceDebug((long)this.GetHashCode(), "DetectNewEndOfLog({0}): 0x{1:X} {2}UTC via SelfCheck={3}", new object[]
				{
					this.DatabaseName,
					logFileNumber,
					writeTimeUtc,
					detectedBySelfCheck
				});
				if (detectedBySelfCheck && writeTimeUtc != null)
				{
					TimeSpan timeSpan = DateTime.UtcNow - writeTimeUtc.Value;
					if (timeSpan.TotalMilliseconds > (double)RegistryParameters.SlowIoThresholdInMs)
					{
						string text = this.BuildLogFileName(logFileNumber);
						ReplayEventLogConstants.Tuple_SlowIoDetected.LogEvent(text, new object[]
						{
							text,
							this.DatabaseCopyName,
							timeSpan.TotalMilliseconds.ToString(),
							"MissingFileNotification"
						});
					}
				}
				this.IssueNotifications(MonitoredDatabase.NotifyPurpose.NewLog);
			}
		}

		private bool TryUpdateEndOfLog(long newGenNum, DateTime? writeTimeUtc, bool canGoBackward = false)
		{
			lock (this.m_endOfLogLock)
			{
				if (newGenNum > this.m_currentEndOfLog.Generation || canGoBackward)
				{
					DateTime utc = writeTimeUtc ?? this.m_currentEndOfLog.Utc;
					this.m_currentEndOfLog = new EndOfLog(newGenNum, utc);
					return true;
				}
			}
			return false;
		}

		private void IssueNotifications(MonitoredDatabase.NotifyPurpose purpose)
		{
			List<LogCopyServerContext> list = null;
			lock (this.m_activeLogCopyClients)
			{
				if (this.m_activeLogCopyClients.Count > 0)
				{
					list = new List<LogCopyServerContext>();
					foreach (LogCopyServerContext item in this.m_activeLogCopyClients)
					{
						list.Add(item);
					}
					if (purpose == MonitoredDatabase.NotifyPurpose.Terminating)
					{
						this.m_activeLogCopyClients.Clear();
					}
				}
			}
			if (list != null)
			{
				foreach (LogCopyServerContext logCopyServerContext in list)
				{
					if (purpose == MonitoredDatabase.NotifyPurpose.NewLog)
					{
						logCopyServerContext.NewLogNotification();
					}
					else if (purpose == MonitoredDatabase.NotifyPurpose.Terminating)
					{
						logCopyServerContext.SourceIsStopping();
					}
				}
			}
		}

		private void CheckIoLatency(string filename, long msLatency, string operationContext)
		{
			if (msLatency > (long)RegistryParameters.SlowIoThresholdInMs)
			{
				ReplayEventLogConstants.Tuple_SlowIoDetected.LogEvent(filename, new object[]
				{
					filename,
					this.DatabaseCopyName,
					msLatency.ToString(),
					operationContext
				});
			}
		}

		public Exception TrySendLogWithStandardHandling(long logGen, NetworkChannel ch)
		{
			Exception result = null;
			try
			{
				this.SendLog(logGen, ch, null);
			}
			catch (FileIOonSourceException ex)
			{
				result = ex;
				Exception innerException = ex.InnerException;
				bool flag = false;
				if (innerException is CorruptLogDetectedException)
				{
					flag = true;
				}
				else if (innerException is IOException)
				{
					int hresult = 0;
					if (FileOperations.IsCorruptedIOException(innerException as IOException, out hresult))
					{
						flag = true;
						int num;
						FileOperations.ConvertHResultToWin32(hresult, out num);
						ReplayEventLogConstants.Tuple_FatalIOErrorEncountered.LogEvent(this.Identity, new object[]
						{
							this.DatabaseName,
							ex.FileFullPath,
							innerException.Message,
							num,
							innerException.ToString()
						});
					}
				}
				if (flag)
				{
					ReplayEventLogConstants.Tuple_LogCopierErrorOnSourceTriggerFailover.LogEvent(this.Identity, new object[]
					{
						this.DatabaseName,
						ch.PartnerNodeName,
						innerException.Message
					});
					this.ProcessSourceLogCorruption(logGen, innerException);
				}
				else
				{
					ReplayEventLogConstants.Tuple_LogCopierErrorOnSource.LogEvent(this.Identity, new object[]
					{
						this.DatabaseName,
						ch.PartnerNodeName,
						ex.Message
					});
				}
				ch.SendException(ex);
			}
			return result;
		}

		public void ProcessSourceLogCorruption(long badGeneration, Exception ex)
		{
			if (Monitor.TryEnter(this.m_sourceLogCorruptionLock))
			{
				try
				{
					long num = this.EndOfLogGeneration - badGeneration;
					if (num >= (long)RegistryParameters.CorruptLogRequiredRange)
					{
						if (badGeneration > this.m_mostRecentSourceLogCorruptionGen)
						{
							this.m_mostRecentSourceLogCorruptionGen = badGeneration;
							FailureItemPublisherHelper.PublishAction(FailureTag.SourceLogCorruptionOutsideRequiredRange, this.DatabaseGuid, this.DatabaseName, null);
						}
					}
					else
					{
						ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
						ReplicaInstance replicaInstance = null;
						if (replicaInstanceManager.TryGetReplicaInstance(this.DatabaseGuid, out replicaInstance) && replicaInstance.CurrentContext.IsBroken)
						{
							MonitoredDatabase.Tracer.TraceError((long)this.GetHashCode(), "ProcessSourceLogCorruption ignores error since copy is already broken");
						}
						else
						{
							this.m_mostRecentSourceLogCorruptionGen = badGeneration;
							this.DismountForSourceLogCorruption(ex);
							FailureItemPublisherHelper.PublishAction(FailureTag.SourceLogCorruption, this.DatabaseGuid, this.DatabaseName, null);
						}
					}
				}
				finally
				{
					Monitor.Exit(this.m_sourceLogCorruptionLock);
				}
			}
		}

		private void DismountForSourceLogCorruption(Exception corruptionEx)
		{
			ReplayEventLogConstants.Tuple_CorruptLogRecoveryIsImmediatelyAttempted.LogEvent(null, new object[]
			{
				this.DatabaseCopyName,
				corruptionEx.Message
			});
			try
			{
				ReplicaInstanceManager replicaInstanceManager = Dependencies.ReplayCoreManager.ReplicaInstanceManager;
				int num;
				string text = ReplayEventLogConstants.Tuple_CorruptLogRecoveryIsImmediatelyAttempted.EventLogToString(out num, new object[]
				{
					this.DatabaseCopyName,
					corruptionEx.Message
				});
				uint eventViewerEventId = DiagCore.GetEventViewerEventId(ReplayEventLogConstants.Tuple_CorruptLogRecoveryIsImmediatelyAttempted);
				replicaInstanceManager.RequestSuspendAndFail(this.DatabaseGuid, eventViewerEventId, text, text, false, false, false, false);
				MonitoredDatabase.Tracer.TraceDebug((long)this.GetHashCode(), "DismountForSourceLogCorruption successfully suspended");
			}
			catch (TaskServerException ex)
			{
				MonitoredDatabase.Tracer.TraceError<TaskServerException>((long)this.GetHashCode(), "DismountForSourceLogCorruption failed to suspend: {0}", ex);
				ReplayEventLogConstants.Tuple_CorruptLogRecoveryFailedToSuspend.LogEvent(null, new object[]
				{
					this.DatabaseName,
					ex.Message
				});
			}
			finally
			{
				Exception ex2 = this.TryToDismountClean();
				if (ex2 != null)
				{
					ReplayEventLogConstants.Tuple_CorruptLogRecoveryFailedToDismount.LogEvent(null, new object[]
					{
						this.DatabaseName,
						ex2.Message
					});
				}
			}
		}

		private Exception TryToDismountClean()
		{
			Exception result = null;
			try
			{
				AmStoreHelper.RemoteDismount(null, this.DatabaseGuid, UnmountFlags.None, true);
			}
			catch (AmServerException ex)
			{
				result = ex;
			}
			catch (AmServerTransientException ex2)
			{
				result = ex2;
			}
			catch (MapiPermanentException ex3)
			{
				result = ex3;
			}
			catch (MapiRetryableException ex4)
			{
				result = ex4;
			}
			return result;
		}

		private bool m_fPassive;

		private string m_sourceDir;

		private string m_logPrefix;

		private string m_logSuffix;

		private volatile EndOfLog m_currentEndOfLog = new EndOfLog();

		private object m_endOfLogLock = new object();

		private MonitoredDatabase.FileWatcher m_fileWatcher = new MonitoredDatabase.FileWatcher();

		private bool m_selfCheckEnabled;

		private bool m_stopped;

		private List<LogCopyServerContext> m_activeLogCopyClients;

		private IReplayConfiguration m_config;

		private readonly TimeSpan m_probeMinGenInterval = TimeSpan.FromMilliseconds((double)RegistryParameters.LogTruncationTimerDuration);

		private ExDateTime m_nextProbeMinGenDue = ExDateTime.Now;

		private long m_minLogGen;

		private JET_SIGNATURE? m_logfileSignature;

		private object m_sigLock = new object();

		private object m_sourceLogCorruptionLock = new object();

		private long m_mostRecentSourceLogCorruptionGen;

		private enum NotifyPurpose
		{
			NewLog = 1,
			Terminating
		}

		protected class FileWatcher : FileSystemWatcher
		{
			protected override void Dispose(bool disposing)
			{
				base.Dispose(disposing);
			}

			internal MonitoredDatabase Database;
		}
	}
}
