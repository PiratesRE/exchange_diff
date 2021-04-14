using System;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal static class IPListEntryUtils
	{
		public static bool IsSupportedRole(Server server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			return server.IsEdgeServer || server.IsHubTransportServer;
		}
	}
}
