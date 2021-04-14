using System;

namespace System.Runtime.Remoting.Channels
{
	internal class RegisteredChannel
	{
		internal RegisteredChannel(IChannel chnl)
		{
			this.channel = chnl;
			this.flags = 0;
			if (chnl is IChannelSender)
			{
				this.flags |= 1;
			}
			if (chnl is IChannelReceiver)
			{
				this.flags |= 2;
			}
		}

		internal virtual IChannel Channel
		{
			get
			{
				return this.channel;
			}
		}

		internal virtual bool IsSender()
		{
			return (this.flags & 1) > 0;
		}

		internal virtual bool IsReceiver()
		{
			return (this.flags & 2) > 0;
		}

		private IChannel channel;

		private byte flags;

		private const byte SENDER = 1;

		private const byte RECEIVER = 2;
	}
}
