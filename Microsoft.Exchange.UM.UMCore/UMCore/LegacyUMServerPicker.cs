using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.UMCore
{
	internal class LegacyUMServerPicker : ServerPickerBase<InternalExchangeServer, ADObjectId>
	{
		public LegacyUMServerPicker()
		{
			this.version = VersionEnum.E12Legacy;
		}

		private LegacyUMServerPicker(VersionEnum version) : base(LegacyUMServerPicker.UMDefaultRetryInterval, LegacyUMServerPicker.UMDefaultRefreshInterval, LegacyUMServerPicker.UMDefaultRefreshIntervalOnFailure, ExTraceGlobals.UtilTracer)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this.GetHashCode(), "LegacyUMServerPicker for version : {0}", new object[]
			{
				version
			});
			this.version = version;
		}

		public static LegacyUMServerPicker GetInstance(VersionEnum version)
		{
			switch (version)
			{
			case VersionEnum.E12Legacy:
				if (LegacyUMServerPicker.e12staticInstance == null)
				{
					lock (LegacyUMServerPicker.lockObj)
					{
						if (LegacyUMServerPicker.e12staticInstance == null)
						{
							LegacyUMServerPicker.e12staticInstance = new LegacyUMServerPicker(VersionEnum.E12Legacy);
						}
					}
				}
				return LegacyUMServerPicker.e12staticInstance;
			case VersionEnum.E14Legacy:
				if (LegacyUMServerPicker.e14staticInstance == null)
				{
					lock (LegacyUMServerPicker.lockObj)
					{
						if (LegacyUMServerPicker.e14staticInstance == null)
						{
							LegacyUMServerPicker.e14staticInstance = new LegacyUMServerPicker(VersionEnum.E14Legacy);
						}
					}
				}
				return LegacyUMServerPicker.e14staticInstance;
			default:
				throw new ArgumentException(version.ToString());
			}
		}

		protected override List<InternalExchangeServer> LoadConfiguration()
		{
			List<InternalExchangeServer> list = new List<InternalExchangeServer>(10);
			ADTopologyLookup adtopologyLookup = ADTopologyLookup.CreateLocalResourceForestLookup();
			IEnumerable<Server> enabledExchangeServers = adtopologyLookup.GetEnabledExchangeServers(this.version, ServerRole.UnifiedMessaging);
			foreach (Server server in enabledExchangeServers)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this.GetHashCode(), "LegacyUMServerPicker::LoadConfiguration() Adding legacy Server={0} Serial={1} UMRole={2} Status={3}", new object[]
				{
					server.Fqdn,
					server.SerialNumber,
					server.IsUnifiedMessagingServer,
					server.Status
				});
				ExAssert.RetailAssert(server.Status == ServerStatus.Enabled, "Server {0} is enabled", new object[]
				{
					server.Fqdn
				});
				list.Add(new InternalExchangeServer(server));
			}
			return list;
		}

		protected override bool IsHealthy(ADObjectId dialPlanId, InternalExchangeServer server)
		{
			return Util.PingServerWithRpc(server.Server, dialPlanId.ObjectGuid, (this.version == VersionEnum.E12Legacy) ? LegacyUMServerPicker.FakeE12Server : LegacyUMServerPicker.FakeE14Server);
		}

		protected override bool IsValid(ADObjectId dialPlanId, InternalExchangeServer candidate)
		{
			foreach (ADObjectId adobjectId in candidate.Server.DialPlans)
			{
				if (adobjectId.ObjectGuid.Equals(dialPlanId.ObjectGuid))
				{
					return true;
				}
			}
			return false;
		}

		internal static readonly TimeSpan UMDefaultRetryInterval = TimeSpan.FromMinutes(1.0);

		internal static readonly TimeSpan UMDefaultRefreshInterval = TimeSpan.FromMinutes(2.0);

		internal static readonly TimeSpan UMDefaultRefreshIntervalOnFailure = TimeSpan.FromMinutes(1.0);

		private static readonly string FakeE12Server = "FakeE12UmServer";

		private static readonly string FakeE14Server = "FakeE14UmServer";

		private static LegacyUMServerPicker e12staticInstance;

		private static LegacyUMServerPicker e14staticInstance;

		private static object lockObj = new object();

		private VersionEnum version;
	}
}
