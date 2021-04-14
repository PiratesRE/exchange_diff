using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class ClientAsyncCallState_GetNextNotification : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_pszNotification != IntPtr.Zero)
			{
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszNotification);
				IntPtr intPtr2 = intPtr;
				if (intPtr != IntPtr.Zero)
				{
					<Module>.MIDL_user_free(intPtr2.ToPointer());
				}
				Marshal.FreeHGlobal(this.m_pszNotification);
				this.m_pszNotification = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_GetNextNotification(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr bindingHandle, int consumerId, Guid ackNotificationId) : base("GetNextNotification", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = bindingHandle;
				this.m_consumerId = consumerId;
				this.m_ackNotificationId = ackNotificationId;
				this.m_pszNotification = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr pszNotification = Marshal.AllocHGlobal(IntPtr.Size);
					this.m_pszNotification = pszNotification;
					Marshal.WriteIntPtr(this.m_pszNotification, IntPtr.Zero);
					flag = true;
				}
				finally
				{
					if (!flag)
					{
						this.Cleanup();
					}
				}
			}
			catch
			{
				base.Dispose(true);
				throw;
			}
		}

		private void ~ClientAsyncCallState_GetNextNotification()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			Guid ackNotificationId = this.m_ackNotificationId;
			_GUID guid = <Module>.ToGUID(ref ackNotificationId);
			<Module>.cli_GetNextNotification((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), this.m_consumerId, guid, (ushort**)this.m_pszNotification.ToPointer());
		}

		public BrokerStatus End(out string notification)
		{
			BrokerStatus result;
			try
			{
				BrokerStatus brokerStatus = (BrokerStatus)base.CheckCompletion();
				notification = null;
				IntPtr intPtr = Marshal.ReadIntPtr(this.m_pszNotification);
				if (intPtr != IntPtr.Zero)
				{
					notification = Marshal.PtrToStringUni(intPtr);
				}
				result = brokerStatus;
			}
			finally
			{
				this.Cleanup();
			}
			return result;
		}

		[HandleProcessCorruptedStateExceptions]
		protected override void Dispose([MarshalAs(UnmanagedType.U1)] bool A_0)
		{
			if (A_0)
			{
				try
				{
					this.Cleanup();
					return;
				}
				finally
				{
					base.Dispose(true);
				}
			}
			base.Dispose(false);
		}

		private IntPtr m_pRpcBindingHandle;

		private int m_consumerId;

		private Guid m_ackNotificationId;

		private IntPtr m_pszNotification;
	}
}
