using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal class RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register : RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, RpcHttpConnectionRegistrationAsyncRpcServer asyncServer, IntPtr bindingHandle, IntPtr pAssociationGroupId, IntPtr pToken, IntPtr pServerTarget, IntPtr pSessionCookie, IntPtr pClientIp, IntPtr pRequestId, IntPtr ppFailureMessage, IntPtr ppFailureDetails)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.m_bindingHandle = bindingHandle;
			this.m_pAssociationGroupId = pAssociationGroupId;
			this.m_pToken = pToken;
			this.m_pServerTarget = pServerTarget;
			this.m_pSessionCookie = pSessionCookie;
			this.m_pClientIp = pClientIp;
			this.m_pRequestId = pRequestId;
			this.m_ppFailureMessage = ppFailureMessage;
			this.m_ppFailureDetails = ppFailureDetails;
		}

		public override void Reset()
		{
			base.InternalReset();
			this.m_bindingHandle = IntPtr.Zero;
			this.m_pAssociationGroupId = IntPtr.Zero;
			this.m_pToken = IntPtr.Zero;
			this.m_pServerTarget = IntPtr.Zero;
			this.m_pSessionCookie = IntPtr.Zero;
			this.m_pClientIp = IntPtr.Zero;
			this.m_pRequestId = IntPtr.Zero;
			this.m_ppFailureMessage = IntPtr.Zero;
			this.m_ppFailureDetails = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			Guid associationGroupId = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.CopyIntPtrToGuid(this.m_pAssociationGroupId);
			string token = Marshal.PtrToStringAnsi(this.m_pToken);
			string serverTarget = Marshal.PtrToStringAnsi(this.m_pServerTarget);
			string sessionCookie = Marshal.PtrToStringAnsi(this.m_pSessionCookie);
			string clientIp = Marshal.PtrToStringAnsi(this.m_pClientIp);
			Guid requestId = RpcHttpConnectionRegistrationAsyncDispatchRpcStatePool<Microsoft::Exchange::Rpc::RpcHttpConnectionRegistration::RpcHttpConnectionRegistrationAsyncDispatchRpcState_Register>.CopyIntPtrToGuid(this.m_pRequestId);
			asyncDispatch.BeginRegister(associationGroupId, token, serverTarget, sessionCookie, clientIp, requestId, asyncCallback, this);
		}

		public unsafe override int InternalEnd(ICancelableAsyncResult asyncResult, IRpcHttpConnectionRegistrationAsyncDispatch asyncDispatch)
		{
			string text = null;
			string text2 = null;
			bool flag = false;
			byte* ptr = null;
			byte* ptr2 = null;
			int result;
			try
			{
				text = null;
				text2 = null;
				int num = asyncDispatch.EndRegister(asyncResult, out text, out text2);
				if (this.m_ppFailureMessage != IntPtr.Zero && text != null)
				{
					ptr = (byte*)<Module>.StringToUnmanagedMultiByte(text, 0U);
					*(long*)this.m_ppFailureMessage.ToPointer() = ptr;
				}
				if (this.m_ppFailureDetails != IntPtr.Zero && text2 != null)
				{
					ptr2 = (byte*)<Module>.StringToUnmanagedMultiByte(text2, 0U);
					*(long*)this.m_ppFailureDetails.ToPointer() = ptr2;
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag)
				{
					if (ptr != null)
					{
						<Module>.FreeString((ushort*)ptr);
						if (this.m_ppFailureMessage != IntPtr.Zero)
						{
							*(long*)this.m_ppFailureMessage.ToPointer() = 0L;
						}
					}
					if (ptr2 != null)
					{
						<Module>.FreeString((ushort*)ptr2);
						if (this.m_ppFailureDetails != IntPtr.Zero)
						{
							*(long*)this.m_ppFailureDetails.ToPointer() = 0L;
						}
					}
				}
			}
			return result;
		}

		private IntPtr m_bindingHandle;

		private IntPtr m_pAssociationGroupId;

		private IntPtr m_pToken;

		private IntPtr m_pServerTarget;

		private IntPtr m_pSessionCookie;

		private IntPtr m_pClientIp;

		private IntPtr m_pRequestId;

		private IntPtr m_ppFailureMessage;

		private IntPtr m_ppFailureDetails;
	}
}
