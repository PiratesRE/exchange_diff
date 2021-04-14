using System;
using System.Collections.Generic;
using Microsoft.Exchange.AddressBook.Service;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.AddressBook.Service;
using Microsoft.Exchange.Nspi;

namespace Microsoft.Exchange.AddressBook.Nspi.Client
{
	internal class NspiConnectionPool
	{
		internal NspiConnectionPool(string server)
		{
			this.server = server;
		}

		internal string Server
		{
			get
			{
				return this.server;
			}
		}

		internal static NspiConnection GetConnection(string server, PartitionId partitionId)
		{
			NspiConnectionPool.NspiConnectionTracer.TraceDebug<string>(0L, "NspiConnectionPool.GetConnection: {0}", server ?? "(null)");
			if (string.IsNullOrEmpty(server))
			{
				if (!string.IsNullOrEmpty(Configuration.NspiTestServer))
				{
					server = Configuration.NspiTestServer;
					NspiConnectionPool.NspiConnectionTracer.TraceDebug<string>(0L, "Using test server: {0}", server ?? "(null)");
				}
				else
				{
					ADRunspaceServerSettingsProvider instance = ADRunspaceServerSettingsProvider.GetInstance();
					bool flag;
					server = instance.GetGcFromToken(partitionId.ForestFQDN, null, out flag, false).Fqdn;
					NspiConnectionPool.NspiConnectionTracer.TraceDebug<string>(0L, "Using GC: {0}", server ?? "(null)");
				}
			}
			NspiConnectionPool nspiConnectionPool;
			lock (NspiConnectionPool.pools)
			{
				if (!NspiConnectionPool.pools.TryGetValue(server, out nspiConnectionPool))
				{
					nspiConnectionPool = new NspiConnectionPool(server);
					NspiConnectionPool.pools[server] = nspiConnectionPool;
				}
			}
			return nspiConnectionPool.GetConnectionFromPool();
		}

		private NspiConnection GetConnectionFromPool()
		{
			NspiConnection nspiConnection = null;
			bool flag = false;
			NspiConnection result;
			try
			{
				lock (this.connections)
				{
					int num = this.connectionsInUse;
					this.connectionsInUse++;
					if (this.connections.Count > 0)
					{
						int index = this.connections.Count - 1;
						nspiConnection = this.connections[index];
						this.connections.RemoveAt(index);
						flag = true;
						return nspiConnection;
					}
				}
				nspiConnection = new NspiConnection(this);
				if (nspiConnection.Connect() != NspiStatus.Success)
				{
					result = null;
				}
				else
				{
					flag = true;
					result = nspiConnection;
				}
			}
			finally
			{
				if (!flag && nspiConnection != null)
				{
					nspiConnection.Dispose();
					nspiConnection = null;
				}
			}
			return result;
		}

		internal void ReturnToPool(NspiConnection connection)
		{
			lock (this.connections)
			{
				this.connectionsInUse--;
				if (this.connections.Count == 10)
				{
					this.connections[0].Dispose();
					this.connections.RemoveAt(0);
				}
				this.connections.Add(connection);
			}
		}

		private const int MaxPoolSize = 10;

		private const int MaxConnectionsInUse = 25;

		internal static readonly Trace NspiConnectionTracer = ExTraceGlobals.NspiConnectionTracer;

		private static readonly TimeSpan expireOlderThan = TimeSpan.FromMinutes(5.0);

		private static readonly Dictionary<string, NspiConnectionPool> pools = new Dictionary<string, NspiConnectionPool>(StringComparer.OrdinalIgnoreCase);

		private readonly string server;

		private List<NspiConnection> connections = new List<NspiConnection>();

		private int connectionsInUse;
	}
}
