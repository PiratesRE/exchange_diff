using System;
using Microsoft.Exchange.Connections.Common;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Connections.Pop
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	public sealed class Pop3ConnectionContext : DisposeTrackableBase
	{
		internal Pop3ConnectionContext(ConnectionParameters connectionParameters, IMonitorEvents eventsMonitor = null)
		{
			this.connectionParameters = connectionParameters;
		}

		internal ILog Log
		{
			get
			{
				return this.ConnectionParameters.Log;
			}
		}

		internal ConnectionParameters ConnectionParameters
		{
			get
			{
				base.CheckDisposed();
				return this.connectionParameters;
			}
		}

		internal Pop3AuthenticationParameters AuthenticationParameters
		{
			get
			{
				base.CheckDisposed();
				return this.authenticationParameters;
			}
			set
			{
				base.CheckDisposed();
				this.authenticationParameters = value;
			}
		}

		internal ServerParameters ServerParameters
		{
			get
			{
				base.CheckDisposed();
				return this.serverParameters;
			}
			set
			{
				base.CheckDisposed();
				this.serverParameters = value;
			}
		}

		internal Pop3Client Client
		{
			get
			{
				base.CheckDisposed();
				return this.client;
			}
			set
			{
				base.CheckDisposed();
				this.client = value;
			}
		}

		internal string UserName
		{
			get
			{
				base.CheckDisposed();
				if (this.AuthenticationParameters == null || this.AuthenticationParameters.NetworkCredential == null)
				{
					return string.Empty;
				}
				return this.AuthenticationParameters.NetworkCredential.UserName;
			}
		}

		internal string Server
		{
			get
			{
				base.CheckDisposed();
				if (this.ServerParameters == null)
				{
					return string.Empty;
				}
				return this.ServerParameters.Server;
			}
		}

		protected override void InternalDispose(bool disposing)
		{
			if (disposing && this.client != null)
			{
				this.client.Dispose();
				this.client = null;
			}
		}

		protected override DisposeTracker InternalGetDisposeTracker()
		{
			return DisposeTracker.Get<Pop3ConnectionContext>(this);
		}

		private readonly ConnectionParameters connectionParameters;

		private ServerParameters serverParameters;

		private Pop3AuthenticationParameters authenticationParameters;

		private Pop3Client client;
	}
}
