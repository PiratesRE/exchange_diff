using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory
{
	internal class ADConfigLoader<T> where T : ADConfigurationObject, IUsnChanged, new()
	{
		public IEnumerable<T> GetRefreshedData()
		{
			return this.GetRefreshedData(false);
		}

		public IEnumerable<T> GetDataWithFullReload()
		{
			return this.GetRefreshedData(true);
		}

		private IEnumerable<T> GetRefreshedData(bool forceFullReload)
		{
			IEnumerable<T> values;
			lock (this.syncRoot)
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.NonCacheSessionFactory.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 96, "GetRefreshedData", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\ADConfigLoader.cs");
				long val = this.maxUsnChanged;
				string arg;
				bool flag2 = this.NeedFullReload(topologyConfigurationSession, forceFullReload, out arg);
				if (!flag2)
				{
					QueryFilter filter = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, SharedPropertyDefinitions.UsnChanged, this.maxUsnChanged + 1L);
					int num = 0;
					int num2 = 0;
					IEnumerable<T> enumerable = this.LoadData(topologyConfigurationSession, filter, true);
					foreach (T value in enumerable)
					{
						val = Math.Max(val, value.UsnChanged);
						if (value.Id.IsDeleted)
						{
							T t;
							if (this.currentConfigObjects.TryGetValue(value.Id.ObjectGuid, out t))
							{
								this.currentConfigObjects.Remove(value.Id.ObjectGuid);
								num2++;
							}
						}
						else if (value.IsValid)
						{
							this.currentConfigObjects[value.Id.ObjectGuid] = value;
							num++;
						}
					}
					if (!this.CheckIfDomainControllerIsTheSame(topologyConfigurationSession, out arg))
					{
						flag2 = true;
					}
					ExTraceGlobals.ADConfigLoaderTracer.TraceDebug<int, int, string>((long)this.GetHashCode(), "Found {0} updated and {1} deleted objects of type {2}", num, num2, ADConfigLoader<T>.typeName);
				}
				if (flag2)
				{
					ExTraceGlobals.ADConfigLoaderTracer.TraceDebug<string>((long)this.GetHashCode(), "We need full reload. Reason: {0}", arg);
					val = 0L;
					this.currentConfigObjects = new Dictionary<Guid, T>(1024);
					foreach (T value2 in this.LoadData(topologyConfigurationSession, null, false))
					{
						if (value2.IsValid)
						{
							this.currentConfigObjects.Add(value2.Id.ObjectGuid, value2);
							val = Math.Max(val, value2.UsnChanged);
						}
					}
					ExTraceGlobals.ADConfigLoaderTracer.TraceDebug<int, string>((long)this.GetHashCode(), "Found {0} objects of type {1}", this.currentConfigObjects.Count, ADConfigLoader<T>.typeName);
				}
				this.maxUsnChanged = val;
				this.domainController = topologyConfigurationSession.Source;
				values = this.currentConfigObjects.Values;
			}
			return values;
		}

		private IEnumerable<T> LoadData(ITopologyConfigurationSession session, QueryFilter filter, bool includeDeletedObjects)
		{
			ExTraceGlobals.ADConfigLoaderTracer.TraceDebug<string, object>((long)this.GetHashCode(), "Loading objects of type {0} with filter {1} from AD", ADConfigLoader<T>.typeName, filter ?? "<null>");
			ADPagedReader<T> adpagedReader = session.FindPaged<T>(session.GetConfigurationNamingContext(), QueryScope.SubTree, filter, null, ADGenericPagedReader<T>.DefaultPageSize);
			adpagedReader.IncludeDeletedObjects = includeDeletedObjects;
			ExTraceGlobals.ADConfigLoaderTracer.TraceDebug<string>((long)this.GetHashCode(), "Loaded objects of type {0} from AD", ADConfigLoader<T>.typeName);
			return adpagedReader;
		}

		private bool NeedFullReload(ITopologyConfigurationSession session, bool forceFullReload, out string reason)
		{
			if (forceFullReload)
			{
				reason = "Full reload forced";
				return true;
			}
			if (string.IsNullOrEmpty(this.domainController))
			{
				reason = "First read";
				return true;
			}
			string configDCForLocalForest = TopologyProvider.GetInstance().GetConfigDCForLocalForest();
			if (!this.domainController.Equals(configDCForLocalForest, StringComparison.OrdinalIgnoreCase))
			{
				reason = string.Format("Config DC changed from {0} to {1}", this.domainController, configDCForLocalForest);
				return true;
			}
			ExTraceGlobals.ADConfigLoaderTracer.TraceDebug((long)this.GetHashCode(), "Skipping full reload");
			reason = null;
			return false;
		}

		private bool CheckIfDomainControllerIsTheSame(ITopologyConfigurationSession session, out string reason)
		{
			if (!string.Equals(session.Source, this.domainController, StringComparison.InvariantCultureIgnoreCase))
			{
				reason = string.Format("Session Domain controller changed from {0} to {1}", this.domainController, session.Source);
				return false;
			}
			reason = null;
			return true;
		}

		private static string typeName = typeof(T).Name;

		private long maxUsnChanged;

		private Dictionary<Guid, T> currentConfigObjects;

		private object syncRoot = new object();

		private string domainController;
	}
}
