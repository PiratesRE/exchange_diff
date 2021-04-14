using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.UM.TroubleshootingTool.Shared;
using Microsoft.Exchange.UM.UMCommon;
using Microsoft.Exchange.UM.UMCore.Exceptions;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class GatewayResolver : ICallHandler
	{
		public void HandleCall(CafeRoutingContext context)
		{
			ValidateArgument.NotNull(context, "RoutingContext");
			context.Tracer.Trace("GatewayResolver : TryHandleCall", new object[0]);
			UMIPGateway gateway = null;
			string remoteMatchedFqdn = null;
			if (RouterUtils.ShouldAccept(context.CallInfo, context.TenantGuid, out remoteMatchedFqdn, out gateway))
			{
				context.RemoteMatchedFqdn = remoteMatchedFqdn;
				context.Gateway = gateway;
				context.Tracer.Trace("GatewayResolver : TryHandleCall : Referrred-By header : {0},  RemoteMatchedFqdn :{1}, Gateway :{2}", new object[]
				{
					context.ReferredByHeader ?? "null",
					context.RemoteMatchedFqdn,
					context.Gateway.Name
				});
				UmGlobals.ExEvent.LogEvent(UMEventLogConstants.Tuple_CallRouterInboundCallParams, null, new object[]
				{
					CommonUtil.ToEventLogString(context.CallInfo.CallingParty.Uri),
					CommonUtil.ToEventLogString(context.CallInfo.CalledParty.Uri),
					CommonUtil.ToEventLogString(RouterUtils.GetDiversionLogString(context.CallInfo.DiversionInfo)),
					CommonUtil.ToEventLogString(context.ReferredByHeader),
					context.CallInfo.CallId,
					context.CallInfo.RemotePeer
				});
				return;
			}
			throw CallRejectedException.Create(Strings.CallFromInvalidGateway(context.CallInfo.RemotePeer.ToString()), CommonConstants.UseDataCenterCallRouting ? CallEndingReason.UnAuthorizedGatewayDatacenter : CallEndingReason.UnAuthorizedGateway, null, new object[0]);
		}
	}
}
