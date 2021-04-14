using System;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class ForeignConnectorTaskUtil
	{
		public static void ValidateObject(ForeignConnector connector, IConfigurationSession session, Task task)
		{
			ForeignConnectorTaskUtil.ValidateSourceServers(connector, session, task);
			ForeignConnectorTaskUtil.ValidateDropDirectory(connector);
		}

		public static void CheckTopology()
		{
			if (TopologyProvider.IsAdamTopology())
			{
				throw new CannotRunForeignConnectorTaskOnEdgeException();
			}
		}

		public static bool IsHubServer(Server server)
		{
			return server != null && server.IsExchange2007OrLater && server.IsHubTransportServer;
		}

		public static void ValidateSourceServers(ForeignConnector connector, IConfigurationSession session, Task task)
		{
			ADObjectId sourceRoutingGroup = connector.SourceRoutingGroup;
			bool flag;
			bool flag2;
			LocalizedException ex = ManageSendConnectors.ValidateTransportServers(session, connector, ref sourceRoutingGroup, false, true, task, out flag, out flag2);
			if (ex != null)
			{
				throw ex;
			}
			if (flag2)
			{
				throw new MultiSiteSourceServersException();
			}
		}

		public static void ValidateDropDirectory(ForeignConnector connector)
		{
			if (string.IsNullOrEmpty(connector.DropDirectory))
			{
				throw new ForeignConnectorNullOrEmptyDropDirectoryException();
			}
		}
	}
}
