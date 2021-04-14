using System;
using System.Net;
using System.Net.Sockets;

namespace Microsoft.Exchange.SenderId
{
	internal sealed class IPNetwork
	{
		private IPNetwork()
		{
		}

		public static IPNetwork Create(IPAddress network, int cidrLength)
		{
			return IPNetwork.InternalCreate(network, cidrLength);
		}

		public static IPNetwork Create(IPAddress network, int ip4CidrLength, int ip6CidrLength)
		{
			AddressFamily addressFamily = network.AddressFamily;
			if (addressFamily == AddressFamily.InterNetwork)
			{
				return IPNetwork.InternalCreate(network, ip4CidrLength);
			}
			if (addressFamily != AddressFamily.InterNetworkV6)
			{
				throw new ArgumentOutOfRangeException("network", network, "Invalid address family");
			}
			return IPNetwork.InternalCreate(network, ip6CidrLength);
		}

		public static IPNetwork Create(IPAddress address)
		{
			return new IPNetwork
			{
				network = address,
				networkBytes = null,
				cidrLength = -1
			};
		}

		private static IPNetwork InternalCreate(IPAddress network, int cidrLength)
		{
			if (cidrLength < 0)
			{
				return null;
			}
			IPNetwork ipnetwork = new IPNetwork();
			ipnetwork.network = network;
			ipnetwork.networkBytes = IPNetwork.GetMaskedAddress(network, cidrLength);
			int num = ipnetwork.networkBytes.Length * 8;
			if (cidrLength > num)
			{
				return null;
			}
			if (num > cidrLength)
			{
				ipnetwork.cidrLength = cidrLength;
				ipnetwork.bytesToConsider = ipnetwork.cidrLength / 8;
				if (ipnetwork.cidrLength % 8 != 0)
				{
					ipnetwork.bytesToConsider++;
				}
			}
			else
			{
				ipnetwork.cidrLength = -1;
			}
			return ipnetwork;
		}

		public bool Contains(IPAddress address)
		{
			if (this.network.AddressFamily != address.AddressFamily)
			{
				return false;
			}
			if (this.cidrLength == -1)
			{
				return this.network.Equals(address);
			}
			byte[] maskedAddress = IPNetwork.GetMaskedAddress(address, this.cidrLength);
			for (int i = 0; i < this.bytesToConsider; i++)
			{
				if (this.networkBytes[i] != maskedAddress[i])
				{
					return false;
				}
			}
			return true;
		}

		private static byte[] GetMaskedAddress(IPAddress address, int cidrLength)
		{
			byte[] addressBytes = address.GetAddressBytes();
			byte[] array = new byte[addressBytes.Length];
			int i;
			for (i = 0; i < cidrLength / 8; i++)
			{
				array[i] = addressBytes[i];
			}
			int num = cidrLength % 8;
			if (num != 0)
			{
				int num2 = 8 - num;
				byte b = (byte)(255 << num2);
				array[i] = (addressBytes[i] & b);
			}
			return array;
		}

		private const int FullCIDRLength = -1;

		private IPAddress network;

		private byte[] networkBytes;

		private int cidrLength;

		private int bytesToConsider;
	}
}
