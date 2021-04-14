using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Notifications.Broker;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal class ClientAsyncCallState_Unsubscribe : ClientAsyncCallState
	{
		private void Cleanup()
		{
			if (this.m_szSubscription != IntPtr.Zero)
			{
				Marshal.FreeHGlobal(this.m_szSubscription);
				this.m_szSubscription = IntPtr.Zero;
			}
		}

		public ClientAsyncCallState_Unsubscribe(CancelableAsyncCallback asyncCallback, object asyncState, IntPtr bindingHandle, string subscription) : base("Unsubscribe", asyncCallback, asyncState)
		{
			try
			{
				this.m_pRpcBindingHandle = bindingHandle;
				this.m_szSubscription = IntPtr.Zero;
				bool flag = false;
				try
				{
					IntPtr szSubscription = Marshal.StringToHGlobalUni(subscription);
					this.m_szSubscription = szSubscription;
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

		private void ~ClientAsyncCallState_Unsubscribe()
		{
			this.Cleanup();
		}

		public unsafe override void InternalBegin()
		{
			<Module>.cli_Unsubscribe((_RPC_ASYNC_STATE*)base.RpcAsyncState().ToPointer(), this.m_pRpcBindingHandle.ToPointer(), (ushort*)this.m_szSubscription.ToPointer());
		}

		public BrokerStatus End()
		{
			BrokerStatus result;
			try
			{
				result = (BrokerStatus)base.CheckCompletion();
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

		private IntPtr m_szSubscription;
	}
}
