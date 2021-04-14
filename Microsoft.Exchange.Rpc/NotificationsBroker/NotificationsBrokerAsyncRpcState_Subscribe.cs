using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class NotificationsBrokerAsyncRpcState_Subscribe : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NotificationsBroker::NotificationsBrokerAsyncRpcState_Subscribe,Microsoft::Exchange::Rpc::NotificationsBroker::NotificationsBrokerAsyncRpcServer,Microsoft::Exchange::Rpc::INotificationsBrokerAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NotificationsBrokerAsyncRpcServer asyncServer, IntPtr bindingHandle, IntPtr pSubscription)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.m_bindingHandle = bindingHandle;
			this.m_pSubscription = pSubscription;
			this.m_clientBinding = null;
		}

		public override void InternalReset()
		{
			this.m_bindingHandle = IntPtr.Zero;
			this.m_pSubscription = IntPtr.Zero;
			this.m_clientBinding = null;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			string subscription = null;
			if (this.m_pSubscription != IntPtr.Zero)
			{
				subscription = Marshal.PtrToStringUni(this.m_pSubscription);
			}
			RpcClientBinding clientBinding = new RpcClientBinding(this.m_bindingHandle, base.AsyncState);
			this.m_clientBinding = clientBinding;
			base.AsyncDispatch.BeginSubscribe(null, clientBinding, subscription, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			return (int)base.AsyncDispatch.EndSubscribe(asyncResult);
		}

		private IntPtr m_bindingHandle;

		private IntPtr m_pSubscription;

		private ClientBinding m_clientBinding;
	}
}
