using System;

namespace Microsoft.Exchange.Rpc.OfflineRms
{
	internal abstract class OfflineRmsRpcServer : RpcServerBase
	{
		public abstract byte[] AcquireTenantLicenses(int version, byte[] inputParameterBytes);

		public abstract byte[] AcquireUseLicenses(int version, byte[] inputParameterBytes);

		public abstract byte[] GetTenantActiveCryptoMode(int version, byte[] inputParameterBytes);

		public OfflineRmsRpcServer()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IOfflineRms_v1_0_s_ifspec;
	}
}
