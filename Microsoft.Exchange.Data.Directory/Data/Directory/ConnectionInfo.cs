using System;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ConnectionInfo
	{
		internal ConnectionInfo(ADServerInfo serverInfo)
		{
			this.serverInfo = serverInfo;
			this.state = 0;
			ExTraceGlobals.ConnectionDetailsTracer.TraceDebug<string>((long)this.GetHashCode(), "Creating ConnectionInfo for {0}", serverInfo.FqdnPlusPort);
		}

		internal ADServerInfo ADServerInfo
		{
			get
			{
				return this.serverInfo;
			}
		}

		internal PooledLdapConnection PooledLdapConnection
		{
			get
			{
				return this.connection;
			}
			set
			{
				ExTraceGlobals.ConnectionDetailsTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Adding PooledLdapConnection {0} to ConnectionInfo for {1}", value.GetHashCode(), this.serverInfo.FqdnPlusPort);
				this.connection = value;
			}
		}

		internal ConnectionState ConnectionState
		{
			get
			{
				return (ConnectionState)this.state;
			}
		}

		internal Exception LastLdapException
		{
			get
			{
				return this.lastLdapException;
			}
		}

		internal void MakeEmpty()
		{
			ExTraceGlobals.ConnectionTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Emptying ConnectionInfo for {0}, conn={1}", this.serverInfo.FqdnPlusPort, this.PooledLdapConnection.GetHashCode());
			this.state = 0;
			this.connection.ReturnToPool();
			this.connection = null;
		}

		internal void MakeDisconnected()
		{
			if (this.PooledLdapConnection != null)
			{
				ExTraceGlobals.ConnectionTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Disconnecting ConnectionInfo for {0}, conn={1}", this.serverInfo.FqdnPlusPort, this.PooledLdapConnection.GetHashCode());
			}
			else
			{
				ExTraceGlobals.ConnectionTracer.TraceDebug<string>((long)this.GetHashCode(), "No connections for {0}", this.serverInfo.FqdnPlusPort);
			}
			this.state = 3;
			if (this.connection != null)
			{
				this.connection.ReturnToPool();
				this.connection = null;
			}
		}

		internal bool TryMakeConnecting()
		{
			bool flag = 0 == Interlocked.CompareExchange(ref this.state, 1, 0);
			ExTraceGlobals.ConnectionTracer.TraceDebug<string, string>((long)this.GetHashCode(), "TryMakeConnecting {0} {1}", this.serverInfo.FqdnPlusPort, flag ? "succeeded" : "failed");
			return flag;
		}

		internal void MakeConnected()
		{
			ExTraceGlobals.ConnectionTracer.TraceDebug<string, int>((long)this.GetHashCode(), "Connecting ConnectionInfo for {0}, conn={1}", this.serverInfo.FqdnPlusPort, this.PooledLdapConnection.GetHashCode());
			Interlocked.Exchange(ref this.state, 2);
		}

		internal bool TrySetNamingContexts()
		{
			this.serverInfo = this.PooledLdapConnection.ADServerInfo;
			ADErrorRecord aderrorRecord;
			bool flag = this.PooledLdapConnection.TrySetNamingContexts(out aderrorRecord);
			if (!flag)
			{
				this.lastLdapException = aderrorRecord.InnerException;
			}
			return flag;
		}

		internal bool TryBindWithRetry(int maxBindRetryAttempts)
		{
			ADErrorRecord aderrorRecord;
			bool flag = this.PooledLdapConnection.TryBindWithRetry(maxBindRetryAttempts, out aderrorRecord);
			if (!flag)
			{
				this.lastLdapException = aderrorRecord.InnerException;
			}
			return flag;
		}

		internal bool TryCreatePooledLdapConnection(ADServerRole role, bool isNotify, NetworkCredential networkCredential)
		{
			bool result = false;
			try
			{
				this.connection = new PooledLdapConnection(this.ADServerInfo, role, isNotify, networkCredential);
				result = true;
			}
			catch (LdapException ex)
			{
				this.lastLdapException = ex;
			}
			return result;
		}

		private ADServerInfo serverInfo;

		private PooledLdapConnection connection;

		private int state;

		private Exception lastLdapException;
	}
}
