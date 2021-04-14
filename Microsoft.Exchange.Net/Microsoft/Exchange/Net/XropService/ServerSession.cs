using System;
using System.ServiceModel;
using Microsoft.Exchange.Diagnostics.Components.Net;

namespace Microsoft.Exchange.Net.XropService
{
	[ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
	internal sealed class ServerSession : IService, IDisposable
	{
		public ServerSession(IServerSession sessionInstance)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "MapiService instance created");
			this.sessionInstance = sessionInstance;
		}

		public IAsyncResult BeginConnect(ConnectRequestMessage request, AsyncCallback asyncCallback, object asyncState)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginConnect requested");
			IAsyncResult result;
			try
			{
				if (request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "ConnectRequestMessage is empty.");
					throw new FaultException("XropService.BeginConnect: ConnectRequestMessage is empty.");
				}
				if (request.Request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "ConnectRequest is empty.");
					throw new FaultException("XropService.BeginConnect: ConnectRequest is empty.");
				}
				result = this.sessionInstance.BeginConnect(request.Request, asyncCallback, asyncState);
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginConnect completed");
			}
			return result;
		}

		public ConnectResponseMessage EndConnect(IAsyncResult asyncResult)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndConnect requested");
			ConnectResponseMessage result;
			try
			{
				result = new ConnectResponseMessage
				{
					Response = this.sessionInstance.EndConnect(asyncResult)
				};
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndConnect completed");
			}
			return result;
		}

		public IAsyncResult BeginExecute(ExecuteRequestMessage request, AsyncCallback asyncCallback, object asyncState)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginExecute requested");
			IAsyncResult result;
			try
			{
				if (request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "ExecuteRequestMessage is empty.");
					throw new FaultException("XropService.BeginExecute: ExecuteRequestMessage is empty.");
				}
				if (request.Request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "ExecuteRequest is empty.");
					throw new FaultException("XropService.BeginExecute: ExecuteRequest is empty.");
				}
				result = this.sessionInstance.BeginExecute(request.Request, asyncCallback, asyncState);
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginExecute completed");
			}
			return result;
		}

		public ExecuteResponseMessage EndExecute(IAsyncResult asyncResult)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndExecute requested");
			ExecuteResponseMessage result;
			try
			{
				result = new ExecuteResponseMessage
				{
					Response = this.sessionInstance.EndExecute(asyncResult)
				};
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndExecute completed");
			}
			return result;
		}

		public IAsyncResult BeginDisconnect(DisconnectRequestMessage request, AsyncCallback asyncCallback, object asyncState)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginDisconnect requested");
			IAsyncResult result;
			try
			{
				if (request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "DisconnectRequestMessage is empty.");
					throw new FaultException("XropService.BeginDisconnect: DisconnectRequestMessage is empty.");
				}
				if (request.Request == null)
				{
					ExTraceGlobals.XropServiceServerTracer.TraceError((long)this.GetHashCode(), "DisconnectRequest is empty.");
					throw new FaultException("XropService.BeginDisconnect: DisconnectRequest is empty.");
				}
				result = this.sessionInstance.BeginDisconnect(request.Request, asyncCallback, asyncState);
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "BeginDisconnect completed");
			}
			return result;
		}

		public DisconnectResponseMessage EndDisconnect(IAsyncResult asyncResult)
		{
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndDisconnect requested");
			DisconnectResponseMessage result;
			try
			{
				result = new DisconnectResponseMessage
				{
					Response = this.sessionInstance.EndDisconnect(asyncResult)
				};
			}
			finally
			{
				ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "EndDisconnect completed");
			}
			return result;
		}

		public void Dispose()
		{
			IDisposable disposable = this.sessionInstance as IDisposable;
			if (disposable != null)
			{
				disposable.Dispose();
			}
			ExTraceGlobals.XropServiceServerTracer.TraceDebug((long)this.GetHashCode(), "MapiService instance disposed");
		}

		private IServerSession sessionInstance;
	}
}
