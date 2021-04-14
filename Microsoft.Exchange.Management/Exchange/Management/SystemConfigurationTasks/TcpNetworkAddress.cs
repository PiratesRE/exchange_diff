using System;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class TcpNetworkAddress : NetworkAddress
	{
		public TcpNetworkAddress(NetworkProtocol protocol, string address) : base(protocol, address)
		{
		}
	}
}
