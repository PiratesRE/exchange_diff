using System;
using System.Runtime.InteropServices;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Rpc.ExchangeServer
{
	internal class ExchangeAsyncRpcState_Disconnect : BaseAsyncRpcState<Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcState_Disconnect,Microsoft::Exchange::Rpc::ExchangeServer::ExchangeAsyncRpcServer_EMSMDB,Microsoft::Exchange::Rpc::IExchangeAsyncDispatch>
	{
		public void Initialize(SafeRpcAsyncStateHandle asyncState, ExchangeAsyncRpcServer_EMSMDB asyncServer, IntPtr pContextHandle)
		{
			base.InternalInitialize(asyncState, asyncServer);
			this.pContextHandle = pContextHandle;
		}

		public override void InternalReset()
		{
			this.pContextHandle = IntPtr.Zero;
		}

		public override void InternalBegin(CancelableAsyncCallback asyncCallback)
		{
			IntPtr contextHandle = Marshal.ReadIntPtr(this.pContextHandle);
			base.AsyncDispatch.BeginDisconnect(null, contextHandle, asyncCallback, this);
		}

		public override int InternalEnd(ICancelableAsyncResult asyncResult)
		{
			IntPtr zero = IntPtr.Zero;
			int result = base.AsyncDispatch.EndDisconnect(asyncResult, out zero);
			Marshal.WriteIntPtr(this.pContextHandle, zero);
			return result;
		}

		private IntPtr pContextHandle;
	}
}
