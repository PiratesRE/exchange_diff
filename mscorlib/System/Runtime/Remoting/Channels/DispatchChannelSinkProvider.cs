using System;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	internal class DispatchChannelSinkProvider : IServerChannelSinkProvider
	{
		internal DispatchChannelSinkProvider()
		{
		}

		[SecurityCritical]
		public void GetChannelData(IChannelDataStore channelData)
		{
		}

		[SecurityCritical]
		public IServerChannelSink CreateSink(IChannelReceiver channel)
		{
			return new DispatchChannelSink();
		}

		public IServerChannelSinkProvider Next
		{
			[SecurityCritical]
			get
			{
				return null;
			}
			[SecurityCritical]
			set
			{
				throw new NotSupportedException();
			}
		}
	}
}
