using System;
using System.Security.AccessControl;
using System.Threading;
using Microsoft.Exchange.Common;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.UM;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal sealed class UMMwiDeliveryRpcServer : UMMwiDeliveryRpcServerBase, IUMAsyncComponent
	{
		public AutoResetEvent StoppedEvent
		{
			get
			{
				return UMMwiDeliveryRpcServer.loadBalancer.ShutDownEvent;
			}
		}

		public bool IsInitialized
		{
			get
			{
				return UMMwiDeliveryRpcServer.initialized;
			}
		}

		public string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		internal static UMMwiDeliveryRpcServer Instance
		{
			get
			{
				return UMMwiDeliveryRpcServer.instance;
			}
		}

		public void StartNow(StartupStage stage)
		{
			if (stage == StartupStage.WPActivation)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, 0, "{0} starting in stage {1}", new object[]
				{
					this.Name,
					stage
				});
				UMMwiDeliveryRpcServer.loadBalancer = new MwiLoadBalancer(ExTraceGlobals.MWITracer, UMIPGatewayMwiTargetPicker.Instance, new UMIPGatewayMwiErrorStrategy());
				uint accessMask = 1U;
				FileSecurity allowExchangeServerSecurity = Util.GetAllowExchangeServerSecurity();
				RpcServerBase.RegisterServer(typeof(UMMwiDeliveryRpcServer), allowExchangeServerSecurity, accessMask);
				UMMwiDeliveryRpcServer.initialized = true;
			}
		}

		public void StopAsync()
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.MWITracer, 0, "UMMwiDeliveryRpcServer.Stop(): Stopping RPC server", new object[0]);
			if (UMMwiDeliveryRpcServer.loadBalancer != null)
			{
				RpcServerBase.StopServer(UMMwiDeliveryRpcServerBase.RpcIntfHandle);
				UMMwiDeliveryRpcServer.loadBalancer.ShutdownAsync();
			}
		}

		public void CleanupAfterStopped()
		{
			if (UMMwiDeliveryRpcServer.loadBalancer != null)
			{
				UMMwiDeliveryRpcServer.loadBalancer.CleanupAfterAsyncShutdown();
				UMMwiDeliveryRpcServer.loadBalancer = null;
			}
		}

		public override int SendMwiMessage(Guid mailboxGuid, Guid dialPlanGuid, string userExtension, string userName, int unreadVoicemailCount, int totalVoicemailCount, int assistantLatencyMsec, Guid tenantGuid)
		{
			int result = 0;
			try
			{
				GrayException.MapAndReportGrayExceptions(delegate()
				{
					ExDateTime eventTimeUtc = ExDateTime.UtcNow.Subtract(TimeSpan.FromMilliseconds((double)assistantLatencyMsec));
					MwiMessage mwiMessage = new MwiMessage(mailboxGuid, dialPlanGuid, userName, userExtension, unreadVoicemailCount, totalVoicemailCount, UMMwiDeliveryRpcServer.MessageExpirationTime, eventTimeUtc, tenantGuid);
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMMwiDeliveryRpcServer.SendMwiMessage(message={0})", new object[]
					{
						mwiMessage
					});
					UMMwiDeliveryRpcServer.loadBalancer.SendMessage(mwiMessage);
					CallIdTracer.TraceDebug(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMMwiDeliveryRpcServer.SendMwiMessage(message={0}) loadBalancer.SendMessage has finished.", new object[]
					{
						mwiMessage
					});
				});
			}
			catch (GrayException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.RpcTracer, this.GetHashCode(), "UMMwiDeliveryRpcServer.SendMwiMessage failed: {0} ", new object[]
				{
					ex
				});
				result = -2147466752;
			}
			return result;
		}

		private static readonly TimeSpan MessageExpirationTime = TimeSpan.FromSeconds(30.0);

		private static MwiLoadBalancer loadBalancer;

		private static bool initialized;

		private static UMMwiDeliveryRpcServer instance = new UMMwiDeliveryRpcServer();
	}
}
