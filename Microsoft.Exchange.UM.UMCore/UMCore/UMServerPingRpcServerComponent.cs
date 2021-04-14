using System;
using System.Security.AccessControl;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMServerPingRpcServerComponent : UMRPCComponentBase
	{
		internal static UMServerPingRpcServerComponent Instance
		{
			get
			{
				return UMServerPingRpcServerComponent.instance;
			}
		}

		internal override void RegisterServer()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcServerComponent::RegisterServer()", new object[0]);
			uint accessMask = 1U;
			FileSecurity allowExchangeServerSecurity = Util.GetAllowExchangeServerSecurity();
			RpcServerBase.RegisterServer(typeof(UMServerPingRpcServerComponent.UMServerPingRpcServer), allowExchangeServerSecurity, accessMask);
		}

		private static UMServerPingRpcServerComponent instance = new UMServerPingRpcServerComponent();

		internal sealed class UMServerPingRpcServer : UMServerPingRpcServerBase
		{
			public override int Ping(Guid dialPlanGuid, ref bool availableToTakeCalls)
			{
				int result = 0;
				availableToTakeCalls = false;
				if (!UMServerPingRpcServerComponent.Instance.GuardBeforeExecution())
				{
					return result;
				}
				try
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcServer::Ping() executing rpc request.", new object[0]);
					availableToTakeCalls = (Util.VerifyServerIsInDialPlan(Utils.GetLocalUMServer(), dialPlanGuid, false) && !Util.MaxCallLimitExceeded());
				}
				catch (ExchangeServerNotFoundException ex)
				{
					CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcServer::Ping() Exception {0}", new object[]
					{
						ex
					});
				}
				catch (ADTransientException ex2)
				{
					CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcServer::Ping() Exception {0}", new object[]
					{
						ex2
					});
				}
				finally
				{
					UMServerPingRpcServerComponent.Instance.GuardAfterExecution();
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMServerPingRpcServer::Ping() AvailableToTakeCalls = {0}", new object[]
				{
					availableToTakeCalls
				});
				return result;
			}
		}
	}
}
