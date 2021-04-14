using System;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal abstract class MigrationProxyRpcServer : RpcServerBase
	{
		public static MigrationProxyRpcServer GetServerInstance()
		{
			return (MigrationProxyRpcServer)RpcServerBase.GetServerInstance(MigrationProxyRpcServer.RpcIntfHandle);
		}

		public abstract int NspiQueryRows(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		public abstract int NspiGetRecipient(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		public abstract int NspiSetRecipient(int version, byte[] inBlob, out byte[] outBlob);

		public abstract int NspiGetGroupMembers(int version, byte[] inBlob, out byte[] outBlob, out SafeRpcMemoryHandle rowsetHandle);

		public abstract uint NspiRfrGetNewDSA(int version, byte[] inBlob, out byte[] outBlob);

		public abstract void AutodiscoverGetUserSettings(int version, byte[] inBlob, out byte[] outBlob);

		public MigrationProxyRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.MigrationProxyRpc_v1_0_s_ifspec;
	}
}
