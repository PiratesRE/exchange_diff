using System;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class CopyLogRequest : NetworkChannelDatabaseRequest
	{
		internal CopyLogRequest(NetworkChannel channel, Guid dbGuid, long logNum) : base(channel, NetworkChannelMessage.MessageType.CopyLogRequest, dbGuid)
		{
			this.m_logGeneration = logNum;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_logGeneration);
		}

		internal CopyLogRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CopyLogRequest, packetContent)
		{
			this.m_logGeneration = base.Packet.ExtractInt64();
		}

		public override void Execute()
		{
			ExTraceGlobals.LogCopyServerTracer.TraceDebug<long>((long)this.GetHashCode(), "Requesting log 0x{0:x}, d({0})", this.m_logGeneration);
			base.Channel.MonitoredDatabase.TrySendLogWithStandardHandling(this.m_logGeneration, base.Channel);
		}

		private long m_logGeneration;
	}
}
