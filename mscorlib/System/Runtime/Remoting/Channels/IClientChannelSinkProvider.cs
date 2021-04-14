using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientChannelSinkProvider
	{
		[SecurityCritical]
		IClientChannelSink CreateSink(IChannelSender channel, string url, object remoteChannelData);

		IClientChannelSinkProvider Next { [SecurityCritical] get; [SecurityCritical] set; }
	}
}
