using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.RpcHttpConnectionRegistration
{
	internal abstract class RpcHttpConnectionRegistrationAsyncRpcServer : RpcServerBase
	{
		public RpcHttpConnectionRegistrationAsyncRpcServer()
		{
		}

		public abstract IRpcHttpConnectionRegistrationAsyncDispatch GetRpcHttpConnectionRegistrationAsyncDispatch();

		[return: MarshalAs(UnmanagedType.U1)]
		public abstract bool IsShuttingDown();

		public static readonly IntPtr RpcIntfHandle = (IntPtr)<Module>.RpcHttpConnectionRegistrationAsync_v1_0_s_ifspec;
	}
}
