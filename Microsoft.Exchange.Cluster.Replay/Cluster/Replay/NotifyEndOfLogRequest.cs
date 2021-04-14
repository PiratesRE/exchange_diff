using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NotifyEndOfLogRequest : NetworkChannelDatabaseRequest
	{
		internal NotifyEndOfLogRequest(NetworkChannel channel, Guid dbGuid, long nextGenOfInterest) : base(channel, NetworkChannelMessage.MessageType.NotifyEndOfLogRequest, dbGuid)
		{
			this.m_nextGenOfInterest = nextGenOfInterest;
		}

		protected override void Serialize()
		{
			base.Serialize();
			base.Packet.Append(this.m_nextGenOfInterest);
		}

		internal NotifyEndOfLogRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.NotifyEndOfLogRequest, packetContent)
		{
			this.m_nextGenOfInterest = base.Packet.ExtractInt64();
		}

		public override void Execute()
		{
			MonitoredDatabase monitoredDatabase = base.Channel.MonitoredDatabase;
			EndOfLog currentEndOfLog = monitoredDatabase.CurrentEndOfLog;
			NotifyEndOfLogReply notifyEndOfLogReply = new NotifyEndOfLogReply(base.Channel, NetworkChannelMessage.MessageType.NotifyEndOfLogReply, currentEndOfLog.Generation, currentEndOfLog.Utc);
			notifyEndOfLogReply.Send();
		}

		private long m_nextGenOfInterest;
	}
}
