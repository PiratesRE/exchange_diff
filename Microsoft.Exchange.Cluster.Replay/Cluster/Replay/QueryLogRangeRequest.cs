using System;
using System.IO;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class QueryLogRangeRequest : NetworkChannelDatabaseRequest
	{
		internal QueryLogRangeRequest(NetworkChannel channel, Guid dbGuid) : base(channel, NetworkChannelMessage.MessageType.QueryLogRangeRequest, dbGuid)
		{
		}

		internal QueryLogRangeRequest(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.QueryLogRangeRequest, packetContent)
		{
		}

		public override void Execute()
		{
			try
			{
				long firstAvailableGeneration = base.Channel.MonitoredDatabase.DetermineStartOfLog();
				EndOfLog currentEndOfLog = base.Channel.MonitoredDatabase.CurrentEndOfLog;
				new QueryLogRangeReply(base.Channel)
				{
					FirstAvailableGeneration = firstAvailableGeneration,
					EndOfLogGeneration = currentEndOfLog.Generation,
					EndOfLogUtc = currentEndOfLog.Utc
				}.Send();
			}
			catch (IOException ex)
			{
				base.Channel.SendException(ex);
				base.Channel.Close();
			}
		}
	}
}
