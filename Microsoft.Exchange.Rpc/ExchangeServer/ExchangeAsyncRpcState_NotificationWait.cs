using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class ExchangeAsyncRpcState_NotificationWait : BaseAsyncRpcState<Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcState_NotificationWait,Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcServer_AsyncEMSMDB,Microsoft::Exchange::Rpc::IExchangeAsyncDispatch>
	{
		public override int PoolSize()
		{
			return 65536;
		}

		public void Initialize(SafeRpcAsyncStateHandle asyncState, ExchangeAsyncRpcServer_AsyncEMSMDB asyncServer, IntPtr contextHandle, uint flagsIn, IntPtr pFlagsOut)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.flagsIn = flagsIn;
			this.pFlagsOut = pFlagsOut;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.flagsIn = 0U;
			this.pFlagsOut = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			bool flag = false;
			IntPtr intPtr = this.contextHandle;
			base.AsyncServer.RegisterConnectionDroppedNotification(base.AsyncState, intPtr);
			try
			{
				base.AsyncDispatch.BeginNotificationWait(null, this.contextHandle, (int)this.flagsIn, asyncCallback, this);
				flag = true;
			}
			finally
			{
				if (!flag)
				{
					base.AsyncServer.UnregisterConnectionDroppedNotification(base.AsyncState);
				}
			}
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			int result;
			try
			{
				int val = 0;
				int num = base.AsyncDispatch.EndNotificationWait(asyncResult, out val);
				Marshal.WriteInt32(this.pFlagsOut, val);
				result = num;
			}
			finally
			{
				base.AsyncServer.UnregisterConnectionDroppedNotification(base.AsyncState);
			}
			return result;
		}

		private IntPtr contextHandle;

		private uint flagsIn;

		private IntPtr pFlagsOut;
	}
}
