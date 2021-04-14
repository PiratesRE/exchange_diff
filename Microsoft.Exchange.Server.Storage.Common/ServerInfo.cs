using System;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Server.Storage.Common
{
	public class ServerInfo
	{
		public Guid Guid { get; private set; }

		public string ServerName { get; private set; }

		public string ForestName { get; private set; }

		public string ExchangeLegacyDN { get; private set; }

		public string InstallPath { get; private set; }

		public SecurityDescriptor NTSecurityDescriptor { get; private set; }

		public bool IsMultiRole { get; private set; }

		public int? MaxRpcThreads { get; private set; }

		public long ContinuousReplicationMaxMemoryPerDatabase { get; private set; }

		public int? MaxActiveDatabases { get; private set; }

		public ServerEditionType Edition { get; private set; }

		public int MaxRecoveryDatabases { get; private set; }

		public int MaxTotalDatabases { get; private set; }

		public bool IsDAGMember { get; private set; }

		public DatabaseOptions DatabaseOptions { get; private set; }

		public ServerInfo(string serverName, Guid guidServer, string exchangeLegacyDN, string installPath, SecurityDescriptor ntSecurityDescriptor, bool isMultiRole, int? maxRpcThreads, long continuousReplicationMaxMemoryPerDatabase, int? maxActiveDatabases, ServerEditionType edition, int maxRecoveryDatabases, int maxTotalDatabases, bool isDAGMember, string forestName, DatabaseOptions databaseOptions)
		{
			this.ServerName = serverName;
			this.Guid = guidServer;
			this.ExchangeLegacyDN = exchangeLegacyDN;
			this.InstallPath = installPath;
			this.NTSecurityDescriptor = ntSecurityDescriptor;
			this.IsMultiRole = isMultiRole;
			this.MaxRpcThreads = maxRpcThreads;
			this.ContinuousReplicationMaxMemoryPerDatabase = continuousReplicationMaxMemoryPerDatabase;
			this.MaxActiveDatabases = maxActiveDatabases;
			this.Edition = edition;
			this.MaxRecoveryDatabases = maxRecoveryDatabases;
			this.MaxTotalDatabases = maxTotalDatabases;
			this.IsDAGMember = isDAGMember;
			this.ForestName = forestName;
			this.DatabaseOptions = databaseOptions;
		}
	}
}
