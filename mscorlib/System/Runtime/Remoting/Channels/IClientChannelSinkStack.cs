using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientChannelSinkStack : IClientResponseChannelSinkStack
	{
		[SecurityCritical]
		void Push(IClientChannelSink sink, object state);

		[SecurityCritical]
		object Pop(IClientChannelSink sink);
	}
}
