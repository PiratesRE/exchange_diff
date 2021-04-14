using System;
using System.Collections.Generic;
using System.Net;
using System.Web;
using Microsoft.Exchange.Net.WebApplicationClient;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal sealed class OutboundProxyRequest : AsyncResult
	{
		public OutboundProxyRequest(IEnumerable<ProxyConnection> proxyConnections, HttpContext context, AsyncCallback requestCompletedCallback, object requestCompletedData) : base(requestCompletedCallback, requestCompletedData)
		{
			this.Context = context;
			this.proxyConnections = proxyConnections.GetEnumerator();
			this.TryNextServer();
		}

		public HttpContext Context { get; private set; }

		public bool AllServersFailed { get; private set; }

		private void TryNextServer()
		{
			this.canSendProxyLogon = true;
			if (!this.proxyConnections.MoveNext())
			{
				this.AllServersFailed = true;
				base.Complete(null, false);
				return;
			}
			ProxyConnection proxyConnection = this.proxyConnections.Current;
			if (proxyConnection.IsAlive)
			{
				this.SendProxyRequest(proxyConnection);
				return;
			}
			proxyConnection.Ping(new Action<ProxyConnection>(this.SendProxyRequest));
		}

		private void SendProxyRequest(ProxyConnection proxyConnection)
		{
			try
			{
				if (proxyConnection.IsCompatible && proxyConnection.IsAlive)
				{
					proxyConnection.ProxyWebSession.SendProxyRequest(this.Context, new Action(this.ProxyCallSucceeded), new Action<HttpContext, HttpWebResponse, Exception>(this.ProxyCallFailed));
				}
				else
				{
					this.TryNextServer();
				}
			}
			catch (Exception exception)
			{
				base.Complete(exception, false);
			}
		}

		private void ProxyCallSucceeded()
		{
			base.Complete(null, false);
		}

		private void ProxyCallFailed(HttpContext context, HttpWebResponse response, Exception exception)
		{
			try
			{
				if (response.StatusCode == (HttpStatusCode)441 && this.canSendProxyLogon)
				{
					this.SendProxyLogon();
				}
				else
				{
					this.TryNextServer();
				}
			}
			catch (Exception exception2)
			{
				base.Complete(exception2, false);
			}
		}

		private void SendProxyLogon()
		{
			ProxyConnection proxyConnection = this.proxyConnections.Current;
			OutboundProxySession session = (OutboundProxySession)this.Context.User;
			proxyConnection.ProxyWebSession.SendProxyLogon(proxyConnection.BaseUri, session, new Action<HttpStatusCode>(this.ProxyLogonResponseReceived), new Action<Exception>(this.ProxyLogonFailed));
		}

		private void ProxyLogonResponseReceived(HttpStatusCode responseCode)
		{
			try
			{
				this.canSendProxyLogon = false;
				if (responseCode == (HttpStatusCode)241)
				{
					this.SendProxyRequest(this.proxyConnections.Current);
				}
				else
				{
					this.TryNextServer();
				}
			}
			catch (Exception exception)
			{
				base.Complete(exception, false);
			}
		}

		private void ProxyLogonFailed(Exception proxyLogonException)
		{
			try
			{
				this.TryNextServer();
			}
			catch (Exception exception)
			{
				base.Complete(exception, false);
			}
		}

		private IEnumerator<ProxyConnection> proxyConnections;

		private bool canSendProxyLogon;
	}
}
