using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class CancelCiFileReply : NetworkChannelMessage
	{
		internal CancelCiFileReply(NetworkChannel channel) : base(channel, NetworkChannelMessage.MessageType.CancelCiFileReply)
		{
		}

		internal CancelCiFileReply(NetworkChannel channel, byte[] packetContent) : base(channel, NetworkChannelMessage.MessageType.CancelCiFileReply, packetContent)
		{
		}
	}
}
