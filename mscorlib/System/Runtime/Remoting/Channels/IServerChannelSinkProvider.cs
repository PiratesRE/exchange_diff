using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IServerChannelSinkProvider
	{
		[SecurityCritical]
		void GetChannelData(IChannelDataStore channelData);

		[SecurityCritical]
		IServerChannelSink CreateSink(IChannelReceiver channel);

		IServerChannelSinkProvider Next { [SecurityCritical] get; [SecurityCritical] set; }
	}
}
