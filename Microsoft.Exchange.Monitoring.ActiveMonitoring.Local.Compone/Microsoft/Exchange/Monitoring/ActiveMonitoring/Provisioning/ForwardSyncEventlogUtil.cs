using System;
using System.Diagnostics.Eventing.Reader;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Provisioning
{
	public class ForwardSyncEventlogUtil
	{
		public static FowardSyncEventRecord GetArbitrationEventLog()
		{
			string query = string.Format("<QueryList>  <Query Id=\"0\" Path=\"{0}\">    <Select Path=\"{0}\">        *[System[Provider[@Name='{1}'] and        (EventID={2}) and        TimeCreated[timediff(@SystemTime) &lt;= {3}]]]    </Select>  </Query></QueryList>", new object[]
			{
				"ForwardSync",
				"MSExchangeForwardSync",
				5015,
				2400000
			});
			FowardSyncEventRecord fowardSyncEventRecord = null;
			using (EventLogReader eventLogReader = new EventLogReader(new EventLogQuery("ForwardSync", PathType.LogName, query)
			{
				ReverseDirection = true
			}))
			{
				using (EventRecord eventRecord = eventLogReader.ReadEvent())
				{
					if (eventRecord != null)
					{
						fowardSyncEventRecord = new FowardSyncEventRecord();
						fowardSyncEventRecord.ServiceInstanceName = eventRecord.Properties[0].Value.ToString();
						fowardSyncEventRecord.Status = "Active";
						fowardSyncEventRecord.TimeCreated = eventRecord.TimeCreated;
					}
				}
			}
			return fowardSyncEventRecord;
		}

		public static string GetServiceInstancename()
		{
			string result = string.Empty;
			FowardSyncEventRecord arbitrationEventLog = ForwardSyncEventlogUtil.GetArbitrationEventLog();
			if (arbitrationEventLog != null)
			{
				result = arbitrationEventLog.ServiceInstanceName;
			}
			return result;
		}

		public static bool IsForwardSyncActiveServer()
		{
			return !string.IsNullOrEmpty(ForwardSyncEventlogUtil.GetServiceInstancename());
		}

		private const string LogName = "ForwardSync";

		private const string EventSource = "MSExchangeForwardSync";

		private const int EventId = 5015;

		private const int EventIntervalMilliSeconds = 2400000;

		private const string QueryStringFormat = "<QueryList>  <Query Id=\"0\" Path=\"{0}\">    <Select Path=\"{0}\">        *[System[Provider[@Name='{1}'] and        (EventID={2}) and        TimeCreated[timediff(@SystemTime) &lt;= {3}]]]    </Select>  </Query></QueryList>";
	}
}
