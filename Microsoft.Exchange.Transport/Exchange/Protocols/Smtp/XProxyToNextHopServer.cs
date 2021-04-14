using System;
using System.Globalization;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Protocols.Smtp
{
	internal class XProxyToNextHopServer : INextHopServer
	{
		private XProxyToNextHopServer(string fqdn)
		{
			if (string.IsNullOrEmpty(fqdn))
			{
				throw new ArgumentException("fqdn is null or empty");
			}
			this.fqdn = fqdn;
		}

		private XProxyToNextHopServer(IPAddress ipAddress)
		{
			this.ipAddress = ipAddress;
		}

		public IPAddress Address
		{
			get
			{
				return this.ipAddress;
			}
		}

		public bool IsIPAddress
		{
			get
			{
				return !this.ipAddress.Equals(IPAddress.Any);
			}
		}

		public string Fqdn
		{
			get
			{
				return this.fqdn;
			}
		}

		public bool IsFrontendAndHubColocatedServer
		{
			get
			{
				return false;
			}
		}

		public static bool TryParse(string address, out XProxyToNextHopServer nextHopServer)
		{
			nextHopServer = null;
			if (string.IsNullOrEmpty(address))
			{
				return false;
			}
			address = address.Trim();
			int length = address.Length;
			if (address[0] == '[' && address[length - 1] == ']')
			{
				address = address.Substring(1, length - 2);
			}
			IPAddress ipaddress;
			RoutingHostName routingHostName;
			if (IPAddress.TryParse(address, out ipaddress))
			{
				if (ipaddress.Equals(IPAddress.Any))
				{
					return false;
				}
				nextHopServer = new XProxyToNextHopServer(ipaddress);
			}
			else if (RoutingHostName.TryParse(address, out routingHostName))
			{
				nextHopServer = new XProxyToNextHopServer(routingHostName.ToString());
			}
			else
			{
				SmtpDomain smtpDomain;
				if (!SmtpDomain.TryParse(address, out smtpDomain))
				{
					return false;
				}
				nextHopServer = new XProxyToNextHopServer(smtpDomain.Domain);
			}
			return true;
		}

		public static string ConvertINextHopServerToString(INextHopServer nextHopServer)
		{
			if (!nextHopServer.IsIPAddress)
			{
				return nextHopServer.Fqdn;
			}
			return string.Format(CultureInfo.InvariantCulture, "[{0}]", new object[]
			{
				nextHopServer.Address
			});
		}

		private readonly string fqdn = string.Empty;

		private IPAddress ipAddress = IPAddress.Any;
	}
}
