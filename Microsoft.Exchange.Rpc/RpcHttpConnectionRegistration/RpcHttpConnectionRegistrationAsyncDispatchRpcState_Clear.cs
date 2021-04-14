using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear : RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Clear>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, RpcHttpConnectionRegistrationAsyncRpcServer asyncServer, IntPtr bindingHandle)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.m_bindingHandle = bindingHandle;
		}

		public override void Reset()
		{
			base.InternalReset();
			this.m_bindingHandle = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			asyncDispatch.BeginClear(asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			return asyncDispatch.EndClear(asyncResult);
		}

		private IntPtr m_bindingHandle;
	}
}
