using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class Provider
	{
		public Provider(IPListProvider provider, Server server)
		{
			this.provider = provider;
			this.dns = Provider.GetAndInitializeDns(server);
		}

		public static bool Query(Server server, IPListProvider provider, IPAddress ipAddress, out IPAddress[] providerResult)
		{
			Provider provider2 = new Provider(provider, server);
			IAsyncResult asyncResult = provider2.BeginQuery(ipAddress, null, null);
			asyncResult.AsyncWaitHandle.WaitOne();
			return provider2.EndQuery(asyncResult, out providerResult);
		}

		public IAsyncResult BeginQuery(IPAddress ipAddress, AsyncCallback asyncCallback, object asyncState)
		{
			Provider.QueryAsyncResult queryAsyncResult = new Provider.QueryAsyncResult(asyncCallback, asyncState);
			queryAsyncResult.SetAsync();
			string domainName = string.Format("{0}.{1}", this.ReverseIP(ipAddress), this.provider.LookupDomain);
			IAsyncResult asyncResult = this.dns.BeginResolveToAddresses(domainName, AddressFamily.InterNetwork, new AsyncCallback(this.DnsQueryCallback), queryAsyncResult);
			if (asyncResult.CompletedSynchronously)
			{
				queryAsyncResult.ResetAsync();
			}
			return queryAsyncResult;
		}

		public bool EndQuery(IAsyncResult ar, out IPAddress[] providerResult)
		{
			Provider.QueryAsyncResult queryAsyncResult = ar as Provider.QueryAsyncResult;
			if (queryAsyncResult != null && queryAsyncResult.IsCompleted)
			{
				providerResult = queryAsyncResult.ProviderResult;
				return queryAsyncResult.IsMatch;
			}
			throw new InvalidOperationException();
		}

		internal static Dns GetAndInitializeDns(Server server)
		{
			Dns dns = new Dns();
			dns.Options = Provider.GetDnsOptions(server.ExternalDNSProtocolOption);
			MultiValuedProperty<IPAddress> externalDNSServers = server.ExternalDNSServers;
			if (server.ExternalDNSAdapterEnabled || MultiValuedPropertyBase.IsNullOrEmpty(externalDNSServers))
			{
				dns.AdapterServerList(server.ExternalDNSAdapterGuid);
			}
			else
			{
				IPAddress[] array = new IPAddress[externalDNSServers.Count];
				externalDNSServers.CopyTo(array, 0);
				dns.ServerList.Initialize(array);
			}
			return dns;
		}

		private static DnsQueryOptions GetDnsOptions(ProtocolOption protocolOption)
		{
			DnsQueryOptions dnsQueryOptions = DnsQueryOptions.None;
			switch (protocolOption)
			{
			case ProtocolOption.UseUdpOnly:
				dnsQueryOptions |= DnsQueryOptions.AcceptTruncatedResponse;
				break;
			case ProtocolOption.UseTcpOnly:
				dnsQueryOptions |= DnsQueryOptions.UseTcpOnly;
				break;
			}
			return dnsQueryOptions;
		}

		private void DnsQueryCallback(IAsyncResult ar)
		{
			IPAddress[] array;
			DnsStatus dnsStatus = Dns.EndResolveToAddresses(ar, out array);
			bool isMatch = false;
			if (dnsStatus == DnsStatus.Success)
			{
				isMatch = this.MatchResult(array);
			}
			Provider.QueryAsyncResult queryAsyncResult = (Provider.QueryAsyncResult)ar.AsyncState;
			queryAsyncResult.QueryCompleted(isMatch, array);
		}

		private string ReverseIP(IPAddress address)
		{
			byte[] addressBytes = address.GetAddressBytes();
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				addressBytes[3],
				addressBytes[2],
				addressBytes[1],
				addressBytes[0]
			});
		}

		private bool MatchResult(IPAddress[] addresses)
		{
			int i = 0;
			while (i < addresses.Length)
			{
				IPAddress ipaddress = addresses[i];
				if (!this.provider.AnyMatch)
				{
					if (this.provider.BitmaskMatch != null)
					{
						byte[] addressBytes = this.provider.BitmaskMatch.GetAddressBytes();
						byte[] addressBytes2 = ipaddress.GetAddressBytes();
						for (int j = 0; j < addressBytes.Length; j++)
						{
							if ((addressBytes[j] & addressBytes2[j]) != 0)
							{
								return true;
							}
						}
					}
					if (this.provider.IPAddressesMatch != null)
					{
						foreach (IPAddress obj in this.provider.IPAddressesMatch)
						{
							if (ipaddress.Equals(obj))
							{
								return true;
							}
						}
					}
					i++;
					continue;
				}
				return true;
			}
			return false;
		}

		private IPListProvider provider;

		private Dns dns;

		private class QueryAsyncResult : IAsyncResult
		{
			public QueryAsyncResult(AsyncCallback asyncCallback, object asyncState)
			{
				this.asyncCallback = asyncCallback;
				this.asyncState = asyncState;
				this.isCompleted = false;
				this.isMatch = false;
				this.providerResult = null;
				if (asyncCallback == null)
				{
					this.manualResetEvent = new ManualResetEvent(false);
					return;
				}
				this.manualResetEvent = null;
			}

			public bool IsMatch
			{
				get
				{
					this.ThrowIfNotCompleted();
					return this.isMatch;
				}
			}

			public IPAddress[] ProviderResult
			{
				get
				{
					this.ThrowIfNotCompleted();
					return this.providerResult;
				}
			}

			private void ThrowIfNotCompleted()
			{
				if (!this.isCompleted)
				{
					throw new InvalidOperationException();
				}
			}

			public void QueryCompleted(bool isMatch, IPAddress[] providerResult)
			{
				this.isMatch = isMatch;
				this.providerResult = providerResult;
				this.isCompleted = true;
				if (this.manualResetEvent != null)
				{
					this.manualResetEvent.Set();
				}
				if (this.asyncCallback != null)
				{
					this.asyncCallback(this);
				}
			}

			public void SetAsync()
			{
				this.completedSynchronously = false;
			}

			public void ResetAsync()
			{
				this.completedSynchronously = true;
			}

			public object AsyncState
			{
				get
				{
					return this.asyncState;
				}
			}

			public bool CompletedSynchronously
			{
				get
				{
					return this.completedSynchronously;
				}
			}

			public WaitHandle AsyncWaitHandle
			{
				get
				{
					if (this.manualResetEvent != null)
					{
						return this.manualResetEvent;
					}
					throw new InvalidOperationException();
				}
			}

			public bool IsCompleted
			{
				get
				{
					return this.isCompleted;
				}
			}

			private AsyncCallback asyncCallback;

			private object asyncState;

			private bool isCompleted;

			private bool completedSynchronously;

			private bool isMatch;

			private IPAddress[] providerResult;

			private ManualResetEvent manualResetEvent;
		}
	}
}
