using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IServerChannelSinkStack : IServerResponseChannelSinkStack
	{
		[SecurityCritical]
		void Push(IServerChannelSink sink, object state);

		[SecurityCritical]
		object Pop(IServerChannelSink sink);

		[SecurityCritical]
		void Store(IServerChannelSink sink, object state);

		[SecurityCritical]
		void StoreAndDispatch(IServerChannelSink sink, object state);

		[SecurityCritical]
		void ServerCallback(IAsyncResult ar);
	}
}
