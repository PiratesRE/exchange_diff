using System;

namespace Microsoft.Exchange.Rpc.NotificationsBroker
{
	internal abstract class NotificationsBrokerAsyncRpcServer : BaseAsyncRpcServer<Microsoft::Exchange::Rpc::INotificationsBrokerAsyncDispatch>
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

		public NotificationsBrokerAsyncRpcServer()
		{
		}

		// Note: this type is marked as 'beforefieldinit'.
		static NotificationsBrokerAsyncRpcServer()
		{
			IntPtr rpcIntfHandle = new IntPtr(<Module>.INotificationsBrokerRpc_v1_0_s_ifspec);
			NotificationsBrokerAsyncRpcServer.RpcIntfHandle = rpcIntfHandle;
		}

		public static readonly IntPtr RpcIntfHandle;
	}
}
