using System;
using System.Diagnostics;
using System.IO;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.ManagedStore.HA;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.Server.Storage.HA;

namespace Microsoft.Exchange.Server.Storage.BlockMode
{
	internal class BlockModeSender
	{
		public BlockModeSender(string passiveName, BlockModeCollector collector, BlockModeMessageStream.SenderPosition startPos)
		{
			this.PassiveName = passiveName;
			this.DatabaseName = collector.DatabaseName;
			this.CopyName = string.Format("{0}\\{1}", this.DatabaseName, this.PassiveName);
			this.Collector = collector;
			this.Position = startPos;
			this.perfInstance = ActiveDatabaseSenderPerformanceCounters.GetInstance(this.CopyName);
			this.logCopyStatus = new LogCopyStatus(CopyType.BlockModePassive, this.PassiveName, false, 0UL, 0UL, 0UL);
		}

		public static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.BlockModeSenderTracer;
			}
		}

		public string PassiveName { get; private set; }

		public string DatabaseName { get; private set; }

		public string CopyName { get; private set; }

		public BlockModeMessageStream.SenderPosition Position { get; private set; }

		public LogCopyStatus LogCopyStatus
		{
			get
			{
				return this.logCopyStatus;
			}
		}

		public bool CompressionDesired { get; private set; }

		private BlockModeCollector Collector { get; set; }

		private NetworkChannel NetChannel { get; set; }

		private Stream NetStream
		{
			get
			{
				return this.NetChannel.TcpChannel.Stream;
			}
		}

		public void PassiveIsReady(NetworkChannel channelToPassive)
		{
			this.NetChannel = channelToPassive;
			this.CompressionDesired = channelToPassive.NetworkPath.Compress;
		}

		public void Close()
		{
			if (this.NetChannel != null)
			{
				this.NetChannel.Close();
			}
		}

		public BlockModeSender.WriteStatus TryStartWrite()
		{
			bool flag = false;
			bool flag2 = false;
			BlockModeSender.WriteStatus result;
			try
			{
				while (this.NetChannel != null)
				{
					if (this.writeIsActive)
					{
						BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TryStartWrite({0}) did nothing because a pending write is active", this.CopyName);
						return BlockModeSender.WriteStatus.WritePending;
					}
					if (this.Position.NextSendOffset < this.Position.CurrentBuffer.AppendOffset)
					{
						BlockModeSender.IoReq ioReq = new BlockModeSender.IoReq();
						ioReq.Length = this.Position.CurrentBuffer.AppendOffset - this.Position.NextSendOffset;
						BlockModeSender.Tracer.TraceDebug<string, int>((long)this.GetHashCode(), "TryStartWrite({0}) sending {1} bytes", this.CopyName, ioReq.Length);
						flag2 = true;
						this.writeIsActive = true;
						this.passiveRequestingAck = false;
						Stopwatch stopwatch = Stopwatch.StartNew();
						IAsyncResult asyncResult = this.NetStream.BeginWrite(this.Position.CurrentBuffer.Buffer, this.Position.NextSendOffset, ioReq.Length, new AsyncCallback(this.CompletionOfWrite), ioReq);
						flag = true;
						long elapsedTicks = stopwatch.ElapsedTicks;
						this.perfInstance.AverageWriteTime.IncrementBy(elapsedTicks);
						this.perfInstance.AverageWriteTimeBase.Increment();
						this.perfInstance.TotalBytesSent.IncrementBy((long)ioReq.Length);
						this.perfInstance.TotalNetworkWrites.Increment();
						this.perfInstance.WritesPerSec.Increment();
						this.perfInstance.WriteThruput.IncrementBy((long)ioReq.Length);
						this.perfInstance.AverageWriteSize.IncrementBy((long)ioReq.Length);
						this.perfInstance.AverageWriteSizeBase.Increment();
						if (asyncResult.CompletedSynchronously)
						{
							BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TryStartWrite({0}) sync completion, so we might want to keep looping", this.CopyName);
						}
						return BlockModeSender.WriteStatus.DataWritten;
					}
					if (this.Position.CurrentBuffer.NextBuffer != null)
					{
						if (this.Position.NextSendOffset >= this.Position.CurrentBuffer.AppendOffset)
						{
							this.Position.CurrentBuffer = this.Position.CurrentBuffer.NextBuffer;
							this.Position.NextSendOffset = 0;
						}
					}
					else
					{
						if (this.passiveRequestingAck)
						{
							BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TryStartWrite({0}) sending ping", this.CopyName);
							new PingMessage(this.NetChannel)
							{
								ReplyAckCounter = this.passiveRequestingAckLastTimestamp
							}.Send();
							this.passiveRequestingAck = false;
							return BlockModeSender.WriteStatus.PingSent;
						}
						BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TryStartWrite({0}) did nothing because all data has already been sent", this.CopyName);
						return BlockModeSender.WriteStatus.AlreadySent;
					}
				}
				BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "TryStartWrite({0}) did nothing because this passive is joining", this.CopyName);
				result = BlockModeSender.WriteStatus.WritePending;
			}
			finally
			{
				if (flag2 && !flag)
				{
					this.writeIsActive = false;
				}
			}
			return result;
		}

		public BlockModeSender.ActiveReadRequest StartRead()
		{
			NetworkChannel netChannel = this.NetChannel;
			BlockModeSender.ActiveReadRequest activeReadRequest = new BlockModeSender.ActiveReadRequest(this, netChannel);
			bool flag = false;
			try
			{
				netChannel.StartRead(new NetworkChannelCallback(BlockModeSender.ReadCallback), activeReadRequest);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					BlockModeSender.Tracer.TraceError((long)this.GetHashCode(), "Failed to starting async read");
				}
			}
			return activeReadRequest;
		}

		private static void ReadCallback(object asyncState, int bytesAvailable, bool completionIsSynchronous, Exception e)
		{
			BlockModeSender.ActiveReadRequest activeReadRequest = (BlockModeSender.ActiveReadRequest)asyncState;
			activeReadRequest.Sender.ProcessReadCallback(activeReadRequest, bytesAvailable, completionIsSynchronous, e);
		}

		private void CompletionOfWrite(IAsyncResult ar)
		{
			BlockModeSender.IoReq ioReq = (BlockModeSender.IoReq)ar.AsyncState;
			Exception ex = NetworkChannel.RunNetworkFunction(delegate
			{
				this.NetStream.EndWrite(ar);
				this.Position.NextSendOffset += ioReq.Length;
			});
			this.writeIsActive = false;
			if (ex != null)
			{
				BlockModeSender.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "CompletionOfWrite({0}) failed: {1}", this.CopyName, ex);
				this.Collector.HandleSenderFailed(this, ex);
				return;
			}
			if (!ar.CompletedSynchronously)
			{
				BlockModeSender.Tracer.TraceDebug<string>((long)this.GetHashCode(), "CompletionOfWrite({0}) is trying to trigger more writes", this.CopyName);
				this.Collector.TryStartWrites();
			}
		}

		private void ProcessReadCallback(BlockModeSender.ActiveReadRequest ioReq, int bytesAvailable, bool completionIsSynchronous, Exception readEx)
		{
			bool ackRequested = false;
			if (completionIsSynchronous)
			{
				ioReq.CompletedSynchronously = completionIsSynchronous;
			}
			Exception ex;
			if (readEx != null)
			{
				BlockModeSender.Tracer.TraceError<string, Exception>((long)this.GetHashCode(), "ProcessReadCallback({0}) read failed: ex={1}", this.DatabaseName, readEx);
				ex = new NetworkCommunicationException(this.PassiveName, readEx.Message, readEx);
			}
			else if (bytesAvailable == 0)
			{
				BlockModeSender.Tracer.TraceError<string>((long)this.GetHashCode(), "ProcessReadCallback({0}) passive closed connection", this.DatabaseName);
				ex = new NetworkEndOfDataException(this.PassiveName, Strings.NetworkReadEOF);
			}
			else
			{
				ex = NetworkChannel.RunNetworkFunction(delegate
				{
					NetworkChannelMessage message = ioReq.Channel.GetMessage();
					PassiveStatusMsg passiveStatusMsg = message as PassiveStatusMsg;
					if (passiveStatusMsg == null)
					{
						string text = string.Format("Active received unexpected message from copy '{0}'. MsgType={1}", this.CopyName, message.Type);
						BlockModeSender.Tracer.TraceError((long)this.GetHashCode(), text);
						throw new NetworkUnexpectedMessageException(this.PassiveName, text);
					}
					BlockModeSender.Tracer.TraceDebug((long)this.GetHashCode(), "PassiveStatusMsg({0}) gen 0x{1:X} sent at {2}UTC flags 0x{3:X}", new object[]
					{
						this.CopyName,
						passiveStatusMsg.GenInNetworkBuffer,
						passiveStatusMsg.MessageUtc,
						(long)passiveStatusMsg.FlagsUsed
					});
					if (BitMasker.IsOn64((long)passiveStatusMsg.FlagsUsed, 1L))
					{
						this.passiveRequestingAckLastTimestamp = passiveStatusMsg.AckCounter;
						this.passiveRequestingAck = true;
						ackRequested = true;
					}
					else if (BitMasker.IsOn64((long)passiveStatusMsg.FlagsUsed, 2L))
					{
						long systemPerformanceCounter = Win32StopWatch.GetSystemPerformanceCounter();
						long num = Win32StopWatch.ComputeElapsedTimeInUSec(systemPerformanceCounter, passiveStatusMsg.AckCounter) / 1000L;
						BlockModeSender.Tracer.TracePerformance<long, uint>((long)this.GetHashCode(), "RoundTripLatency: {0} mSec to gen 0x{1:X}", num, passiveStatusMsg.GenInNetworkBuffer);
						this.perfInstance.AverageWriteAckLatency.IncrementBy(num);
						this.perfInstance.AverageWriteAckLatencyBase.Increment();
					}
					this.perfInstance.AcknowledgedGenerationNumber.RawValue = (long)((ulong)passiveStatusMsg.GenInNetworkBuffer);
					this.logCopyStatus = new LogCopyStatus(CopyType.BlockModePassive, this.PassiveName, passiveStatusMsg.IsCrossSite, (ulong)passiveStatusMsg.GenInNetworkBuffer, (ulong)passiveStatusMsg.LastGenInspected, (ulong)passiveStatusMsg.LastGenReplayed);
					this.Collector.TriggerThrottlingUpdate();
					if (!completionIsSynchronous)
					{
						BlockModeSender.ActiveReadRequest activeReadRequest;
						do
						{
							activeReadRequest = this.StartRead();
						}
						while (activeReadRequest.CompletedSynchronously);
						return;
					}
					BlockModeSender.Tracer.TraceDebug((long)this.GetHashCode(), "Recursive read avoided.");
				});
			}
			if (ex != null)
			{
				this.Collector.HandleSenderFailed(this, ex);
				return;
			}
			if (ackRequested)
			{
				this.Collector.StartWrites();
			}
		}

		private volatile bool writeIsActive;

		private volatile bool passiveRequestingAck;

		private long passiveRequestingAckLastTimestamp;

		private ActiveDatabaseSenderPerformanceCountersInstance perfInstance;

		private LogCopyStatus logCopyStatus;

		public enum WriteStatus
		{
			WritePending = 1,
			DataWritten,
			PingSent,
			AlreadySent
		}

		internal class ActiveReadRequest
		{
			public ActiveReadRequest(BlockModeSender sender, NetworkChannel channel)
			{
				this.Channel = channel;
				this.Sender = sender;
			}

			public NetworkChannel Channel { get; set; }

			public BlockModeSender Sender { get; set; }

			public bool CompletedSynchronously { get; set; }
		}

		private class IoReq
		{
			public int Length { get; set; }
		}
	}
}
