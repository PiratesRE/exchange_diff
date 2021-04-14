using System;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.Server.Storage.Common;

namespace Microsoft.Exchange.Server.Storage.RpcProxy
{
	internal interface IPoolSessionManager
	{
		ErrorCode CreateProxySession(ClientSecurityContext callerSecurityContext, uint flags, string userDn, uint connectionMode, uint codePageId, uint localeIdString, uint localeIdSort, short[] clientVersion, byte[] auxiliaryIn, Action<ErrorCode, uint> notificationPendingCallback, out uint sessionHandle, out byte[] auxiliaryOut);

		ErrorCode BeginPoolDoRpc(ref uint sessionHandle, uint flags, uint maximumResponseSize, ArraySegment<byte> request, ArraySegment<byte> auxiliaryIn, DoRpcCompleteCallback callback, Action<RpcException> exceptionCallback);

		ErrorCode QueueNotificationWait(ref uint sessionHandle, IProxyAsyncWaitCompletion completion);

		void CloseSession(uint sessionHandle);
	}
}
