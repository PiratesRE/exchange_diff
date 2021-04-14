using System;
using System.Net;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.RpcClientAccess.Parser;

namespace Microsoft.Exchange.RpcClientAccess
{
	internal class ClientInfo
	{
		public ClientMode Mode { get; private set; }

		public MapiVersion Version { get; private set; }

		public IPAddress IpAddress { get; private set; }

		public string MachineName { get; private set; }

		public string ProcessName { get; private set; }

		public byte[] ClientSessionInfo { get; private set; }

		public ClientInfo(IPAddress ipAddress, string machineName, string processName, MapiVersion version, ClientMode mode, byte[] clientSessionInfo)
		{
			this.IpAddress = ipAddress;
			this.MachineName = machineName;
			this.ProcessName = processName;
			this.Version = version;
			this.Mode = mode;
			this.ClientSessionInfo = clientSessionInfo;
		}

		internal static ClientInfo CreateForTest(MapiVersion version)
		{
			return new ClientInfo(IPAddress.Loopback, "TestMachine", "TestProcess", version, ClientMode.Classic, null);
		}
	}
}
