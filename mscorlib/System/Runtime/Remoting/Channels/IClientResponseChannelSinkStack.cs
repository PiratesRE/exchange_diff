using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientResponseChannelSinkStack
	{
		[SecurityCritical]
		void AsyncProcessResponse(ITransportHeaders headers, Stream stream);

		[SecurityCritical]
		void DispatchReplyMessage(IMessage msg);

		[SecurityCritical]
		void DispatchException(Exception e);
	}
}
