using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Microsoft.Exchange.Data
{
	[ImmutableObject(true)]
	public abstract class NetworkProtocol : Protocol
	{
		protected NetworkProtocol(string protocolName, string displayName) : base(protocolName, displayName)
		{
		}

		public static NetworkProtocol Parse(string expression)
		{
			if (!NetworkProtocol.supportedNetworkProtocols.ContainsKey(expression))
			{
				throw new ArgumentException(DataStrings.ExceptionUnsupportedNetworkProtocol);
			}
			return NetworkProtocol.supportedNetworkProtocols[expression];
		}

		public static bool TryParse(string expression, out NetworkProtocol protocol)
		{
			return NetworkProtocol.supportedNetworkProtocols.TryGetValue(expression, out protocol);
		}

		public abstract NetworkAddress GetNetworkAddress(string address);

		public static NetworkProtocol[] GetSupportedProtocols()
		{
			NetworkProtocol[] array = new NetworkProtocol[NetworkProtocol.supportedNetworkProtocols.Count];
			NetworkProtocol.supportedNetworkProtocols.Values.CopyTo(array, 0);
			return array;
		}

		public static bool IsSupportedProtocol(NetworkProtocol protocol)
		{
			return NetworkProtocol.supportedNetworkProtocols.ContainsValue(protocol);
		}

		public static readonly NetworkProtocol TcpIP = new CustomNetworkProtocol("ncacn_ip_tcp", DataStrings.ProtocolTcpIP);

		public static readonly NetworkProtocol NetBios = new CustomNetworkProtocol("netbios", DataStrings.ProtocolNetBios);

		public static readonly NetworkProtocol Spx = new CustomNetworkProtocol("ncacn_spx", DataStrings.ProtocolSpx);

		public static readonly NetworkProtocol LocalRpc = new CustomNetworkProtocol("ncalrpc", DataStrings.ProtocolLocalRpc);

		public static readonly NetworkProtocol AppleTalk = new CustomNetworkProtocol("ncacn_at_dsp", DataStrings.ProtocolAppleTalk);

		public static readonly NetworkProtocol NamedPipes = new CustomNetworkProtocol("ncacn_np", DataStrings.ProtocolNamedPipes);

		public static readonly NetworkProtocol VnsSpp = new CustomNetworkProtocol("ncacn_vns_spp", DataStrings.ProtocolVnsSpp);

		private static readonly Dictionary<string, NetworkProtocol> supportedNetworkProtocols = new NetworkProtocol[]
		{
			NetworkProtocol.TcpIP,
			NetworkProtocol.NetBios,
			NetworkProtocol.Spx,
			NetworkProtocol.LocalRpc,
			NetworkProtocol.AppleTalk,
			NetworkProtocol.NamedPipes,
			NetworkProtocol.VnsSpp
		}.ToDictionary((NetworkProtocol networtkProtocol) => networtkProtocol.ProtocolName);

		public static readonly NetworkProtocol DecNet = new CustomNetworkProtocol("ncacn_dnet_nsp");

		public static readonly NetworkProtocol UdpIP = new CustomNetworkProtocol("ncadg_ip_udp");

		public static readonly NetworkProtocol Ipx = new CustomNetworkProtocol("ncadg_ipx");

		public static readonly NetworkProtocol Msmq = new CustomNetworkProtocol("ncadg_mq");

		public static readonly NetworkProtocol Http = new CustomNetworkProtocol("ncacn_http");
	}
}
