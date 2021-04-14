using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IClientFormatterSink : IMessageSink, IClientChannelSink, IChannelSinkBase
	{
	}
}
