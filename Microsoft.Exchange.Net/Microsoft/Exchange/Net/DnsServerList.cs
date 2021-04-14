using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Net
{
	internal class DnsServerList : IDisposable
	{
		public int Count
		{
			get
			{
				if (this.addresses != null)
				{
					return this.addresses.Count;
				}
				return 0;
			}
		}

		public IList<IPAddress> Addresses
		{
			get
			{
				return this.addresses;
			}
		}

		public DnsCache Cache
		{
			get
			{
				return this.cache;
			}
		}

		public static IPAddress[] GetAdapterDnsServerList(Guid adapterGuid, bool excludeServersFromLoopbackAdapters, bool excludeIPv6SiteLocalAddresses)
		{
			IPAddress[] result;
			try
			{
				result = DnsServerList.GetMachineDnsServerList(adapterGuid, 100, excludeServersFromLoopbackAdapters, excludeIPv6SiteLocalAddresses);
			}
			catch (NetworkInformationException ex)
			{
				Dns.EventLogger.LogEvent(CommonEventLogConstants.Tuple_DnsServerConfigurationFetchFailed, null, new object[]
				{
					ex
				});
				DnsLog.Log(adapterGuid, "Getting DNS server information failed. Exception {0}", new object[]
				{
					ex
				});
				result = null;
			}
			return result;
		}

		public static IPAddress[] GetMachineDnsServerList()
		{
			return DnsServerList.GetMachineDnsServerList(Guid.Empty, int.MaxValue, false, false);
		}

		private static IPAddress[] GetMachineDnsServerList(Guid adapterGuid, int maxTargets, bool excludeServersFromLoopbackAdapters, bool excludeIPv6SiteLocalAddresses)
		{
			HashSet<IPAddress> hashSet = new HashSet<IPAddress>();
			string text = (Guid.Empty == adapterGuid) ? null : adapterGuid.ToString("B").ToUpperInvariant();
			NetworkInterface[] allNetworkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
			foreach (NetworkInterface networkInterface in allNetworkInterfaces)
			{
				if ((!(adapterGuid != Guid.Empty) || text.Equals(networkInterface.Id, StringComparison.OrdinalIgnoreCase)) && (!excludeServersFromLoopbackAdapters || networkInterface.NetworkInterfaceType != NetworkInterfaceType.Loopback))
				{
					IPInterfaceProperties ipproperties = networkInterface.GetIPProperties();
					bool flag = ipproperties.IsDnsEnabled || ipproperties.IsDynamicDnsEnabled;
					if (flag)
					{
						foreach (IPAddress ipaddress in ipproperties.DnsAddresses)
						{
							if (hashSet.Count >= maxTargets)
							{
								return hashSet.ToArray<IPAddress>();
							}
							if (!excludeIPv6SiteLocalAddresses || !ipaddress.IsIPv6SiteLocal)
							{
								hashSet.Add(ipaddress);
							}
						}
					}
				}
			}
			return hashSet.ToArray<IPAddress>();
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		public bool Initialize(IPAddress[] addresses)
		{
			return this.Initialize(addresses, null);
		}

		public bool Initialize(IPAddress[] addresses, string hostsFileName)
		{
			this.addresses = ((addresses == null) ? Array.AsReadOnly<IPAddress>(new IPAddress[0]) : Array.AsReadOnly<IPAddress>(addresses));
			this.cache = (string.IsNullOrEmpty(hostsFileName) ? DnsCache.CreateFromSystem() : DnsCache.CreateFromFile(hostsFileName));
			return true;
		}

		public void FlushCache()
		{
			if (this.cache != null)
			{
				this.cache.FlushCache();
			}
		}

		public bool IsAddressListIdentical(IPAddress[] addresses)
		{
			ReadOnlyCollection<IPAddress> readOnlyCollection = this.addresses;
			if (readOnlyCollection == null && addresses == null)
			{
				return true;
			}
			if (readOnlyCollection == null || addresses == null)
			{
				return false;
			}
			if (readOnlyCollection.Count != addresses.Length)
			{
				return false;
			}
			for (int i = 0; i < addresses.Length; i++)
			{
				if (!addresses[i].Equals(readOnlyCollection[i]))
				{
					return false;
				}
			}
			return true;
		}

		public override string ToString()
		{
			IList<IPAddress> list = this.addresses;
			if (list.Count == 0)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder(list.Count * 40);
			foreach (IPAddress ipaddress in list)
			{
				if (stringBuilder.Length != 0)
				{
					stringBuilder.Append(';');
				}
				stringBuilder.Append(ipaddress.ToString());
			}
			return stringBuilder.ToString();
		}

		protected void Dispose(bool disposing)
		{
			if (disposing && this.cache != null)
			{
				this.cache.Close();
			}
		}

		private DnsCache cache;

		private ReadOnlyCollection<IPAddress> addresses;
	}
}
