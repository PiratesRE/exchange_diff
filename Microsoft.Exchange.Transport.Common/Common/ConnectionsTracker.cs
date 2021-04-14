using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Xml.Linq;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Transport.Common
{
	internal class ConnectionsTracker
	{
		public ConnectionsTracker(ConnectionsTracker.GetExPerfCounterDelegate getConnectionsCurrent, ConnectionsTracker.GetExPerfCounterDelegate getConnectionsTotal)
		{
			ArgumentValidator.ThrowIfNull("GetConnectionsCurrent", getConnectionsCurrent);
			ArgumentValidator.ThrowIfNull("getConnectionsTotal", getConnectionsTotal);
			this.getConnectionsCurrent = getConnectionsCurrent;
			this.getConnectionsTotal = getConnectionsTotal;
		}

		public void IncrementProxyCount(string forest)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("forest", forest);
			ConnectionsTracker.Connections orAdd = this.proxyConnectionsDictionary.GetOrAdd(forest, (string c) => new ConnectionsTracker.Connections());
			orAdd.Increment();
			this.getConnectionsCurrent(forest).Increment();
			this.getConnectionsTotal(forest).Increment();
		}

		public void DecrementProxyCount(string forest)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("forest", forest);
			ConnectionsTracker.Connections connections;
			if (this.proxyConnectionsDictionary.TryGetValue(forest, out connections))
			{
				connections.Decrement();
				this.getConnectionsCurrent(forest).Decrement();
			}
		}

		public int GetUsage(string forest)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("forest", forest);
			int result = 0;
			ConnectionsTracker.Connections connections;
			if (this.proxyConnectionsDictionary.TryGetValue(forest, out connections))
			{
				result = connections.Count;
			}
			return result;
		}

		public IEnumerable<XElement> GetDiagnosticInfo(string xmlNodeName)
		{
			ArgumentValidator.ThrowIfNullOrWhiteSpace("xmlNodeName", xmlNodeName);
			List<XElement> list = new List<XElement>();
			foreach (KeyValuePair<string, ConnectionsTracker.Connections> keyValuePair in this.proxyConnectionsDictionary)
			{
				XElement xelement = new XElement(xmlNodeName);
				xelement.SetAttributeValue("Name", keyValuePair.Key);
				xelement.SetAttributeValue("Connections", keyValuePair.Value.Count);
				list.Add(xelement);
			}
			return list;
		}

		private readonly ConcurrentDictionary<string, ConnectionsTracker.Connections> proxyConnectionsDictionary = new ConcurrentDictionary<string, ConnectionsTracker.Connections>(Environment.ProcessorCount, 25, StringComparer.InvariantCultureIgnoreCase);

		private readonly ConnectionsTracker.GetExPerfCounterDelegate getConnectionsCurrent;

		private readonly ConnectionsTracker.GetExPerfCounterDelegate getConnectionsTotal;

		public delegate ExPerformanceCounter GetExPerfCounterDelegate(string instanceName);

		private class Connections
		{
			public int Count
			{
				get
				{
					return this.count;
				}
			}

			public void Increment()
			{
				Interlocked.Increment(ref this.count);
			}

			public void Decrement()
			{
				Interlocked.Decrement(ref this.count);
			}

			private int count;
		}
	}
}
