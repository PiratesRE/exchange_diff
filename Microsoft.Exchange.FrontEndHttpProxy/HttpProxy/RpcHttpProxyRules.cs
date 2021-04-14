using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web.Configuration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.HttpProxy
{
	internal class RpcHttpProxyRules
	{
		internal RpcHttpProxyRules() : this(null)
		{
		}

		internal RpcHttpProxyRules(IDirectory rule)
		{
			this.directory = (rule ?? new Directory());
			this.RefreshServerList(null);
		}

		internal static RpcHttpProxyRules DefaultRpcHttpProxyRules
		{
			get
			{
				if (RpcHttpProxyRules.defaultHttpProxyRule == null)
				{
					lock (RpcHttpProxyRules.lockObject)
					{
						if (RpcHttpProxyRules.defaultHttpProxyRule == null)
						{
							RpcHttpProxyRules.defaultHttpProxyRule = new RpcHttpProxyRules();
						}
					}
				}
				return RpcHttpProxyRules.defaultHttpProxyRule;
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Dictionary<string, ProxyDestination> dictionary = this.proxyDestinations;
			foreach (KeyValuePair<string, ProxyDestination> keyValuePair in dictionary)
			{
				stringBuilder.AppendFormat("{0} : {1}\n", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		internal static void ApplyManualOverrides(Dictionary<string, ProxyDestination> proxyDestinations, string manualOverrides)
		{
			Regex regex = new Regex("^\\+(.+)=(.+):(\\d+)");
			Regex regex2 = new Regex("^\\-(.+)");
			string[] array = manualOverrides.Split(new char[]
			{
				';'
			});
			foreach (string input in array)
			{
				Match match = regex.Match(input);
				if (match.Success)
				{
					string value = match.Groups[1].Value;
					string value2 = match.Groups[2].Value;
					int port;
					if (int.TryParse(match.Groups[3].Value, out port))
					{
						proxyDestinations.Add(value, RpcHttpProxyRules.CreateFixedDestination(Server.E15MinVersion, value2, port));
					}
				}
				Match match2 = regex2.Match(input);
				if (match2.Success)
				{
					string value3 = match2.Groups[1].Value;
					proxyDestinations.Remove(value3);
				}
			}
		}

		internal bool TryGetProxyDestination(string rpcServerFqdn, out ProxyDestination destination)
		{
			destination = null;
			Dictionary<string, ProxyDestination> dictionary = this.proxyDestinations;
			if (dictionary != null)
			{
				dictionary.TryGetValue(rpcServerFqdn, out destination);
			}
			return destination != null;
		}

		private static void AddTwoMapsOfDestinations(Dictionary<string, ProxyDestination> dict, Server server, ProxyDestination destination)
		{
			dict[server.Fqdn] = destination;
			dict[server.Name] = destination;
		}

		private static ProxyDestination CreateFixedDestination(int version, string serverFqdn, int port)
		{
			return new ProxyDestination(version, port, serverFqdn);
		}

		private void RefreshServerList(object stateInfo)
		{
			ADSite[] adsites = this.directory.GetADSites();
			ClientAccessArray[] clientAccessArrays = this.directory.GetClientAccessArrays();
			Server[] servers = this.directory.GetServers();
			if (adsites != null && servers != null)
			{
				Dictionary<string, ProxyDestination> dictionary = new Dictionary<string, ProxyDestination>(StringComparer.OrdinalIgnoreCase);
				foreach (Server server5 in from s in servers
				where s.IsE15OrLater && s.IsMailboxServer
				select s)
				{
					RpcHttpProxyRules.AddTwoMapsOfDestinations(dictionary, server5, RpcHttpProxyRules.CreateFixedDestination(Server.E15MinVersion, server5.Fqdn, 444));
				}
				ADSite[] array = adsites;
				for (int i = 0; i < array.Length; i++)
				{
					ADSite site = array[i];
					IEnumerable<Server> source = from s in servers
					where s.ServerSite != null && s.ServerSite.Name == site.Name
					select s;
					IEnumerable<Server> enumerable = from s in source
					where s.IsE14OrLater && !s.IsE15OrLater && s.IsClientAccessServer
					select s;
					IEnumerable<Server> source2 = from s in enumerable
					where !(bool)s[ActiveDirectoryServerSchema.IsOutOfService]
					select s;
					ProxyDestination proxyDestination = null;
					if (source2.Count<Server>() > 0)
					{
						proxyDestination = new ProxyDestination(Server.E14MinVersion, 443, (from server in enumerable
						select server.Fqdn).OrderBy((string str) => str, StringComparer.OrdinalIgnoreCase).ToArray<string>(), (from server in source2
						select server.Fqdn).OrderBy((string str) => str, StringComparer.OrdinalIgnoreCase).ToArray<string>());
					}
					foreach (Server server2 in enumerable)
					{
						RpcHttpProxyRules.AddTwoMapsOfDestinations(dictionary, server2, RpcHttpProxyRules.CreateFixedDestination(Server.E14MinVersion, server2.Fqdn, 443));
					}
					if (proxyDestination != null)
					{
						foreach (Server server3 in from s in source
						where s.IsE14OrLater && !s.IsE15OrLater && !s.IsClientAccessServer && s.IsMailboxServer
						select s)
						{
							RpcHttpProxyRules.AddTwoMapsOfDestinations(dictionary, server3, proxyDestination);
						}
						if (clientAccessArrays != null && clientAccessArrays.Count<ClientAccessArray>() > 0)
						{
							foreach (ClientAccessArray clientAccessArray in from arr in clientAccessArrays
							where arr.SiteName == site.Name
							select arr)
							{
								dictionary[clientAccessArray.Fqdn] = proxyDestination;
							}
						}
					}
					IEnumerable<Server> source3 = from s in source
					where !s.IsE14OrLater && s.IsExchange2007OrLater && s.IsClientAccessServer
					select s;
					ProxyDestination proxyDestination2 = null;
					if (source3.Count<Server>() > 0)
					{
						string[] array2 = (from server in source3
						select server.Fqdn).OrderBy((string str) => str, StringComparer.OrdinalIgnoreCase).ToArray<string>();
						proxyDestination2 = new ProxyDestination(Server.E2007MinVersion, 443, array2, array2);
					}
					else if (proxyDestination != null)
					{
						proxyDestination2 = proxyDestination;
					}
					if (proxyDestination2 != null)
					{
						foreach (Server server4 in from s in source
						where s.IsExchange2007OrLater && !s.IsE14OrLater && s.IsMailboxServer
						select s)
						{
							RpcHttpProxyRules.AddTwoMapsOfDestinations(dictionary, server4, proxyDestination2);
						}
					}
				}
				string text = WebConfigurationManager.AppSettings["OverrideProxyingRules"];
				if (!string.IsNullOrEmpty(text))
				{
					RpcHttpProxyRules.ApplyManualOverrides(dictionary, text);
				}
				this.proxyDestinations = dictionary;
			}
			if (this.refreshTimer != null)
			{
				this.refreshTimer.Change((int)RpcHttpProxyRules.TopologyRefreshInterval.TotalMilliseconds, -1);
				return;
			}
			this.refreshTimer = new Timer(new TimerCallback(this.RefreshServerList), null, (int)RpcHttpProxyRules.TopologyRefreshInterval.TotalMilliseconds, -1);
		}

		private const int BrickBackEndPort = 444;

		private const int OriginalRpcVDirPort = 443;

		private const string AppSettingsOverrideProxyingRules = "OverrideProxyingRules";

		private static readonly TimeSpan TopologyRefreshInterval = TimeSpan.FromMinutes(15.0);

		private static RpcHttpProxyRules defaultHttpProxyRule = null;

		private static object lockObject = new object();

		private IDirectory directory;

		private Dictionary<string, ProxyDestination> proxyDestinations;

		private Timer refreshTimer;
	}
}
