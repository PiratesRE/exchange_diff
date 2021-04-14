using System;

namespace Microsoft.Exchange.Rpc.Dar
{
	internal abstract class ExDarHostRpcServer : RpcServerBase
	{
		public abstract byte[] SendHostRequest(int version, int type, byte[] inputParameterBytes);

		public ExDarHostRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IExDarHost_v1_0_s_ifspec;
	}
}
