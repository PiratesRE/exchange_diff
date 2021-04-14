using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal class VersionedQueueViewerClient : QueueViewerRpcClient
	{
		public VersionedQueueViewerClient(string serverName) : base(serverName)
		{
			this.serverName = serverName;
		}

		public byte[] GetQueueViewerObjectPage(QVObjectType objectType, byte[] queryFilterBytes, byte[] sortOrderBytes, bool searchForward, int pageSize, byte[] bookmarkObjectBytes, int bookmarkIndex, bool includeBookmark, bool includeDetails, byte[] inArgsBytes, ref int totalCount, ref int pageOffset)
		{
			bool usePreE14R4API = !VersionedQueueViewerClient.IsLocalHost(this.serverName) && !VersionedQueueViewerClient.ServerLaterThanVersion(VersionedQueueViewerClient.latencyComponentMinVersion, this.serverName);
			return base.GetQueueViewerObjectPage(objectType, queryFilterBytes, sortOrderBytes, searchForward, pageSize, bookmarkObjectBytes, bookmarkIndex, includeBookmark, includeDetails, usePreE14R4API, inArgsBytes, ref totalCount, ref pageOffset);
		}

		internal static bool UsePropertyBagBasedAPI(string serverName)
		{
			Server server = VersionedQueueViewerClient.GetServer(serverName);
			return VersionedQueueViewerClient.UsePropertyBagBasedAPI(server);
		}

		internal static bool UsePropertyBagBasedAPI(Server targetServer)
		{
			return VersionedQueueViewerClient.IsLocalHost(targetServer) || VersionedQueueViewerClient.ServerLaterThanVersion(VersionedQueueViewerClient.newAPIVersion, targetServer);
		}

		internal static bool UseCustomSerialization(string serverName)
		{
			Server server = VersionedQueueViewerClient.GetServer(serverName);
			return VersionedQueueViewerClient.UseCustomSerialization(server);
		}

		internal static bool UseCustomSerialization(Server targetServer)
		{
			return VersionedQueueViewerClient.IsLocalHost(targetServer) || VersionedQueueViewerClient.ServerLaterThan14Or15Version(VersionedQueueViewerClient.customSerialzationApiE14Version, VersionedQueueViewerClient.customSerialzationApiE15Version, targetServer);
		}

		internal static bool IsLocalHost(string serverName)
		{
			return serverName.Equals("localhost", StringComparison.OrdinalIgnoreCase);
		}

		internal static bool IsLocalHost(Server targetServer)
		{
			return targetServer.Name.Equals(Environment.MachineName, StringComparison.OrdinalIgnoreCase);
		}

		internal static bool ServerLaterThanVersion(int version, string serverName)
		{
			Server server = VersionedQueueViewerClient.GetServer(serverName);
			return VersionedQueueViewerClient.ServerLaterThanVersion(version, server);
		}

		internal static bool ServerLaterThanVersion(int version, Server targetServer)
		{
			int versionNumber = targetServer.VersionNumber;
			bool result;
			if (versionNumber >= Server.E2007SP2MinVersion && versionNumber < version)
			{
				result = false;
			}
			else
			{
				if (versionNumber < version)
				{
					throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_DATA);
				}
				result = true;
			}
			return result;
		}

		internal static Server GetServer(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				throw new ArgumentException("serverName is invalid", "servername");
			}
			Server targetServer = null;
			ADOperationResult adoperationResult = ADNotificationAdapter.TryRunADOperation(delegate()
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(false, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 258, "GetServer", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Queuing\\QueueViewer\\VersionedQueueViewerClient.cs");
				if (VersionedQueueViewerClient.IsLocalHost(serverName))
				{
					targetServer = topologyConfigurationSession.FindLocalServer();
					return;
				}
				if (serverName.Contains("."))
				{
					targetServer = topologyConfigurationSession.FindServerByFqdn(serverName);
					return;
				}
				targetServer = topologyConfigurationSession.FindServerByName(serverName);
			}, 0);
			if (!adoperationResult.Succeeded || targetServer == null)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_RPC_SERVER_UNAVAILABLE);
			}
			return targetServer;
		}

		private static bool ServerLaterThan14Or15Version(int versionIf14, int versionIf15, string serverName)
		{
			Server server = VersionedQueueViewerClient.GetServer(serverName);
			return VersionedQueueViewerClient.ServerLaterThan14Or15Version(versionIf14, versionIf15, server);
		}

		private static bool ServerLaterThan14Or15Version(int versionIf14, int versionIf15, Server targetServer)
		{
			int versionNumber = targetServer.VersionNumber;
			return (versionNumber >= Server.E15MinVersion && versionNumber > versionIf15) || (versionNumber >= Server.E14MinVersion && versionNumber < Server.E15MinVersion && versionNumber >= versionIf14);
		}

		private const int PropertyBagBasedAPIBuildCutOff = 562;

		private static readonly int latencyComponentMinVersion = new ServerVersion(14, 0, 562, 0).ToInt();

		private static readonly int newAPIVersion = new ServerVersion(14, 1, 218, 0).ToInt();

		private static readonly int customSerialzationApiE15Version = new ServerVersion(15, 0, 419, 0).ToInt();

		private static readonly int customSerialzationApiE14Version = new ServerVersion(14, 0, 562, 0).ToInt();

		protected string serverName;
	}
}
