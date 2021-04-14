using System;
using System.Net;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class AddDagServerWmiIpInformation
	{
		public AddDagServerWmiIpInformation(IPAddress ipAddress, uint netmask, bool dhcpEnabled)
		{
			this.m_ipAddress = ipAddress;
			this.m_netmask = netmask;
			this.m_fDhcpEnabled = dhcpEnabled;
		}

		public readonly IPAddress m_ipAddress;

		public readonly uint m_netmask;

		public readonly bool m_fDhcpEnabled;
	}
}
