using System;
using System.Collections;
using System.Net;
using System.Runtime.InteropServices;

namespace Microsoft.Office.Datacenter.ActiveMonitoring
{
	public class HostnameResolver
	{
		public HostnameResolver(string hostname) : this(hostname, true)
		{
		}

		public HostnameResolver(string hostname, bool flush)
		{
			this.hostname = hostname;
			this.ResolveInternal(flush);
		}

		public void Resolve()
		{
			this.ResolveInternal(true);
		}

		public IEnumerator GetEnumerator()
		{
			return new HostnameResolver.HostnameIPEnumerator(this.ipAddresses);
		}

		[DllImport("dnsapi.dll")]
		private static extern uint DnsFlushResolverCache();

		private void ResolveInternal(bool flush)
		{
			if (flush)
			{
				HostnameResolver.DnsFlushResolverCache();
			}
			this.ipAddresses = Dns.GetHostAddresses(this.hostname);
		}

		private readonly string hostname;

		private IPAddress[] ipAddresses;

		public class HostnameIPEnumerator : IEnumerator
		{
			public HostnameIPEnumerator(IPAddress[] list)
			{
				this.ipAddresses = list;
			}

			object IEnumerator.Current
			{
				get
				{
					return this.ipAddresses[this.position];
				}
			}

			public bool MoveNext()
			{
				this.position++;
				return this.position < this.ipAddresses.Length;
			}

			public void Reset()
			{
				this.position = -1;
			}

			private IPAddress[] ipAddresses;

			private int position = -1;
		}
	}
}
