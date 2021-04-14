using System;
using System.Diagnostics;
using System.IO;
using System.Security;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Common.HA;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Rpc.Cluster;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogCopier : IStartStop
	{
		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogCopierTracer;
			}
		}

		internal PassiveBlockMode PassiveBlockMode
		{
			get
			{
				return this.m_passiveBlockMode;
			}
		}

		internal static string FormatLogGeneration(long gen)
		{
			return string.Format("0x{0:X}({0})", gen);
		}

		private static void ReadCallback(object asyncState, int bytesAvailable, bool completionIsSynchronous, Exception e)
		{
			LogCopier logCopier = (LogCopier)asyncState;
			logCopier.ProcessReadCallback(bytesAvailable, e);
		}

		private static void WakeUpCallback(object context)
		{
			LogCopier logCopier = (LogCopier)context;
			logCopier.ProcessWakeUp();
		}

		private static LogCopier FindCopier(string nodeName, Guid dbGuid, bool throwOnError)
		{
			LogCopier logCopier = null;
			IFindComponent componentFinder = Dependencies.ComponentFinder;
			if (componentFinder != null)
			{
				logCopier = componentFinder.FindLogCopier(nodeName, dbGuid);
			}
			if (logCopier == null)
			{
				LogCopier.Tracer.TraceError<Guid>(0L, "FindCopier failed to find Copier {0}", dbGuid);
				if (throwOnError)
				{
					throw new ReplayServiceUnknownReplicaInstanceException("FindCopier:Copier not active", dbGuid.ToString());
				}
			}
			return logCopier;
		}

		public static void TestDisconnectCopier(Guid dbGuid)
		{
			LogCopier logCopier = LogCopier.FindCopier(null, dbGuid, true);
			logCopier.TestDisconnect();
		}

		public LogCopier(IPerfmonCounters perfmonCounters, string fromPrefix, long fromNumber, string fromSuffix, string to, string toFinal, IReplayConfiguration replayConfiguration, FileState fileState, ISetBroken setBroken, ISetDisconnected setDisconnected, ISetGeneration setGeneration, NetworkPath netPath, bool runningAcll)
		{
			this.m_fromPrefix = fromPrefix;
			this.m_fromNumber = fromNumber;
			this.m_fromSuffix = fromSuffix;
			this.m_logCopierSetBroken = new ShipLogsSetBroken(setBroken, setDisconnected);
			this.m_setBroken = this.m_logCopierSetBroken;
			this.m_setDisconnected = this.m_logCopierSetBroken;
			this.m_setGeneration = setGeneration;
			this.m_fConstructedForAcll = runningAcll;
			this.m_initialNetPath = netPath;
			this.m_perfmonCounters = perfmonCounters;
			this.m_config = replayConfiguration;
			this.m_fileState = fileState;
			this.LocalNodeName = AmServerName.GetSimpleName(replayConfiguration.ServerName);
			LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "LogCopier constructed for database '{0}': SourceNode='{1}'. CopyTarget= '{2}'. FinalTarget= '{3}'. FromNumber= 0x{4:x}. FromPrefix='{5}'", new object[]
			{
				this.m_config.DatabaseName,
				this.m_config.SourceMachine,
				to,
				toFinal,
				fromNumber,
				fromPrefix
			});
			this.m_to = to;
			this.m_srv = this.m_config.GetAdServerObject();
			this.m_passiveBlockMode = new PassiveBlockMode(this, setBroken, this.GetMaxBlockModeDepthInBytes());
		}

		internal string LocalNodeName { get; private set; }

		private int GetMaxBlockModeDepthInBytes()
		{
			return PassiveBlockMode.GetMaxMemoryPerDatabase(this.m_srv);
		}

		internal void SetReportingCallbacks(ISetBroken setBroken, ISetDisconnected setDisconnected)
		{
			this.m_logCopierSetBroken.SetReportingCallbacksForAcll(setBroken, setDisconnected);
		}

		public IPerfmonCounters PerfmonCounters
		{
			get
			{
				return this.m_perfmonCounters;
			}
		}

		public IReplayConfiguration Configuration
		{
			get
			{
				return this.m_config;
			}
		}

		protected long FromNumber
		{
			get
			{
				return this.m_fromNumber;
			}
			set
			{
				this.m_fromNumber = value;
			}
		}

		public long NextGenExpected
		{
			get
			{
				return this.m_fromNumber;
			}
		}

		public void UpdateNextGenExpected(long nextGen)
		{
			this.m_fromNumber = nextGen;
		}

		protected string FromPrefix
		{
			get
			{
				return this.m_fromPrefix;
			}
		}

		protected string FromSuffix
		{
			get
			{
				return this.m_fromSuffix;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.m_config.IdentityGuid;
			}
		}

		internal string DatabaseName
		{
			get
			{
				return this.m_config.DatabaseName;
			}
		}

		public long HighestCopiedGeneration
		{
			get
			{
				if (this.FromNumber == 0L)
				{
					return 0L;
				}
				return this.FromNumber - 1L;
			}
		}

		private NetworkChannel Channel
		{
			get
			{
				return this.m_copyClient.Channel;
			}
		}

		private long LastKnownGeneration
		{
			get
			{
				return this.m_config.ReplayState.CopyNotificationGenerationNumber;
			}
		}

		internal void TraceDebug(string format, params object[] args)
		{
			if (ExTraceGlobals.LogCopierTracer.IsTraceEnabled(TraceType.DebugTrace))
			{
				string arg = string.Format(format, args);
				ExTraceGlobals.LogCopierTracer.TraceDebug<string, string>((long)this.GetHashCode(), "{0}: {1}", this.DatabaseName, arg);
			}
		}

		internal void TraceError(string format, params object[] args)
		{
			string arg = string.Format(format, args);
			ExTraceGlobals.LogCopierTracer.TraceError<string, string>((long)this.GetHashCode(), "{0}: {1}", this.DatabaseName, arg);
		}

		[Conditional("DEBUG")]
		private void AssertWorkerLockIsHeldByMe()
		{
		}

		private void TestDelaySleep()
		{
			int logCopyDelayInMsec = RegistryParameters.LogCopyDelayInMsec;
			if (logCopyDelayInMsec > 0)
			{
				Thread.Sleep(logCopyDelayInMsec);
			}
		}

		public void TrackInspectorGeneration(long releasedGeneration, DateTime writeTimeUtc)
		{
			if (releasedGeneration == 0L)
			{
				return;
			}
			this.m_highestGenReleasedToInspector = releasedGeneration;
			this.FromNumber = this.m_highestGenReleasedToInspector + 1L;
			if (this.m_setGeneration != null)
			{
				this.m_setGeneration.SetCopyGeneration(releasedGeneration, writeTimeUtc);
			}
		}

		public void TrackKnownEndOfLog(long highestKnownGeneration, DateTime writeTimeUtc)
		{
			if (this.m_setGeneration != null)
			{
				this.m_setGeneration.SetCopyNotificationGeneration(highestKnownGeneration, writeTimeUtc);
			}
		}

		public void TrackLastContactTime(DateTime lastContactUtc)
		{
			this.m_config.ReplayState.LatestCopierContactTime = lastContactUtc;
		}

		private void ReceiveLogFile(CopyLogReply incomingLog)
		{
			long thisLogGeneration = incomingLog.ThisLogGeneration;
			if (this.m_setGeneration != null)
			{
				this.m_setGeneration.SetCopyNotificationGeneration(incomingLog.EndOfLogGeneration, incomingLog.EndOfLogUtc);
			}
			this.TrackLastContactTime(incomingLog.MessageUtc);
			this.TraceDebug("ReceiveLogFile 0x{0:X}. EOL=0x{1:X}", new object[]
			{
				thisLogGeneration,
				incomingLog.EndOfLogGeneration
			});
			string text = EseHelper.MakeLogfileName(this.FromPrefix, this.FromSuffix, thisLogGeneration);
			string destFileName = Path.Combine(this.m_to, text);
			string path = 'S' + text.Substring(1);
			string text2 = Path.Combine(this.m_to, path);
			CheckSummer summer = null;
			if (this.Channel.ChecksumDataTransfer)
			{
				summer = new CheckSummer();
			}
			incomingLog.ReceiveFile(text2, this.m_perfmonCounters, summer);
			if (this.m_perfmonCounters != null)
			{
				this.m_perfmonCounters.RecordLogCopyThruput(incomingLog.FileSize);
			}
			File.Move(text2, destFileName);
			this.TrackInspectorGeneration(thisLogGeneration, incomingLog.LastWriteUtc);
		}

		public void CollectConnectionStatus(RpcDatabaseCopyStatus2 copyStatus)
		{
			string nodeNameFromFqdn = MachineName.GetNodeNameFromFqdn(this.m_config.SourceMachine);
			string network = null;
			LogCopyClient copyClient = this.m_copyClient;
			if (copyClient != null)
			{
				NetworkChannel channel = copyClient.Channel;
				if (channel != null)
				{
					NetworkPath networkPath = channel.NetworkPath;
					if (networkPath != null)
					{
						network = networkPath.NetworkName;
					}
				}
			}
			string lastFailure = null;
			if (this.m_failureOnNetwork && this.m_lastException != null)
			{
				lastFailure = this.m_lastException.Message;
			}
			ConnectionStatus obj = new ConnectionStatus(nodeNameFromFqdn, network, lastFailure, ConnectionDirection.Incoming, false);
			byte[] incomingLogCopyingNetwork = Serialization.ObjectToBytes(obj);
			copyStatus.IncomingLogCopyingNetwork = incomingLogCopyingNetwork;
		}

		private Exception CleanupOldConnection(int timeoutInMsec)
		{
			LogCopier.Tracer.TraceDebug<string>((long)this.GetHashCode(), "CleanupOldConnection({0})", this.DatabaseName);
			LogCopyClient curConnection = this.m_copyClient;
			Exception ex = NetworkChannel.RunNetworkFunction(delegate
			{
				if (curConnection != null)
				{
					if (this.m_readIsActive)
					{
						this.WaitForReadCallback(false, timeoutInMsec);
					}
					curConnection.Close();
				}
			});
			this.m_readIsActive = false;
			this.m_copyClient = null;
			this.m_incomingMessage = null;
			this.m_readCallbackException = null;
			this.m_connectionIsBeingDiscarded = false;
			if (ex != null)
			{
				LogCopier.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "CleanupOldConnection({0}) failed: {1}", this.DatabaseName, ex);
			}
			return ex;
		}

		private bool EstablishConnection()
		{
			if (this.m_connectionIsBeingDiscarded)
			{
				this.CleanupOldConnection(this.GetLogShipTimeoutMs());
			}
			if (this.m_copyClient == null)
			{
				LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "EstablishConnection establishing new connection");
				this.m_copyClient = new LogCopyClient(this.m_config, this.m_perfmonCounters, this.m_initialNetPath, this.GetLogShipTimeoutMs());
				this.m_initialNetPath = null;
				NetworkChannel networkChannel = this.m_copyClient.OpenChannel();
				this.m_perfmonCounters.CompressionEnabled = (networkChannel.IsCompressionEnabled ? 1L : 0L);
				this.m_perfmonCounters.EncryptionEnabled = (networkChannel.IsEncryptionEnabled ? 1L : 0L);
				this.m_lastSentPingCounter = 0L;
				this.m_setDisconnected.ClearDisconnected();
				IADServer server = Dependencies.ADConfig.GetServer(this.m_config.SourceMachine);
				bool flag = false;
				if (server == null)
				{
					LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "AD lookup for active {0} failed.", this.m_config.SourceMachine);
				}
				else if (RegistryParameters.TreatLogCopyPartnerAsDownlevel)
				{
					LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "TreatLogCopyPartnerAsDownlevel regkey forcing V1 repl");
				}
				else
				{
					ServerVersion serverVersion = new ServerVersion(server.VersionNumber);
					ServerVersion arg = new ServerVersion(this.m_config.ServerVersion);
					LogCopier.Tracer.TraceDebug<string, ServerVersion>((long)this.GetHashCode(), "Active {0} has version {1}", this.m_config.SourceMachine, serverVersion);
					LogCopier.Tracer.TraceDebug<ServerVersion>((long)this.GetHashCode(), "Local has version {0}", arg);
					if (ServerVersion.Compare(LogCopier.FirstVersionSupportingQueryVersion, serverVersion) <= 0)
					{
						flag = true;
					}
					else if (ServerVersion.Compare(LogCopier.FirstVersionSupportingV2, serverVersion) <= 0)
					{
						flag = true;
					}
				}
				if (!flag)
				{
					LogCopier.Tracer.TraceError((long)this.GetHashCode(), "Using V1 filemode");
					this.DeclareServerIsDownLevel();
				}
				this.m_passiveBlockMode.SetCrossSiteFlag(server, this.m_srv);
				return true;
			}
			return false;
		}

		private void CancelConnection()
		{
			LogCopyClient copyClient = this.m_copyClient;
			if (copyClient != null)
			{
				copyClient.Cancel();
			}
		}

		private void DiscardConnection()
		{
			LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "DiscardConnection entered");
			this.CancelConnection();
			this.m_connectionIsBeingDiscarded = true;
			LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "DiscardConnection exits");
		}

		private void Initialize()
		{
			long num = this.m_copyClient.QueryLogRange();
			if (num == 0L)
			{
				Exception ex = new LogCopierFailedNoLogsOnSourceException(this.m_config.SourceMachine);
				this.m_setBroken.SetBroken(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierFoundNoLogsOnSource, ex, new string[]
				{
					this.m_config.SourceMachine
				});
				throw ex;
			}
			if (this.m_fromNumber == 0L)
			{
				this.m_fromNumber = num;
				ReplayCrimsonEvents.LogCopierStartingWithLowestGenOnSource.Log<string, string, Guid, string>(this.m_config.DatabaseName, Environment.MachineName, this.m_config.DatabaseGuid, LogCopier.FormatLogGeneration(num));
			}
			else if (this.m_fromNumber < num)
			{
				bool flag;
				long num2;
				long num3;
				this.m_fileState.GetLowestAndHighestGenerationsRequired(out flag, out num2, out num3);
				if (flag || this.m_fromNumber >= num2)
				{
					string text = EseHelper.MakeLogfileName(this.FromPrefix, this.FromSuffix, this.m_fromNumber);
					Exception ex = new LogCopierFailsBecauseLogGapException(this.m_config.SourceMachine, text);
					this.m_setBroken.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogFileGapFound, ex, new string[]
					{
						text
					});
					throw ex;
				}
				this.TraceDebug("LogCopier is skipping past log generation {0} since it is lower than the required range (lowestReqGen = {2}).", new object[]
				{
					this.m_fromNumber,
					num2
				});
				long num4 = Math.Min(num2, num);
				this.m_setGeneration.SetLogStreamStartGeneration(num4);
				ReplayEventLogConstants.Tuple_LogFileCorruptOrGapFoundOutsideRequiredRange.LogEvent(null, new object[]
				{
					this.m_config.DatabaseName,
					EseHelper.MakeLogfileName(this.m_config.LogFilePrefix, this.m_config.LogFileSuffix, this.m_fromNumber),
					num4
				});
				this.m_setBroken.RestartInstanceSoon(true);
				throw new LogCopierInitFailedActiveTruncatingException(this.m_config.SourceMachine, this.m_fromNumber, num);
			}
			LogCopier.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "First generation to copy for db '{0}' is 0x{1:X8}", this.m_config.DatabaseName, this.FromNumber);
			this.m_highestGenReleasedToInspector = this.FromNumber - 1L;
		}

		private Exception InvokeWithCatch(LogCopier.CatchableOperation op)
		{
			Exception ex = null;
			try
			{
				op();
			}
			catch (GranularReplicationTerminatedException ex2)
			{
				ex = ex2;
			}
			catch (ArgumentException ex3)
			{
				ex = ex3;
			}
			catch (FileSharingViolationOnSourceException ex4)
			{
				ex = ex4;
			}
			catch (IOException ex5)
			{
				ex = ex5;
			}
			catch (NetworkRemoteException ex6)
			{
				ex = ex6;
			}
			catch (NetworkTransportException ex7)
			{
				ex = ex7;
			}
			catch (NotSupportedException ex8)
			{
				ex = ex8;
			}
			catch (ObjectDisposedException ex9)
			{
				ex = ex9;
			}
			catch (SecurityException ex10)
			{
				ex = ex10;
			}
			catch (UnauthorizedAccessException ex11)
			{
				ex = ex11;
			}
			catch (OperationAbortedException ex12)
			{
				ex = ex12;
			}
			catch (SetBrokenControlTransferException ex13)
			{
				ex = ex13;
			}
			catch (LogCopierInitFailedActiveTruncatingException ex14)
			{
				ex = ex14;
			}
			catch (TransientException ex15)
			{
				ex = ex15;
			}
			catch (OperationCanceledException ex16)
			{
				ex = ex16;
			}
			if (ex != null)
			{
				this.TraceError("InvokeWithCatch caught {0}", new object[]
				{
					ex
				});
			}
			return ex;
		}

		private void ShortRetryPolicy()
		{
			this.m_nextWait = this.m_lastWait + 10000;
			if (this.m_nextWait > 30000)
			{
				this.m_nextWait = 30000;
			}
		}

		private void HandleWorkerException(Exception e)
		{
			Exception ex;
			if (this.HandleWorkerExceptionInternal(e, out ex))
			{
				this.TraceError("Setting broken. Was remote exception: {0}", new object[]
				{
					this.m_failureOnSource
				});
				if (this.m_failureOnSource)
				{
					this.SetBrokenDueToSource(ex, false);
					return;
				}
				this.SetBrokenDueToTarget(ex);
			}
		}

		private bool HandleWorkerExceptionInternal(Exception e, out Exception exForBroken)
		{
			bool flag = false;
			exForBroken = e;
			if (e is LogCopierInitFailedActiveTruncatingException)
			{
				return this.RunningAcll;
			}
			if (e is SetBrokenControlTransferException)
			{
				return flag;
			}
			if (this.m_setBroken.IsBroken)
			{
				return flag;
			}
			this.m_failureOnSource = false;
			this.m_failureOnNetwork = false;
			if (e is NetworkRemoteException)
			{
				this.m_failureOnSource = true;
				this.m_resetAfterError = true;
				e = e.InnerException;
				exForBroken = e;
				LogCopier.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Network forwarded remote exception from the source: {0}", e);
			}
			else
			{
				LogCopier.Tracer.TraceError<Exception>((long)this.GetHashCode(), "HandleWorkerException {0}", e);
			}
			this.m_lastException = e;
			if (e is GranularReplicationTerminatedException || e is GranularReplicationOverflowException)
			{
				return true;
			}
			if (e is NetworkTransportException)
			{
				this.m_failureOnNetwork = true;
				this.ShortRetryPolicy();
				this.DiscardConnection();
				this.m_setDisconnected.SetDisconnected(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierFailedToCommunicate, new string[]
				{
					this.m_config.SourceMachine,
					e.Message
				});
				return false;
			}
			if (e is SourceDatabaseNotFoundException)
			{
				this.ShortRetryPolicy();
				this.DiscardConnection();
				this.m_setDisconnected.SetDisconnected(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierReceivedSourceSideError, new string[]
				{
					this.m_config.SourceMachine,
					e.Message
				});
				return false;
			}
			flag = this.RunningAcll;
			if (this.m_failureOnSource)
			{
				Exception ex;
				if (e.TryGetExceptionOrInnerOfType(out ex) || e.TryGetExceptionOrInnerOfType(out ex) || e.TryGetExceptionOrInnerOfType(out ex))
				{
					long fromNumber = this.FromNumber;
					this.TraceError("LogCopier got source-side corrupt or missing log file exception for log gen {0}. Exception: {1}", new object[]
					{
						fromNumber,
						ex.Message
					});
					bool flag2;
					long num;
					long num2;
					this.m_fileState.GetLowestAndHighestGenerationsRequired(out flag2, out num, out num2);
					this.TraceDebug("Database IsConsistent={0}, LowestRequiredGen={1}, HighestRequiredGen={2}", new object[]
					{
						flag2,
						num,
						num2
					});
					if (flag2 || fromNumber > num2)
					{
						if (ex is SourceLogBreakStallsPassiveException)
						{
							this.TraceError("LogCopier is stalled by exception on source: {0}", new object[]
							{
								ex.Message
							});
							this.m_stalledDueToSourceLogBreak = true;
							this.m_sourceLogCorruptEx = ex;
							this.m_stalledSince = new DateTime?(DateTime.UtcNow);
							ReplayEventLogConstants.Tuple_LogCopierIsStalledDueToSource.LogEvent(null, new object[]
							{
								this.m_config.DatabaseName,
								this.m_config.SourceMachine,
								ex.Message
							});
							return flag;
						}
						if (ex is CorruptLogDetectedException)
						{
							this.m_sourceLogCorruptEx = ex;
							this.m_setBroken.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogCopierIsStalledDueToSource, ex, new string[]
							{
								this.m_config.SourceMachine,
								ex.Message
							});
							return false;
						}
						if (ex is FileNotFoundException)
						{
							e = ex;
							exForBroken = e;
						}
					}
					else
					{
						if (fromNumber < num)
						{
							this.TraceDebug("LogCopier is skipping past log generation {0} since it is lower than the required range.", new object[]
							{
								fromNumber
							});
							long num3 = Math.Min(num, fromNumber + 1L);
							this.m_setGeneration.SetLogStreamStartGeneration(num3);
							ReplayEventLogConstants.Tuple_LogFileCorruptOrGapFoundOutsideRequiredRange.LogEvent(null, new object[]
							{
								this.m_config.DatabaseName,
								EseHelper.MakeLogfileName(this.m_config.LogFilePrefix, this.m_config.LogFileSuffix, fromNumber),
								num3
							});
							this.m_setBroken.RestartInstanceSoon(true);
							return false;
						}
						this.m_sourceLogCorruptEx = ex;
						exForBroken = ex;
						this.SetBrokenDueToSource(ex, true);
						return false;
					}
				}
				else if (e is FileIOonSourceException)
				{
					e = e.InnerException;
					exForBroken = e;
				}
			}
			if (!flag && (e is ArgumentException || e is PathTooLongException || e is ObjectDisposedException || e is OperationCanceledException || e is OperationAbortedException || e is NotSupportedException || e is AcllFailedException))
			{
				flag = true;
			}
			if (!flag)
			{
				if (this.m_failureOnSource)
				{
					ReplayEventLogConstants.Tuple_LogCopierReceivedSourceSideError.LogEvent(this.m_config.Identity, new object[]
					{
						this.m_config.DatabaseName,
						this.m_config.SourceMachine,
						e.Message
					});
				}
				this.m_totalWait += this.m_lastWait;
				int num4 = this.m_failureOnSource ? (RegistryParameters.LogCopierStalledToFailedThresholdInSecs * 1000) : 30000;
				if (this.m_totalWait < num4)
				{
					this.ShortRetryPolicy();
					return flag;
				}
				this.TraceError("Retry time expired", new object[0]);
				flag = true;
				if (!this.m_failureOnSource)
				{
					IOException ex2 = e as IOException;
					if (ex2 != null && FileOperations.IsDiskFullException(ex2))
					{
						ReplayEventLogConstants.Tuple_LogCopierBlockedByFullDisk.LogEvent(this.m_config.Identity, new object[]
						{
							this.m_config.DatabaseName,
							this.m_to
						});
					}
				}
			}
			this.TraceError("LogCopier.HandleWorkerExceptionInternal returning '{0}' for exception: {1}", new object[]
			{
				flag,
				e
			});
			return flag;
		}

		private void SetBrokenDueToSource(Exception e, bool passiveNeedsReseed)
		{
			if (e is FileNotFoundException)
			{
				this.m_setBroken.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogFileGapFound, e, new string[]
				{
					((FileNotFoundException)e).FileName
				});
				return;
			}
			this.m_setBroken.SetBroken(passiveNeedsReseed ? FailureTag.Reseed : FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierFailedDueToSource, e, new string[]
			{
				this.m_config.SourceMachine,
				e.GetMessageWithHResult()
			});
		}

		private void SetBrokenDueToTarget(Exception e)
		{
			FailureTag failureTag = FailureTag.NoOp;
			IOException ex = e as IOException;
			if (ex != null)
			{
				if (FileOperations.IsDiskFullException(ex))
				{
					failureTag = FailureTag.Space;
				}
				else if (ex is DirectoryNotFoundException || ex is FileNotFoundException || ex is EndOfStreamException || ex is DriveNotFoundException || ex is PathTooLongException || ex is FileLoadException)
				{
					failureTag = FailureTag.AlertOnly;
				}
			}
			else if (e is SecurityException || e is UnauthorizedAccessException)
			{
				failureTag = FailureTag.AlertOnly;
			}
			this.m_setBroken.SetBroken(failureTag, ReplayEventLogConstants.Tuple_LogCopierFailedDueToTarget, e, new string[]
			{
				e.GetMessageWithHResult()
			});
		}

		private void WorkerEntryPoint()
		{
			this.m_workerIsPreparingToExit = false;
			if (this.m_prepareToStopCalled)
			{
				this.TraceDebug("WorkerEntryPoint(): Bailing because PrepareToStop has been called", new object[0]);
				return;
			}
			this.DisableWakeUp();
			if (this.m_passiveBlockMode.IsBlockModeActive)
			{
				this.TraceDebug("WorkerEntryPoint: exits because BlockMode is active", new object[0]);
				return;
			}
			if (this.m_setBroken.IsBroken)
			{
				this.TraceDebug("WorkerEntryPoint(): Bailing because SetBroken() has been called", new object[0]);
				return;
			}
			if (this.RunningAcll)
			{
				this.TraceDebug("WorkerEntryPoint(): Bailing because ACLL is going to run.", new object[0]);
				return;
			}
			if (this.m_disconnectedForTest)
			{
				this.TraceDebug("WorkerEntryPoint(): Bailing because m_disconnectedForTest is set", new object[0]);
				return;
			}
			this.m_nextWait = RegistryParameters.LogShipTimeoutInMsec;
			Exception ex = this.InvokeWithCatch(delegate
			{
				if (!this.m_initialized)
				{
					this.EstablishConnection();
					this.Initialize();
					this.m_initialized = true;
					this.SendLogRequest(true, false);
					this.GetResponse();
				}
				else if (this.m_readIsActive)
				{
					if (this.m_readCompleteEvent.WaitOne(0, false))
					{
						this.m_readIsActive = false;
						this.m_waitingForHealthCheck = false;
						if (this.m_readCallbackException != null)
						{
							Exception readCallbackException = this.m_readCallbackException;
							this.m_readCallbackException = null;
							throw readCallbackException;
						}
						this.GetResponse();
					}
					else
					{
						this.m_responseTimer.Stop();
						long elapsedMilliseconds = this.m_responseTimer.ElapsedMilliseconds;
						if (this.m_waitingForHealthCheck)
						{
							this.m_waitingForHealthCheck = false;
							LogCopier.Tracer.TraceError<long>((long)this.GetHashCode(), "Timeout after {0} ms", elapsedMilliseconds);
							throw new NetworkTimeoutException(this.m_config.SourceMachine, ReplayStrings.NetworkReadTimeout((int)(elapsedMilliseconds / 1000L)));
						}
						this.TraceDebug("Log Copy has been idle for {0} ms. Requesting an immediate reponse from the source.", new object[]
						{
							elapsedMilliseconds
						});
						this.SendLogRequest(false, false);
						this.m_waitingForHealthCheck = true;
						return;
					}
				}
				else if (this.m_incomingMessage == null)
				{
					this.TraceError("WorkEntryPoint has no input. This should be an error retry.", new object[0]);
					bool sendInitialRequest = this.EstablishConnection();
					this.SendLogRequest(sendInitialRequest, false);
					this.GetResponse();
				}
				this.ProcessMessage();
				this.TestDelaySleep();
				if (this.RunningAcll)
				{
					this.TraceDebug("Exiting WorkEntryPoint because ACLL is active", new object[0]);
					return;
				}
				if (this.UsePullModel())
				{
					this.TraceDebug("pull model, next log will be 0x{0:X}", new object[]
					{
						this.FromNumber
					});
					this.SendLogRequest(true, false);
				}
				this.m_workerIsPreparingToExit = true;
				if (!this.PreparingToEnterBlockMode)
				{
					this.StartRead();
				}
			});
			if (ex != null)
			{
				this.HandleWorkerException(ex);
			}
			this.ScheduleWakeUp();
			if (this.m_workerIsScheduled)
			{
				this.TraceDebug("WorkerEntryPoint exits and a wakeup scheduled in {0}mSec", new object[]
				{
					this.m_nextWait
				});
				return;
			}
			this.TraceError("WorkerEntryPoint exits with no work scheduled", new object[0]);
		}

		private bool UsePullModel()
		{
			return RegistryParameters.LogCopyPull != 0;
		}

		private void DeclareServerIsDownLevel()
		{
			this.m_serverIsDownlevel = true;
		}

		private void SendLogRequest(bool sendInitialRequest, bool fRunningAcll = false)
		{
			if (this.m_serverIsDownlevel)
			{
				this.SendV1LogRequest(fRunningAcll);
				return;
			}
			if (sendInitialRequest || this.m_resetAfterError)
			{
				this.SendInitialRequest(fRunningAcll);
				this.m_resetAfterError = false;
				return;
			}
			this.SendPing();
		}

		private void SendInitialRequest(bool fRunningAcll = false)
		{
			ContinuousLogCopyRequest2.Flags flags = ContinuousLogCopyRequest2.Flags.None;
			if (fRunningAcll)
			{
				flags |= ContinuousLogCopyRequest2.Flags.ForAcll;
				this.TraceDebug("SendLogRequest with ForAcll flag", new object[0]);
			}
			else if (GranularReplication.IsEnabled())
			{
				flags |= ContinuousLogCopyRequest2.Flags.UseGranular;
				this.TraceDebug("SendLogRequest with IsGranularReplicationEnabled flag", new object[0]);
			}
			ContinuousLogCopyRequest2 continuousLogCopyRequest = new ContinuousLogCopyRequest2(this.LocalNodeName, this.Channel, this.DatabaseGuid, this.FromNumber, flags);
			if (this.UsePullModel())
			{
				continuousLogCopyRequest.LastGeneration = this.FromNumber;
			}
			this.m_responseIsBeingTimed = true;
			this.m_responseTimer.Reset();
			this.m_responseTimer.Start();
			continuousLogCopyRequest.Send();
		}

		private void SendV1LogRequest(bool fRunningAcll = false)
		{
			ContinuousLogCopyRequest.Flags flags = ContinuousLogCopyRequest.Flags.None;
			if (fRunningAcll)
			{
				flags |= ContinuousLogCopyRequest.Flags.ForAcll;
				this.TraceDebug("SendLogRequest with ForAcll flag", new object[0]);
			}
			long lastLogNum = 0L;
			if (this.UsePullModel())
			{
				lastLogNum = this.FromNumber;
			}
			ContinuousLogCopyRequest continuousLogCopyRequest = new ContinuousLogCopyRequest(this.Channel, this.DatabaseGuid, this.FromNumber, lastLogNum, flags);
			this.m_responseIsBeingTimed = true;
			this.m_responseTimer.Reset();
			this.m_responseTimer.Start();
			continuousLogCopyRequest.Send();
		}

		private void GetResponse()
		{
			this.m_incomingMessage = this.Channel.GetMessage();
			this.m_responseTimer.Stop();
			long elapsedMilliseconds = this.m_responseTimer.ElapsedMilliseconds;
			if (this.m_responseIsBeingTimed)
			{
				ExTraceGlobals.LogCopierTracer.TraceDebug<long>((long)this.GetHashCode(), "Log Copy Response took: {0} ms", elapsedMilliseconds);
				this.m_responseIsBeingTimed = false;
				return;
			}
			this.TraceDebug("Copier was idle for : {0} ms", new object[]
			{
				elapsedMilliseconds
			});
		}

		private void ReportHealthy()
		{
			this.m_totalWait = 0;
		}

		private NetworkChannelMessage ProcessMessage()
		{
			NetworkChannelMessage networkChannelMessage = null;
			if (!this.m_prepareToStopCalled && !this.m_setBroken.IsBroken)
			{
				if (this.RunningAcll && !this.m_fAcllHasControl)
				{
					this.TraceDebug("ProcessMessage() exiting because ACLL has been requested on another thread. Control should transfer to ACLL.", new object[0]);
				}
				else
				{
					if (this.m_incomingMessage == null)
					{
						this.m_incomingMessage = this.Channel.GetMessage();
					}
					networkChannelMessage = this.m_incomingMessage;
					this.m_incomingMessage = null;
					CopyLogReply copyLogReply = networkChannelMessage as CopyLogReply;
					if (copyLogReply != null)
					{
						this.m_perfmonCounters.GranularReplication = 0L;
						if (copyLogReply.ThisLogGeneration != this.FromNumber)
						{
							if (copyLogReply.ThisLogGeneration != 0L || !this.RunningAcll)
							{
								string text = string.Format("Unexpected log received: 0x{0:X}, expected: 0x{1:X}", copyLogReply.ThisLogGeneration, this.FromNumber);
								this.TraceError(text, new object[0]);
								throw new NetworkUnexpectedMessageException(this.Channel.PartnerNodeName, text);
							}
							this.TraceDebug("e00 is incoming", new object[0]);
						}
						this.ReceiveLogFile(copyLogReply);
						if (copyLogReply.ThisLogGeneration == 0L)
						{
							this.m_acllSuccess = true;
							return networkChannelMessage;
						}
						this.m_setGeneration.ClearLogStreamStartGeneration();
					}
					else if (networkChannelMessage is EnterBlockModeMsg)
					{
						this.PrepareToEnterBlockMode(networkChannelMessage as EnterBlockModeMsg);
					}
					else
					{
						NotifyEndOfLogReply notifyEndOfLogReply = networkChannelMessage as NotifyEndOfLogReply;
						if (notifyEndOfLogReply != null)
						{
							this.m_setGeneration.SetCopyNotificationGeneration(notifyEndOfLogReply.EndOfLogGeneration, notifyEndOfLogReply.EndOfLogUtc);
							this.TrackLastContactTime(notifyEndOfLogReply.MessageUtc);
							this.TraceDebug("We are in sync with a V1 server. Target waiting for 0x{0:x} Source has 0x{1:x}", new object[]
							{
								this.FromNumber,
								notifyEndOfLogReply.EndOfLogGeneration
							});
						}
						else
						{
							this.Channel.ThrowUnexpectedMessage(networkChannelMessage);
						}
					}
					this.ReportHealthy();
				}
			}
			return networkChannelMessage;
		}

		private void StartRead()
		{
			this.TraceDebug("Starting async read", new object[0]);
			if (this.m_readCompleteEvent == null)
			{
				this.m_readCompleteEvent = new ManualResetEvent(true);
			}
			this.m_responseTimer.Reset();
			this.m_responseTimer.Start();
			this.m_readIsActive = true;
			this.m_readCompleteEvent.Reset();
			bool flag = false;
			try
			{
				this.Channel.StartRead(new NetworkChannelCallback(LogCopier.ReadCallback), this);
				flag = true;
				this.m_nextWait = this.m_copyClient.DefaultTimeoutInMs;
			}
			finally
			{
				if (!flag)
				{
					this.TraceError("Failed to starting async read", new object[0]);
					this.m_readCompleteEvent.Set();
					this.m_readIsActive = false;
				}
			}
		}

		private void ProcessReadCallback(int bytesAvailable, Exception e)
		{
			this.TraceDebug("ProcessReadCallback. bytesAvailable={0},ex={1}", new object[]
			{
				bytesAvailable,
				e
			});
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			try
			{
				flag3 = Monitor.TryEnter(this.m_globalWorkerLock);
				if (!flag3 && this.m_workerIsPreparingToExit && !this.m_connectionIsBeingDiscarded)
				{
					flag2 = true;
					this.TraceDebug("ReadCallback will need to wait for the lock and do work", new object[0]);
				}
				if (e != null)
				{
					this.m_readCallbackException = new NetworkCommunicationException(this.m_config.SourceMachine, e.Message, e);
				}
				else if (bytesAvailable == 0)
				{
					this.m_readCallbackException = new NetworkEndOfDataException(this.m_config.SourceMachine, ReplayStrings.NetworkReadEOF);
				}
				this.m_readCompleteEvent.Set();
				flag = true;
				if (flag2)
				{
					Monitor.Enter(this.m_globalWorkerLock);
					flag3 = true;
				}
				if (flag3)
				{
					this.WorkerEntryPoint();
				}
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(this.m_globalWorkerLock);
				}
				if (!flag)
				{
					this.m_readCompleteEvent.Set();
				}
			}
		}

		private void WaitForReadCallback()
		{
			if (this.m_copyClient != null)
			{
				int defaultTimeoutInMs = this.m_copyClient.DefaultTimeoutInMs;
				this.WaitForReadCallback(false, defaultTimeoutInMs);
			}
		}

		private void WaitForReadCallback(bool waitForever, int msTimeout)
		{
			if (this.m_readCompleteEvent != null)
			{
				bool flag = false;
				while (!this.m_readCompleteEvent.WaitOne(msTimeout, false))
				{
					this.TraceError("Timeout waiting for ReadCallback", new object[0]);
					if (!waitForever)
					{
						this.Channel.ThrowTimeoutException(ReplayStrings.NetworkReadTimeout(msTimeout / 1000));
					}
					if (flag)
					{
						string message = string.Format("LogCopier({0}).WaitForReadCallback timeout after {1} sec", this.DatabaseName, msTimeout / 1000);
						LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), message);
						TimeoutException exception = new TimeoutException(message);
						ExWatson.SendReportAndCrashOnAnotherThread(exception);
						DiagCore.RetailAssert(false, "Crash request must not return", new object[0]);
						break;
					}
					flag = true;
					this.CancelConnection();
					msTimeout = RegistryParameters.LogCopierHungIoLimitInMsec;
				}
				this.TraceDebug("WaitForReadCallback is successful, marking read as not active", new object[0]);
				this.m_readIsActive = false;
			}
		}

		private void ProcessWakeUp()
		{
			if (Monitor.TryEnter(this.m_globalWorkerLock))
			{
				try
				{
					this.TraceDebug("ProcessWakeUp is running as the worker", new object[0]);
					if (this.PreparingToEnterBlockMode)
					{
						this.TraceError("Timed out waiting to enter blockMode", new object[0]);
						this.PreparingToEnterBlockMode = false;
					}
					if (!this.m_stalledDueToSourceLogBreak)
					{
						this.WorkerEntryPoint();
						return;
					}
					TimeSpan timeSpan = DateTime.UtcNow.Subtract(this.m_stalledSince.Value);
					string arg = timeSpan.ToString();
					LogCopier.Tracer.TraceError<string, string>((long)this.GetHashCode(), "{0}:Stalled for {1}", this.DatabaseName, arg);
					if (RegistryParameters.LogCopierStalledToFailedThresholdInSecs > 0 && timeSpan.TotalSeconds > (double)RegistryParameters.LogCopierStalledToFailedThresholdInSecs)
					{
						this.DisableWakeUp();
						this.m_setBroken.SetBroken(FailureTag.Reseed, ReplayEventLogConstants.Tuple_LogCopierIsStalledDueToSource, this.m_sourceLogCorruptEx, new string[]
						{
							this.m_config.SourceMachine,
							this.m_sourceLogCorruptEx.Message
						});
						return;
					}
					this.ScheduleWakeUp();
					return;
				}
				finally
				{
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
			this.TraceDebug("ProcessWakeUp found the worker busy", new object[0]);
		}

		private void DisableWakeUp()
		{
			this.m_workerIsScheduled = false;
			if (this.m_wakeTimer != null)
			{
				this.m_wakeTimer.Change(-1, -1);
			}
		}

		private void ScheduleWakeUp()
		{
			if (this.m_wakeTimer != null && !this.m_prepareToStopCalled && !this.m_setBroken.IsBroken && !this.RunningAcll)
			{
				this.m_lastWait = this.m_nextWait;
				this.m_wakeTimer.Change(this.m_nextWait, this.m_nextWait + 10000);
				this.m_workerIsScheduled = true;
				return;
			}
			this.TraceDebug("ScheduleWakeUp is disabled", new object[0]);
		}

		public void Start()
		{
			this.m_wakeTimer = new Timer(new TimerCallback(LogCopier.WakeUpCallback), this, 0, -1);
		}

		public void PrepareToStop()
		{
			this.TraceDebug("PrepareToStop invoked.", new object[0]);
			lock (this)
			{
				if (!this.m_prepareToStopCalled)
				{
					this.m_prepareToStopCalled = true;
					this.CancelConnection();
					this.TerminateBlockMode();
				}
			}
		}

		private bool PrepareToStopCalled
		{
			get
			{
				return this.m_prepareToStopCalled;
			}
		}

		public void Stop()
		{
			this.TraceDebug("Stop invoked", new object[0]);
			bool flag = false;
			lock (this)
			{
				if (!this.m_stopped)
				{
					if (!this.m_prepareToStopCalled)
					{
						this.PrepareToStop();
					}
					this.m_stopped = true;
					flag = true;
				}
			}
			if (flag)
			{
				bool flag3 = false;
				try
				{
					Monitor.Enter(this.m_globalWorkerLock);
					flag3 = true;
					this.m_workerIsPreparingToExit = false;
					this.DisableWakeUp();
					int logShipTimeoutMs = this.GetLogShipTimeoutMs();
					this.WaitForReadCallback(true, logShipTimeoutMs);
					this.CleanupOldConnection(logShipTimeoutMs);
				}
				finally
				{
					if (this.m_wakeTimer != null)
					{
						this.m_wakeTimer.Dispose();
					}
					if (flag3)
					{
						Monitor.Exit(this.m_globalWorkerLock);
					}
				}
				this.TerminateBlockMode();
				this.m_passiveBlockMode.Destroy();
			}
		}

		private bool RunningAcll
		{
			get
			{
				return this.m_fAttemptFinalCopyCalled;
			}
		}

		public bool GranuleUsedAsE00 { get; private set; }

		public Result AttemptFinalCopy(AcllPerformanceTracker acllPerf, out LocalizedString errorString)
		{
			LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "AttemptFinalCopy called");
			ExTraceGlobals.PFDTracer.TracePfd<int>((long)this.GetHashCode(), "PFD CRS {0} AttemptFinalCopy called", 23003);
			errorString = LocalizedString.Empty;
			Result result = Result.GiveUp;
			this.RunAcll(acllPerf);
			bool flag = true;
			if (this.IsBroken)
			{
				errorString = this.m_setBroken.ErrorMessage;
			}
			else if (this.IsDisconnected)
			{
				errorString = this.m_setDisconnected.ErrorMessage;
			}
			else if (this.PrepareToStopCalled)
			{
				errorString = ReplayStrings.PrepareToStopCalled;
				flag = false;
			}
			else if (!this.m_acllSuccess)
			{
				errorString = new LocalizedString(this.m_lastException.Message);
			}
			else
			{
				result = Result.Success;
				flag = false;
			}
			if (flag && this.m_passiveBlockMode.UsePartialLogsDuringAcll(this.FromNumber))
			{
				this.GranuleUsedAsE00 = true;
			}
			return result;
		}

		private bool IsBroken
		{
			get
			{
				return this.m_setBroken.IsBroken;
			}
		}

		private bool IsDisconnected
		{
			get
			{
				return this.m_setDisconnected.IsDisconnected;
			}
		}

		private void RunAcll(AcllPerformanceTracker acllPerf)
		{
			this.m_fAttemptFinalCopyCalled = true;
			this.m_fAcllHasControl = false;
			bool held = false;
			try
			{
				acllPerf.RunTimedOperation(AcllTimedOperation.AcllEnterLogCopierWorkerLock, delegate
				{
					if (Monitor.TryEnter(this.m_globalWorkerLock, RegistryParameters.LogShipACLLTimeoutInMsec))
					{
						held = true;
						return;
					}
					LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "we failed to gain control over the logCopier");
					acllPerf.IsAcllCouldNotControlLogCopier = true;
					ReplayCrimsonEvents.AcllCouldNotControlCopier.Log<string, AmServerName>(this.DatabaseName, AmServerName.LocalComputerName);
				});
				if (!held || this.m_disconnectedForTest)
				{
					NetworkTimeoutException ex = new NetworkTimeoutException(this.m_config.SourceMachine, ReplayStrings.NetworkConnectionTimeout(this.LogShipACLLTimeoutInSecs));
					if (this.m_lastException == null)
					{
						this.m_lastException = ex;
					}
					this.m_setDisconnected.SetDisconnected(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierFailedToCommunicate, new string[]
					{
						this.m_config.SourceMachine,
						ex.Message
					});
				}
				else
				{
					this.m_workerIsPreparingToExit = false;
					this.m_fAcllHasControl = true;
					if (this.PrepareForAcll(acllPerf))
					{
						this.ReadAcllMessages(acllPerf);
					}
				}
			}
			finally
			{
				if (held)
				{
					this.m_fAcllHasControl = false;
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
		}

		private bool PrepareForAcll(AcllPerformanceTracker acllPerf)
		{
			bool success = false;
			Exception ex = this.InvokeWithCatch(delegate
			{
				this.DisableWakeUp();
				if (this.m_stalledDueToSourceLogBreak)
				{
					this.SetBrokenDueToSource(this.m_lastException, false);
					return;
				}
				ExTraceGlobals.FaultInjectionTracer.TraceTest(4167445821U);
				int timeoutInMsec = this.GetLogShipTimeoutMs();
				bool isBlockModeActive = this.m_passiveBlockMode.IsBlockModeActive;
				if (isBlockModeActive)
				{
					this.m_passiveBlockMode.PrepareForAcll();
				}
				if (this.m_connectionIsBeingDiscarded)
				{
					if (!isBlockModeActive)
					{
						ReplayCrimsonEvents.AcllFoundDeadConnection.Log<string, AmServerName>(this.DatabaseName, AmServerName.LocalComputerName);
					}
					this.CleanupOldConnection(timeoutInMsec);
				}
				if (this.m_copyClient != null)
				{
					this.m_copyClient.SetTimeoutInMsec(this.GetLogShipTimeoutMs());
				}
				this.EstablishConnection();
				acllPerf.IsLogCopierInitializedForAcll = this.m_initialized;
				if (!this.m_initialized)
				{
					acllPerf.RunTimedOperation(AcllTimedOperation.AcllLogCopierFirstInit, delegate
					{
						this.Initialize();
					});
					this.m_initialized = true;
				}
				if (isBlockModeActive)
				{
					this.m_passiveBlockMode.FinishForAcll(timeoutInMsec);
				}
				this.SendLogRequest(true, true);
				acllPerf.RunTimedOperation(AcllTimedOperation.AcllInitWaitForReadCallback, delegate
				{
					this.WaitForReadCallback(false, timeoutInMsec);
				});
				success = true;
			});
			if (ex != null)
			{
				LogCopier.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "PrepareForAcll({0}) failed: {1}", this.DatabaseName, ex);
				this.HandleWorkerException(ex);
			}
			return success;
		}

		private void ReadAcllMessages(AcllPerformanceTracker acllPerf)
		{
			LogCopier.Tracer.TraceDebug<string>((long)this.GetHashCode(), "ReadAcllMessages({0}) invoked", this.DatabaseName);
			Exception ex = this.InvokeWithCatch(delegate
			{
				if (this.m_incomingMessage == null)
				{
					this.GetResponse();
				}
				for (;;)
				{
					NetworkChannelMessage networkChannelMessage = this.ProcessMessage();
					if (this.m_acllSuccess)
					{
						break;
					}
					if (networkChannelMessage == null)
					{
						goto Block_3;
					}
					if (!(networkChannelMessage is CopyLogReply) && !(networkChannelMessage is NotifyEndOfLogReply) && !(networkChannelMessage is EnterBlockModeMsg))
					{
						this.Channel.ThrowUnexpectedMessage(networkChannelMessage);
					}
					this.TestDelaySleep();
					if (this.UsePullModel())
					{
						this.TraceDebug("pull model, next log will be 0x{0:X}", new object[]
						{
							this.FromNumber
						});
						this.SendLogRequest(true, true);
					}
				}
				return;
				Block_3:
				LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "ReadAcllMessages({0}) No message. Abnormal exit", this.DatabaseName);
			});
			if (ex != null)
			{
				LogCopier.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "PrepareForAcll({0}) failed: {1}", this.DatabaseName, ex);
				this.HandleWorkerException(ex);
			}
		}

		private int GetLogShipTimeoutMs()
		{
			return LogSource.GetLogShipTimeoutInMsec(this.RunningAcll || this.m_fConstructedForAcll);
		}

		private void SendPing()
		{
			this.m_lastSentPingCounter = Win32StopWatch.GetSystemPerformanceCounter();
			PingMessage pingMessage = new PingMessage(this.Channel);
			pingMessage.RequestAckCounter = this.m_lastSentPingCounter;
			LogCopier.Tracer.TraceDebug<long>((long)this.GetHashCode(), "SendPing: expecting ack with 0x{0:X}", this.m_lastSentPingCounter);
			pingMessage.Send();
		}

		private void TerminateBlockMode()
		{
			this.m_passiveBlockMode.Terminate();
		}

		private bool PreparingToEnterBlockMode { get; set; }

		private void PrepareToEnterBlockMode(EnterBlockModeMsg msg)
		{
			LogCopier.Tracer.TraceDebug<string>((long)this.GetHashCode(), "PrepareToEnterBlockMode({0}) received", this.DatabaseName);
			if (msg.FirstGenerationToExpect != this.FromNumber)
			{
				string text = string.Format("PrepareToEnterBlockMode({0}) failed. ExpectedGen(0x{1:X}) ServerWillSend(0x{2:X})", this.DatabaseName, this.FromNumber, msg.FirstGenerationToExpect);
				LogCopier.Tracer.TraceError((long)this.GetHashCode(), text);
				throw new NetworkUnexpectedMessageException(this.Channel.PartnerNodeName, text);
			}
			if (this.RunningAcll)
			{
				LogCopier.Tracer.TraceError((long)this.GetHashCode(), "Rejecting blockmode due to ACLL");
				msg.FlagsUsed = EnterBlockModeMsg.Flags.PassiveReject;
				msg.Send();
				return;
			}
			msg.FlagsUsed = EnterBlockModeMsg.Flags.PassiveIsReady;
			msg.Send();
			this.PreparingToEnterBlockMode = true;
			this.m_nextWait = 3 * RegistryParameters.LogShipTimeoutInMsec;
		}

		public static void EnterBlockMode(EnterBlockModeMsg msg, NetworkChannel channel)
		{
			LogCopier logCopier = LogCopier.FindCopier(channel.LocalNodeName, msg.DatabaseGuid, false);
			if (logCopier == null)
			{
				LogCopier.Tracer.TraceError<string>(0L, "EnterBlockMode fails because copier doesn't exist. Active={0}", msg.ActiveNodeName);
				return;
			}
			logCopier.EnterBlockModeInternal(msg, channel);
		}

		private void EnterBlockModeInternal(EnterBlockModeMsg msg, NetworkChannel channel)
		{
			if (this.RunningAcll)
			{
				LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "EnterBlockMode({0}) rejected due to ACLL", this.DatabaseName);
				return;
			}
			if (Monitor.TryEnter(this.m_globalWorkerLock, RegistryParameters.LogShipACLLTimeoutInMsec))
			{
				try
				{
					if (!this.PreparingToEnterBlockMode)
					{
						LogCopier.Tracer.TraceError((long)this.GetHashCode(), "EnterBlockMode fails because Preparing was cancelled");
						return;
					}
					if (this.m_prepareToStopCalled)
					{
						LogCopier.Tracer.TraceError((long)this.GetHashCode(), "EnterBlockMode fails because we are stopping.");
						return;
					}
					if (this.m_passiveBlockMode.EnterBlockMode(msg, channel, this.GetMaxBlockModeDepthInBytes()))
					{
						this.DisableWakeUp();
						this.DiscardConnection();
					}
					return;
				}
				finally
				{
					this.PreparingToEnterBlockMode = false;
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
			LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "EnterBlockMode({0}) failed to obtain worker lock", this.DatabaseName);
		}

		public void NotifyBlockModeTermination()
		{
			if (this.RunningAcll)
			{
				LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "NotifyBlockModeTermination({0}) found ACLL active.", this.DatabaseName);
				return;
			}
			if (Monitor.TryEnter(this.m_globalWorkerLock, RegistryParameters.LogShipTimeoutInMsec))
			{
				try
				{
					this.m_nextWait = 0;
					this.ScheduleWakeUp();
					return;
				}
				finally
				{
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
			LogCopier.Tracer.TraceError<string>((long)this.GetHashCode(), "NotifyBlockModeTermination({0}) failed to obtain worker lock", this.DatabaseName);
			ReplayCrimsonEvents.BlockModeTerminationLockConflict.Log<string, TimeSpan>(this.DatabaseName, TimeSpan.FromMilliseconds((double)RegistryParameters.LogShipTimeoutInMsec));
		}

		public bool TestHungPassiveBlockMode { get; private set; }

		public static void SetCopyProperty(Guid dbGuid, string propName, string propVal)
		{
			LogCopier logCopier = LogCopier.FindCopier(null, dbGuid, false);
			if (logCopier == null)
			{
				throw new ArgumentException(string.Format("LogCopier for database '{0}' not active", dbGuid));
			}
			if (!SharedHelper.StringIEquals(propName, "TestHungPassiveBlockMode"))
			{
				throw new ArgumentException(string.Format("'{0}' is not recognized", propName));
			}
			bool flag;
			if (!bool.TryParse(propVal, out flag))
			{
				throw new ArgumentException("TestHungPassiveBlockMode must be a bool");
			}
			logCopier.TestHungPassiveBlockMode = flag;
			LogCopier.Tracer.TraceError<string, bool>((long)logCopier.GetHashCode(), "TestHungPassiveBlockMode({0}) set to {1}", logCopier.DatabaseName, flag);
		}

		private void TestDisconnect()
		{
			bool flag = false;
			LogCopier.Tracer.TraceError((long)this.GetHashCode(), "TestDisconnect invoked, disconnecting");
			try
			{
				Monitor.Enter(this.m_globalWorkerLock);
				flag = true;
				this.m_disconnectedForTest = true;
				this.DisableWakeUp();
				this.DiscardConnection();
				this.m_passiveBlockMode.Terminate();
				this.DisableWakeUp();
				this.m_lastException = new NetworkCommunicationException(this.m_config.SourceMachine, "Copier stopped by test hook.");
				this.m_setDisconnected.SetDisconnected(FailureTag.NoOp, ReplayEventLogConstants.Tuple_LogCopierFailedToCommunicate, new string[]
				{
					this.m_config.SourceMachine,
					"Disconnected by test hook"
				});
				LogCopier.Tracer.TraceError((long)this.GetHashCode(), "TestDisconnect succeeded");
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
		}

		public static void TestConnectCopier(Guid dbGuid)
		{
			LogCopier logCopier = LogCopier.FindCopier(null, dbGuid, true);
			logCopier.TestConnect();
		}

		private void TestConnect()
		{
			bool flag = false;
			LogCopier.Tracer.TraceError((long)this.GetHashCode(), "TestConnect invoked");
			Timer timer = null;
			try
			{
				Monitor.Enter(this.m_globalWorkerLock);
				flag = true;
				if (this.m_prepareToStopCalled)
				{
					throw new ReplayServiceUnknownReplicaInstanceException("FindCopier:Copier not active", this.m_config.Identity);
				}
				if (this.m_connectionIsBeingDiscarded)
				{
					LogCopier.Tracer.TraceDebug((long)this.GetHashCode(), "TestConnect cleaning up old connection");
					this.CleanupOldConnection(this.GetLogShipTimeoutMs());
				}
				this.m_disconnectedForTest = false;
				this.m_workerIsScheduled = true;
				timer = this.m_wakeTimer;
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.m_globalWorkerLock);
				}
			}
			LogCopier.Tracer.TraceError((long)this.GetHashCode(), "TestConnect succeeded");
			timer.Change(0, 10000);
		}

		public static void TestIgnoreGranularCompletions(Guid dbGuid)
		{
		}

		private const int MaxTotalWait = 30000;

		protected const int ShortWait = 10000;

		protected const int LongWait = 30000;

		private readonly string m_fromPrefix;

		private readonly string m_fromSuffix;

		private readonly int LogShipACLLTimeoutInSecs = (int)Math.Ceiling((double)RegistryParameters.LogShipACLLTimeoutInMsec / 1000.0);

		private Timer m_wakeTimer;

		private bool m_workerIsScheduled;

		private int m_lastWait;

		private int m_nextWait;

		private int m_totalWait;

		protected Exception m_lastException;

		private bool m_failureOnSource;

		private bool m_failureOnNetwork;

		private bool m_resetAfterError;

		private long m_fromNumber;

		private IADServer m_srv;

		private ShipLogsSetBroken m_logCopierSetBroken;

		private ISetBroken m_setBroken;

		private ISetDisconnected m_setDisconnected;

		private ISetGeneration m_setGeneration;

		private NetworkPath m_initialNetPath;

		private LogCopyClient m_copyClient;

		private bool m_fConstructedForAcll;

		private bool m_connectionIsBeingDiscarded;

		private long m_lastSentPingCounter;

		private DateTime? m_stalledSince = null;

		private PassiveBlockMode m_passiveBlockMode;

		private readonly string m_to;

		private readonly IPerfmonCounters m_perfmonCounters;

		private readonly IReplayConfiguration m_config;

		private readonly FileState m_fileState;

		private bool m_initialized;

		private volatile bool m_prepareToStopCalled;

		private bool m_stopped;

		private bool m_stalledDueToSourceLogBreak;

		private Exception m_sourceLogCorruptEx;

		private NetworkChannelMessage m_incomingMessage;

		private ReplayStopwatch m_responseTimer = new ReplayStopwatch();

		private bool m_responseIsBeingTimed;

		private bool m_readIsActive;

		private Exception m_readCallbackException;

		private ManualResetEvent m_readCompleteEvent;

		private object m_globalWorkerLock = new object();

		private volatile bool m_workerIsPreparingToExit;

		private long m_highestGenReleasedToInspector;

		private static readonly ServerVersion FirstVersionSupportingQueryVersion = new ServerVersion(16, 0, 472, 0);

		private static readonly ServerVersion FirstVersionSupportingV2 = new ServerVersion(15, 0, 466, 0);

		private bool m_waitingForHealthCheck;

		private bool m_serverIsDownlevel;

		private bool m_fAttemptFinalCopyCalled;

		private bool m_fAcllHasControl;

		private bool m_acllSuccess;

		private bool m_disconnectedForTest;

		private delegate void CatchableOperation();
	}
}
