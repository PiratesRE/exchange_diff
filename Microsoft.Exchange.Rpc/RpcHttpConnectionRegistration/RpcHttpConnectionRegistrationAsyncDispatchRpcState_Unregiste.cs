using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class RpcHttpConnectionRegistrationAsyncDispatchRpcState_Unregister : RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Unregister>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, RpcHttpConnectionRegistrationAsyncRpcServer asyncServer, IntPtr bindingHandle, IntPtr pAssociationGroupId, IntPtr pRequestId)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.m_bindingHandle = bindingHandle;
			this.m_pAssociationGroupId = pAssociationGroupId;
			this.m_pRequestId = pRequestId;
		}

		public override void Reset()
		{
			base.InternalReset();
			this.m_bindingHandle = IntPtr.Zero;
			this.m_pAssociationGroupId = IntPtr.Zero;
			this.m_pRequestId = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			Guid associationGroupId = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Unregister>.CopyIntPtrToGuid(this.m_pAssociationGroupId);
			Guid requestId = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Unregister>.CopyIntPtrToGuid(this.m_pRequestId);
			asyncDispatch.BeginUnregister(associationGroupId, requestId, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			return asyncDispatch.EndUnregister(asyncResult);
		}

		private IntPtr m_bindingHandle;

		private IntPtr m_pAssociationGroupId;

		private IntPtr m_pRequestId;
	}
}
