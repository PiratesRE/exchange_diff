using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.RpcClientAccess.Messages;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcAsynchronousServer : ExchangeAsyncRpcServer_AsyncEMSMDB
	{
		internal static void Initialize(IExchangeAsyncDispatch exchangeAsyncDispatch, int maximumConcurrentCalls, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(exchangeAsyncDispatch, "exchangeAsyncDispatch");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			RpcAsynchronousServer.exchangeAsyncDispatch = exchangeAsyncDispatch;
			RpcAsynchronousServer.eventLog = eventLog;
			RpcAsynchronousServer.maximumConcurrentCalls = maximumConcurrentCalls;
		}

		internal static void Start()
		{
			bool flag = false;
			if (RpcAsynchronousServer.server == null)
			{
				try
				{
					RpcAsynchronousServer.server = (RpcAsynchronousServer)RpcServerBase.RegisterAutoListenInterfaceSupportingAnonymous(typeof(RpcAsynchronousServer), RpcAsynchronousServer.maximumConcurrentCalls, null, true);
					RpcAsynchronousServer.server.StartDroppedConnectionNotificationThread();
					RpcAsynchronousServer.server.StartRundownQueue();
					flag = true;
				}
				catch (DuplicateRpcEndpointException ex)
				{
					RpcAsynchronousServer.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_DuplicateRpcEndpoint, string.Empty, new object[]
					{
						ex.Message
					});
					throw new RpcServiceAbortException("RpcAsynchronousServer is being aborted the service due to DuplicateRpcEndpointException", ex);
				}
				finally
				{
					if (!flag)
					{
						RpcAsynchronousServer.Stop();
						RpcAsynchronousServer.exchangeAsyncDispatch = null;
					}
				}
			}
		}

		internal static void Stop()
		{
			if (RpcAsynchronousServer.server != null)
			{
				RpcAsynchronousServer.server.StopDroppedConnectionNotificationThread();
				RpcServerBase.UnregisterInterface(ExchangeAsyncRpcServer_AsyncEMSMDB.RpcIntfHandle);
				RpcAsynchronousServer.server.StopRundownQueue();
				RpcAsynchronousServer.server = null;
			}
		}

		public override IExchangeAsyncDispatch GetAsyncDispatch()
		{
			if (RpcClientAccessService.IsShuttingDown)
			{
				return null;
			}
			return RpcAsynchronousServer.exchangeAsyncDispatch;
		}

		private static RpcAsynchronousServer server = null;

		private static IExchangeAsyncDispatch exchangeAsyncDispatch = null;

		private static int maximumConcurrentCalls;

		private static ExEventLog eventLog;
	}
}
