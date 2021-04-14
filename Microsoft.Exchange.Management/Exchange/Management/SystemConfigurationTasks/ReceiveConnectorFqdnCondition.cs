using System;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ReceiveConnectorFqdnCondition
	{
		public static bool Verify(ReceiveConnector connector, Server server, out LocalizedException exception)
		{
			if (connector == null)
			{
				throw new ArgumentNullException("connector");
			}
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			exception = null;
			if (server.Fqdn.Equals(connector.Fqdn, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			if (server.Name.Equals(connector.Fqdn, StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			exception = new InvalidFqdnUnderExchangeServerAuthException(server.Fqdn, server.Name);
			return false;
		}
	}
}
