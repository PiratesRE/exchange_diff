using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.Rfri;

namespace Microsoft.Exchange.Nspi.Rfri.Client
{
	internal class RfriClient : IDisposeTrackable, IDisposable
	{
		public RfriClient(string host) : this(host, null)
		{
		}

		public RfriClient(string host, NetworkCredential nc)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(host, "ncacn_ip_tcp", nc);
		}

		public RfriClient(string machinename, string proxyserver, NetworkCredential nc)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(machinename, proxyserver, "ncacn_http:6002", nc);
		}

		public RfriClient(string machinename, string proxyserver, string protocolSequence, NetworkCredential nc)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(machinename, proxyserver, protocolSequence, nc);
		}

		public RfriClient(string machinename, string proxyserver, string protocolSequence, NetworkCredential nc, HTTPAuthentication httpAuth, AuthenticationService authService)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(machinename, proxyserver, protocolSequence, true, nc, (HttpAuthenticationScheme)httpAuth, authService);
		}

		public RfriClient(string machinename, string proxyserver, string protocolSequence, NetworkCredential nc, HTTPAuthentication httpAuth, AuthenticationService authService, string instanceName)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(machinename, proxyserver, protocolSequence, nc, (HttpAuthenticationScheme)httpAuth, authService, instanceName);
		}

		public RfriClient(string machinename, string proxyserver, string protocolSequence, NetworkCredential nc, HTTPAuthentication httpAuth, AuthenticationService authService, string instanceName, string certificateSubjectName)
		{
			this.disposeTracker = this.GetDisposeTracker();
			this.client = new RfriRpcClient(machinename, proxyserver, protocolSequence, nc, (HttpAuthenticationScheme)httpAuth, authService, instanceName, true, certificateSubjectName);
		}

		public DisposeTracker GetDisposeTracker()
		{
			return DisposeTracker.Get<RfriClient>(this);
		}

		public void SuppressDisposeTracker()
		{
			if (this.disposeTracker != null)
			{
				this.disposeTracker.Suppress();
			}
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public RfriStatus GetNewDSA(string userDN, out string server)
		{
			return this.client.GetNewDSA(userDN, out server);
		}

		public RfriStatus GetFQDNFromLegacyDN(string serverDN, out string serverFQDN)
		{
			return this.client.GetFQDNFromLegacyDN(serverDN, out serverFQDN);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.client != null)
				{
					this.client.Dispose();
					this.client = null;
				}
				if (this.disposeTracker != null)
				{
					this.disposeTracker.Dispose();
					this.disposeTracker = null;
				}
			}
		}

		private RfriRpcClient client;

		private DisposeTracker disposeTracker;
	}
}
