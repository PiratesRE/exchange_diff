using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal static class LocalServer
	{
		public static bool AllowsTokenSerializationBy(ClientSecurityContext clientContext)
		{
			return LocalServer.HasExtendedRightOnServer(clientContext, WellKnownGuid.TokenSerializationRightGuid);
		}

		public static bool HasExtendedRightOnServer(ClientSecurityContext clientContext, Guid extendedRight)
		{
			return LocalServer.GetServer().HasExtendedRight(clientContext, extendedRight);
		}

		public static Server GetServer()
		{
			if (LocalServer.server == null)
			{
				lock (LocalServer.lockObject)
				{
					if (LocalServer.server == null)
					{
						ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 71, "GetServer", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\SystemConfiguration\\LocalServer.cs");
						LocalServer.server = topologyConfigurationSession.FindLocalServer();
					}
				}
			}
			return LocalServer.server;
		}

		private static object lockObject = new object();

		private static Server server = null;
	}
}
