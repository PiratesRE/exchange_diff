using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelReceiverHook
	{
		string ChannelScheme { [SecurityCritical] get; }

		bool WantsToListen { [SecurityCritical] get; }

		IServerChannelSink ChannelSinkChain { [SecurityCritical] get; }

		[SecurityCritical]
		void AddHookChannelUri(string channelUri);
	}
}
