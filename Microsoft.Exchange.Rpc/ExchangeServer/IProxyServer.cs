using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	public interface IProxyServer
	{
		int EcDoConnectEx(ClientSecurityContext callerSecurityContext, out IntPtr contextHandle, string userDn, uint flags, uint conMod, uint cpid, uint lcidString, uint lcidSort, out uint pollsMax, out uint retryCount, out uint retryDelay, out string displayName, short[] clientVersion, byte[] auxIn, out byte[] auxOut);

		int EcDoDisconnect(ref IntPtr contextHandle);

		int EcDoRpcExt2(ref IntPtr contextHandle, ref uint flags, ArraySegment<byte> request, uint maximumResponseSize, out ArraySegment<byte> response, ArraySegment<byte> auxIn, out ArraySegment<byte> auxOut);

		int EcDoAsyncConnectEx(IntPtr contextHandle, out IntPtr asynchronousContextHandle);

		int DoAsyncWaitEx(IntPtr asynchronousContextHandle, uint ulFlagsIn, IProxyAsyncWaitCompletion completionObject);

		ushort GetVersionDelta();
	}
}
