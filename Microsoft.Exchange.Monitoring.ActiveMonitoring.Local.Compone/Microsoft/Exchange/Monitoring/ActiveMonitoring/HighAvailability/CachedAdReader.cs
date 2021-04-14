using System;
using System.Linq;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Win32;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.HighAvailability
{
	internal class CachedAdReader
	{
		private CachedAdReader()
		{
			int value = HighAvailabilityUtility.NonCachedRegReader.GetValue<int>(Registry.LocalMachine, "SOFTWARE\\Microsoft\\ExchangeServer\\v15\\ActiveMonitoring\\HighAvailability\\Parameters", "AdCacheExpirationInSeconds", 900);
			this.DefaultTimeout = TimeSpan.FromSeconds((double)value);
			this.cachedLocalServerObject = new CachedObject<IADServer>(() => AdObjectLookupHelper.FindLocalServer(CachedAdReader.adLookup.ServerLookup), this.DefaultTimeout, true);
			this.cachedLocalDagObject = new CachedObject<IADDatabaseAvailabilityGroup>(delegate()
			{
				if (this.cachedLocalServerObject.GetValue.DatabaseAvailabilityGroup == null)
				{
					return null;
				}
				return CachedAdReader.adLookup.DagLookup.ReadAdObjectByObjectId(this.cachedLocalServerObject.GetValue.DatabaseAvailabilityGroup);
			}, this.DefaultTimeout, false);
			this.cachedAllDatabases = new CachedObject<IADDatabase[]>(() => AdObjectLookupHelper.GetAllDatabases(Dependencies.ReplayAdObjectLookup.DatabaseLookup, this.cachedLocalServerObject.GetValue), this.DefaultTimeout, false);
			this.cachedServersInDag = new CachedObject<IADServer[]>(delegate()
			{
				IADDatabaseAvailabilityGroup getValue = this.cachedLocalDagObject.GetValue;
				if (getValue == null)
				{
					return null;
				}
				return (from serverId in getValue.Servers
				select CachedAdReader.adLookup.ServerLookup.ReadAdObjectByObjectId(serverId)).ToArray<IADServer>();
			}, this.DefaultTimeout, false);
		}

		public static CachedAdReader Instance
		{
			get
			{
				if (CachedAdReader.cachedAdReaderInstance == null)
				{
					lock (CachedAdReader.instanceCreationLock)
					{
						CachedAdReader.cachedAdReaderInstance = new CachedAdReader();
					}
				}
				return CachedAdReader.cachedAdReaderInstance;
			}
		}

		public IADServer LocalServer
		{
			get
			{
				return this.cachedLocalServerObject.GetValue;
			}
		}

		public IADDatabaseAvailabilityGroup LocalDAG
		{
			get
			{
				return this.cachedLocalDagObject.GetValue;
			}
		}

		public IADDatabase[] AllLocalDatabases
		{
			get
			{
				return this.cachedAllDatabases.GetValue;
			}
		}

		public IADServer[] AllServersInLocalDag
		{
			get
			{
				return this.cachedServersInDag.GetValue;
			}
		}

		public IADDatabase GetDatabaseOnLocalServer(Guid mdbGuid)
		{
			IADDatabase[] allLocalDatabases = this.AllLocalDatabases;
			if (allLocalDatabases == null || allLocalDatabases.Length < 1)
			{
				return null;
			}
			if ((from db in allLocalDatabases
			select db.Guid).Contains(mdbGuid))
			{
				return (from db in allLocalDatabases
				where db.Guid.Equals(mdbGuid)
				select db).FirstOrDefault<IADDatabase>();
			}
			this.cachedAllDatabases.ForceUpdate();
			allLocalDatabases = this.AllLocalDatabases;
			if (!(from db in allLocalDatabases
			select db.Guid).Contains(mdbGuid))
			{
				throw new HighAvailabilityMAUtilityException(string.Format("MDB with Guid {0} does not exists on this server!", mdbGuid.ToString()));
			}
			return (from db in allLocalDatabases
			where db.Guid.Equals(mdbGuid)
			select db).FirstOrDefault<IADDatabase>();
		}

		public readonly TimeSpan DefaultTimeout;

		private static CachedAdReader cachedAdReaderInstance = null;

		private static object instanceCreationLock = new object();

		private static IReplayAdObjectLookup adLookup = Dependencies.ReplayAdObjectLookup;

		private CachedObject<IADServer> cachedLocalServerObject;

		private CachedObject<IADDatabaseAvailabilityGroup> cachedLocalDagObject;

		private CachedObject<IADDatabase[]> cachedAllDatabases;

		private CachedObject<IADServer[]> cachedServersInDag;
	}
}
