using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Microsoft.Exchange.Data.Transport
{
	[Serializable]
	public class RoutingHost : INextHopServer
	{
		public RoutingHost(string address)
		{
			if (string.IsNullOrEmpty(address))
			{
				throw new ArgumentException("A null or empty routing host name was specified");
			}
			address = address.Trim();
			int length = address.Length;
			if (address[0] == '[' && address[length - 1] == ']')
			{
				address = address.Substring(1, length - 2);
			}
			if (IPAddress.TryParse(address, out this.ipAddress))
			{
				if (this.ipAddress.Equals(IPAddress.Any) || this.ipAddress.Equals(IPAddress.IPv6Any))
				{
					throw new ArgumentException(string.Format("The specified IP address '{0}' isn't valid as a routing host", this.ipAddress));
				}
				return;
			}
			else
			{
				this.ipAddress = IPAddress.Any;
				if (!RoutingHostName.TryParse(address, out this.domain))
				{
					throw new ArgumentException(string.Format("The specified routing host name '{0}' isn't valid", address));
				}
				return;
			}
		}

		private RoutingHost()
		{
		}

		public IPAddress Address
		{
			get
			{
				return this.ipAddress;
			}
		}

		public string HostName
		{
			get
			{
				return this.domain.ToString();
			}
		}

		public bool IsIPAddress
		{
			get
			{
				return !this.ipAddress.Equals(IPAddress.Any);
			}
		}

		internal RoutingHostName Domain
		{
			get
			{
				return this.domain;
			}
		}

		string INextHopServer.Fqdn
		{
			get
			{
				return this.HostName;
			}
		}

		bool INextHopServer.IsFrontendAndHubColocatedServer
		{
			get
			{
				return this.isFrontendAndHubColocatedServer;
			}
		}

		public static RoutingHost Parse(string address)
		{
			return new RoutingHost(address);
		}

		public static bool TryParse(string address, out RoutingHost routinghost)
		{
			routinghost = null;
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			address = address.Trim();
			RoutingHost routingHost = new RoutingHost();
			int length = address.Length;
			if (address[0] == '[' && address[length - 1] == ']')
			{
				address = address.Substring(1, length - 2);
			}
			if (IPAddress.TryParse(address, out routingHost.ipAddress))
			{
				if (routingHost.ipAddress.Equals(IPAddress.Any))
				{
					return false;
				}
			}
			else
			{
				if (!RoutingHostName.TryParse(address, out routingHost.domain))
				{
					return false;
				}
				routingHost.ipAddress = IPAddress.Any;
			}
			routinghost = routingHost;
			return true;
		}

		public bool Equals(RoutingHost value)
		{
			if (object.ReferenceEquals(this, value))
			{
				return true;
			}
			if (value == null)
			{
				return false;
			}
			if (this.IsIPAddress != value.IsIPAddress)
			{
				return false;
			}
			if (this.IsIPAddress)
			{
				return this.ipAddress.Equals(value.ipAddress);
			}
			return this.domain.Equals(value.domain);
		}

		public override bool Equals(object comparand)
		{
			RoutingHost routingHost = comparand as RoutingHost;
			return routingHost != null && this.Equals(routingHost);
		}

		public override int GetHashCode()
		{
			if (this.IsIPAddress)
			{
				return this.ipAddress.GetHashCode();
			}
			return this.domain.GetHashCode();
		}

		public override string ToString()
		{
			if (this.IsIPAddress)
			{
				return this.IpAddressToString();
			}
			return this.domain.ToString();
		}

		internal static string ConvertRoutingHostsToString<T>(IList<T> routingHostWrappers, Func<T, RoutingHost> routingHostGetter)
		{
			if (routingHostWrappers == null || routingHostWrappers.Count == 0 || routingHostGetter == null)
			{
				return string.Empty;
			}
			StringBuilder stringBuilder = new StringBuilder();
			int num = 0;
			foreach (T arg in routingHostWrappers)
			{
				RoutingHost routingHost = routingHostGetter(arg);
				num++;
				stringBuilder.Append(routingHost.ToString());
				if (num < routingHostWrappers.Count)
				{
					stringBuilder.Append(',');
				}
			}
			return stringBuilder.ToString();
		}

		internal static List<T> GetRoutingHostsFromString<T>(string routingHostsString, Func<RoutingHost, T> routingHostWrapperGetter)
		{
			if (routingHostWrapperGetter == null)
			{
				throw new ArgumentNullException("routingHostWrapperGetter");
			}
			List<T> list = new List<T>();
			int num;
			for (int i = 0; i < routingHostsString.Length; i = num + 1)
			{
				num = routingHostsString.IndexOfAny(RoutingHost.routingHostDelimiters, i);
				if (-1 == num)
				{
					num = routingHostsString.Length;
				}
				RoutingHost routingHost = RoutingHost.InternalParseRoutingHost(routingHostsString, i, num - i);
				if (routingHost != null)
				{
					T item = routingHostWrapperGetter(routingHost);
					if (!list.Contains(item))
					{
						list.Add(item);
					}
				}
			}
			return list;
		}

		internal void SetIsColocatedFrontendAndHub(bool value)
		{
			this.isFrontendAndHubColocatedServer = value;
		}

		private static RoutingHost InternalParseRoutingHost(string routingHostString, int startPos, int count)
		{
			string text = routingHostString.Substring(startPos, count);
			text = text.Trim();
			if (text.Length == 0)
			{
				return null;
			}
			RoutingHost result;
			try
			{
				if ('[' != text[0])
				{
					result = RoutingHost.Parse(text);
				}
				else if (']' != text[count - 1])
				{
					result = null;
				}
				else
				{
					result = RoutingHost.Parse(text.Substring(1, count - 2));
				}
			}
			catch (ArgumentException)
			{
				result = null;
			}
			catch (FormatException)
			{
				result = null;
			}
			return result;
		}

		private string IpAddressToString()
		{
			return string.Format("[{0}]", this.ipAddress);
		}

		private static readonly char[] routingHostDelimiters = new char[]
		{
			';',
			','
		};

		private RoutingHostName domain = RoutingHostName.Empty;

		private IPAddress ipAddress;

		private bool isFrontendAndHubColocatedServer;
	}
}
