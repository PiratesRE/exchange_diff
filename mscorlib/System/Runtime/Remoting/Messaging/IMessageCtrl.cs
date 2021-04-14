using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Messaging
{
	[ComVisible(true)]
	public interface IMessageCtrl
	{
		[SecurityCritical]
		void Cancel(int msToCancel);
	}
}
