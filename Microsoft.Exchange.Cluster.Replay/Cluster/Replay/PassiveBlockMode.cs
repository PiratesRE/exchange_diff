using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class PassiveBlockMode
	{
		public static bool IsDebugTraceEnabled
		{
			get
			{
				return ExTraceGlobals.PassiveBlockModeTracer.IsTraceEnabled(TraceType.DebugTrace);
			}
		}

		private LogCopier Copier { get; set; }

		private IReplayConfiguration Configuration { get; set; }

		private string ActiveServerName
		{
			get
			{
				return this.Configuration.SourceMachine;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.Configuration.IdentityGuid;
			}
		}

		internal string DatabaseName
		{
			get
			{
				return this.Configuration.DatabaseName;
			}
		}

		internal bool IsBlockModeActive { get; private set; }

		public PassiveBlockMode(LogCopier copier, ISetBroken setBroken, int maxConsumerDepthInBytes)
		{
			this.Copier = copier;
			this.Configuration = copier.Configuration;
			this.m_maxConsumerDepthInBytes = maxConsumerDepthInBytes;
			this.m_maxBuffersInUse = PassiveBlockMode.GetMaxBuffersPerDatabase(this.m_maxConsumerDepthInBytes);
			this.m_consumer = new GranularWriter(copier, copier.PerfmonCounters, copier.Configuration, setBroken);
			this.m_timer = new Timer(new TimerCallback(this.WakeUpCallback));
		}

		public static int GetMaxMemoryPerDatabase(IADServer srv)
		{
			int maxBlockModeConsumerDepthInBytes = RegistryParameters.MaxBlockModeConsumerDepthInBytes;
			if (maxBlockModeConsumerDepthInBytes != 0)
			{
				return maxBlockModeConsumerDepthInBytes;
			}
			int result = 10485760;
			if (srv != null)
			{
				long? continuousReplicationMaxMemoryPerDatabase = srv.ContinuousReplicationMaxMemoryPerDatabase;
				if (continuousReplicationMaxMemoryPerDatabase != null)
				{
					if (continuousReplicationMaxMemoryPerDatabase.Value > 104857600L)
					{
						PassiveBlockMode.Tracer.TraceError<long>(0L, "AD.ContinuousReplicationMaxMemoryPerDatabase too big: {0}bytes", continuousReplicationMaxMemoryPerDatabase.Value);
						result = 104857600;
					}
					else if (continuousReplicationMaxMemoryPerDatabase.Value < 3145728L)
					{
						PassiveBlockMode.Tracer.TraceError<long>(0L, "AD.ContinuousReplicationMaxMemoryPerDatabase too small: {0}", continuousReplicationMaxMemoryPerDatabase.Value);
						result = 3145728;
					}
					else
					{
						result = (int)continuousReplicationMaxMemoryPerDatabase.Value;
					}
				}
			}
			return result;
		}

		public static int GetMaxBuffersPerDatabase(int maxMemInBytesPerDatabase)
		{
			int num = maxMemInBytesPerDatabase / 1048576;
			int num2 = maxMemInBytesPerDatabase % 1048576;
			if (num2 != 0)
			{
				num++;
			}
			return num;
		}

		internal void SetCrossSiteFlag(IADServer activeServer, IADServer passiveServer)
		{
			this.m_isCrossSite = false;
			if (activeServer == null)
			{
				PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "Unable to determine the cross-site flag, because active server AD object is null.");
				return;
			}
			if (passiveServer == null)
			{
				PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "Unable to determine the cross-site flag, because passive server AD object is null.");
				return;
			}
			if (passiveServer.ServerSite == null && activeServer.ServerSite != null)
			{
				PassiveBlockMode.Tracer.TraceDebug<string>((long)this.GetHashCode(), "This block mode client is cross-site because passive server has no site and active server is in site '{0}'.", activeServer.ServerSite.Name);
				this.m_isCrossSite = true;
				return;
			}
			if (passiveServer.ServerSite != null && activeServer.ServerSite == null)
			{
				PassiveBlockMode.Tracer.TraceDebug<string>((long)this.GetHashCode(), "This block mode client is cross-site because active server has no site and passive server is in site '{0}'.", passiveServer.ServerSite.Name);
				this.m_isCrossSite = true;
				return;
			}
			if (!SharedHelper.StringIEquals(activeServer.ServerSite.Name, passiveServer.ServerSite.Name))
			{
				PassiveBlockMode.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "This block mode client is cross-site. Active site is '{0}' and passive site is '{1}'.", activeServer.ServerSite.Name, passiveServer.ServerSite.Name);
				this.m_isCrossSite = true;
			}
		}

		public void Destroy()
		{
			lock (this.m_workerLock)
			{
				this.m_timer.Dispose();
				this.m_timer = null;
			}
		}

		private static void ReadCallback(object asyncState, int bytesAvailable, bool completionIsSynchronous, Exception e)
		{
			PassiveReadRequest passiveReadRequest = (PassiveReadRequest)asyncState;
			passiveReadRequest.Manager.ProcessReadCallback(passiveReadRequest, bytesAvailable, completionIsSynchronous, e);
		}

		private void WakeUpCallback(object context)
		{
			PassiveBlockMode.Tracer.TraceFunction<string>((long)this.GetHashCode(), "WakeUpCallback({0})", this.DatabaseName);
			lock (this.m_workerLock)
			{
				if (!this.IsBlockModeActive)
				{
					PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "Timer fired after termination");
				}
				else if (this.Copier.TestHungPassiveBlockMode)
				{
					PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "TestHungPassiveBlockMode is active. Timer ignored");
				}
				else if (this.m_timeoutPending)
				{
					PassiveBlockMode.Tracer.TraceError<ExDateTime, ExDateTime>((long)this.GetHashCode(), "Active did not respond by {0}, cur time is {1}", this.m_timeoutLimit, ExDateTime.Now);
					this.Terminate();
				}
				else
				{
					this.m_timeoutPending = true;
					this.m_timeoutLimit = ExDateTime.Now.AddMilliseconds((double)RegistryParameters.LogShipTimeoutInMsec);
					NetworkTransportException ex;
					bool flag2;
					this.TrySendStatusMessageToActive(PassiveStatusMsg.Flags.PassiveIsRequestingAck, Win32StopWatch.GetSystemPerformanceCounter(), out ex, out flag2);
					if (ex == null)
					{
						this.ScheduleTimer();
					}
					else
					{
						PassiveBlockMode.Tracer.TraceError<NetworkTransportException>((long)this.GetHashCode(), "WakeUpCallback Failed to ping Active: {0}", ex);
						this.Terminate();
					}
				}
			}
		}

		public bool UsePartialLogsDuringAcll(long expectedGen)
		{
			return this.m_consumer.UsePartialLogsDuringAcll(expectedGen);
		}

		public bool EnterBlockMode(EnterBlockModeMsg msg, NetworkChannel channel, int maxConsumerQDepth)
		{
			lock (this.m_workerLock)
			{
				Exception ex = null;
				try
				{
					if (msg.FirstGenerationToExpect != this.Copier.NextGenExpected)
					{
						string text = string.Format("EnterBlockMode({0}) received gen 0x{1:X} but was expecting 0x{2:X}", this.DatabaseName, msg.FirstGenerationToExpect, this.Copier.NextGenExpected);
						PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), text);
						throw new NetworkUnexpectedMessageException(this.Configuration.SourceMachine, text);
					}
					PassiveBlockMode.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "EnterBlockMode({0}) starts at gen 0x{1:X}", this.DatabaseName, msg.FirstGenerationToExpect);
					this.m_oldestMessage = null;
					this.m_newestMessage = null;
					this.m_maxConsumerDepthInBytes = maxConsumerQDepth;
					this.m_maxBuffersInUse = PassiveBlockMode.GetMaxBuffersPerDatabase(this.m_maxConsumerDepthInBytes);
					this.m_consumer.Initialize();
					channel.KeepAlive = true;
					channel.NetworkChannelManagesAsyncReads = false;
					this.m_netChannel = channel;
					this.IsBlockModeActive = true;
					this.Copier.PerfmonCounters.GranularReplication = 1L;
					this.Copier.PerfmonCounters.EncryptionEnabled = (channel.IsEncryptionEnabled ? 1L : 0L);
					this.StartRead();
					msg.Send();
					this.ScheduleTimer();
					return true;
				}
				catch (NetworkTransportException ex2)
				{
					ex = ex2;
				}
				catch (GranularReplicationTerminatedException ex3)
				{
					ex = ex3;
				}
				catch (GranularReplicationInitFailedException ex4)
				{
					ex = ex4;
				}
				finally
				{
					if (ex != null)
					{
						PassiveBlockMode.Tracer.TraceError<Exception>((long)this.GetHashCode(), "EnterBlockMode failed: {0}", ex);
						this.Terminate();
					}
					else
					{
						PassiveBlockMode.Tracer.TraceDebug<string>((long)this.GetHashCode(), "EnterBlockMode({0}) succeeded", this.DatabaseName);
					}
				}
			}
			return false;
		}

		private PassiveReadRequest StartRead()
		{
			NetworkChannel netChannel = this.m_netChannel;
			PassiveReadRequest passiveReadRequest = new PassiveReadRequest(this, netChannel);
			bool flag = false;
			try
			{
				if (!this.Copier.TestHungPassiveBlockMode)
				{
					netChannel.StartRead(new NetworkChannelCallback(PassiveBlockMode.ReadCallback), passiveReadRequest);
					flag = true;
				}
			}
			finally
			{
				if (!flag)
				{
					PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "Failed to start async read");
				}
			}
			return passiveReadRequest;
		}

		private void ProcessReadCallback(PassiveReadRequest ioReq, int bytesAvailable, bool completionIsSynchronous, Exception readEx)
		{
			PassiveBlockMode.Tracer.TraceFunction<string>((long)this.GetHashCode(), "ProcessReadCallback({0}) entered", this.DatabaseName);
			lock (this.m_workerLock)
			{
				this.DisableTimer();
				Exception ex = null;
				bool flag2 = false;
				try
				{
					this.m_recursionDepth++;
					DiagCore.RetailAssert(this.m_recursionDepth == 1 || completionIsSynchronous, "recursive async completion", new object[0]);
					ioReq.CompletionWasProcessed = true;
					if (completionIsSynchronous)
					{
						ioReq.CompletedSynchronously = completionIsSynchronous;
					}
					if (readEx != null)
					{
						PassiveBlockMode.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "ProcessReadCallback({0}) read failed: ex={1}", this.DatabaseName, readEx);
						ex = new NetworkCommunicationException(this.Configuration.SourceMachine, readEx.Message, readEx);
					}
					else if (bytesAvailable == 0)
					{
						PassiveBlockMode.Tracer.TraceError<string>((long)this.GetHashCode(), "ProcessReadCallback({0}) active closed connection", this.DatabaseName);
						ex = new NetworkEndOfDataException(this.Configuration.SourceMachine, ReplayStrings.NetworkReadEOF);
					}
					else if (ioReq.Channel != this.m_netChannel)
					{
						ex = new NetworkEndOfDataException(this.Configuration.SourceMachine, ReplayStrings.NetworkReadEOF);
						flag2 = true;
						ioReq.Channel.Close();
					}
					else if (!this.IsBlockModeActive)
					{
						PassiveBlockMode.Tracer.TraceError<string>((long)this.GetHashCode(), "Discarding read since BM was already terminated", this.DatabaseName);
					}
					else
					{
						QueuedBlockMsg queuedBlockMsg = this.ReadInputMessage(ioReq.Channel);
						if (queuedBlockMsg != null)
						{
							this.ProcessInput(queuedBlockMsg);
							if (BitMasker.IsOn((int)queuedBlockMsg.EmitContext.grbitOperationalFlags, 2))
							{
								this.HandleActiveDismount();
								return;
							}
							if (BitMasker.IsOn((int)queuedBlockMsg.EmitContext.grbitOperationalFlags, 16))
							{
								this.Copier.TrackLastContactTime(queuedBlockMsg.MessageUtc);
							}
						}
						if (this.m_recursionDepth == 1)
						{
							while (this.IsBlockModeActive)
							{
								PassiveReadRequest passiveReadRequest = this.StartRead();
								if (!passiveReadRequest.CompletionWasProcessed)
								{
									if (!this.m_runningAcll)
									{
										this.ScheduleTimer();
										break;
									}
									break;
								}
							}
						}
						else
						{
							PassiveBlockMode.Tracer.TraceDebug<int>((long)this.GetHashCode(), "Recursive read avoided. Depth={0}", this.m_recursionDepth);
						}
					}
				}
				catch (NetworkTransportException ex2)
				{
					ex = ex2;
				}
				catch (NetworkRemoteException ex3)
				{
					ex = ex3;
				}
				catch (GranularReplicationOverflowException ex4)
				{
					ex = ex4;
				}
				finally
				{
					this.m_recursionDepth--;
					if (ex != null && !flag2)
					{
						this.Terminate();
					}
					PassiveBlockMode.Tracer.TraceFunction<string>((long)this.GetHashCode(), "ProcessReadCallback({0}) exits", this.DatabaseName);
				}
			}
		}

		private QueuedBlockMsg ReadInputMessage(NetworkChannel netChan)
		{
			this.m_timeoutPending = false;
			byte[] networkReadWorkingBuf = this.m_networkReadWorkingBuf;
			StopwatchStamp stamp = StopwatchStamp.GetStamp();
			NetworkChannelMessageHeader msgHdr = NetworkChannelMessage.ReadHeaderFromNet(netChan, networkReadWorkingBuf, 0);
			NetworkChannelMessage.MessageType messageType = msgHdr.MessageType;
			QueuedBlockMsg queuedBlockMsg;
			if (messageType != NetworkChannelMessage.MessageType.BlockModeCompressedData)
			{
				if (messageType == NetworkChannelMessage.MessageType.Ping)
				{
					PingMessage pingMessage = PingMessage.ReadFromNet(netChan, networkReadWorkingBuf, 0);
					long systemPerformanceCounter = Win32StopWatch.GetSystemPerformanceCounter();
					long arg = Win32StopWatch.ComputeElapsedTimeInUSec(systemPerformanceCounter, pingMessage.ReplyAckCounter) / 1000L;
					this.Copier.TrackLastContactTime(msgHdr.MessageUtc);
					PassiveBlockMode.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "ProcessReadCallback({0}) received a ping after {1}ms, so channel is healthy", this.DatabaseName, arg);
					return null;
				}
				if (messageType != NetworkChannelMessage.MessageType.GranularLogData)
				{
					throw new NetworkUnexpectedMessageException(netChan.PartnerNodeName, string.Format("Unknown Type {0}", msgHdr.MessageType));
				}
				queuedBlockMsg = this.ReadUncompressedMsg(netChan);
				this.Copier.PerfmonCounters.CompressionEnabled = 0L;
			}
			else
			{
				queuedBlockMsg = this.ReadCompressedMsg(netChan, msgHdr);
				this.Copier.PerfmonCounters.CompressionEnabled = 1L;
			}
			queuedBlockMsg.ReadDurationInTics = stamp.ElapsedTicks;
			queuedBlockMsg.MessageUtc = msgHdr.MessageUtc;
			this.Copier.PerfmonCounters.RecordLogCopierNetworkReadLatency(queuedBlockMsg.ReadDurationInTics);
			return queuedBlockMsg;
		}

		private QueuedBlockMsg ReadUncompressedMsg(NetworkChannel netChan)
		{
			byte[] networkReadWorkingBuf = this.m_networkReadWorkingBuf;
			GranularLogDataMsg granularLogDataMsg = GranularLogDataMsg.ReadFromNet(netChan, networkReadWorkingBuf, 0);
			byte[] array = null;
			int num = 0;
			if (granularLogDataMsg.LogDataLength > 0)
			{
				this.GetAppendSpace(granularLogDataMsg.LogDataLength);
				array = this.m_currentBuffer.Buffer;
				num = this.m_currentBuffer.AppendOffset;
				netChan.Read(array, num, granularLogDataMsg.LogDataLength);
				this.m_currentBuffer.AppendOffset = num + granularLogDataMsg.LogDataLength;
			}
			return new QueuedBlockMsg(granularLogDataMsg.EmitContext, array, num, 0)
			{
				RequestAckCounter = granularLogDataMsg.RequestAckCounter,
				IOBuffer = this.m_currentBuffer
			};
		}

		private QueuedBlockMsg ReadCompressedMsg(NetworkChannel netChan, NetworkChannelMessageHeader msgHdr)
		{
			byte[] networkReadWorkingBuf = this.m_networkReadWorkingBuf;
			BlockModeCompressedDataMsg blockModeCompressedDataMsg = BlockModeCompressedDataMsg.ReadFromNet(netChan, networkReadWorkingBuf, 0);
			byte[] array = null;
			int num = 0;
			int num2 = 0;
			if (blockModeCompressedDataMsg.LogDataLength > 0)
			{
				this.GetAppendSpace(blockModeCompressedDataMsg.LogDataLength);
				array = this.m_currentBuffer.Buffer;
				num = this.m_currentBuffer.AppendOffset;
				int num3 = blockModeCompressedDataMsg.LogDataLength;
				int num4 = num;
				foreach (int num5 in blockModeCompressedDataMsg.CompressedLengths)
				{
					num2 += num5;
					netChan.Read(networkReadWorkingBuf, 0, num5);
					int num6 = Math.Min(num3, 65536);
					if (!Xpress.Decompress(networkReadWorkingBuf, 0, num5, array, num4, num6))
					{
						throw new NetworkCorruptDataException(this.m_netChannel.PartnerNodeName);
					}
					num3 -= num6;
					num4 += num6;
				}
				this.m_currentBuffer.AppendOffset = num + blockModeCompressedDataMsg.LogDataLength;
			}
			return new QueuedBlockMsg(blockModeCompressedDataMsg.EmitContext, array, num, num2)
			{
				RequestAckCounter = blockModeCompressedDataMsg.RequestAckCounter,
				IOBuffer = this.m_currentBuffer
			};
		}

		private void ProcessInput(QueuedBlockMsg dataMsg)
		{
			dataMsg.GetMessageSize();
			if (this.m_newestMessage != null)
			{
				this.m_newestMessage.NextMsg = dataMsg;
				this.m_newestMessage = dataMsg;
			}
			else
			{
				this.m_newestMessage = dataMsg;
				this.m_oldestMessage = dataMsg;
			}
			bool flag = BitMasker.IsOn((int)dataMsg.EmitContext.grbitOperationalFlags, 16);
			if (PassiveBlockMode.IsDebugTraceEnabled)
			{
				long num = StopwatchStamp.TicksToMicroSeconds(dataMsg.ReadDurationInTics);
				PassiveBlockMode.Tracer.TraceDebug((long)this.GetHashCode(), "MessageArrived({0}) Gen=0x{1:X} Sector=0x{2:X} JBits=0x{3:X} EmitSeq=0x{4:X} LogDataLen=0x{5:X} ReadUSec={6}", new object[]
				{
					this.DatabaseName,
					dataMsg.EmitContext.lgposLogData.lGeneration,
					dataMsg.EmitContext.lgposLogData.isec,
					(int)dataMsg.EmitContext.grbitOperationalFlags,
					dataMsg.EmitContext.qwSequenceNum,
					dataMsg.LogDataLength,
					num
				});
			}
			this.Copier.PerfmonCounters.RecordGranularBytesReceived((long)dataMsg.LogDataLength, flag);
			ExchangeNetworkPerfmonCounters perfCounters = this.m_netChannel.PerfCounters;
			if (perfCounters != null)
			{
				perfCounters.RecordLogCopyThruputReceived((long)dataMsg.LogDataLength);
				if (dataMsg.CompressedLogDataLength > 0)
				{
					perfCounters.RecordCompressedDataReceived(dataMsg.CompressedLogDataLength, dataMsg.LogDataLength, NetworkPath.ConnectionPurpose.LogCopy);
				}
			}
			this.TriggerConsumer();
			if (flag || DateTime.UtcNow >= this.m_nextPingDue)
			{
				uint lGeneration = (uint)dataMsg.EmitContext.lgposLogData.lGeneration;
				if (flag)
				{
					this.Copier.TrackKnownEndOfLog((long)((ulong)lGeneration), dataMsg.EmitContext.logtimeEmit);
				}
				NetworkTransportException ex;
				bool flag2;
				this.TrySendStatusMessageToActive(flag ? PassiveStatusMsg.Flags.AckEndOfGeneration : PassiveStatusMsg.Flags.None, dataMsg.RequestAckCounter, out ex, out flag2);
				if (ex != null)
				{
					throw ex;
				}
			}
		}

		private void TrySendStatusMessageToActive(PassiveStatusMsg.Flags statusFlags, long requestAckCounter, out NetworkTransportException sendException, out bool sendSkippedBecauseChannelBusy)
		{
			sendException = null;
			sendSkippedBecauseChannelBusy = false;
			NetworkChannel netCh = this.m_netChannel;
			if (this.m_asyncNetWritePending || netCh == null)
			{
				PassiveBlockMode.Tracer.TraceError((long)this.GetHashCode(), "SendStatusMessageToActive skipped because channel is busy");
				sendSkippedBecauseChannelBusy = true;
				return;
			}
			ReplayState replayState = this.Configuration.ReplayState;
			long highestCompleteGen = replayState.CopyNotificationGenerationNumber;
			byte[] statusMsgBuf = PassiveStatusMsg.SerializeToBytes(statusFlags, requestAckCounter, (uint)highestCompleteGen, (uint)replayState.CopyGenerationNumber, (uint)replayState.InspectorGenerationNumber, (uint)replayState.ReplayGenerationNumber, this.m_isCrossSite);
			bool writeStarted = false;
			try
			{
				Exception ex = NetworkChannel.RunNetworkFunction(delegate
				{
					ExTraceGlobals.FaultInjectionTracer.TraceTest(PassiveBlockMode.ackMessageSendFailed);
					PassiveBlockMode.Tracer.TraceDebug<string, long>((long)this.GetHashCode(), "Sending PassiveStatusMsg({0}) Gen 0x{1:X}", this.DatabaseName, highestCompleteGen);
					this.m_nextPingDue = DateTime.UtcNow.AddMilliseconds((double)RegistryParameters.LogShipTimeoutInMsec);
					this.m_asyncNetWritePending = true;
					netCh.TcpChannel.Stream.BeginWrite(statusMsgBuf, 0, statusMsgBuf.Length, new AsyncCallback(this.NetWriteCompletion), netCh);
					writeStarted = true;
				});
				if (ex != null)
				{
					PassiveBlockMode.Tracer.TraceError<Exception>((long)this.GetHashCode(), "SendStatusMessageToActive Failed to ping Active: {0}", ex);
					ReplayEventLogConstants.Tuple_NotifyActiveSendFailed.LogEvent(null, new object[]
					{
						this.DatabaseName,
						replayState.ReplayGenerationNumber,
						replayState.InspectorGenerationNumber,
						replayState.CopyGenerationNumber,
						ex.Message
					});
					sendException = (ex as NetworkTransportException);
					if (sendException == null)
					{
						sendException = new NetworkCommunicationException(netCh.PartnerNodeName, ex.Message, ex);
					}
				}
			}
			finally
			{
				if (!writeStarted)
				{
					this.m_asyncNetWritePending = false;
				}
			}
		}

		private void NetWriteCompletion(IAsyncResult ar)
		{
			PassiveBlockMode.Tracer.TraceFunction<string>((long)this.GetHashCode(), "NetWriteCompletion({0})", this.DatabaseName);
			NetworkChannel netCh = (NetworkChannel)ar.AsyncState;
			Exception ex = NetworkChannel.RunNetworkFunction(delegate
			{
				netCh.TcpChannel.Stream.EndWrite(ar);
			});
			if (netCh == this.m_netChannel)
			{
				this.m_asyncNetWritePending = false;
				if (ex != null)
				{
					PassiveBlockMode.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "NetWriteCompletion({0}) failed: {1}", this.DatabaseName, ex);
					this.Terminate();
				}
			}
		}

		private void CloseChannel()
		{
			NetworkChannel netChannel = this.m_netChannel;
			if (netChannel != null)
			{
				this.m_netChannel = null;
				netChannel.Close();
				this.m_asyncNetWritePending = false;
			}
		}

		public void Terminate()
		{
			PassiveBlockMode.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Terminate({0})", this.DatabaseName);
			lock (this.m_workerLock)
			{
				this.DisableTimer();
				this.IsBlockModeActive = false;
				this.Copier.PerfmonCounters.GranularReplication = 0L;
				this.m_consumer.Terminate();
				this.CloseChannel();
				lock (this.m_consumerLock)
				{
					this.m_oldestMessage = null;
					this.m_newestMessage = null;
				}
				this.FreeAllBuffers();
			}
			this.Copier.NotifyBlockModeTermination();
		}

		private void DisableTimer()
		{
			if (this.m_timer != null)
			{
				this.m_timer.Change(-1, -1);
			}
		}

		private void ScheduleTimer(int msecUntilDue)
		{
			this.m_timer.Change(msecUntilDue, -1);
		}

		private void ScheduleTimer()
		{
			this.ScheduleTimer(RegistryParameters.LogShipTimeoutInMsec);
		}

		private void TriggerConsumer()
		{
			if (Interlocked.Exchange(ref this.m_consumerScheduled, 1) == 0)
			{
				PassiveBlockMode.Tracer.TraceDebug((long)this.GetHashCode(), "Queuing Consumer");
				ThreadPool.UnsafeQueueUserWorkItem(new WaitCallback(this.ConsumerEntryPoint), null);
				return;
			}
			PassiveBlockMode.Tracer.TraceDebug((long)this.GetHashCode(), "Consumer is already scheduled or running");
		}

		private void ConsumerEntryPoint(object dummy)
		{
			Exception ex = null;
			try
			{
				lock (this.m_consumerLock)
				{
					this.ConsumeData();
				}
			}
			catch (GranularReplicationTerminatedException ex2)
			{
				ex = ex2;
			}
			finally
			{
				Interlocked.Exchange(ref this.m_consumerScheduled, 0);
			}
			if (ex == null)
			{
				if (this.m_oldestMessage != this.m_newestMessage)
				{
					this.TriggerConsumer();
					return;
				}
			}
			else
			{
				this.Terminate();
			}
		}

		private void ConsumeData()
		{
			while (this.IsBlockModeActive)
			{
				QueuedBlockMsg queuedBlockMsg = this.m_oldestMessage;
				if (queuedBlockMsg == null)
				{
					return;
				}
				if (queuedBlockMsg.WasProcessed)
				{
					queuedBlockMsg = queuedBlockMsg.NextMsg;
					if (queuedBlockMsg == null)
					{
						return;
					}
				}
				queuedBlockMsg.WasProcessed = true;
				this.m_oldestMessage = queuedBlockMsg;
				this.m_consumer.Write(queuedBlockMsg.EmitContext, queuedBlockMsg.LogDataBuf, queuedBlockMsg.LogDataStartOffset);
				if (this.m_oldestMessage.IOBuffer != this.m_oldestBuffer && this.m_oldestMessage.IOBuffer != null)
				{
					this.RemoveOldestBuffer();
				}
			}
		}

		private void RemoveOldestBuffer()
		{
			lock (this.m_bufferManangementLock)
			{
				IOBuffer oldestBuffer = this.m_oldestBuffer;
				this.m_oldestBuffer = oldestBuffer.NextBuffer;
				this.m_buffersInUseCount--;
				this.ReleaseBuffer(oldestBuffer);
			}
		}

		private void GetAppendSpace(int len)
		{
			if (len > 1048576)
			{
				throw new NetworkUnexpectedMessageException(this.ActiveServerName, string.Format("Invalid BlockMode length: {0}", len));
			}
			if (this.m_currentBuffer == null || this.m_currentBuffer.RemainingSpace < len)
			{
				if (this.m_buffersInUseCount >= this.m_maxBuffersInUse)
				{
					this.CheckForOverflow();
				}
				this.AppendBuffer();
			}
		}

		private void AppendBuffer()
		{
			lock (this.m_bufferManangementLock)
			{
				IOBuffer iobuffer = this.AcquireBuffer();
				if (this.m_currentBuffer != null)
				{
					this.m_currentBuffer.NextBuffer = iobuffer;
				}
				else
				{
					this.m_oldestBuffer = iobuffer;
				}
				this.m_currentBuffer = iobuffer;
				this.m_buffersInUseCount++;
			}
		}

		private void ReleaseBuffer(IOBuffer buf)
		{
			if (this.m_freeBuffers.Count >= 3 || !buf.PreAllocated)
			{
				IOBufferPool.Free(buf);
				return;
			}
			this.m_freeBuffers.Push(buf);
		}

		private IOBuffer AcquireBuffer()
		{
			IOBuffer iobuffer;
			if (this.m_freeBuffers.Count > 0)
			{
				iobuffer = this.m_freeBuffers.Pop();
			}
			else
			{
				iobuffer = IOBufferPool.Allocate();
			}
			iobuffer.AppendOffset = 0;
			iobuffer.NextBuffer = null;
			return iobuffer;
		}

		private void FreeAllBuffers()
		{
			lock (this.m_bufferManangementLock)
			{
				while (this.m_oldestBuffer != null)
				{
					IOBuffer iobuffer = this.m_oldestBuffer;
					this.m_oldestBuffer = iobuffer.NextBuffer;
					IOBufferPool.Free(iobuffer);
				}
				this.m_currentBuffer = null;
				this.m_buffersInUseCount = 0;
				while (this.m_freeBuffers.Count > 0)
				{
					IOBuffer iobuffer = this.m_freeBuffers.Pop();
					IOBufferPool.Free(iobuffer);
				}
			}
		}

		public void PrepareForAcll()
		{
			this.m_runningAcll = true;
		}

		public void FinishForAcll(int timeoutInMsec)
		{
			lock (this.m_workerLock)
			{
				this.DrainConsumerQ(new TimeSpan(0, 0, 0, 0, timeoutInMsec), true);
			}
		}

		private void HandleActiveDismount()
		{
			PassiveBlockMode.Tracer.TraceDebug<string>((long)this.GetHashCode(), "HandleActiveDismount({0})", this.DatabaseName);
			this.CloseChannel();
			this.DrainConsumerQ(new TimeSpan(0, 2, 0), true);
		}

		private bool DrainConsumerQ(TimeSpan maxWaitForConsumer, bool alwaysTerminate)
		{
			bool result = false;
			bool flag = false;
			try
			{
				Monitor.TryEnter(this.m_consumerLock, maxWaitForConsumer, ref flag);
				if (flag)
				{
					if (this.m_oldestMessage != null)
					{
						this.ConsumeData();
					}
					result = true;
				}
				else
				{
					PassiveBlockMode.Tracer.TraceError<TimeSpan>((long)this.GetHashCode(), "Drain failed to obtain consumer Lock after {0}", maxWaitForConsumer);
					ReplayCrimsonEvents.PasssiveBlockModeDrainTimeout.Log<string, TimeSpan, StackTrace>(this.DatabaseName, maxWaitForConsumer, new StackTrace());
				}
			}
			catch (GranularReplicationTerminatedException arg)
			{
				PassiveBlockMode.Tracer.TraceError<string, GranularReplicationTerminatedException>((long)this.GetHashCode(), "Drain({0}) caught: {1}", this.DatabaseName, arg);
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(this.m_consumerLock);
				}
				if (alwaysTerminate)
				{
					this.Terminate();
				}
			}
			return result;
		}

		private void CheckForOverflow()
		{
			if (RegistryParameters.DisableGranularReplicationOverflow)
			{
				return;
			}
			ReplayCrimsonEvents.BlockModeOverflowOnPassive.Log<string, int>(this.DatabaseName, this.m_maxConsumerDepthInBytes);
			GranularReplicationOverflowException ex = new GranularReplicationOverflowException();
			if (RegistryParameters.WatsonOnBlockModeConsumerOverflow)
			{
				ExWatson.SendReportAndCrashOnAnotherThread(ex);
			}
			if (this.DrainConsumerQ(new TimeSpan(0, 0, 5), false))
			{
				return;
			}
			throw ex;
		}

		private const int MinFreeBufferCount = 3;

		private static readonly Microsoft.Exchange.Diagnostics.Trace Tracer = ExTraceGlobals.PassiveBlockModeTracer;

		private static uint ackMessageSendFailed = 3678809405U;

		private GranularWriter m_consumer;

		private NetworkChannel m_netChannel;

		private bool m_isCrossSite;

		private object m_bufferManangementLock = new object();

		private int m_buffersInUseCount;

		private IOBuffer m_oldestBuffer;

		private IOBuffer m_currentBuffer;

		private Stack<IOBuffer> m_freeBuffers = new Stack<IOBuffer>(3);

		private QueuedBlockMsg m_oldestMessage;

		private QueuedBlockMsg m_newestMessage;

		private Timer m_timer;

		private object m_workerLock = new object();

		private bool m_timeoutPending;

		private ExDateTime m_timeoutLimit;

		private int m_maxConsumerDepthInBytes;

		private int m_maxBuffersInUse;

		private int m_recursionDepth;

		private byte[] m_networkReadWorkingBuf = new byte[65536];

		private DateTime m_nextPingDue = DateTime.UtcNow;

		private volatile bool m_asyncNetWritePending;

		private object m_consumerLock = new object();

		private int m_consumerScheduled;

		private bool m_runningAcll;
	}
}
