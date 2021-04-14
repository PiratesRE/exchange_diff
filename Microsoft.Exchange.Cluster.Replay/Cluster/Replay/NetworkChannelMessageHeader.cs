using System;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal struct NetworkChannelMessageHeader
	{
		public NetworkChannelMessage.MessageType MessageType;

		public int MessageLength;

		public DateTime MessageUtc;
	}
}
