using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Channels
{
	[ComVisible(true)]
	public interface IChannelSinkBase
	{
		IDictionary Properties { [SecurityCritical] get; }
	}
}
