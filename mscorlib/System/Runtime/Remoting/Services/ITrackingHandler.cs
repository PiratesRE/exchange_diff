using System;
using System.Runtime.InteropServices;
using System.Security;

namespace System.Runtime.Remoting.Services
{
	[ComVisible(true)]
	public interface ITrackingHandler
	{
		[SecurityCritical]
		void MarshaledObject(object obj, ObjRef or);

		[SecurityCritical]
		void UnmarshaledObject(object obj, ObjRef or);

		[SecurityCritical]
		void DisconnectedObject(object obj);
	}
}
