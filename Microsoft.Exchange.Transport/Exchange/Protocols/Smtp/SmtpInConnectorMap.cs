using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class SmtpInConnectorMap<TData> where TData : class
	{
		public void AddEntry(IPBinding[] bindings, IPRange[] ranges, TData data)
		{
			if (bindings == null || ranges == null)
			{
				return;
			}
			foreach (IPBinding ipbinding in bindings)
			{
				if (ipbinding.Address.AddressFamily != AddressFamily.InterNetwork && ipbinding.Address.AddressFamily != AddressFamily.InterNetworkV6)
				{
					throw new ArgumentException("Unsupported address type or family");
				}
				IPRangeRemote[] array = Util.FilterIpRangesByAddressFamily(ranges, ipbinding.Address.AddressFamily);
				if (array.Length != 0)
				{
					Dictionary<int, SmtpInConnectorMap<TData>.PortEntry<TData>> dictionary;
					if (this.localIPTable.TryGetValue(ipbinding.Address, out dictionary))
					{
						SmtpInConnectorMap<TData>.PortEntry<TData> portEntry;
						if (dictionary.TryGetValue(ipbinding.Port, out portEntry))
						{
							portEntry.Add(array, data);
						}
						else
						{
							dictionary.Add(ipbinding.Port, new SmtpInConnectorMap<TData>.PortEntry<TData>(ipbinding.Port, array, data));
						}
					}
					else
					{
						dictionary = new Dictionary<int, SmtpInConnectorMap<TData>.PortEntry<TData>>
						{
							{
								ipbinding.Port,
								new SmtpInConnectorMap<TData>.PortEntry<TData>(ipbinding.Port, array, data)
							}
						};
						this.localIPTable.Add(ipbinding.Address, dictionary);
					}
				}
			}
		}

		public TData Lookup(IPAddress localIp, int localPort, IPAddress remoteIpAddress)
		{
			IPRangeRemote v = null;
			TData tdata = default(TData);
			IPvxAddress remoteIP = new IPvxAddress(remoteIpAddress);
			Dictionary<int, SmtpInConnectorMap<TData>.PortEntry<TData>> dictionary;
			SmtpInConnectorMap<TData>.PortEntry<TData> portEntry;
			if (this.localIPTable.TryGetValue(localIp, out dictionary) && dictionary.TryGetValue(localPort, out portEntry))
			{
				tdata = portEntry.Lookup(remoteIP, out v);
			}
			if (localIp.AddressFamily == AddressFamily.InterNetwork)
			{
				this.localIPTable.TryGetValue(IPAddress.Any, out dictionary);
			}
			else
			{
				if (localIp.AddressFamily != AddressFamily.InterNetworkV6)
				{
					return default(TData);
				}
				this.localIPTable.TryGetValue(IPAddress.IPv6Any, out dictionary);
			}
			if (dictionary == null)
			{
				return tdata;
			}
			if (!dictionary.TryGetValue(localPort, out portEntry))
			{
				return tdata;
			}
			if (tdata == null)
			{
				return portEntry.Lookup(remoteIP);
			}
			IPRangeRemote v2;
			TData tdata2 = portEntry.Lookup(remoteIP, out v2);
			if (tdata2 == null || IPRangeRemote.Compare(v, v2) <= 0)
			{
				return tdata;
			}
			return tdata2;
		}

		private readonly Dictionary<IPAddress, Dictionary<int, SmtpInConnectorMap<TData>.PortEntry<TData>>> localIPTable = new Dictionary<IPAddress, Dictionary<int, SmtpInConnectorMap<TData>.PortEntry<TData>>>();

		internal class PortEntry<TPortData> where TPortData : class
		{
			public PortEntry(int port, IPRangeRemote[] ranges, TPortData data)
			{
				this.Port = port;
				SmtpInConnectorMap<TData>.RangesEntry<TPortData> item = new SmtpInConnectorMap<TData>.RangesEntry<TPortData>(ranges, data);
				this.RangeEntries.Add(item);
			}

			public void Add(IPRangeRemote[] ranges, TPortData data)
			{
				SmtpInConnectorMap<TData>.RangesEntry<TPortData> item = new SmtpInConnectorMap<TData>.RangesEntry<TPortData>(ranges, data);
				this.RangeEntries.Add(item);
			}

			public TPortData Lookup(IPvxAddress remoteIP)
			{
				IPRangeRemote iprangeRemote = null;
				TPortData result = default(TPortData);
				foreach (SmtpInConnectorMap<TData>.RangesEntry<TPortData> rangesEntry in this.RangeEntries)
				{
					IPRangeRemote iprangeRemote2 = rangesEntry.Match(remoteIP);
					if (iprangeRemote2 != null && (iprangeRemote == null || IPRangeRemote.Compare(iprangeRemote2, iprangeRemote) < 0))
					{
						iprangeRemote = iprangeRemote2;
						result = rangesEntry.Data;
					}
				}
				return result;
			}

			public TPortData Lookup(IPvxAddress remoteIP, out IPRangeRemote bestMatchRange)
			{
				bool flag = false;
				TPortData result = default(TPortData);
				bestMatchRange = null;
				foreach (SmtpInConnectorMap<TData>.RangesEntry<TPortData> rangesEntry in this.RangeEntries)
				{
					IPRangeRemote iprangeRemote = rangesEntry.Match(remoteIP);
					if (iprangeRemote != null && (!flag || IPRangeRemote.Compare(iprangeRemote, bestMatchRange) < 0))
					{
						bestMatchRange = iprangeRemote;
						result = rangesEntry.Data;
						flag = true;
					}
				}
				return result;
			}

			public override int GetHashCode()
			{
				return this.Port;
			}

			public readonly int Port;

			public List<SmtpInConnectorMap<TData>.RangesEntry<TPortData>> RangeEntries = new List<SmtpInConnectorMap<TData>.RangesEntry<TPortData>>();
		}

		internal class RangesEntry<TPortData> where TPortData : class
		{
			public RangesEntry(IPRangeRemote[] ranges, TPortData data)
			{
				this.Ranges = ranges;
				this.Data = data;
			}

			public IPRangeRemote Match(IPvxAddress ipAddress)
			{
				IPRangeRemote iprangeRemote = null;
				foreach (IPRangeRemote iprangeRemote2 in this.Ranges)
				{
					if (iprangeRemote2.Contains(ipAddress) && (iprangeRemote == null || IPRangeRemote.Compare(iprangeRemote2, iprangeRemote) < 0))
					{
						iprangeRemote = iprangeRemote2;
					}
				}
				return iprangeRemote;
			}

			public IPRangeRemote[] Ranges;

			public TPortData Data;
		}
	}
}
