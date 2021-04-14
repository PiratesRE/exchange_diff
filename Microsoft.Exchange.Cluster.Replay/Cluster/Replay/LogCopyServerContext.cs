using System;
using System.IO;
using System.Threading;
using Microsoft.Exchange.Cluster.ActiveManagerServer;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Isam.Esent.Interop;
using Microsoft.Mapi;
using Microsoft.Win32.SafeHandles;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogCopyServerContext
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.LogCopyServerTracer;
			}
		}

		private NetworkChannel Channel
		{
			get
			{
				return this.m_channel;
			}
		}

		internal MonitoredDatabase Database
		{
			get
			{
				return this.m_database;
			}
		}

		public long CurrentLogGeneration
		{
			get
			{
				return this.m_currentLogGeneration;
			}
		}

		private long MaxGenerationToSend
		{
			get
			{
				return this.m_currentLogCopyRequest.LastGeneration;
			}
		}

		private bool ForAcll
		{
			get
			{
				return this.m_currentLogCopyRequest.ForAcll;
			}
		}

		private SourceDatabasePerformanceCountersInstance SourceDatabasePerfCounters { get; set; }

		internal static void StartContinuousLogTransmission(NetworkChannel channel, ContinuousLogCopyRequest oldReq)
		{
			LogCopyServerContext logCopyServerContext = new LogCopyServerContext(channel, channel.MonitoredDatabase);
			logCopyServerContext.m_clientIsDownLevel = true;
			LogCopyServerContext.Tracer.TraceDebug<string, bool>((long)logCopyServerContext.GetHashCode(), "Passive({0}) is downlevel {1}}", channel.PartnerNodeName, logCopyServerContext.m_clientIsDownLevel);
			ContinuousLogCopyRequest2 initialRequest = LogCopyServerContext.UpgradeRequest(channel, oldReq);
			logCopyServerContext.InitContinuousLogTransmission(initialRequest);
		}

		private static ContinuousLogCopyRequest2 UpgradeRequest(NetworkChannel channel, ContinuousLogCopyRequest oldReq)
		{
			ContinuousLogCopyRequest2.Flags flagsUsed = (ContinuousLogCopyRequest2.Flags)oldReq.FlagsUsed;
			return new ContinuousLogCopyRequest2(null, channel, oldReq.DatabaseGuid, oldReq.FirstGeneration, flagsUsed)
			{
				LastGeneration = oldReq.LastGeneration,
				ClientNodeName = channel.PartnerNodeName
			};
		}

		internal static void StartContinuousLogTransmission(NetworkChannel channel, ContinuousLogCopyRequest2 initialRequest)
		{
			LogCopyServerContext logCopyServerContext = new LogCopyServerContext(channel, channel.MonitoredDatabase);
			logCopyServerContext.InitContinuousLogTransmission(initialRequest);
		}

		private static void NetworkReadComplete(object context, int bytesAvailable, bool completionIsSynchronous, Exception e)
		{
			LogCopyServerContext logCopyServerContext = (LogCopyServerContext)context;
			if (bytesAvailable > 0)
			{
				logCopyServerContext.HandleIncomingMessage();
				return;
			}
			if (e == null)
			{
				LogCopyServerContext.Tracer.TraceDebug((long)logCopyServerContext.GetHashCode(), "NetworkReadComplete: Client closed the channel");
			}
			else
			{
				LogCopyServerContext.Tracer.TraceError<Exception>((long)logCopyServerContext.GetHashCode(), "NetworkReadComplete: Channel exception: {0}", e);
			}
			logCopyServerContext.MarkForTermination();
		}

		private void MarkForTermination()
		{
			LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MarkForTermination({0})", this.PassiveCopyName);
			this.m_markedForTermination = true;
			this.SignalWorkPending();
		}

		internal LogCopyServerContext(NetworkChannel channel, MonitoredDatabase database)
		{
			this.m_channel = channel;
			this.m_database = database;
		}

		private string LocalNodeName
		{
			get
			{
				return this.m_channel.LocalNodeName;
			}
		}

		public bool IsBlockModeEnabled
		{
			get
			{
				return this.m_currentLogCopyRequest.UseGranular && GranularReplication.IsEnabled();
			}
		}

		private bool EnteredBlockMode { get; set; }

		public ConnectionStatus CollectConnectionStatus()
		{
			return new ConnectionStatus(this.m_clientNodeName, this.Channel.NetworkName, null, ConnectionDirection.Outgoing, false);
		}

		private void Terminate()
		{
			LogCopyServerContext.Tracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Closing", this.PassiveCopyName);
			this.Channel.Abort();
			lock (this.m_networkReadLock)
			{
				lock (this.m_networkWriteLock)
				{
					lock (this)
					{
						this.m_sendDataEnabled = false;
						if (this.m_linkedWithMonitoredDatabaseTable)
						{
							this.m_database.RemoveActiveLogCopyClient(this);
							this.m_linkedWithMonitoredDatabaseTable = false;
						}
						this.Channel.Close();
						ManualResetEvent workIsPendingEvent = this.m_workIsPendingEvent;
						this.m_workIsPendingEvent = null;
						if (workIsPendingEvent != null)
						{
							workIsPendingEvent.Close();
						}
					}
				}
			}
			LogCopyServerContext.Tracer.TraceFunction<string>((long)this.GetHashCode(), "{0}: Closed", this.PassiveCopyName);
		}

		internal void LinkWithMonitoredDatabase()
		{
			lock (this)
			{
				if (!this.m_linkedWithMonitoredDatabaseTable)
				{
					if (!this.m_database.AddActiveLogCopyClient(this))
					{
						LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Database stopped during linkage. Forcing client to reconnect to database '{0}'", this.Database.DatabaseName);
						throw new NetworkCancelledException();
					}
					this.m_linkedWithMonitoredDatabaseTable = true;
				}
			}
		}

		public void SourceIsStopping()
		{
			LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SourceIsStopping for '{0}'", this.Database.DatabaseName);
			lock (this)
			{
				this.m_linkedWithMonitoredDatabaseTable = false;
			}
			this.MarkForTermination();
		}

		private bool TryGetNetworkWriteLock()
		{
			return Monitor.TryEnter(this.m_networkWriteLock);
		}

		private void GetNetworkWriteLock()
		{
			Monitor.Enter(this.m_networkWriteLock);
		}

		private void ReleaseNetworkWriteLock()
		{
			Monitor.Exit(this.m_networkWriteLock);
		}

		public string PassiveCopyName
		{
			get
			{
				if (this.m_passiveCopyName == null)
				{
					this.m_passiveCopyName = string.Format("{0}\\{1}", this.Database.DatabaseName, this.m_clientNodeName);
				}
				return this.m_passiveCopyName;
			}
		}

		private void InitContinuousLogTransmission(ContinuousLogCopyRequest2 initialRequest)
		{
			this.m_clientNodeName = initialRequest.ClientNodeName;
			LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "StartContinuousLogTransmission({0}): First=0x{1:X} Max=0x{2:X} Flags=0x{3:X}", new object[]
			{
				this.PassiveCopyName,
				initialRequest.FirstGeneration,
				initialRequest.LastGeneration,
				initialRequest.FlagsUsed
			});
			this.m_currentLogCopyRequest = initialRequest;
			this.m_nextLogCopyRequest = initialRequest;
			this.m_currentLogGeneration = initialRequest.FirstGeneration;
			this.m_sendDataEnabled = true;
			this.Channel.NetworkChannelManagesAsyncReads = false;
			try
			{
				this.m_workIsPendingEvent = new ManualResetEvent(false);
				this.LinkWithMonitoredDatabase();
			}
			catch (NetworkCancelledException arg)
			{
				LogCopyServerContext.Tracer.TraceError<NetworkCancelledException>((long)this.GetHashCode(), "InitContinuousLogTransmission cancelled {0}", arg);
				this.Terminate();
				return;
			}
			this.SourceDatabasePerfCounters = SourceDatabasePerformanceCounters.GetInstance(this.PassiveCopyName);
			try
			{
				this.StartNetworkRead();
			}
			catch (NetworkTransportException arg2)
			{
				LogCopyServerContext.Tracer.TraceError<NetworkTransportException>((long)this.GetHashCode(), "InitContinuousLogTransmission caught {0}", arg2);
				this.MarkForTermination();
			}
			this.SignalWorkPending();
		}

		private void HandleIncomingMessage()
		{
			try
			{
				if (!this.m_markedForTermination)
				{
					lock (this.m_networkReadLock)
					{
						NetworkChannelMessage message = this.Channel.GetMessage();
						PingMessage pingMessage = message as PingMessage;
						if (pingMessage != null)
						{
							LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "HandleIncomingMessage: Ping received");
							Interlocked.Exchange(ref this.m_pingPending, 1);
							this.m_sendDataEnabled = true;
						}
						else
						{
							ContinuousLogCopyRequest2 continuousLogCopyRequest = message as ContinuousLogCopyRequest2;
							if (continuousLogCopyRequest == null)
							{
								if (message is ContinuousLogCopyRequest)
								{
									continuousLogCopyRequest = LogCopyServerContext.UpgradeRequest(this.Channel, message as ContinuousLogCopyRequest);
								}
								else
								{
									LogCopyServerContext.Tracer.TraceError<NetworkChannelMessage>((long)this.GetHashCode(), "HandleIncomingMessage: UnexpectedMsg:{0}", message);
									this.Channel.ThrowUnexpectedMessage(message);
								}
							}
							LogCopyServerContext.Tracer.TraceDebug<long, long, ContinuousLogCopyRequest2.Flags>((long)this.GetHashCode(), "HandleIncomingMessage: First=0x{0:X} Max=0x{1:X} Flags=0x{2:X}", continuousLogCopyRequest.FirstGeneration, continuousLogCopyRequest.LastGeneration, continuousLogCopyRequest.FlagsUsed);
							this.m_sendDataEnabled = true;
							this.m_nextLogCopyRequest = continuousLogCopyRequest;
						}
						this.StartNetworkRead();
						this.SignalWorkPending();
					}
				}
			}
			catch (NetworkTransportException arg)
			{
				LogCopyServerContext.Tracer.TraceError<NetworkTransportException>((long)this.GetHashCode(), "HandleIncomingMessage: Channel exception: {0}", arg);
				this.MarkForTermination();
			}
		}

		private void StartNetworkRead()
		{
			this.Channel.StartRead(new NetworkChannelCallback(LogCopyServerContext.NetworkReadComplete), this);
		}

		private void SendException(Exception e)
		{
			LogCopyServerContext.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "SendException({0}):{1}", this.PassiveCopyName, e);
			this.m_sendDataEnabled = false;
			this.Channel.SendException(e);
		}

		private void SignalWorkPending()
		{
			bool flag = false;
			ManualResetEvent manualResetEvent = null;
			lock (this.m_senderThreadLock)
			{
				if (!this.m_senderIsScheduled)
				{
					this.m_senderIsScheduled = true;
					flag = true;
				}
				else if (this.m_senderIsWaiting || this.m_markedForTermination)
				{
					manualResetEvent = this.m_workIsPendingEvent;
				}
			}
			if (flag)
			{
				LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SignalWorkPending({0}) scheduled worker", this.PassiveCopyName);
				ThreadPool.QueueUserWorkItem(new WaitCallback(this.SendLogsEntryPoint));
				return;
			}
			if (manualResetEvent != null)
			{
				try
				{
					LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SignalWorkPending({0}): Signalling", this.PassiveCopyName);
					manualResetEvent.Set();
					return;
				}
				catch (ObjectDisposedException)
				{
					LogCopyServerContext.Tracer.TraceError<string>((long)this.GetHashCode(), "SignalWorkPending({0}): ObjectDisposedException", this.PassiveCopyName);
					return;
				}
			}
			LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SignalWorkPending({0}): Sender was busy so we skipped the signal", this.PassiveCopyName);
		}

		private void TriggerSendLogs()
		{
			if (this.m_sendDataEnabled)
			{
				this.SignalWorkPending();
				return;
			}
			LogCopyServerContext.Tracer.TraceError<string, long>((long)this.GetHashCode(), "TriggerSendLogs({0}): Transmission disabled. Target is still at 0x{1:X}", this.PassiveCopyName, this.m_currentLogGeneration);
		}

		private void SendLogsEntryPoint(object dummy)
		{
			bool flag = true;
			this.GetNetworkWriteLock();
			while (!this.m_markedForTermination && flag)
			{
				Exception ex = null;
				try
				{
					this.SendLogs();
				}
				catch (NetworkTransportException ex2)
				{
					ex = ex2;
				}
				finally
				{
					flag = false;
					if (ex != null)
					{
						this.m_markedForTermination = true;
						LogCopyServerContext.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SendLogsEntryPoint caught: {0}", ex);
					}
					else
					{
						LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "MarkSenderAsIdling({0})", this.PassiveCopyName);
						this.m_workIsPendingEvent.Reset();
					}
					lock (this.m_senderThreadLock)
					{
						if (!this.m_markedForTermination)
						{
							if (this.WorkIsPending())
							{
								this.m_senderIsWaiting = false;
								flag = true;
							}
							else
							{
								this.m_senderIsWaiting = true;
							}
						}
						else
						{
							this.m_senderIsWaiting = false;
						}
					}
					if (this.m_senderIsWaiting)
					{
						bool flag3 = this.m_workIsPendingEvent.WaitOne(RegistryParameters.LogShipTimeoutInMsec);
						lock (this.m_senderThreadLock)
						{
							this.m_senderIsWaiting = false;
							if (flag3 || this.WorkIsPending())
							{
								flag = true;
							}
							else
							{
								LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "SendLogsEntryPoint({0}) timed out.", this.PassiveCopyName);
								this.m_senderIsScheduled = false;
							}
						}
					}
				}
			}
			this.ReleaseNetworkWriteLock();
			if (this.m_markedForTermination)
			{
				this.Terminate();
			}
		}

		private void SendLogs()
		{
			long num = 0L;
			bool flag = false;
			while (!this.m_markedForTermination && this.m_sendDataEnabled && !flag)
			{
				int num2 = Interlocked.Exchange(ref this.m_pingPending, 0);
				switch (this.SendNextLog())
				{
				case LogCopyServerContext.SendLogStatus.InSync:
					if (num2 != 0 || num == 0L)
					{
						this.SendInSyncMessage();
					}
					flag = true;
					break;
				case LogCopyServerContext.SendLogStatus.SentData:
					num += 1L;
					break;
				case LogCopyServerContext.SendLogStatus.SentE00:
					return;
				case LogCopyServerContext.SendLogStatus.EnteredBlockMode:
					this.MarkForTermination();
					return;
				case LogCopyServerContext.SendLogStatus.KeepChannelAlive:
					this.SendInSyncMessage();
					break;
				}
			}
		}

		private bool WorkIsPending()
		{
			return this.m_currentLogGeneration <= this.Database.EndOfLogGeneration || this.m_pingPending != 0 || !object.ReferenceEquals(this.m_currentLogCopyRequest, this.m_nextLogCopyRequest);
		}

		private LogCopyServerContext.SendLogStatus SendNextLog()
		{
			if (!object.ReferenceEquals(this.m_currentLogCopyRequest, this.m_nextLogCopyRequest))
			{
				LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "SendLogs noticed a new copier request");
				this.m_currentLogCopyRequest = this.m_nextLogCopyRequest;
				this.m_sendDataEnabled = true;
			}
			if (this.MaxGenerationToSend != 0L && this.CurrentLogGeneration > this.MaxGenerationToSend)
			{
				LogCopyServerContext.Tracer.TraceDebug<long, long>((long)this.GetHashCode(), "target in pull model. Requested that we stop at 0x{0:X}. Target currGen at 0x{1:X}", this.MaxGenerationToSend, this.CurrentLogGeneration);
				return LogCopyServerContext.SendLogStatus.StopForPullModel;
			}
			ExTraceGlobals.FaultInjectionTracer.TraceTest(3219533117U);
			if (this.m_currentLogGeneration <= this.Database.EndOfLogGeneration)
			{
				LogCopyServerContext.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "SendingFullLog({0}):0x{1:x}", this.PassiveCopyName, this.m_currentLogGeneration);
				try
				{
					this.Database.SendLog(this.m_currentLogGeneration, this.m_channel, this.SourceDatabasePerfCounters);
				}
				catch (FileIOonSourceException wrappedEx)
				{
					this.m_sendDataEnabled = false;
					this.HandleReadError(wrappedEx);
					return LogCopyServerContext.SendLogStatus.StopForException;
				}
				this.m_currentLogGeneration += 1L;
				return LogCopyServerContext.SendLogStatus.SentData;
			}
			if (!this.ForAcll)
			{
				if (this.IsBlockModeEnabled)
				{
					if (this.m_blockModeEntryGeneration < this.m_currentLogGeneration)
					{
						if (this.EnterBlockMode())
						{
							return LogCopyServerContext.SendLogStatus.EnteredBlockMode;
						}
						this.m_blockModeEntryGeneration = this.m_currentLogGeneration;
						LogCopyServerContext.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "SendNextLog({0}): BlockMode failed. Will retry after 0x{1:X}", this.PassiveCopyName, this.m_blockModeEntryGeneration);
					}
				}
				else
				{
					LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "BlockMode disabled.");
				}
			}
			if (this.ForAcll)
			{
				return this.SendE00();
			}
			LogCopyServerContext.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "SendNextLog({0}): In sync at 0x{1:X}", this.PassiveCopyName, this.m_currentLogGeneration);
			return LogCopyServerContext.SendLogStatus.InSync;
		}

		private bool EnterBlockMode()
		{
			TimeSpan timeout = TimeSpan.FromSeconds(5.0);
			bool flag;
			Exception ex = AmStoreHelper.IsDatabaseMounted(this.Database.DatabaseGuid, this.LocalNodeName, timeout, out flag);
			if (ex != null)
			{
				LogCopyServerContext.Tracer.TraceError<Exception>((long)this.GetHashCode(), "Store may not be running. Mount check failed: {0}", ex);
				return false;
			}
			if (!flag)
			{
				LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Db {0} not mounted. BlockMode is not possible", this.Database.DatabaseName);
				return false;
			}
			EnterBlockModeMsg enterBlockModeMsg = new EnterBlockModeMsg(this.Channel, EnterBlockModeMsg.Flags.PrepareToEnter, this.Database.DatabaseGuid, this.m_currentLogGeneration);
			bool flag2 = false;
			bool result;
			lock (this.m_networkReadLock)
			{
				if (this.m_markedForTermination)
				{
					result = false;
				}
				else
				{
					int readTimeoutInMs = this.Channel.TcpChannel.ReadTimeoutInMs;
					int writeTimeoutInMs = this.Channel.TcpChannel.WriteTimeoutInMs;
					try
					{
						this.Channel.TcpChannel.ReadTimeoutInMs = RegistryParameters.LogShipACLLTimeoutInMsec;
						this.Channel.TcpChannel.WriteTimeoutInMs = RegistryParameters.LogShipACLLTimeoutInMsec;
						LogCopyServerContext.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EnterBlockMode requesting PrepareToEnter for {0}", this.PassiveCopyName);
						enterBlockModeMsg.Send();
						EnterBlockModeMsg enterBlockModeMsg2;
						string text;
						for (;;)
						{
							NetworkChannelMessage message = this.Channel.GetMessage();
							PingMessage pingMessage = message as PingMessage;
							if (pingMessage != null)
							{
								LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "PingMessage ignored");
							}
							else
							{
								ContinuousLogCopyRequest2 continuousLogCopyRequest = message as ContinuousLogCopyRequest2;
								if (continuousLogCopyRequest != null)
								{
									this.m_nextLogCopyRequest = continuousLogCopyRequest;
									LogCopyServerContext.Tracer.TraceDebug<long, long, ContinuousLogCopyRequest2.Flags>((long)this.GetHashCode(), "ContinuousLogCopyRequest2: First=0x{0:X} Max=0x{1:X} Flags=0x{2:X}", continuousLogCopyRequest.FirstGeneration, continuousLogCopyRequest.LastGeneration, continuousLogCopyRequest.FlagsUsed);
								}
								else
								{
									enterBlockModeMsg2 = (message as EnterBlockModeMsg);
									text = null;
									if (enterBlockModeMsg2 == null)
									{
										text = string.Format("Passive({0}) sent unexpected msg: {1}", this.PassiveCopyName, message.GetType());
									}
									else if (enterBlockModeMsg2.AckCounter != enterBlockModeMsg.AckCounter)
									{
										text = string.Format("Passive({0}) is out of sync. BlockModeEntry Aborted", this.PassiveCopyName);
									}
									if (text != null)
									{
										break;
									}
									if (enterBlockModeMsg2.FlagsUsed != EnterBlockModeMsg.Flags.PassiveIsReady)
									{
										goto Block_13;
									}
									if (!this.RequestBlockModeInStore())
									{
										goto Block_15;
									}
								}
							}
						}
						LogCopyServerContext.Tracer.TraceError((long)this.GetHashCode(), text);
						throw new NetworkUnexpectedMessageException(this.m_clientNodeName, text);
						Block_13:
						if (enterBlockModeMsg2.FlagsUsed == EnterBlockModeMsg.Flags.PassiveReject)
						{
							LogCopyServerContext.Tracer.TraceError<string>((long)this.GetHashCode(), "BlockMode rejected by passive {0}", this.PassiveCopyName);
							flag2 = true;
							return false;
						}
						text = string.Format("Passive({0}) passed unexpected flags 0x{1X}", this.PassiveCopyName, enterBlockModeMsg2.FlagsUsed);
						throw new NetworkUnexpectedMessageException(this.m_clientNodeName, text);
						Block_15:
						flag2 = true;
						result = false;
					}
					finally
					{
						if (flag2)
						{
							this.Channel.TcpChannel.ReadTimeoutInMs = readTimeoutInMs;
							this.Channel.TcpChannel.WriteTimeoutInMs = writeTimeoutInMs;
							this.StartNetworkRead();
						}
					}
				}
			}
			return result;
		}

		private bool RequestBlockModeInStore()
		{
			Exception ex = null;
			using (IStoreRpc newStoreControllerInstance = Dependencies.GetNewStoreControllerInstance(this.LocalNodeName))
			{
				try
				{
					newStoreControllerInstance.StartBlockModeReplicationToPassive(this.Database.DatabaseGuid, this.m_clientNodeName, (uint)this.m_currentLogGeneration);
					this.EnteredBlockMode = true;
					LogCopyServerContext.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "BlockMode entered for copy '{0}' at 0x{1:X}", this.PassiveCopyName, this.m_currentLogGeneration);
					return true;
				}
				catch (MapiPermanentException ex2)
				{
					ex = ex2;
				}
				catch (MapiRetryableException ex3)
				{
					ex = ex3;
				}
				if (ex != null)
				{
					LogCopyServerContext.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "EnterBlockMode({0}) failed from store: {1}", this.PassiveCopyName, ex);
				}
			}
			return false;
		}

		private void SendInSyncMessage()
		{
			NotifyEndOfLogAsyncReply notifyEndOfLogAsyncReply = new NotifyEndOfLogAsyncReply(this.m_channel, this.CurrentLogGeneration - 1L, DateTime.UtcNow);
			notifyEndOfLogAsyncReply.Send();
		}

		private void HandleReadError(FileIOonSourceException wrappedEx)
		{
			Exception innerException = wrappedEx.InnerException;
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
					ReplayEventLogConstants.Tuple_FatalIOErrorEncountered.LogEvent(this.Database.Identity, new object[]
					{
						this.Database.DatabaseName,
						wrappedEx.FileFullPath,
						innerException.Message,
						num,
						innerException.ToString()
					});
				}
			}
			if (flag)
			{
				this.HandleCorruptLog(innerException);
				return;
			}
			this.HandleSourceReadError(wrappedEx);
		}

		private void HandleCorruptLog(Exception readEx)
		{
			this.SendException(new SourceLogBreakStallsPassiveException(Environment.MachineName, readEx.Message, readEx));
			ReplayEventLogConstants.Tuple_LogCopierErrorOnSourceTriggerFailover.LogEvent(this.Database.Identity, new object[]
			{
				this.Database.DatabaseName,
				this.Channel.PartnerNodeName,
				readEx.Message
			});
			this.Database.ProcessSourceLogCorruption(this.CurrentLogGeneration, readEx);
		}

		public void NewLogNotification()
		{
			this.TriggerSendLogs();
		}

		private LogCopyServerContext.SendLogStatus SendE00()
		{
			string text = this.Database.BuildLogFileName(0L);
			long num = 0L;
			FileIOonSourceException ex;
			if (this.m_dismountWorker == null)
			{
				ex = this.Database.GetE00Generation(out num, text);
			}
			else
			{
				ex = this.m_dismountWorker.LastE00ReadException;
			}
			if (ex != null)
			{
				bool flag = true;
				Exception innerException = ex.InnerException;
				if (innerException != null && innerException is EsentFileAccessDeniedException && !AmStoreServiceMonitor.WasKillTriggered())
				{
					if (this.m_dismountWorker == null)
					{
						LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "SendE00(): E00 still in use so starting a DismountDatabaseOrKillStore() in background...");
						this.m_dismountWorker = new DismountBackgroundWorker(new DismountBackgroundWorker.DismountDelegate(this.DismountDatabaseOrKillStore));
						this.m_dismountWorker.LastE00ReadException = ex;
						this.m_dismountWorker.Start();
					}
					if (this.m_dismountWorker.CompletedEvent.WaitOne(1000))
					{
						if (this.m_dismountWorker.DismountException == null)
						{
							flag = false;
							LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "SendE00(): DismountDatabaseOrKillStore() operation completed successfully");
						}
						else
						{
							LogCopyServerContext.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SendE00(): DismountDatabaseOrKillStore() operation completed but encountered an exception: {0}", this.m_dismountWorker.DismountException);
						}
						this.m_dismountWorker.Dispose();
						this.m_dismountWorker = null;
					}
					else if ((ExDateTime.Now - this.m_dismountWorker.StartTime).TotalMilliseconds < (double)(RegistryParameters.AcllDismountOrKillTimeoutInSec2 * 1000))
					{
						return LogCopyServerContext.SendLogStatus.KeepChannelAlive;
					}
				}
				if (!flag)
				{
					this.Database.ProbeForMoreLogs(this.m_currentLogGeneration);
					return LogCopyServerContext.SendLogStatus.KeepTrying;
				}
				this.HandleSourceReadError(ex);
				return LogCopyServerContext.SendLogStatus.StopForException;
			}
			else if (num != this.m_currentLogGeneration)
			{
				LogCopyServerContext.Tracer.TraceDebug<long, long>((long)this.GetHashCode(), "SendE00 finds e00 at 0x{0:X} but expected 0x{1:X}", num, this.m_currentLogGeneration);
				if (num > this.m_currentLogGeneration)
				{
					this.Database.SyncWithE00(num);
					return LogCopyServerContext.SendLogStatus.KeepTrying;
				}
				AcllFailedException e = new AcllFailedException(ReplayStrings.LogCopierE00InconsistentError(num, this.m_currentLogGeneration));
				this.SendException(e);
				return LogCopyServerContext.SendLogStatus.StopForException;
			}
			else
			{
				Exception ex2 = null;
				try
				{
					using (SafeFileHandle safeFileHandle = LogCopy.OpenLogForRead(text))
					{
						CopyLogReply copyLogReply = new CopyLogReply(this.Channel);
						copyLogReply.ThisLogGeneration = 0L;
						copyLogReply.EndOfLogGeneration = num;
						copyLogReply.EndOfLogUtc = DateTime.UtcNow;
						CheckSummer summer = null;
						if (this.Channel.ChecksumDataTransfer)
						{
							summer = new CheckSummer();
						}
						this.Channel.SendLogFileTransferReply(copyLogReply, text, safeFileHandle, this.SourceDatabasePerfCounters, summer);
					}
				}
				catch (IOException ex3)
				{
					ex2 = new FileIOonSourceException(Environment.MachineName, text, ex3.Message, ex3);
				}
				catch (UnauthorizedAccessException ex4)
				{
					ex2 = new FileIOonSourceException(Environment.MachineName, text, ex4.Message, ex4);
				}
				catch (FileIOonSourceException ex5)
				{
					ex2 = ex5;
				}
				if (ex2 != null)
				{
					this.HandleSourceReadError(ex2);
					return LogCopyServerContext.SendLogStatus.StopForException;
				}
				return LogCopyServerContext.SendLogStatus.SentE00;
			}
		}

		private Exception DismountDatabaseOrKillStore()
		{
			LogCopyServerContext.Tracer.TraceDebug((long)this.GetHashCode(), "DismountDatabaseOrKillStore() called.");
			Exception ex = AmStoreHelper.Dismount(this.Database.DatabaseGuid, UnmountFlags.SkipCacheFlush);
			if (ex != null)
			{
				ex = new DatabaseDismountOrKillStoreException(this.Database.DatabaseName, Environment.MachineName, ex.Message, ex);
				LogCopyServerContext.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "DismountDatabaseOrKillStore() for DB '{0}' failed: {1}", this.Database.DatabaseName, ex);
			}
			return ex;
		}

		private void HandleSourceReadError(Exception ex)
		{
			ReplayEventLogConstants.Tuple_LogCopierErrorOnSource.LogEvent(this.Database.Identity, new object[]
			{
				this.Database.DatabaseName,
				this.Channel.PartnerNodeName,
				ex.Message
			});
			this.SendException(ex);
		}

		private NetworkChannel m_channel;

		private MonitoredDatabase m_database;

		private bool m_linkedWithMonitoredDatabaseTable;

		private volatile bool m_sendDataEnabled = true;

		private ContinuousLogCopyRequest2 m_currentLogCopyRequest;

		private volatile ContinuousLogCopyRequest2 m_nextLogCopyRequest;

		private long m_currentLogGeneration;

		private long m_blockModeEntryGeneration;

		private DismountBackgroundWorker m_dismountWorker;

		private volatile bool m_markedForTermination;

		private object m_networkReadLock = new object();

		private object m_networkWriteLock = new object();

		private string m_clientNodeName;

		private bool m_clientIsDownLevel;

		private string m_passiveCopyName;

		private int m_pingPending;

		private object m_senderThreadLock = new object();

		private bool m_senderIsScheduled;

		private bool m_senderIsWaiting;

		private ManualResetEvent m_workIsPendingEvent;

		private enum SendLogStatus
		{
			InSync,
			SentData,
			SentE00,
			StopForPullModel,
			StopForException,
			EnteredBlockMode,
			KeepTrying,
			KeepChannelAlive
		}
	}
}
