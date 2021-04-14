using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.Rpc;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.Management.Tasks.UM
{
	internal class UMClientRpc : UMRpcClient
	{
		public UMClientRpc(string serverName) : base(serverName)
		{
			ExTraceGlobals.DiagnosticTracer.TraceDebug((long)this.GetHashCode(), "In UMClientRpc");
		}

		public ActiveCalls[] GetUmActiveCallList(bool isDialPlan, string dialPlan, bool isIpGateway, string ipGateway)
		{
			ExTraceGlobals.DiagnosticTracer.TraceDebug((long)this.GetHashCode(), "In UMClientRpc.GetUmActiveCallList");
			byte[] umActiveCalls = base.GetUmActiveCalls(isDialPlan, dialPlan, isIpGateway, ipGateway);
			return (ActiveCalls[])Serialization.BytesToObject(umActiveCalls);
		}
	}
}
