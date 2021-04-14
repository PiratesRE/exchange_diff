using System;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.Cluster.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class ReplayAdObjectLookup : IReplayAdObjectLookup
	{
		public ReplayAdObjectLookup()
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
			ReplayAdObjectLookup.Tracer.TraceDebug((long)this.GetHashCode(), "Clearing cache");
			this.DagLookup.Clear();
			this.DatabaseLookup.Clear();
			this.ServerLookup.Clear();
			this.MiniServerLookup.Clear();
		}

		protected void Initialize(ConsistencyMode adConsistencyMode)
		{
			ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(adConsistencyMode, ADSessionSettings.FromRootOrgScopeSet(), 101, "Initialize", "f:\\15.00.1497\\sources\\dev\\cluster\\src\\Replay\\Service\\ReplayAdObjectLookup.cs");
			IADToplogyConfigurationSession adSession = ADSessionFactory.CreateWrapper(topologyConfigurationSession);
			TimeSpan timeToLive = TimeSpan.FromSeconds((double)RegistryParameters.AdObjectCacheHitTtlInSec);
			TimeSpan timeToNegativeLive = TimeSpan.FromSeconds((double)RegistryParameters.AdObjectCacheMissTtlInSec);
			this.AdSession = topologyConfigurationSession;
			this.DagLookup = new AdObjectLookupCache<IADDatabaseAvailabilityGroup>(adSession, timeToLive, timeToNegativeLive);
			this.DatabaseLookup = new AdObjectLookupCache<IADDatabase>(adSession, timeToLive, timeToNegativeLive);
			this.ServerLookup = new AdObjectLookupCache<IADServer>(adSession, timeToLive, timeToNegativeLive);
			this.MiniServerLookup = new MiniServerLookupCache(adSession, timeToLive, timeToNegativeLive);
		}

		public static readonly Trace Tracer = ExTraceGlobals.ADCacheTracer;
	}
}
