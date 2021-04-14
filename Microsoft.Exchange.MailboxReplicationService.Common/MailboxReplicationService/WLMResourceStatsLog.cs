using System;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.MailboxReplicationService
{
	internal class WLMResourceStatsLog : ObjectLog<WLMResourceStatsData>
	{
		private WLMResourceStatsLog() : base(new WLMResourceStatsLog.WLMResourceStatsLogSchema(), new SimpleObjectLogConfiguration("WLMResourceStats", "WLMResourceStatsLogEnabled", "WLMResourceStatsLogMaxDirSize", "WLMResourceStatsLogMaxFileSize"))
		{
		}

		public static void Write(WLMResourceStatsData loggingStatsData)
		{
			WLMResourceStatsLog.instance.LogObject(loggingStatsData);
		}

		public const int MaxDataContextLength = 1000;

		private static WLMResourceStatsLog instance = new WLMResourceStatsLog();

		internal class WLMResourceStatsLogSchema : ObjectLogSchema
		{
			public override string Software
			{
				get
				{
					return "Microsoft Exchange Mailbox Replication Service";
				}
			}

			public override string LogType
			{
				get
				{
					return "WLMResourceStats Log";
				}
			}

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> OwnerResourceName = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("OwnerResourceName", (WLMResourceStatsData d) => d.OwnerResourceName);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> OwnerResourceGuid = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("OwnerResourceGuid", (WLMResourceStatsData d) => d.OwnerResourceGuid);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> OwnerResourceType = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("OwnerResourceType", (WLMResourceStatsData d) => d.OwnerResourceType);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> ResourceKey = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("ResourceKey", (WLMResourceStatsData d) => d.WlmResourceKey);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> LoadState = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("LoadState", (WLMResourceStatsData d) => d.LoadState);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> LoadRatio = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("LoadRatio", (WLMResourceStatsData d) => d.LoadRatio.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> Metric = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("Metric", (WLMResourceStatsData d) => d.Metric);

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> DynamicCapacity = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("DynamicCapacity", (WLMResourceStatsData d) => d.DynamicCapacity.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> TimeInterval = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("TimeInterval", (WLMResourceStatsData d) => d.TimeInterval.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> UnderloadedCount = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("UnderloadedCount", (WLMResourceStatsData d) => d.UnderloadedCount.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> FullCount = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("FullCount", (WLMResourceStatsData d) => d.FullCount.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> OverloadedCount = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("OverloadedCount", (WLMResourceStatsData d) => d.OverloadedCount.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> CriticalCount = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("CriticalCount", (WLMResourceStatsData d) => d.CriticalCount.ToString());

			public static readonly ObjectLogSimplePropertyDefinition<WLMResourceStatsData> UnknownCount = new ObjectLogSimplePropertyDefinition<WLMResourceStatsData>("UnknownCount", (WLMResourceStatsData d) => d.UnknownCount.ToString());
		}
	}
}
