using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class NameResolver
	{
		public NameResolver()
		{
		}

		public NameResolver(Dictionary<string, List<NamedVip>> staticMapping)
		{
			this.staticMapping = staticMapping;
		}

		public Uri ResolveUri(HttpWebRequestWrapper requestWrapper)
		{
			Uri requestUri = requestWrapper.RequestUri;
			IPAddress ipaddress;
			if (IPAddress.TryParse(requestUri.Host, out ipaddress))
			{
				return requestUri;
			}
			NameResolver.DnsCacheEntry orAdd;
			if (!this.dnsCache.TryGetValue(requestUri.Host, out orAdd))
			{
				Stopwatch stopwatch = new Stopwatch();
				List<NamedVip> list;
				try
				{
					stopwatch.Start();
					IPHostEntry hostEntry = Dns.GetHostEntry(requestUri.Host);
					list = new List<NamedVip>();
					foreach (IPAddress ipaddress2 in hostEntry.AddressList)
					{
						list.Add(new NamedVip
						{
							IPAddress = ipaddress2,
							Name = requestUri.Host
						});
					}
				}
				catch (Exception innerException)
				{
					throw new NameResolutionException(MonitoringWebClientStrings.NameResolutionFailure(requestUri.Host), requestWrapper, innerException, requestUri.Host);
				}
				finally
				{
					stopwatch.Stop();
					requestWrapper.DnsLatency = stopwatch.Elapsed;
				}
				List<NamedVip> list2 = this.ResolveStaticEntries(requestUri.Host);
				if (list2 != null)
				{
					list = list2;
				}
				list.Shuffle<NamedVip>();
				orAdd = this.dnsCache.GetOrAdd(requestUri.Host, new NameResolver.DnsCacheEntry
				{
					HostEntry = list,
					CurrentEntry = 0
				});
			}
			NamedVip namedVip = orAdd.HostEntry[orAdd.CurrentEntry];
			UriBuilder uriBuilder = new UriBuilder(requestUri.Scheme, namedVip.IPAddress.ToString());
			requestWrapper.TargetVipName = namedVip.Name;
			requestWrapper.TargetVipForestName = namedVip.ForestName;
			if (!requestUri.IsDefaultPort)
			{
				uriBuilder.Port = requestUri.Port;
			}
			uriBuilder.Path = requestUri.AbsolutePath;
			if (!string.IsNullOrEmpty(requestUri.Query))
			{
				uriBuilder.Query = requestUri.Query.TrimStart(new char[]
				{
					'?'
				});
			}
			return uriBuilder.Uri;
		}

		public bool ShouldRetryWithDnsRoundRobin(string hostName)
		{
			NameResolver.DnsCacheEntry dnsCacheEntry = this.dnsCache[hostName];
			dnsCacheEntry.CurrentEntry++;
			if (dnsCacheEntry.CurrentEntry >= dnsCacheEntry.HostEntry.Count)
			{
				dnsCacheEntry.CurrentEntry = 0;
				return false;
			}
			return true;
		}

		private List<NamedVip> ResolveStaticEntries(string hostname)
		{
			if (this.staticMapping == null)
			{
				return null;
			}
			if (!this.staticMapping.ContainsKey(hostname))
			{
				return null;
			}
			return this.staticMapping[hostname];
		}

		private ConcurrentDictionary<string, NameResolver.DnsCacheEntry> dnsCache = new ConcurrentDictionary<string, NameResolver.DnsCacheEntry>();

		private Dictionary<string, List<NamedVip>> staticMapping;

		private class DnsCacheEntry
		{
			public List<NamedVip> HostEntry { get; set; }

			public int CurrentEntry { get; set; }
		}
	}
}
