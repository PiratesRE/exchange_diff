using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Exchange.Cluster.ClusApi;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Cluster.Shared;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.HA.DirectoryServices;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;

namespace Microsoft.Exchange.Cluster.Replay
{
	internal class MonitoringADConfig : IMonitoringADConfig
	{
		public AmServerName TargetServerName { get; private set; }

		public IADServer TargetMiniServer { get; private set; }

		public MonitoringServerRole ServerRole { get; private set; }

		public IADDatabaseAvailabilityGroup Dag { get; private set; }

		public List<IADServer> Servers
		{
			get
			{
				return this.m_serverList;
			}
		}

		public List<AmServerName> AmServerNames
		{
			get
			{
				return this.m_amServerNameList;
			}
		}

		public Dictionary<AmServerName, IEnumerable<IADDatabase>> DatabaseMap { get; private set; }

		public Dictionary<AmServerName, IEnumerable<IADDatabase>> DatabasesIncludingMisconfiguredMap { get; private set; }

		public Dictionary<Guid, IADDatabase> DatabaseByGuidMap { get; private set; }

		public DateTime CreateTimeUtc { get; private set; }

		private static Microsoft.Exchange.Diagnostics.Trace Tracer
		{
			get
			{
				return ExTraceGlobals.MonitoringTracer;
			}
		}

		private IReplayAdObjectLookup AdLookup { get; set; }

		private IReplayAdObjectLookup AdLookupPartiallyConsistent { get; set; }

		private IADToplogyConfigurationSession AdSessionIgnoreInvalid { get; set; }

		private IADToplogyConfigurationSession AdSessionPartiallyConsistent { get; set; }

		protected MonitoringADConfig(AmServerName serverName, IReplayAdObjectLookup adLookup, IReplayAdObjectLookup adLookupPartiallyConsistent, IADToplogyConfigurationSession adSession, IADToplogyConfigurationSession adSessionPartiallyConsistent, Func<bool> isServiceShuttingDownFunc)
		{
			this.m_targetServerName = serverName;
			this.AdLookup = adLookup;
			this.AdLookupPartiallyConsistent = adLookupPartiallyConsistent;
			this.AdSessionIgnoreInvalid = adSession;
			this.AdSessionPartiallyConsistent = adSessionPartiallyConsistent;
			this.m_isServiceShuttingDownFunc = isServiceShuttingDownFunc;
		}

		public static IDisposable SetTestHook(Action testAddDbCopy)
		{
			return MonitoringADConfig.hookableTestAddDbCopy.SetTestHook(testAddDbCopy);
		}

		public static MonitoringADConfig GetConfig(AmServerName serverName, IReplayAdObjectLookup adLookup, IReplayAdObjectLookup adLookupPartiallyConsistent, IADToplogyConfigurationSession adSession, IADToplogyConfigurationSession adSessionPartiallyConsistent, Func<bool> isServiceShuttingDownFunc)
		{
			ReplayServerPerfmon.ADConfigRefreshCalls.Increment();
			ReplayServerPerfmon.ADConfigRefreshCallsPerSec.Increment();
			Stopwatch stopwatch = Stopwatch.StartNew();
			MonitoringADConfig config = new MonitoringADConfig(serverName, adLookup, adLookupPartiallyConsistent, adSession, adSessionPartiallyConsistent, isServiceShuttingDownFunc);
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				config.Refresh();
			}, 2);
			ReplayServerPerfmon.ADConfigRefreshLatency.IncrementBy(stopwatch.ElapsedTicks);
			ReplayServerPerfmon.ADConfigRefreshLatencyBase.Increment();
			ExTraceGlobals.ADCacheTracer.TraceDebug<TimeSpan>((long)config.GetHashCode(), "MonitoringADConfig.GetConfig took {0}", stopwatch.Elapsed);
			if (stopwatch.Elapsed > MonitoringADConfig.MaxHealthyADRefreshDuration)
			{
				ReplayCrimsonEvents.ADConfigRefreshWasSlow.LogPeriodic<TimeSpan>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, stopwatch.Elapsed);
			}
			if (ex != null)
			{
				MonitoringADConfig.Tracer.TraceError<string, string>((long)config.GetHashCode(), "MonitoringADConfig.GetConfig( {0} ): Got exception: {1}", serverName.NetbiosName, AmExceptionHelper.GetExceptionToStringOrNoneString(ex));
				ReplayCrimsonEvents.ADConfigRefreshFailed.LogPeriodic<string, string>(Environment.MachineName, DiagCore.DefaultEventSuppressionInterval, ex.ToString(), Environment.StackTrace);
				throw new MonitoringADConfigException(ex.Message, ex);
			}
			return config;
		}

		public IADServer LookupMiniServerByName(AmServerName serverName)
		{
			IADServer result = null;
			if (this.m_miniServerLookup != null)
			{
				this.m_miniServerLookup.TryGetValue(serverName, out result);
			}
			return result;
		}

		protected void Refresh()
		{
			MonitoringADConfig.Tracer.TraceDebug<AmServerName>((long)this.GetHashCode(), "MonitoringADConfig.Refresh() called against server name '{0}' ...", this.m_targetServerName);
			this.CreateTimeUtc = DateTime.UtcNow;
			this.TargetServerName = this.m_targetServerName;
			this.TargetMiniServer = this.LookupMiniServer(this.m_targetServerName.NetbiosName);
			this.CheckServiceShuttingDown();
			if (this.TargetMiniServer.DatabaseAvailabilityGroup == null)
			{
				this.ServerRole = MonitoringServerRole.Standalone;
				this.InitializeServerStructures(1);
				this.AddServerIfNeeded(this.TargetServerName, this.TargetMiniServer);
			}
			else
			{
				this.ServerRole = MonitoringServerRole.DagMember;
				this.Dag = this.LookupDag(this.TargetMiniServer.DatabaseAvailabilityGroup);
				this.CheckServiceShuttingDown();
				bool flag = false;
				this.InitializeServerStructures(this.Dag.Servers.Count);
				foreach (ADObjectId adobjectId in this.Dag.Servers)
				{
					IADServer iadserver = this.LookupMiniServer(adobjectId.Name);
					MonitoringADConfig.Tracer.TraceDebug<DatabaseCopyAutoActivationPolicyType, bool>((long)this.GetHashCode(), "MonitoringADConfig.Refresh() : Values of DatabaseCopyAutoActivationPolicy={0},  DatabaseCopyActivationDisabledAndMoveNow={1}", iadserver.DatabaseCopyAutoActivationPolicy, iadserver.DatabaseCopyActivationDisabledAndMoveNow);
					this.AddServerIfNeeded(iadserver);
					if (iadserver.Guid.Equals(this.TargetMiniServer.Guid))
					{
						flag = true;
					}
					this.CheckServiceShuttingDown();
				}
				if (!flag)
				{
					this.AddServerIfNeeded(this.TargetServerName, this.TargetMiniServer);
				}
			}
			MonitoringADConfig.hookableTestAddDbCopy.Value();
			this.LookupAndPopulateDatabases();
			DiagCore.RetailAssert(this.m_amServerNameList.Count == this.m_serverList.Count, "m_amServerNameList [{0}] != m_serverList [{1}], in length", new object[]
			{
				this.m_amServerNameList.Count,
				this.m_serverList.Count
			});
			DiagCore.RetailAssert(this.DatabaseMap.Keys.Count == this.m_serverList.Count, "DatabaseMap.Keys.Count [{0}] != m_serverList.Count [{1}]", new object[]
			{
				this.DatabaseMap.Count,
				this.m_serverList.Count
			});
			DiagCore.RetailAssert(this.DatabaseMap.ContainsKey(this.TargetServerName), "DatabaseMap should contain the target server entry!", new object[0]);
		}

		private void LookupAndPopulateDatabases()
		{
			Dictionary<Guid, IADDatabase> dictionary = new Dictionary<Guid, IADDatabase>(Math.Min(160, this.m_serverList.Count * 20));
			Dictionary<AmServerName, IEnumerable<IADDatabase>> dictionary2 = new Dictionary<AmServerName, IEnumerable<IADDatabase>>(this.m_serverList.Count);
			Dictionary<AmServerName, IEnumerable<IADDatabase>> dictionary3 = new Dictionary<AmServerName, IEnumerable<IADDatabase>>(this.m_serverList.Count);
			Dictionary<string, IADDatabase> dictionary4 = new Dictionary<string, IADDatabase>(160, StringComparer.OrdinalIgnoreCase);
			foreach (IADServer iadserver in this.m_serverList)
			{
				AmServerName orConstructAmServerName = this.GetOrConstructAmServerName(iadserver);
				List<IADDatabase> list = new List<IADDatabase>(48);
				List<IADDatabase> list2 = new List<IADDatabase>(48);
				IEnumerable<IADDatabaseCopy> enumerable = this.LookupDatabaseCopies(iadserver);
				foreach (IADDatabaseCopy iaddatabaseCopy in enumerable)
				{
					IADDatabase iaddatabase;
					if (!dictionary4.TryGetValue(iaddatabaseCopy.DatabaseName, out iaddatabase))
					{
						iaddatabase = this.LookupDatabaseFromCopy(iaddatabaseCopy);
						if (iaddatabase == null)
						{
							MonitoringADConfig.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MonitoringADConfig.LookupAndPopulateDatabases(): Found database copy object '{0}\\{1}' but couldn't read its parent Database object. Skipping this copy and moving to the next one on this server.", iaddatabaseCopy.DatabaseName, iaddatabaseCopy.Name);
							continue;
						}
						dictionary4[iaddatabaseCopy.DatabaseName] = iaddatabase;
					}
					if (this.GetDatabaseCopyFromDb(iaddatabase, iaddatabaseCopy) == null)
					{
						MonitoringADConfig.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MonitoringADConfig.LookupAndPopulateDatabases(): Found database copy object '{0}\\{1}' that the cached DB object doesn't have yet. Skipping this copy and moving to the next one on this server.", iaddatabaseCopy.DatabaseName, iaddatabaseCopy.Name);
					}
					else
					{
						list2.Add(iaddatabase);
						bool isValid = iaddatabaseCopy.IsValid;
						bool isValid2 = iaddatabase.IsValid;
						if (isValid && isValid2)
						{
							if (!dictionary.ContainsKey(iaddatabase.Guid))
							{
								dictionary[iaddatabase.Guid] = iaddatabase;
							}
							list.Add(iaddatabase);
						}
						else if (!isValid)
						{
							MonitoringADConfig.Tracer.TraceError<string, string>((long)this.GetHashCode(), "MonitoringADConfig.LookupAndPopulateDatabases(): Found invalid database copy object '{0}\\{1}'", iaddatabase.Name, iaddatabaseCopy.Name);
						}
						else if (!isValid2)
						{
							MonitoringADConfig.Tracer.TraceError<string>((long)this.GetHashCode(), "MonitoringADConfig.LookupAndPopulateDatabases(): Found invalid database object '{0}'", iaddatabase.Name);
						}
						foreach (IADDatabaseCopy iaddatabaseCopy2 in iaddatabase.AllDatabaseCopies)
						{
							AmServerName orConstructAmServerName2 = this.GetOrConstructAmServerName(iaddatabaseCopy2.Name);
							if (!this.m_miniServerLookup.ContainsKey(orConstructAmServerName2))
							{
								this.ExcludeDatabaseCopy(iaddatabase, orConstructAmServerName2);
							}
						}
						this.CheckServiceShuttingDown();
					}
				}
				dictionary2.Add(orConstructAmServerName, list);
				dictionary3.Add(orConstructAmServerName, list2);
				MonitoringADConfig.Tracer.TraceDebug<string, int, int>((long)this.GetHashCode(), "MonitoringADConfig.LookupAndPopulateDatabases(): Server ( {0} ) found: Valid DB Copies ({1}), All DB Copies ({2}).", iadserver.Name, list.Count, list2.Count);
				this.CheckServiceShuttingDown();
			}
			this.DatabaseMap = dictionary2;
			this.DatabaseByGuidMap = dictionary;
			this.DatabasesIncludingMisconfiguredMap = dictionary3;
		}

		private IADDatabaseCopy GetDatabaseCopyFromDb(IADDatabase database, IADDatabaseCopy dbCopy)
		{
			foreach (IADDatabaseCopy iaddatabaseCopy in database.AllDatabaseCopies)
			{
				if (iaddatabaseCopy.Guid.Equals(dbCopy.Guid))
				{
					return iaddatabaseCopy;
				}
			}
			return null;
		}

		private void ExcludeDatabaseCopy(IADDatabase db, AmServerName tmpServer)
		{
			string netbiosName = tmpServer.NetbiosName;
			string name = db.Name;
			IADDatabaseCopy[] allDatabaseCopies = db.AllDatabaseCopies;
			MonitoringADConfig.Tracer.TraceDebug<string, string>((long)this.GetHashCode(), "MonitoringADConfig.Refresh(): Found database copy '{0}\\{1}' for which the host server is not yet in the DAG members list. Excluding this database copy from its database object.", name, netbiosName);
			db.ExcludeDatabaseCopyFromProperties(netbiosName);
		}

		private void InitializeServerStructures(int serverCount)
		{
			this.m_serverList = new List<IADServer>(serverCount);
			this.m_amServerNameList = new List<AmServerName>(serverCount);
			this.m_miniServerLookup = new Dictionary<AmServerName, IADServer>(serverCount);
			this.m_amServerLookup = new Dictionary<string, AmServerName>(serverCount, AmServerName.Comparer);
		}

		private void AddServerIfNeeded(IADServer miniServerToAdd)
		{
			AmServerName orConstructAmServerName = this.GetOrConstructAmServerName(miniServerToAdd);
			this.AddServerIfNeeded(orConstructAmServerName, miniServerToAdd);
		}

		private void AddServerIfNeeded(AmServerName serverToAdd)
		{
			IADServer miniServerToAdd = this.LookupMiniServer(serverToAdd.NetbiosName);
			this.AddServerIfNeeded(serverToAdd, miniServerToAdd);
		}

		private void AddServerIfNeeded(AmServerName serverToAdd, IADServer miniServerToAdd)
		{
			if (!this.m_miniServerLookup.ContainsKey(serverToAdd))
			{
				this.m_miniServerLookup.Add(serverToAdd, miniServerToAdd);
				this.m_serverList.Add(miniServerToAdd);
				this.m_amServerNameList.Add(serverToAdd);
			}
			if (!this.m_amServerLookup.ContainsKey(serverToAdd.NetbiosName))
			{
				this.m_amServerLookup.Add(serverToAdd.NetbiosName, serverToAdd);
			}
		}

		private void CheckServiceShuttingDown()
		{
			if (this.m_isServiceShuttingDownFunc != null && this.m_isServiceShuttingDownFunc())
			{
				throw new MonitoringADServiceShuttingDownException();
			}
		}

		private AmServerName GetOrConstructAmServerName(IADServer miniServer)
		{
			return this.GetOrConstructAmServerName(miniServer.Name);
		}

		private AmServerName GetOrConstructAmServerName(string serverName)
		{
			AmServerName amServerName = null;
			if (!this.m_amServerLookup.TryGetValue(serverName, out amServerName))
			{
				amServerName = new AmServerName(serverName, false);
				this.m_amServerLookup[serverName] = amServerName;
			}
			return amServerName;
		}

		protected virtual IADServer LookupMiniServer(string serverShortName)
		{
			Exception ex = null;
			IADServer iadserver = this.AdLookup.MiniServerLookup.FindMiniServerByShortNameEx(serverShortName, out ex);
			if (iadserver == null)
			{
				MonitoringADConfig.Tracer.TraceError<string, string>((long)this.GetHashCode(), "LookupMiniServer( {0} ): Got exception: {1}", serverShortName, AmExceptionHelper.GetExceptionToStringOrNoneString(ex));
				throw new MonitoringCouldNotFindMiniServerException(serverShortName, ex);
			}
			MonitoringADConfig.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LookupMiniServer( {0} ): Found MiniServer object.", serverShortName);
			return iadserver;
		}

		protected virtual IADDatabaseAvailabilityGroup LookupDag(ADObjectId dagObjectId)
		{
			Exception ex = null;
			IADDatabaseAvailabilityGroup iaddatabaseAvailabilityGroup = this.AdLookup.DagLookup.ReadAdObjectByObjectIdEx(dagObjectId, out ex);
			if (iaddatabaseAvailabilityGroup == null)
			{
				MonitoringADConfig.Tracer.TraceError<ADObjectId, string>((long)this.GetHashCode(), "LookupDag( {0} ): Got exception: {1}", dagObjectId, AmExceptionHelper.GetExceptionToStringOrNoneString(ex));
				throw new MonitoringCouldNotFindDagException(dagObjectId.Name, AmExceptionHelper.GetExceptionMessageOrNoneString(ex), ex);
			}
			MonitoringADConfig.Tracer.TraceDebug<ADObjectId>((long)this.GetHashCode(), "LookupDag( {0} ): Found DAG object.", dagObjectId);
			return iaddatabaseAvailabilityGroup;
		}

		protected virtual IADDatabase LookupDatabaseFromCopy(IADDatabaseCopy dbCopy)
		{
			return this.AdLookup.DatabaseLookup.ReadAdObjectByObjectId(dbCopy.Id.Parent);
		}

		protected virtual IEnumerable<IADDatabaseCopy> LookupDatabaseCopies(IADServer miniServer)
		{
			MonitoringADConfig.Tracer.TraceDebug<string>((long)this.GetHashCode(), "LookupDatabases ( {0} ): Searching for all valid/invalid database copies...", miniServer.Name);
			IEnumerable<IADDatabaseCopy> dbCopies = null;
			IADToplogyConfigurationSession adSession = this.AdSessionPartiallyConsistent;
			Exception ex = ADUtils.RunADOperation(delegate()
			{
				dbCopies = adSession.GetAllDatabaseCopies(miniServer);
			}, 2);
			if (ex != null)
			{
				MonitoringADConfig.Tracer.TraceError<string, string>((long)this.GetHashCode(), "LookupDatabases ( {0} ): Got exception: {1}", miniServer.Name, AmExceptionHelper.GetExceptionToStringOrNoneString(ex));
				throw new MonitoringCouldNotFindDatabasesException(miniServer.Name, ex.Message, ex);
			}
			if (dbCopies == null)
			{
				dbCopies = new IADDatabaseCopy[0];
			}
			return dbCopies;
		}

		private static Hookable<Action> hookableTestAddDbCopy = Hookable<Action>.Create(true, delegate()
		{
		});

		private AmServerName m_targetServerName;

		private static readonly TimeSpan MaxHealthyADRefreshDuration = TimeSpan.FromSeconds(5.0);

		private Dictionary<AmServerName, IADServer> m_miniServerLookup;

		private Dictionary<string, AmServerName> m_amServerLookup;

		private List<IADServer> m_serverList;

		private List<AmServerName> m_amServerNameList;

		private Func<bool> m_isServiceShuttingDownFunc;
	}
}
