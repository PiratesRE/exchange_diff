using System;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	internal class RegisteredChannelList
	{
		internal RegisteredChannelList()
		{
			this._channels = new RegisteredChannel[0];
		}

		internal RegisteredChannelList(RegisteredChannel[] channels)
		{
			this._channels = channels;
		}

		internal RegisteredChannel[] RegisteredChannels
		{
			get
			{
				return this._channels;
			}
		}

		internal int Count
		{
			get
			{
				if (this._channels == null)
				{
					return 0;
				}
				return this._channels.Length;
			}
		}

		internal IChannel GetChannel(int index)
		{
			return this._channels[index].Channel;
		}

		internal bool IsSender(int index)
		{
			return this._channels[index].IsSender();
		}

		internal bool IsReceiver(int index)
		{
			return this._channels[index].IsReceiver();
		}

		internal int ReceiverCount
		{
			get
			{
				if (this._channels == null)
				{
					return 0;
				}
				int num = 0;
				for (int i = 0; i < this._channels.Length; i++)
				{
					if (this.IsReceiver(i))
					{
						num++;
					}
				}
				return num;
			}
		}

		internal int FindChannelIndex(IChannel channel)
		{
			for (int i = 0; i < this._channels.Length; i++)
			{
				if (channel == this.GetChannel(i))
				{
					return i;
				}
			}
			return -1;
		}

		[SecurityCritical]
		internal int FindChannelIndex(string name)
		{
			for (int i = 0; i < this._channels.Length; i++)
			{
				if (string.Compare(name, this.GetChannel(i).ChannelName, StringComparison.OrdinalIgnoreCase) == 0)
				{
					return i;
				}
			}
			return -1;
		}

		private RegisteredChannel[] _channels;
	}
}
