using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Net;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.EdgeSync.Ehf;

namespace Microsoft.Exchange.Management.EdgeSync
{
	internal static class Utils
	{
		public static bool IsLeaseDirectoryValidPath(string directoryPath)
		{
			return !string.IsNullOrEmpty(directoryPath) && directoryPath.IndexOfAny(Path.GetInvalidPathChars()) < 0;
		}

		public static EhfTargetServerConfig CreateEhfTargetConfig(ITopologyConfigurationSession session, EdgeSyncEhfConnectorIdParameter con, Task task)
		{
			return new EhfTargetServerConfig(Utils.GetConnector(session, con, task), Utils.GetInternetWebProxy(session));
		}

		public static Uri GetInternetWebProxy(ITopologyConfigurationSession session)
		{
			Server localServer = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				localServer = session.ReadLocalServer();
			});
			if (localServer != null)
			{
				return localServer.InternetWebProxy;
			}
			return null;
		}

		public static EdgeSyncEhfConnector GetConnector(IConfigurationSession session, EdgeSyncEhfConnectorIdParameter connectorId, Task task)
		{
			EdgeSyncEhfConnector connector = null;
			ADNotificationAdapter.TryRunADOperation(delegate()
			{
				if (connectorId != null)
				{
					if (connectorId.InternalADObjectId != null)
					{
						connector = session.Read<EdgeSyncEhfConnector>(connectorId.InternalADObjectId);
						return;
					}
				}
				else
				{
					connector = Utils.FindEnabledEhfSyncConnector(session, null);
				}
			});
			if (connector == null)
			{
				task.WriteError(new InvalidOperationException("Unable to find EHF connector object"), ErrorCategory.InvalidOperation, null);
			}
			return connector;
		}

		public static EdgeSyncEhfConnector FindEnabledEhfSyncConnector(IConfigurationSession session, ADObjectId connectorIdToIgnore)
		{
			ADPagedReader<EdgeSyncEhfConnector> adpagedReader = session.FindAllPaged<EdgeSyncEhfConnector>();
			if (adpagedReader != null)
			{
				foreach (EdgeSyncEhfConnector edgeSyncEhfConnector in adpagedReader)
				{
					if (edgeSyncEhfConnector.Enabled && (connectorIdToIgnore == null || !connectorIdToIgnore.Equals(edgeSyncEhfConnector.Id)))
					{
						return edgeSyncEhfConnector;
					}
				}
			}
			return null;
		}

		public static List<IPAddress> ParseValidAddresses(string[] ipList)
		{
			List<IPAddress> list = new List<IPAddress>();
			foreach (string ipString in ipList)
			{
				IPAddress item;
				if (IPAddress.TryParse(ipString, out item))
				{
					list.Add(item);
				}
			}
			return list;
		}

		public static IList<string> ConvertIPAddresssesToStrings(IEnumerable<IPAddress> ipList)
		{
			List<string> list = new List<string>();
			foreach (IPAddress ipaddress in ipList)
			{
				list.Add(ipaddress.ToString());
			}
			return list;
		}
	}
}
