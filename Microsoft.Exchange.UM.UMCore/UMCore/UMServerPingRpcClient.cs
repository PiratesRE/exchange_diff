using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMServerPingRpcClient : UMServerPingRpcClientBase
	{
		internal UMServerPingRpcClient(string server) : base(server)
		{
			this.targetServer = server;
		}

		public override void Ping(Guid dialPlanGuid, ref bool availableToTakeCalls)
		{
			try
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcClient: Executing Ping on {0}. DialPlan:{1}", new object[]
				{
					this.targetServer,
					dialPlanGuid
				});
				base.Ping(dialPlanGuid, ref availableToTakeCalls);
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcClient: Ping on {0} succeeded. DialPlan:{1} AvailableToTakeCalls:{2}", new object[]
				{
					this.targetServer,
					dialPlanGuid,
					availableToTakeCalls
				});
			}
			catch (RpcException ex)
			{
				CallIdTracer.TraceWarning(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcClient: Ping on {0} failed. Dialplan:{1}. ErrorCode:{2} CallStack:{3}", new object[]
				{
					this.targetServer,
					dialPlanGuid,
					ex.ErrorCode,
					ex
				});
				throw;
			}
		}

		private string targetServer;
	}
}
