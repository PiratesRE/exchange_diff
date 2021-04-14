using System;
using Microsoft.Exchange.Data.HA.DirectoryServices;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class SingleCopyReplayConfiguration : ReplayConfiguration
	{
		public SingleCopyReplayConfiguration(IADDatabaseAvailabilityGroup dag, IADDatabase database, IADServer server, LockType lockType)
		{
			try
			{
				if (database == null)
				{
					throw new NullDatabaseException();
				}
				if (server == null)
				{
					throw new ErrorNullServerFromDb(database.Name);
				}
				this.m_database = database;
				this.m_type = ReplayConfigType.SingleCopySource;
				this.m_server = server;
				this.m_sourceNodeFqdn = server.Fqdn;
				this.m_replayState = ReplayState.GetReplayState(this.m_sourceNodeFqdn, this.m_sourceNodeFqdn, lockType, this.Identity, this.Database.Name);
				this.m_activationPreference = 1;
				base.PopulatePropertiesFromDag(dag);
			}
			finally
			{
				this.BuildDebugString();
			}
		}
	}
}
