using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannel
	{
		int ChannelPriority { [SecurityCritical] get; }

		string ChannelName { [SecurityCritical] get; }

		[SecurityCritical]
		string Parse(string url, out string objectURI);
	}
}
