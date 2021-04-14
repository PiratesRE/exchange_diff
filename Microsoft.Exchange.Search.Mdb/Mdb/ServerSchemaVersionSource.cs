using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Search.Core.Diagnostics;
using Microsoft.Exchange.Search.OperatorSchema;

namespace Microsoft.Exchange.Search.Mdb
{
	internal class ServerSchemaVersionSource
	{
		public ServerSchemaVersionSource(Guid localServer, IDiagnosticsSession diagnosticsSession)
		{
			Dictionary<Guid, int> dictionary = this.serverSchemaVersion;
			VersionInfo latest = VersionInfo.Latest;
			dictionary.Add(localServer, latest.FeedingVersion);
			this.diagnosticsSession = diagnosticsSession;
		}

		public int GetServerVersion(Guid serverGuid)
		{
			int feedingVersion;
			if (!this.serverSchemaVersion.TryGetValue(serverGuid, out feedingVersion))
			{
				VersionInfo legacy = VersionInfo.Legacy;
				feedingVersion = legacy.FeedingVersion;
			}
			return feedingVersion;
		}

		public void LoadVersions(ICollection<Guid> serverGuids)
		{
			AdDataProvider adDataProvider = AdDataProvider.Create(this.diagnosticsSession);
			MiniServer[] servers = adDataProvider.GetServers(serverGuids, 1000);
			foreach (MiniServer miniServer in servers)
			{
				this.SetSchemaVersion(miniServer.Guid, VersionInfo.GetSchemaVersionForServerVersion(miniServer.AdminDisplayVersion));
			}
		}

		internal void SetSchemaVersion(Guid serverGuid, int version)
		{
			this.serverSchemaVersion[serverGuid] = version;
		}

		private readonly Dictionary<Guid, int> serverSchemaVersion = new Dictionary<Guid, int>();

		private readonly IDiagnosticsSession diagnosticsSession;
	}
}
