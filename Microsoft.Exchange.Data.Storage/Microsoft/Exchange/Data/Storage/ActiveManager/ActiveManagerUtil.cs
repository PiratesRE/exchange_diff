using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;

namespace Microsoft.Exchange.Data.Storage.ActiveManager
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class ActiveManagerUtil
	{
		public static Trace Tracer
		{
			get
			{
				return ExTraceGlobals.ActiveManagerClientTracer;
			}
		}

		private static List<ADObjectId> GetMasterServerIdsForDatabase(IFindAdObject<IADDatabaseAvailabilityGroup> dagLookup, IADDatabase database, out Exception exception)
		{
			IADToplogyConfigurationSession adSession = dagLookup.AdSession;
			List<ADObjectId> list = new List<ADObjectId>(16);
			exception = null;
			try
			{
				ADObjectId masterServerOrAvailabilityGroup = database.MasterServerOrAvailabilityGroup;
				if (masterServerOrAvailabilityGroup == null)
				{
					return null;
				}
				if (masterServerOrAvailabilityGroup.IsDeleted)
				{
					ActiveManagerUtil.Tracer.TraceError<string, string>(0L, "GetMasterServerIdsForDatabase() for database '{0}' found the MasterServerOrAvailabilityGroup to be a link to a deleted object. Returning an empty collection. MasterServerOrAvailabilityGroup = [{1}]", database.Name, masterServerOrAvailabilityGroup.Name);
					return null;
				}
				IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = dagLookup.ReadAdObjectByObjectId(masterServerOrAvailabilityGroup);
				if (iaddatabaseAvailabilityGroup != null)
				{
					list.AddRange(iaddatabaseAvailabilityGroup.Servers);
				}
				else
				{
					IADDatabase iaddatabase = adSession.ReadADObject<IADDatabase>(database.Id);
					ADObjectId adobjectId = null;
					if (iaddatabase != null)
					{
						adobjectId = iaddatabase.MasterServerOrAvailabilityGroup;
						if (!masterServerOrAvailabilityGroup.Equals(adobjectId))
						{
							ActiveManagerUtil.Tracer.TraceDebug<ADObjectId, ADObjectId>(0L, "GetMasterServerIdsForDatabase() re-read the Database object and it made a difference. MasterServerOrDag was {0} and is now {1}", masterServerOrAvailabilityGroup, adobjectId);
							iaddatabaseAvailabilityGroup = adSession.ReadADObject<IADDatabaseAvailabilityGroup>(adobjectId);
							if (iaddatabaseAvailabilityGroup == null)
							{
								goto IL_165;
							}
							HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
							foreach (string fqdn in iaddatabaseAvailabilityGroup.StoppedMailboxServers)
							{
								hashSet.Add(MachineName.GetNodeNameFromFqdn(fqdn));
							}
							using (MultiValuedProperty<ADObjectId>.Enumerator enumerator2 = iaddatabaseAvailabilityGroup.Servers.GetEnumerator())
							{
								while (enumerator2.MoveNext())
								{
									ADObjectId adobjectId2 = enumerator2.Current;
									if (!hashSet.Contains(adobjectId2.Name))
									{
										list.Add(adobjectId2);
									}
								}
								goto IL_165;
							}
						}
						ActiveManagerUtil.Tracer.TraceDebug<ADObjectId>(0L, "GetMasterServerIdsForDatabase: re-reading the Database object made no difference. MasterServerOrDag is still {0}.", masterServerOrAvailabilityGroup);
					}
					IL_165:
					if (iaddatabaseAvailabilityGroup == null && adobjectId != null)
					{
						IADServer iadserver = adSession.ReadMiniServer(adobjectId);
						if (iadserver != null)
						{
							list.Add(adobjectId);
						}
					}
				}
			}
			catch (DataValidationException ex)
			{
				exception = ex;
			}
			catch (ADTransientException ex2)
			{
				exception = ex2;
			}
			catch (ADOperationException ex3)
			{
				exception = ex3;
			}
			catch (ADTopologyUnexpectedException ex4)
			{
				exception = ex4;
			}
			catch (ADTopologyPermanentException ex5)
			{
				exception = ex5;
			}
			if (exception != null)
			{
				ActiveManagerUtil.Tracer.TraceDebug<Exception>(0L, "GetMasterServerIdsForDatabase() got exception: {0}", exception);
				list = null;
			}
			return list;
		}

		public static List<ADObjectId> GetServersForDatabaseInDag(IADDatabase database, DatabaseAvailabilityGroup dag)
		{
			MultiValuedProperty<ADObjectId> servers = dag.Servers;
			ADObjectId[] servers2 = database.Servers;
			List<ADObjectId> list = new List<ADObjectId>(servers2.Length);
			foreach (ADObjectId id in servers2)
			{
				foreach (ADObjectId adobjectId in servers)
				{
					if (adobjectId.Equals(id))
					{
						list.Add(adobjectId);
						break;
					}
				}
			}
			return list;
		}

		public static bool IsNullEncoded(string serverName)
		{
			return !string.IsNullOrEmpty(serverName) && string.Equals(serverName, "*", StringComparison.OrdinalIgnoreCase);
		}

		internal static List<ADObjectId> GetOrderedServerIdsForDatabase(IFindAdObject<IADDatabaseAvailabilityGroup> dagLookup, IADDatabase database, out Exception exception)
		{
			List<ADObjectId> masterServerIdsForDatabase = ActiveManagerUtil.GetMasterServerIdsForDatabase(dagLookup, database, out exception);
			if (exception != null)
			{
				return masterServerIdsForDatabase;
			}
			if (masterServerIdsForDatabase == null || masterServerIdsForDatabase.Count == 0)
			{
				ExTraceGlobals.ActiveManagerClientTracer.TraceError<string>(0L, "Database {0} master is pointing to a non-existent/deleted server or DAG, the database has been deleted, or the database object is corrupted in the AD.", database.Name);
				exception = new AmDatabaseMasterIsInvalid(database.Name);
				return masterServerIdsForDatabase;
			}
			int num = -1;
			string local = MachineName.Local;
			for (int i = 0; i < masterServerIdsForDatabase.Count; i++)
			{
				if (string.Equals(masterServerIdsForDatabase[i].Name, local, StringComparison.OrdinalIgnoreCase))
				{
					num = i;
					break;
				}
			}
			if (num > 0)
			{
				ADObjectId value = masterServerIdsForDatabase[num];
				masterServerIdsForDatabase[num] = masterServerIdsForDatabase[0];
				masterServerIdsForDatabase[0] = value;
			}
			return masterServerIdsForDatabase;
		}

		internal static ADObjectId GetServerSiteFromServer(Server server)
		{
			if (server.IsExchange2007OrLater)
			{
				return server.ServerSite;
			}
			return null;
		}

		internal static ADObjectId GetServerSiteFromMiniServer(IADServer server)
		{
			if (server.IsExchange2007OrLater)
			{
				return server.ServerSite;
			}
			return null;
		}

		internal static string NullEncode(string serverName)
		{
			if (string.IsNullOrEmpty(serverName))
			{
				return "*";
			}
			return serverName;
		}

		internal const string NullEncodeString = "*";
	}
}
