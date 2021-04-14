using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class NotifyEndOfLogAsyncReply : NotifyEndOfLogReply
	{
		internal NotifyEndOfLogAsyncReply(NetworkChannel channel, long endOfLogGen, DateTime utc) : base(channel, NetworkChannelMessage.MessageType.NotifyEndOfLogAsyncReply, endOfLogGen, utc)
		{
		}

		internal NotifyEndOfLogAsyncReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.NotifyEndOfLogAsyncReply, packetContent)
		{
		}
	}
}
