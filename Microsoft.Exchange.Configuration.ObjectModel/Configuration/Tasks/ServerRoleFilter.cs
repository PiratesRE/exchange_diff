using System;
using System.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Configuration.Tasks
{
	public class ServerRoleFilter<T> : IEnumerableFilter<T> where T : IConfigurable, new()
	{
		private ServerRoleFilter(ServerRole serverRole)
		{
			this.serverRole = serverRole;
		}

		public static ServerRoleFilter<T> GetServerRoleFilter(ServerRole serverRole)
		{
			ServerRoleFilter<T> result;
			lock (ServerRoleFilter<T>.syncRoot)
			{
				if (ServerRoleFilter<T>.instances == null)
				{
					ServerRoleFilter<T>.instances = new Hashtable();
				}
				if (ServerRoleFilter<T>.instances.Contains(serverRole))
				{
					result = (ServerRoleFilter<T>)ServerRoleFilter<T>.instances[serverRole];
				}
				else
				{
					ServerRoleFilter<T> serverRoleFilter = new ServerRoleFilter<T>(serverRole);
					ServerRoleFilter<T>.instances.Add(serverRole, serverRoleFilter);
					result = serverRoleFilter;
				}
			}
			return result;
		}

		public bool AcceptElement(T element)
		{
			if (element == null)
			{
				return false;
			}
			Server server = element as Server;
			return server != null && (server.CurrentServerRole & this.serverRole) > ServerRole.None;
		}

		private static Hashtable instances;

		private static object syncRoot = new object();

		private ServerRole serverRole;
	}
}
