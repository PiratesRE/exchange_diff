using System;

namespace Microsoft.Exchange.Rpc.RfriServer
{
	internal abstract class RfriAsyncRpcServer : BaseAsyncRpcServer<Microsoft::Exchange::Rpc::IRfriAsyncDispatch>
	{
		public override void DroppedConnection(IntPtr contextHandle)
		{
		}

		public override void RundownContext(IntPtr contextHandle)
		{
		}

		public override void StartRundownQueue()
		{
		}

		public override void StopRundownQueue()
		{
		}

		public RfriAsyncRpcServer()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static RfriAsyncRpcServer()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.rfri_v1_0_s_ifspec);
			RfriAsyncRpcServer.RpcIntfHandle = rpcIntfHandle;
		}

		public static readonly IntPtr RpcIntfHandle;
	}
}
