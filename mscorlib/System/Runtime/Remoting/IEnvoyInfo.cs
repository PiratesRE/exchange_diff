using System;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using System.Security;

namespace System.Runtime.Remoting
{
	[ComVisible(true)]
	public interface IEnvoyInfo
	{
		IMessageSink EnvoySinks { [SecurityCritical] get; [SecurityCritical] set; }
	}
}
