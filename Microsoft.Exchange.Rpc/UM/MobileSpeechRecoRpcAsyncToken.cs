using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Rpc.UM
{
	internal class MobileSpeechRecoRpcAsyncToken
	{
		public unsafe MobileSpeechRecoRpcAsyncToken(_RPC_ASYNC_STATE* pAsyncState, int* pOutBytesLen, byte** ppOutBytes)
		{
			ExTraceGlobals.RpcTracer.TraceDebug((long)this.GetHashCode(), "Entering MobileSpeechRecoRpcAsyncToken constructor");
			IntPtr handle = new IntPtr((void*)pAsyncState);
			this.asyncStateHandle = new SafeRpcAsyncStateHandle(handle);
			this.pOutBytesLen = pOutBytesLen;
			this.ppOutBytes = ppOutBytes;
		}

		public unsafe void CompleteAsync(int returnVal, byte[] outBlob)
		{
			ExTraceGlobals.RpcTracer.TraceDebug((long)this.GetHashCode(), "Entering MobileSpeechRecoRpcAsyncToken.CompleteAsync");
			if (null != this.asyncStateHandle)
			{
				*(long*)this.ppOutBytes = <Module>.MToUBytesClient(outBlob, this.pOutBytesLen);
				this.asyncStateHandle.CompleteCall(returnVal);
				this.asyncStateHandle = null;
			}
		}

		private SafeRpcAsyncStateHandle asyncStateHandle;

		private unsafe int* pOutBytesLen;

		private unsafe byte** ppOutBytes;
	}
}
