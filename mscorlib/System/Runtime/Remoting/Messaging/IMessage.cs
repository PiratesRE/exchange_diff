using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMessage
	{
		IDictionary Properties { [SecurityCritical] get; }
	}
}
