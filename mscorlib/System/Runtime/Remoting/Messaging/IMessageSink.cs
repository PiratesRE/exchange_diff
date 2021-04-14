using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMessageSink
	{
		[SecurityCritical]
		IMessage SyncProcessMessage(IMessage msg);

		[SecurityCritical]
		IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink);

		IMessageSink NextSink { [SecurityCritical] get; }
	}
}
