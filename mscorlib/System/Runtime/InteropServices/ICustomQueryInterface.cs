using System;
using System.Security;

namespace System.Runtime.InteropServices
{
	[ComVisible(false)]
	[__DynamicallyInvokable]
	public interface ICustomQueryInterface
	{
		[SecurityCritical]
		CustomQueryInterfaceResult GetInterface([In] ref Guid iid, out IntPtr ppv);
	}
}
