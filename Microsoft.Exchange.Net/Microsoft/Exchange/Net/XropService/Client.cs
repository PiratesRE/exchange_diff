using System;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.XropService
{
	internal sealed class Client : IDisposable
	{
		public bool IsClientInteractive
		{
			get
			{
				return this.isClientInteractive;
			}
		}

		public Client(FederatedClientCredentials clientCredentials, Uri endpoint, Uri internetWebProxy, string targetSmtpAddress) : this(clientCredentials, endpoint, internetWebProxy, targetSmtpAddress, true, null)
		{
		}

		public Client(FederatedClientCredentials clientCredentials, Uri endpoint, Uri internetWebProxy, string targetSmtpAddress, bool isClientInteractive) : this(clientCredentials, endpoint, internetWebProxy, targetSmtpAddress, isClientInteractive, null)
		{
		}

		public Client(FederatedClientCredentials clientCredentials, Uri endpoint, Uri internetWebProxy, string targetSmtpAddress, bool isClientInteractive, IClientDiagnosticsHandler clientDiagnosticsHandler)
		{
			ExTraceGlobals.XropServiceClientTracer.TraceDebug<Uri, bool>((long)this.GetHashCode(), "Starting service client for endpoint: {0};Interactive={1}", endpoint, this.isClientInteractive);
			this.isClientInteractive = isClientInteractive;
			this.client = ClientFactory.GetClient(endpoint, internetWebProxy, clientCredentials, targetSmtpAddress, Client.MaxWaitTimeInSeconds, clientDiagnosticsHandler);
			ExTraceGlobals.XropServiceClientTracer.TraceDebug<Uri, bool>((long)this.GetHashCode(), "Started service client for endpoint: {0};Interactive={1}", endpoint, this.isClientInteractive);
		}

		public void Dispose()
		{
			IDisposable disposable = this.client as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Service client disposed");
		}

		public ConnectResponse Connect(ConnectRequest request)
		{
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Connect requested");
			ConnectResponse result;
			try
			{
				ConnectRequestMessage request2 = new ConnectRequestMessage
				{
					Request = request
				};
				ConnectResponseMessage connectResponseMessage = this.client.EndConnect(this.client.BeginConnect(request2, null, null));
				if (connectResponseMessage != null)
				{
					result = connectResponseMessage.Response;
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Connect completed");
			}
			return result;
		}

		public ExecuteResponse Execute(ExecuteRequest request)
		{
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Execute requested");
			ExecuteResponse result;
			try
			{
				ExecuteRequestMessage request2 = new ExecuteRequestMessage
				{
					Request = request
				};
				ExecuteResponseMessage executeResponseMessage = this.client.EndExecute(this.client.BeginExecute(request2, null, null));
				if (executeResponseMessage != null)
				{
					result = executeResponseMessage.Response;
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Execute completed");
			}
			return result;
		}

		public DisconnectResponse Disconnect(DisconnectRequest request)
		{
			ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Disconnect requested");
			DisconnectResponse result;
			try
			{
				DisconnectRequestMessage request2 = new DisconnectRequestMessage
				{
					Request = request
				};
				DisconnectResponseMessage disconnectResponseMessage = this.client.EndDisconnect(this.client.BeginDisconnect(request2, null, null));
				if (disconnectResponseMessage != null)
				{
					result = disconnectResponseMessage.Response;
				}
				else
				{
					result = null;
				}
			}
			finally
			{
				ExTraceGlobals.XropServiceClientTracer.TraceDebug((long)this.GetHashCode(), "Disconnect completed");
			}
			return result;
		}

		private static readonly TimeSpan MaxWaitTimeInSeconds = TimeSpan.FromSeconds(120.0);

		private IService client;

		private bool isClientInteractive = true;
	}
}
