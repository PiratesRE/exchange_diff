using System;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.ExchangeServer;
using Microsoft.Exchange.RpcClientAccess.Messages;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal sealed class RpcServer : ExchangeAsyncRpcServer_EMSMDB
	{
		internal static void Initialize(IExchangeAsyncDispatch exchangeAsyncDispatch, int maximumConcurrentCalls, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(exchangeAsyncDispatch, "exchangeAsyncDispatch");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			RpcServer.exchangeAsyncDispatch = exchangeAsyncDispatch;
			RpcServer.eventLog = eventLog;
			RpcServer.maximumConcurrentCalls = maximumConcurrentCalls;
		}

		internal static void Start()
		{
			bool flag = false;
			if (RpcServer.server == null)
			{
				try
				{
					RpcServer.server = (RpcServer)RpcServerBase.RegisterAutoListenInterfaceSupportingAnonymous(typeof(RpcServer), RpcServer.maximumConcurrentCalls, null, true);
					RpcServer.server.StartRundownQueue();
					flag = true;
				}
				catch (DuplicateRpcEndpointException ex)
				{
					RpcServer.eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_DuplicateRpcEndpoint, string.Empty, new object[]
					{
						ex.Message
					});
					throw new RpcServiceAbortException("RpcServer is being aborted the service due to DuplicateRpcEndpointException", ex);
				}
				finally
				{
					if (!flag)
					{
						RpcServer.Stop();
						RpcServer.exchangeAsyncDispatch = null;
					}
				}
			}
		}

		internal static void Stop()
		{
			if (RpcServer.server != null)
			{
				RpcServerBase.UnregisterInterface(ExchangeAsyncRpcServer_EMSMDB.RpcIntfHandle, true);
				RpcServer.server.StopRundownQueue();
				RpcServer.server = null;
			}
		}

		public override IExchangeAsyncDispatch GetAsyncDispatch()
		{
			if (RpcClientAccessService.IsShuttingDown)
			{
				return null;
			}
			return RpcServer.exchangeAsyncDispatch;
		}

		public override ushort GetVersionDelta()
		{
			return 4000;
		}

		private static RpcServer server = null;

		private static IExchangeAsyncDispatch exchangeAsyncDispatch = null;

		private static int maximumConcurrentCalls;

		private static ExEventLog eventLog;
	}
}
