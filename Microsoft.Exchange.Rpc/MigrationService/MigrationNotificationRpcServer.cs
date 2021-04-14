using System;

namespace Microsoft.Exchange.Rpc.MigrationService
{
	internal abstract class MigrationNotificationRpcServer : RpcServerBase
	{
		public abstract byte[] UpdateMigrationRequest(int version, byte[] pInBytes);

		public MigrationNotificationRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.MigrationNotificationService_v1_0_s_ifspec;
	}
}
