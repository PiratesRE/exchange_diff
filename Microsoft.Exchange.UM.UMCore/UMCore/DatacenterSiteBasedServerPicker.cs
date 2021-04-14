using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class DatacenterSiteBasedServerPicker : ServerPickerBase<InternalExchangeServer, ADObjectId>
	{
		public DatacenterSiteBasedServerPicker() : base(DatacenterSiteBasedServerPicker.UMDefaultRetryInterval, DatacenterSiteBasedServerPicker.UMDefaultRefreshInterval, DatacenterSiteBasedServerPicker.UMDefaultRefreshIntervalOnFailure, ExTraceGlobals.UtilTracer)
		{
		}

		public static DatacenterSiteBasedServerPicker Instance
		{
			get
			{
				if (DatacenterSiteBasedServerPicker.staticInstance == null)
				{
					lock (DatacenterSiteBasedServerPicker.lockObj)
					{
						if (DatacenterSiteBasedServerPicker.staticInstance == null)
						{
							DatacenterSiteBasedServerPicker.staticInstance = new DatacenterSiteBasedServerPicker();
						}
					}
				}
				return DatacenterSiteBasedServerPicker.staticInstance;
			}
		}

		protected override List<InternalExchangeServer> LoadConfiguration()
		{
			List<InternalExchangeServer> list = new List<InternalExchangeServer>(10);
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this.GetHashCode(), "DatacenterSiteBasedServerPicker::LoadConfiguration()", new object[0]);
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			IEnumerable<Server> enabledExchangeServers = adtopologyLookup.GetEnabledExchangeServers(VersionEnum.E14Legacy, ServerRole.UnifiedMessaging);
			foreach (Server server in enabledExchangeServers)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this.GetHashCode(), "DatacenterSiteBasedServerPicker::LoadConfiguration() Adding Server={0} Version={1} UMRole={2} Status={3}", new object[]
				{
					server.Fqdn,
					server.VersionNumber,
					server.IsUnifiedMessagingServer,
					server.Status
				});
				list.Add(new InternalExchangeServer(server));
			}
			return list;
		}

		protected override bool IsValid(ADObjectId siteId, InternalExchangeServer candidate)
		{
			Server server = candidate.Server;
			return server.ServerSite != null && server.ServerSite.ObjectGuid.Equals(siteId.ObjectGuid);
		}

		internal static readonly TimeSpan UMDefaultRetryInterval = TimeSpan.FromMinutes(1.0);

		internal static readonly TimeSpan UMDefaultRefreshInterval = TimeSpan.FromMinutes(2.0);

		internal static readonly TimeSpan UMDefaultRefreshIntervalOnFailure = TimeSpan.FromMinutes(1.0);

		private static DatacenterSiteBasedServerPicker staticInstance;

		private static object lockObj = new object();
	}
}
