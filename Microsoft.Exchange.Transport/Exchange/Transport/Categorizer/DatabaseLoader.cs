using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class DatabaseLoader
	{
		public DatabaseLoader(RoutingContextCore context)
		{
			this.routingContext = context;
			this.syncObject = new object();
		}

		public IEnumerable<MiniDatabase> GetDatabases(DateTime timeToLog, bool forcedReload)
		{
			IEnumerable<MiniDatabase> values;
			lock (this.syncObject)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 87, "GetDatabases", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Routing\\DatabaseLoader.cs");
				long val = this.maxUsnChanged;
				bool flag2;
				string text;
				if (forcedReload)
				{
					flag2 = true;
					text = "Forced reload";
				}
				else
				{
					flag2 = this.NeedFullReload(topologyConfigurationSession, timeToLog, out text);
				}
				if (!flag2)
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, MiniDatabaseSchema.UsnChanged, this.maxUsnChanged + 1L);
					int num = 0;
					int num2 = 0;
					IEnumerable<MiniDatabase> enumerable = this.LoadMiniDatabases(topologyConfigurationSession, filter, timeToLog, true);
					foreach (MiniDatabase miniDatabase in enumerable)
					{
						val = Math.Max(val, miniDatabase.UsnChanged);
						if (miniDatabase.Id.IsDeleted)
						{
							this.databases.Remove(miniDatabase.Id.ObjectGuid);
							num2++;
						}
						else if (miniDatabase.IsValid)
						{
							this.databases[miniDatabase.Id.ObjectGuid] = miniDatabase;
							num++;
						}
					}
					if (!this.CheckIfDomainControllerIsTheSame(topologyConfigurationSession, out text))
					{
						flag2 = true;
					}
					RoutingDiag.Tracer.TraceDebug<DateTime, int, int>((long)this.GetHashCode(), "[{0}] Found {1} updated and {2} deleted mini databases", timeToLog, num, num2);
				}
				if (flag2)
				{
					RoutingDiag.Tracer.TraceDebug<DateTime, string>((long)this.GetHashCode(), "[{0}] We need full reload. Reason: {1}", timeToLog, text);
					RoutingDiag.EventLogger.LogEvent(TransportEventLogConstants.Tuple_RoutingTableDatabaseFullReload, null, new object[]
					{
						text
					});
					val = 0L;
					this.databases = new Dictionary<Guid, MiniDatabase>(1024);
					foreach (MiniDatabase miniDatabase2 in this.LoadMiniDatabases(topologyConfigurationSession, null, timeToLog, false))
					{
						if (miniDatabase2.IsValid && TransportHelpers.AttemptAddToDictionary<Guid, MiniDatabase>(this.databases, miniDatabase2.Id.ObjectGuid, miniDatabase2, new TransportHelpers.DiagnosticsHandler<Guid, MiniDatabase>(RoutingUtils.LogErrorWhenAddToDictionaryFails<Guid, MiniDatabase>)))
						{
							val = Math.Max(val, miniDatabase2.UsnChanged);
						}
					}
					RoutingDiag.Tracer.TraceDebug<DateTime, int>((long)this.GetHashCode(), "[{0}] Found {1} mini databases", timeToLog, this.databases.Count);
					this.lastFullReloadTime = DateTime.UtcNow;
				}
				this.maxUsnChanged = val;
				this.domainController = topologyConfigurationSession.Source;
				values = this.databases.Values;
			}
			return values;
		}

		private IEnumerable<MiniDatabase> LoadMiniDatabases(ITopologyConfigurationSession session, QueryFilter filter, DateTime timeToLog, bool includeDeletedObjects)
		{
			RoutingDiag.Tracer.TraceDebug<DateTime, object>((long)this.GetHashCode(), "[{0}] Loading mini database with filter {1} from AD", timeToLog, filter ?? "<null>");
			ADPagedReader<MiniDatabase> adpagedReader = session.FindPaged<MiniDatabase>(session.ConfigurationNamingContext, QueryScope.SubTree, filter, null, ADGenericPagedReader<MiniDatabase>.DefaultPageSize, DatabaseLoader.databaseSchemaProperties);
			adpagedReader.IncludeDeletedObjects = includeDeletedObjects;
			RoutingDiag.Tracer.TraceDebug<DateTime>((long)this.GetHashCode(), "[{0}] Loaded mini databases from AD", timeToLog);
			return adpagedReader;
		}

		private bool NeedFullReload(ITopologyConfigurationSession session, DateTime timeToLog, out string reason)
		{
			if (string.IsNullOrEmpty(this.domainController))
			{
				reason = "First read";
				return true;
			}
			TimeSpan timeSpan = DateTime.UtcNow - this.lastFullReloadTime;
			if (timeSpan > this.routingContext.Settings.DatabaseFullReloadInterval)
			{
				reason = string.Format("Full reload has not happened for {0}", this.routingContext.Settings.DatabaseFullReloadInterval);
				return true;
			}
			RoutingDiag.Tracer.TraceDebug<DateTime, TimeSpan>((long)this.GetHashCode(), "[{0}] Skipping full reload since it is only {1} since the last full reload", timeToLog, timeSpan);
			reason = null;
			return false;
		}

		private bool CheckIfDomainControllerIsTheSame(ITopologyConfigurationSession session, out string reason)
		{
			if (!string.Equals(session.Source, this.domainController, StringComparison.InvariantCultureIgnoreCase))
			{
				reason = string.Format("Domain controller changed from {0} to {1}", this.domainController, session.Source);
				return false;
			}
			reason = null;
			return true;
		}

		private static ReadOnlyCollection<PropertyDefinition> databaseSchemaProperties = new MiniDatabase().Schema.AllProperties;

		private DateTime lastFullReloadTime;

		private long maxUsnChanged;

		private Dictionary<Guid, MiniDatabase> databases;

		private RoutingContextCore routingContext;

		private object syncObject;

		private string domainController;
	}
}
