using System;
using System.Collections.Generic;
using Microsoft.Exchange.Cluster.Replay;
using Microsoft.Exchange.Cluster.ReplayEventLog;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Cluster.Replay;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Monitoring
{
	internal static class ReplicationEventLookupTable
	{
		static ReplicationEventLookupTable()
		{
			ReplicationEventLookupTable.PopulateMomTable();
			ReplicationEventLookupTable.PopulateChecksTable();
		}

		public static bool TryGetReplicationEventInfo(int momEventId, out MomEventInfo eventInfo)
		{
			eventInfo = null;
			if (ReplicationEventLookupTable.s_momEventsTable.ContainsKey(momEventId))
			{
				eventInfo = ReplicationEventLookupTable.s_momEventsTable[momEventId];
				return true;
			}
			return false;
		}

		public static bool TryGetReplicationEventInfo(CheckId checkId, ReplicationCheckResultEnum result, out ReplicationEventBaseInfo eventInfo)
		{
			eventInfo = null;
			if (ReplicationEventLookupTable.s_checksEventTable.ContainsKey((int)checkId))
			{
				if (ReplicationEventLookupTable.s_checksEventTable[(int)checkId].ContainsKey((int)result))
				{
					eventInfo = ReplicationEventLookupTable.s_checksEventTable[(int)checkId][(int)result];
					return true;
				}
				DiagCore.RetailAssert(false, "Missing event entry for CheckId {0} and result {1}.", new object[]
				{
					checkId,
					result
				});
			}
			return false;
		}

		private static void PopulateMomTable()
		{
			ReplicationEventLookupTable.s_momEventsTable = new Dictionary<int, MomEventInfo>();
			ExTraceGlobals.HealthChecksTracer.TraceDebug((long)ReplicationEventLookupTable.s_momEventsTable.GetHashCode(), "ReplicationEventManager(): Entering static constructor to populate the s_momEventsTable table.");
			ReplicationEventLookupTable.s_momEventsTable.Add(10000, new MomEventInfo(10000, EventTypeEnumeration.Success, false, null));
			ReplicationEventLookupTable.s_momEventsTable.Add(10001, new MomEventInfo(10001, EventTypeEnumeration.Error, false, null));
			ReplicationEventLookupTable.s_momEventsTable.Add(10002, new MomEventInfo(10002, EventTypeEnumeration.Error, false, null));
			ReplicationEventLookupTable.s_momEventsTable.Add(10003, new MomEventInfo(10003, EventTypeEnumeration.Error, false, null));
			ReplicationEventLookupTable.s_momEventsTable.Add(10010, new MomEventInfo(10010, EventTypeEnumeration.Error, false, null));
			ReplicationEventLookupTable.s_momEventsTable.Add(10011, new MomEventInfo(10011, EventTypeEnumeration.Error, false, null));
		}

		private static void PopulateChecksTable()
		{
			ReplicationEventLookupTable.s_checksEventTable = new Dictionary<int, Dictionary<int, ReplicationEventBaseInfo>>();
			ExTraceGlobals.HealthChecksTracer.TraceDebug((long)ReplicationEventLookupTable.s_checksEventTable.GetHashCode(), "ReplicationEventLookupTable(): Entering static constructor to populate the s_checksEventTable table.");
			int capacity = 3;
			int key = 1000;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			Dictionary<int, ReplicationEventBaseInfo> dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringClusterServiceCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringClusterServiceCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1001;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringActiveManagerCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringActiveManagerCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1002;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10008, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.MediumPriorityChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10009, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.MediumPriorityChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1003;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringReplayServiceCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringReplayServiceCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1004;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringDagMembersUpCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringDagMembersUpCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1005;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10008, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.MediumPriorityChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10009, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.MediumPriorityChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1006;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringClusterNetworkCheckFailed));
			dictionary.Add(2, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringClusterNetworkCheckWarning));
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringClusterNetworkCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1007;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringFileShareQuorumCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringFileShareQuorumCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1008;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringQuorumGroupCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringQuorumGroupCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1009;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1010;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1011;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1012;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1013;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1014;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1015;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTasksRpcListenerCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTasksRpcListenerCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1016;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTcpListenerCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTcpListenerCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1017;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTPRListenerCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringTPRListenerCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1018;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1019;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1020;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new MomEventInfo(10006, EventTypeEnumeration.Error, true, new LocalizedString?(Strings.DatabaseChecksFailed)));
			dictionary.Add(2, new MomEventInfo(10007, EventTypeEnumeration.Warning, true, new LocalizedString?(Strings.DatabaseChecksWarning)));
			dictionary.Add(1, new NullEventInfo());
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1021;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringServerLocatorServiceCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringServerLocatorServiceCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
			key = 1022;
			ReplicationEventLookupTable.s_checksEventTable.Add(key, null);
			dictionary = new Dictionary<int, ReplicationEventBaseInfo>(capacity);
			dictionary.Add(3, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringMonitoringServiceCheckFailed));
			dictionary.Add(2, new NullEventInfo());
			dictionary.Add(1, new ApplicationEventInfo(ReplayEventLogConstants.Tuple_MonitoringMonitoringServiceCheckPassed));
			ReplicationEventLookupTable.s_checksEventTable[key] = dictionary;
		}

		private static Dictionary<int, Dictionary<int, ReplicationEventBaseInfo>> s_checksEventTable;

		private static Dictionary<int, MomEventInfo> s_momEventsTable;
	}
}
