using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class ExchangeAsyncRpcState_NotificationConnect : BaseAsyncRpcState<Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcState_NotificationConnect,Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcServer_EMSMDB,Microsoft::Exchange::Rpc::IExchangeAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, ExchangeAsyncRpcServer_EMSMDB asyncServer, IntPtr contextHandle, IntPtr pNotificationContextHandle)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.contextHandle = contextHandle;
			this.pNotificationContextHandle = pNotificationContextHandle;
		}

		public override void InternalReset()
		{
			this.contextHandle = IntPtr.Zero;
			this.pNotificationContextHandle = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			base.AsyncDispatch.BeginNotificationConnect(null, this.contextHandle, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			IntPtr zero = IntPtr.Zero;
			int result = base.AsyncDispatch.EndNotificationConnect(asyncResult, out zero);
			Marshal.WriteIntPtr(this.pNotificationContextHandle, zero);
			return result;
		}

		private IntPtr contextHandle;

		private IntPtr pNotificationContextHandle;
	}
}
