using System;
using System.Runtime.InteropServices;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal abstract class MigrationServiceRpcServer : RpcServerBase
	{
		public abstract byte[] InvokeMigrationServiceEndPoint(int version, byte[] pInBytes);

		[return: MarshalAs(UnmanagedType.U1)]
		public static bool IsRpcConnectionError(int errorCode)
		{
			return errorCode == 1753 || errorCode == 1722 || errorCode == 1727;
		}

		public MigrationServiceRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.MigrationService_v1_0_s_ifspec;
	}
}
