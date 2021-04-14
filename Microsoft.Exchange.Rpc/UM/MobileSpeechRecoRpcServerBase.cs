using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class MobileSpeechRecoRpcServerBase : RpcServerBase
	{
		public void CompleteAsync(int returnVal, byte[] outBlob, object token)
		{
			ExTraceGlobals.RpcTracer.TraceDebug((long)this.GetHashCode(), "Entering MobileSpeechRecoRpcServerBase.CompleteAsync");
			MobileSpeechRecoRpcAsyncToken mobileSpeechRecoRpcAsyncToken = token as MobileSpeechRecoRpcAsyncToken;
			if (null == mobileSpeechRecoRpcAsyncToken)
			{
				throw new ArgumentException("Invalid type", "token");
			}
			mobileSpeechRecoRpcAsyncToken.CompleteAsync(returnVal, outBlob);
		}

		public abstract void ExecuteStepAsync(byte[] inBlob, object token);

		public MobileSpeechRecoRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IMobileSpeechReco_v1_0_s_ifspec;
	}
}
