using System;
using System.Threading;
using Microsoft.Exchange.Cluster.Common;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.EseRepl;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class LogCopyClient : LogSource
	{
		public override string SourcePath
		{
			get
			{
				return "TCP:" + this.m_sourceAddr + "/" + this.m_config.DatabaseName;
			}
		}

		internal Guid DatabaseGuid
		{
			get
			{
				return this.m_config.IdentityGuid;
			}
		}

		public LogCopyClient(IReplayConfiguration config, IPerfmonCounters perfmonCounters, NetworkPath initialNetworkPath, int timeoutMs)
		{
			this.m_config = config;
			this.m_perfmonCounters = perfmonCounters;
			this.m_defaultTimeoutInMs = timeoutMs;
			this.m_sourceAddr = config.SourceMachine;
			this.m_initialNetworkPath = initialNetworkPath;
		}

		public override void Cancel()
		{
			ExTraceGlobals.LogCopyClientTracer.TraceDebug((long)this.GetHashCode(), "Cancelling use of this channel.");
			lock (this)
			{
				this.m_cancelling = true;
				if (this.m_channel != null)
				{
					this.m_channel.Abort();
				}
			}
		}

		public override void Close()
		{
			if (this.m_channel != null)
			{
				this.m_channel.Close();
			}
		}

		public override void SetTimeoutInMsec(int msTimeout)
		{
			this.m_defaultTimeoutInMs = msTimeout;
			if (this.m_channel != null)
			{
				TcpChannel tcpChannel = this.m_channel.TcpChannel;
				tcpChannel.ReadTimeoutInMs = msTimeout;
				tcpChannel.WriteTimeoutInMs = msTimeout;
			}
		}

		public bool TryGetChannelLock()
		{
			return Monitor.TryEnter(this.m_channelLock);
		}

		public void GetChannelLock()
		{
			Monitor.Enter(this.m_channelLock);
		}

		public void ReleaseChannelLock()
		{
			Monitor.Exit(this.m_channelLock);
		}

		public NetworkChannel Channel
		{
			get
			{
				return this.m_channel;
			}
		}

		public NetworkChannel OpenChannel()
		{
			string networkName = null;
			NetworkPath.ConnectionPurpose purpose = NetworkPath.ConnectionPurpose.LogCopy;
			NetworkPath netPath;
			if (this.m_initialNetworkPath == null)
			{
				netPath = NetworkManager.ChooseNetworkPath(this.m_sourceAddr, networkName, purpose);
			}
			else
			{
				netPath = this.m_initialNetworkPath;
				this.m_initialNetworkPath = null;
			}
			this.m_channel = NetworkChannel.Connect(netPath, base.DefaultTimeoutInMs, false);
			return this.m_channel;
		}

		private void OpenChannelIfFirstRequest()
		{
			if (this.m_channel == null)
			{
				this.OpenChannel();
			}
		}

		private void DiscardChannel()
		{
			if (this.m_channel != null)
			{
				NetworkChannel channel = this.m_channel;
				this.m_channel = null;
				channel.Close();
			}
		}

		private void SendMessage(NetworkChannelMessage msg)
		{
			msg.Send();
		}

		private NetworkChannelMessage GetReply()
		{
			return this.m_channel.GetMessage();
		}

		public override long QueryLogRange()
		{
			long num = 0L;
			bool flag = false;
			int num2 = 0;
			TcpChannel tcpChannel = null;
			this.GetChannelLock();
			try
			{
				this.OpenChannelIfFirstRequest();
				tcpChannel = this.m_channel.TcpChannel;
				if (tcpChannel.ReadTimeoutInMs < RegistryParameters.QueryLogRangeTimeoutInMsec)
				{
					num2 = tcpChannel.ReadTimeoutInMs;
					tcpChannel.ReadTimeoutInMs = RegistryParameters.QueryLogRangeTimeoutInMsec;
				}
				QueryLogRangeRequest msg = new QueryLogRangeRequest(this.m_channel, this.DatabaseGuid);
				this.SendMessage(msg);
				NetworkChannelMessage reply = this.GetReply();
				QueryLogRangeReply queryLogRangeReply = reply as QueryLogRangeReply;
				if (queryLogRangeReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(reply);
				}
				this.m_endOfLog.SetValue(queryLogRangeReply.EndOfLogGeneration, new DateTime?(queryLogRangeReply.EndOfLogUtc));
				num = queryLogRangeReply.FirstAvailableGeneration;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long, long>((long)this.GetHashCode(), "LogCopyClient:QueryLogRange: 0x{0:x} .. {1:x}", num, this.m_endOfLog.Generation);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					this.DiscardChannel();
				}
				else if (num2 > 0)
				{
					tcpChannel.ReadTimeoutInMs = num2;
				}
				this.ReleaseChannelLock();
			}
			return num;
		}

		internal static void QueryLogRange(Guid dbGuid, NetworkChannel ch, out long minGen, out long maxGen, out DateTime maxGenUtc)
		{
			minGen = 0L;
			maxGen = 0L;
			maxGenUtc = DateTime.UtcNow;
			bool flag = false;
			int timeoutToRestore = 0;
			TcpChannel tcpChannel = null;
			try
			{
				tcpChannel = ch.TcpChannel;
				if (tcpChannel.ReadTimeoutInMs < RegistryParameters.QueryLogRangeTimeoutInMsec)
				{
					timeoutToRestore = tcpChannel.ReadTimeoutInMs;
					tcpChannel.ReadTimeoutInMs = RegistryParameters.QueryLogRangeTimeoutInMsec;
				}
				QueryLogRangeRequest queryLogRangeRequest = new QueryLogRangeRequest(ch, dbGuid);
				queryLogRangeRequest.Send();
				NetworkChannelMessage message = ch.GetMessage();
				QueryLogRangeReply queryLogRangeReply = message as QueryLogRangeReply;
				if (queryLogRangeReply == null)
				{
					ch.ThrowUnexpectedMessage(message);
				}
				minGen = queryLogRangeReply.FirstAvailableGeneration;
				maxGen = queryLogRangeReply.EndOfLogGeneration;
				maxGenUtc = queryLogRangeReply.EndOfLogUtc;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long, long>((long)ch.GetHashCode(), "LogCopyClient:TryQueryLogRange: 0x{0:x} .. {1:x}", minGen, maxGen);
				flag = true;
			}
			finally
			{
				if (timeoutToRestore > 0)
				{
					if (!flag)
					{
						NetworkChannel.RunNetworkFunction(delegate
						{
							tcpChannel.ReadTimeoutInMs = timeoutToRestore;
						});
					}
					else
					{
						tcpChannel.ReadTimeoutInMs = timeoutToRestore;
					}
				}
			}
		}

		public override long QueryEndOfLog()
		{
			long num = -1L;
			this.GetChannelLock();
			try
			{
				this.OpenChannelIfFirstRequest();
				NotifyEndOfLogRequest msg = new NotifyEndOfLogRequest(this.m_channel, this.DatabaseGuid, 0L);
				this.SendMessage(msg);
				NetworkChannelMessage reply = this.GetReply();
				NotifyEndOfLogReply notifyEndOfLogReply = reply as NotifyEndOfLogReply;
				if (notifyEndOfLogReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(reply);
				}
				this.m_endOfLog.SetValue(notifyEndOfLogReply.EndOfLogGeneration, new DateTime?(notifyEndOfLogReply.EndOfLogUtc));
				num = notifyEndOfLogReply.EndOfLogGeneration;
			}
			finally
			{
				this.ReleaseChannelLock();
			}
			ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)this.GetHashCode(), "LogCopyClient:QueryEndOfLog: 0x{0:x}", num);
			return num;
		}

		public override void CopyLog(long fromNumber, string destinationFileName, out DateTime writeTimeUtc)
		{
			writeTimeUtc = (DateTime)ExDateTime.UtcNow;
			ExTraceGlobals.LogCopyClientTracer.TraceDebug<string>((long)this.GetHashCode(), "CopyLog {0} starting", destinationFileName);
			base.AllocateBuffer();
			this.GetChannelLock();
			try
			{
				this.OpenChannelIfFirstRequest();
				CopyLogRequest msg = new CopyLogRequest(this.m_channel, this.DatabaseGuid, fromNumber);
				this.SendMessage(msg);
				ReplayStopwatch replayStopwatch = new ReplayStopwatch();
				replayStopwatch.Start();
				NetworkChannelMessage reply = this.GetReply();
				CopyLogReply copyLogReply = reply as CopyLogReply;
				if (copyLogReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(reply);
				}
				long elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)this.GetHashCode(), "Log Copy Response took: {0}ms", elapsedMilliseconds);
				this.m_endOfLog.SetValue(copyLogReply.EndOfLogGeneration, new DateTime?(copyLogReply.EndOfLogUtc));
				writeTimeUtc = copyLogReply.LastWriteUtc;
				CheckSummer summer = null;
				if (this.m_channel.ChecksumDataTransfer)
				{
					summer = new CheckSummer();
				}
				copyLogReply.ReceiveFile(destinationFileName, null, summer);
				elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)this.GetHashCode(), "Transmit/Decomp took: {0}ms", elapsedMilliseconds);
				base.RecordThruput(copyLogReply.FileSize);
				ExchangeNetworkPerfmonCounters perfCounters = this.m_channel.PerfCounters;
				if (perfCounters != null)
				{
					perfCounters.RecordLogCopyThruputReceived(copyLogReply.FileSize);
				}
				replayStopwatch.Stop();
				ExTraceGlobals.LogCopyClientTracer.TraceDebug((long)this.GetHashCode(), "{0}: LogCopy success: {1} for {2} after {3}ms", new object[]
				{
					ExDateTime.Now,
					replayStopwatch.ToString(),
					destinationFileName,
					replayStopwatch.ElapsedMilliseconds
				});
			}
			finally
			{
				this.ReleaseChannelLock();
			}
		}

		internal static void CopyLog(Guid dbGuid, NetworkChannel ch, long logGen, string destinationFileName)
		{
			ExTraceGlobals.LogCopyClientTracer.TraceDebug<string>((long)ch.GetHashCode(), "static CopyLog {0} starting", destinationFileName);
			CopyLogRequest copyLogRequest = new CopyLogRequest(ch, dbGuid, logGen);
			copyLogRequest.Send();
			ReplayStopwatch replayStopwatch = new ReplayStopwatch();
			replayStopwatch.Start();
			NetworkChannelMessage message = ch.GetMessage();
			CopyLogReply copyLogReply = message as CopyLogReply;
			if (copyLogReply == null)
			{
				ch.ThrowUnexpectedMessage(message);
			}
			long elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
			ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)ch.GetHashCode(), "Log Copy Response took: {0}ms", elapsedMilliseconds);
			CheckSummer summer = null;
			if (ch.ChecksumDataTransfer)
			{
				summer = new CheckSummer();
			}
			copyLogReply.ReceiveFile(destinationFileName, null, summer);
			elapsedMilliseconds = replayStopwatch.ElapsedMilliseconds;
			ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)ch.GetHashCode(), "Transmit/Decomp took: {0}ms", elapsedMilliseconds);
			ExchangeNetworkPerfmonCounters perfCounters = ch.PerfCounters;
			if (perfCounters != null)
			{
				perfCounters.RecordLogCopyThruputReceived(copyLogReply.FileSize);
			}
			replayStopwatch.Stop();
			ExTraceGlobals.LogCopyClientTracer.TraceDebug((long)ch.GetHashCode(), "{0}: LogCopy success: {1} for {2} after {3}ms", new object[]
			{
				ExDateTime.Now,
				replayStopwatch.ToString(),
				destinationFileName,
				replayStopwatch.ElapsedMilliseconds
			});
		}

		public override long GetE00Generation()
		{
			long num = 0L;
			this.GetChannelLock();
			try
			{
				this.OpenChannelIfFirstRequest();
				GetE00GenerationRequest msg = new GetE00GenerationRequest(this.m_channel, this.DatabaseGuid);
				this.SendMessage(msg);
				NetworkChannelMessage reply = this.GetReply();
				GetE00GenerationReply getE00GenerationReply = reply as GetE00GenerationReply;
				if (getE00GenerationReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(reply);
				}
				num = getE00GenerationReply.LogGeneration;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long>((long)this.GetHashCode(), "LogCopyClient:GetE00Gen: 0x{0:x}", num);
			}
			finally
			{
				this.ReleaseChannelLock();
			}
			return num;
		}

		public override bool LogExists(long logNum)
		{
			bool flag = false;
			this.GetChannelLock();
			try
			{
				this.OpenChannelIfFirstRequest();
				TestLogExistenceRequest msg = new TestLogExistenceRequest(this.m_channel, this.DatabaseGuid, logNum);
				this.SendMessage(msg);
				NetworkChannelMessage reply = this.GetReply();
				TestLogExistenceReply testLogExistenceReply = reply as TestLogExistenceReply;
				if (testLogExistenceReply == null)
				{
					this.m_channel.ThrowUnexpectedMessage(reply);
				}
				flag = testLogExistenceReply.LogExists;
				ExTraceGlobals.LogCopyClientTracer.TraceDebug<long, bool>((long)this.GetHashCode(), "LogCopyClient:LogExists(0x{0:x})={1}", logNum, flag);
			}
			finally
			{
				this.ReleaseChannelLock();
			}
			return flag;
		}

		private object m_channelLock = new object();

		private NetworkChannel m_channel;

		private string m_sourceAddr;

		private NetworkPath m_initialNetworkPath;
	}
}
