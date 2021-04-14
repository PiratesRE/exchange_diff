using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Microsoft.Exchange.Data.Transport.Interop
{
	[ComVisible(true)]
	[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
	[Guid("786E6730-5D95-3D9D-951B-5C9ABD1E158D")]
	[SuppressUnmanagedCodeSecurity]
	public interface IComInvoke
	{
		void ComAsyncInvoke(IProxyCallback callback);
	}
}
