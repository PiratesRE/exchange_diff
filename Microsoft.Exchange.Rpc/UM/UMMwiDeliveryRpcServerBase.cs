using System;

namespace Microsoft.Exchange.Rpc.UM
{
	internal abstract class UMMwiDeliveryRpcServerBase : RpcServerBase
	{
		public abstract int SendMwiMessage(Guid mailboxGuid, Guid dialPlanGuid, string userExtension, string userName, int unreadVoicemailCount, int totalVoicemailCount, int assistantLatencyMsec, Guid tenantGuid);

		public UMMwiDeliveryRpcServerBase()
		{
		}

		public static IntPtr RpcIntfHandle = (IntPtr)<Module>.IUMMwiDelivery_v2_0_s_ifspec;
	}
}
