using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Data.Storage.ActiveManager;
using Microsoft.Exchange.Data.Storage.Cluster;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Storage.ResourceHealth
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal class DatabaseInformationCache : LazyLookupExactTimeoutCache<Guid, DatabaseInformation>, IDatabaseInformationCache
	{
		public DatabaseInformationCache() : base(10000, false, TimeSpan.FromHours(1.0), CacheFullBehavior.ExpireExisting)
		{
		}

		protected override DatabaseInformation CreateOnCacheMiss(Guid key, ref bool shouldAdd)
		{
			shouldAdd = true;
			Dictionary<IADServer, CopyInfo> mailboxDatabaseCopyStatus;
			try
			{
				mailboxDatabaseCopyStatus = DatabaseInformationCache.dumpsterReplication.GetMailboxDatabaseCopyStatus(key);
			}
			catch (Exception ex)
			{
				if (ex is HaRpcServerTransientBaseException || ex is HaRpcServerBaseException || ex is DataBaseNotFoundException)
				{
					ExTraceGlobals.ClientThrottlingTracer.TraceError<Guid, Exception>((long)this.GetHashCode(), "[DatabaseInformationCache.HandleHaExceptions] Copy status lookup failed for MDB '{0}' with exception: '{1}'.", key, ex);
					return null;
				}
				throw;
			}
			if (mailboxDatabaseCopyStatus != null && mailboxDatabaseCopyStatus.Count > 0)
			{
				foreach (KeyValuePair<IADServer, CopyInfo> keyValuePair in mailboxDatabaseCopyStatus)
				{
					if (keyValuePair.Value.Status != null && keyValuePair.Value.Status.IsActiveCopy() && !string.IsNullOrEmpty(keyValuePair.Value.Status.DBName) && !string.IsNullOrEmpty(keyValuePair.Value.Status.DatabaseVolumeName))
					{
						return new DatabaseInformation(keyValuePair.Value.Status.DBGuid, keyValuePair.Value.Status.DBName, keyValuePair.Value.Status.DatabaseVolumeName);
					}
				}
			}
			return null;
		}

		public static DatabaseInformationCache Singleton
		{
			get
			{
				return DatabaseInformationCache.singleton;
			}
		}

		IDatabaseInformation IDatabaseInformationCache.Get(Guid key)
		{
			return base.Get(key);
		}

		private static readonly DatabaseInformationCache singleton = new DatabaseInformationCache();

		private static readonly DumpsterReplicationStatus dumpsterReplication = new DumpsterReplicationStatus();
	}
}
