using System;
using System.Net;

namespace Microsoft.Exchange.Net.MonitoringWebClient
{
	internal class NamedVip
	{
		public string Name { get; set; }

		public IPAddress IPAddress { get; set; }

		public string IPAddressString { get; set; }

		public string ForestName { get; set; }
	}
}
