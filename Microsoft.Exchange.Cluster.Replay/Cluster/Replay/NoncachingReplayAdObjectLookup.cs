using System;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class NoncachingReplayAdObjectLookup : IReplayAdObjectLookup
	{
		public NoncachingReplayAdObjectLookup()
		{
			this.Initialize(ConsistencyMode.IgnoreInvalid);
		}

		public ITopologyConfigurationSession AdSession { get; private set; }

		public IFindAdObject<IADDatabaseAvailabilityGroup> DagLookup { get; private set; }

		public IFindAdObject<IADDatabase> DatabaseLookup { get; private set; }

		public IFindAdObject<IADServer> ServerLookup { get; private set; }

		public IFindMiniServer MiniServerLookup { get; set; }

		public void Clear()
		{
			this.DagLookup.Clear();
			this.DatabaseLookup.Clear();
			this.ServerLookup.Clear();
			this.MiniServerLookup.Clear();
		}

		protected void Initialize(ConsistencyMode adConsistencyMode)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(adConsistencyMode, ADSessionSettings.FromRootOrgScopeSet(), 92, "Initialize", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Service\\NoncachingReplayAdObjectLookup.cs");
			this.AdSession = topologyConfigurationSession;
			IADToplogyConfigurationSession adSession = ADSessionFactory.CreateWrapper(topologyConfigurationSession);
			this.DagLookup = new SimpleAdObjectLookup<IADDatabaseAvailabilityGroup>(adSession);
			this.DatabaseLookup = new SimpleAdObjectLookup<IADDatabase>(adSession);
			this.ServerLookup = new SimpleAdObjectLookup<IADServer>(adSession);
			this.MiniServerLookup = new SimpleMiniServerLookup(adSession);
		}
	}
}
