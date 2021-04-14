using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class SubnetNetworkConfiguration
	{
		public SubnetNetworkConfiguration(NetworkConfiguration ipv4NetworkConfiguration, NetworkConfiguration ipv6NetworkConfiguration)
		{
			this.ipv4NetworkConfiguration = ipv4NetworkConfiguration;
			this.ipv6NetworkConfiguration = ipv6NetworkConfiguration;
		}

		public NetworkConfiguration Ipv4NetworkConfiguration
		{
			get
			{
				return this.ipv4NetworkConfiguration;
			}
		}

		public NetworkConfiguration Ipv6NetworkConfiguration
		{
			get
			{
				return this.ipv6NetworkConfiguration;
			}
		}

		public static bool Equals(SubnetNetworkConfiguration subnetNetworkConfigurationA, SubnetNetworkConfiguration subnetNetworkConfigurationB)
		{
			if (subnetNetworkConfigurationA != null)
			{
				return subnetNetworkConfigurationA.Equals(subnetNetworkConfigurationB);
			}
			return subnetNetworkConfigurationB == null;
		}

		public bool Equals(SubnetNetworkConfiguration subnetNetworkConfiguration)
		{
			return subnetNetworkConfiguration != null && NetworkConfiguration.Equals(this.Ipv4NetworkConfiguration, subnetNetworkConfiguration.Ipv4NetworkConfiguration) && NetworkConfiguration.Equals(this.Ipv6NetworkConfiguration, subnetNetworkConfiguration.Ipv6NetworkConfiguration);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as SubnetNetworkConfiguration);
		}

		public override int GetHashCode()
		{
			int num = (this.Ipv4NetworkConfiguration == null) ? 0 : this.Ipv4NetworkConfiguration.GetHashCode();
			int num2 = (this.Ipv6NetworkConfiguration == null) ? 0 : this.Ipv6NetworkConfiguration.GetHashCode();
			return num + num2;
		}

		private NetworkConfiguration ipv4NetworkConfiguration;

		private NetworkConfiguration ipv6NetworkConfiguration;
	}
}
