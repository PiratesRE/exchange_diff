using System;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class NotificationsBrokerAsyncRpcState_GetNextNotification : BaseAsyncRpcState<Microsoft::Exchange::Rpc::NotificationsBroker::NotificationsBrokerAsyncRpcState_GetNextNotification,Microsoft::Exchange::Rpc::NotificationsBroker::NotificationsBrokerAsyncRpcServer,Microsoft::Exchange::Rpc::INotificationsBrokerAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, NotificationsBrokerAsyncRpcServer asyncServer, IntPtr bindingHandle, int consumerId, Guid ackNotificationId, IntPtr ppNotification)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.m_bindingHandle = bindingHandle;
			this.m_consumerId = consumerId;
			this.m_ackNotificationId = ackNotificationId;
			this.m_ppNotification = ppNotification;
			this.m_clientBinding = null;
		}

		public override void InternalReset()
		{
			this.m_bindingHandle = IntPtr.Zero;
			this.m_consumerId = 0;
			this.m_ackNotificationId = Guid.Empty;
			this.m_ppNotification = IntPtr.Zero;
			this.m_clientBinding = null;
		}

		public unsafe override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			if (this.m_ppNotification != IntPtr.Zero)
			{
				*(long*)this.m_ppNotification.ToPointer() = 0L;
			}
			RpcClientBinding clientBinding = new RpcClientBinding(this.m_bindingHandle, base.AsyncState);
			this.m_clientBinding = clientBinding;
			base.AsyncDispatch.BeginGetNextNotification(null, clientBinding, this.m_consumerId, this.m_ackNotificationId, asyncCallback, this);
		}

		public unsafe override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			string @string = null;
			bool flag = false;
			ushort* ptr = null;
			int result;
			try
			{
				@string = null;
				int num = (int)base.AsyncDispatch.EndGetNextNotification(asyncResult, out @string);
				if (num == 0 && this.m_ppNotification != IntPtr.Zero)
				{
					ptr = <Module>.StringToUnmanaged(@string);
					*(long*)this.m_ppNotification.ToPointer() = ptr;
				}
				flag = true;
				result = num;
			}
			finally
			{
				if (!flag && ptr != null)
				{
					<Module>.FreeString(ptr);
					if (this.m_ppNotification != IntPtr.Zero)
					{
						*(long*)this.m_ppNotification.ToPointer() = 0L;
					}
				}
			}
			return result;
		}

		private IntPtr m_bindingHandle;

		private int m_consumerId;

		private Guid m_ackNotificationId;

		private IntPtr m_ppNotification;

		private ClientBinding m_clientBinding;
	}
}
