using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public interface IChannelInfo
	{
		object[] ChannelData { [SecurityCritical] get; [SecurityCritical] set; }
	}
}
