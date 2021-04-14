using System;

namespace Microsoft.Exchange.EseRepl
{
	internal struct NetworkChannelMessageHeader
	{
		public NetworkChannelMessage.MessageType MessageType;

		public int MessageLength;

		public DateTime MessageUtc;
	}
}
