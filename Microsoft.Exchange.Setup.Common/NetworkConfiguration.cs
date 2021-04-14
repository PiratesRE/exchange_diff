using System;
using System.Net;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Setup.Common
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class NetworkConfiguration
	{
		public NetworkConfiguration(IPAddress ipAddress, string networkName)
		{
			this.ipAddress = ipAddress;
			this.networkName = networkName;
		}

		public IPAddress IPAddress
		{
			get
			{
				return this.ipAddress;
			}
		}

		public string NetworkName
		{
			get
			{
				return this.networkName;
			}
		}

		public static bool Equals(NetworkConfiguration networkConfigurationA, NetworkConfiguration networkConfigurationB)
		{
			if (networkConfigurationA != null)
			{
				return networkConfigurationA.Equals(networkConfigurationB);
			}
			return networkConfigurationB == null;
		}

		public bool Equals(NetworkConfiguration networkConfiguration)
		{
			return networkConfiguration != null && this.IPAddress == networkConfiguration.IPAddress && ((string.IsNullOrEmpty(this.NetworkName) && string.IsNullOrEmpty(networkConfiguration.NetworkName)) || this.NetworkName == networkConfiguration.NetworkName);
		}

		public override bool Equals(object obj)
		{
			return this.Equals(obj as NetworkConfiguration);
		}

		public override int GetHashCode()
		{
			int num = (this.IPAddress == null) ? 0 : this.IPAddress.GetHashCode();
			int num2 = string.IsNullOrEmpty(this.NetworkName) ? 0 : this.NetworkName.GetHashCode();
			return num + num2;
		}

		private IPAddress ipAddress;

		private string networkName;
	}
}
