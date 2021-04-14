using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Rpc.PoolRpc
{
	public interface IPoolRpcServer
	{
		int EcPoolConnect(uint flags, Guid poolGuid, ArraySegment<byte> auxiliaryIn, IPoolConnectCompletion completion);

		int EcPoolDisconnect(IntPtr contextHandle);

		int EcPoolCreateSession(IntPtr contextHandle, ClientSecurityContext callerSecurityContext, byte[] sessionSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, ArraySegment<byte> auxiliaryIn, IPoolCreateSessionCompletion completion);

		int EcPoolCloseSession(IntPtr contextHandle, uint sessionHandle, IPoolCloseSessionCompletion completion);

		int EcPoolSessionDoRpc(IntPtr contextHandle, uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, IPoolSessionDoRpcCompletion completion);

		int EcPoolWaitForNotificationsAsync(IntPtr contextHandle, IPoolWaitForNotificationsCompletion completion);

		ushort GetVersionDelta();
	}
}
