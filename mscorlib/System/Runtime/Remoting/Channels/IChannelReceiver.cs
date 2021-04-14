using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelReceiver : IChannel
	{
		object ChannelData { [SecurityCritical] get; }

		[SecurityCritical]
		string[] GetUrlsForUri(string objectURI);

		[SecurityCritical]
		void StartListening(object data);

		[SecurityCritical]
		void StopListening(object data);
	}
}
