using System;
using System.ServiceModel;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.RpcClientAccess;
using Microsoft.Exchange.Net.XropService;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.RpcClientAccess.Diagnostics;
using Microsoft.Exchange.RpcClientAccess.Messages;

namespace Microsoft.Exchange.RpcClientAccess.Server
{
	internal static class WebServiceEndPoint
	{
		internal static void Start(IExchangeAsyncDispatch exchangeAsyncDispatch, string endpoint, ExEventLog eventLog)
		{
			Util.ThrowOnNullArgument(exchangeAsyncDispatch, "exchangeAsyncDispatch");
			Util.ThrowOnNullArgument(endpoint, "endpoint");
			Util.ThrowOnNullArgument(eventLog, "eventLog");
			lock (WebServiceEndPoint.initializeLock)
			{
				if (!WebServiceEndPoint.endpointRegistered)
				{
					try
					{
						WebServiceEndPoint.exchangeAsyncDispatch = exchangeAsyncDispatch;
						ExternalAuthentication current = ExternalAuthentication.GetCurrent();
						if (current != null && current.Enabled)
						{
							Uri endpoint2 = new Uri(endpoint);
							try
							{
								Server.InitializeGlobalErrorHandlers(new WebServiceEndPoint.WebServiceDiagnosticsInfo());
								WebServiceEndPoint.server = new Server(endpoint2, current.TokenValidator, new WebServiceAuthorizationManager(), new WebServiceServerSessionProvider(), new WebServiceEndPoint.WebServiceDiagnosticsInfo());
								eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_WebServiceEndPointRegistered, string.Empty, new object[]
								{
									endpoint
								});
								goto IL_D8;
							}
							catch (AddressAccessDeniedException ex)
							{
								eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_CannotRegisterEndPointAccessDenied, ex.ToString(), new object[0]);
								goto IL_D8;
							}
						}
						eventLog.LogEvent(RpcClientAccessServiceEventLogConstants.Tuple_FederatedAuthentication, string.Empty, new object[0]);
						IL_D8:
						WebServiceEndPoint.endpointRegistered = true;
					}
					finally
					{
						if (!WebServiceEndPoint.endpointRegistered)
						{
							WebServiceEndPoint.Stop();
						}
					}
				}
			}
		}

		internal static void Stop()
		{
			lock (WebServiceEndPoint.initializeLock)
			{
				Server.TerminateGlobalErrorHandlers();
				if (WebServiceEndPoint.endpointRegistered)
				{
					if (WebServiceEndPoint.server != null)
					{
						WebServiceEndPoint.server.Dispose();
						WebServiceEndPoint.server = null;
					}
					WebServiceEndPoint.exchangeAsyncDispatch = null;
					WebServiceEndPoint.endpointRegistered = false;
				}
			}
		}

		internal static IExchangeAsyncDispatch Dispatch
		{
			get
			{
				return WebServiceEndPoint.exchangeAsyncDispatch;
			}
		}

		internal static bool IsShuttingDown
		{
			get
			{
				return WebServiceEndPoint.isShuttingDown;
			}
			set
			{
				WebServiceEndPoint.isShuttingDown = value;
			}
		}

		internal static void LogFailure(string message, Exception exception, string emailAddress, string domain, string organization, Trace trace)
		{
			string protocolSequence;
			if (string.IsNullOrEmpty(emailAddress))
			{
				if (string.IsNullOrEmpty(domain))
				{
					protocolSequence = RpcDispatch.WebServiceProtocolSequencePrefix;
				}
				else
				{
					protocolSequence = RpcDispatch.WebServiceProtocolSequencePrefix + domain;
				}
			}
			else
			{
				protocolSequence = string.Format("{0}{1}[{0}{2}]", RpcDispatch.WebServiceProtocolSequencePrefix, domain, emailAddress);
			}
			ProtocolLog.LogWebServiceFailure("xrop: failure", message ?? string.Empty, exception, emailAddress ?? string.Empty, organization ?? string.Empty, protocolSequence, trace);
		}

		private static readonly object initializeLock = new object();

		private static bool endpointRegistered = false;

		private static bool isShuttingDown;

		private static Server server = null;

		private static IExchangeAsyncDispatch exchangeAsyncDispatch = null;

		internal sealed class WebServiceDiagnosticsInfo : IServerDiagnosticsHandler
		{
			public void AnalyseException(ref Exception exception)
			{
			}

			public void LogException(Exception exception)
			{
				ProtocolLog.LogWebServiceFailure("xrop: WCF exception", null, exception, null, null, RpcDispatch.WebServiceProtocolSequencePrefix, ExTraceGlobals.ConnectXropTracer);
			}

			public void LogMessage(string message)
			{
				ProtocolLog.LogWebServiceFailure("xrop: WCF message", message, null, null, null, RpcDispatch.WebServiceProtocolSequencePrefix, ExTraceGlobals.ConnectXropTracer);
			}
		}
	}
}
