using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.Exchange.Monitoring.ActiveMonitoring.Common
{
	public class EventMatchingRule
	{
		public string LogName { get; set; }

		public string ProviderName { get; set; }

		public IEnumerable<int> EventIds { get; set; }

		public int ResourceNameIndex { get; set; }

		public bool EvaluateMessage { get; set; }

		public bool PopulatePropertiesXml { get; set; }

		public EventMatchingRule.CustomMatching OnMatching { get; set; }

		public EventMatchingRule.CustomNotification OnNotify { get; set; }

		public EventMatchingRule(string logName, string providerName, IEnumerable<int> eventIds, int resourceNameIndex = -1, bool evaluateMessage = false, bool populatePropertiesXml = false, EventMatchingRule.CustomMatching onMatching = null, EventMatchingRule.CustomNotification onNotify = null)
		{
			this.LogName = logName;
			this.ProviderName = providerName;
			this.EventIds = eventIds;
			this.ResourceNameIndex = resourceNameIndex;
			this.EvaluateMessage = evaluateMessage;
			this.PopulatePropertiesXml = populatePropertiesXml;
			this.OnMatching = onMatching;
			this.OnNotify = onNotify;
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("LognNme={0}", this.LogName);
			stringBuilder.AppendFormat("ProviderName={0}", this.ProviderName);
			StringBuilder stringBuilder2 = stringBuilder;
			string format = "EventIds={0}";
			object arg;
			if (this.EventIds == null)
			{
				arg = "NULL";
			}
			else
			{
				arg = string.Join(",", from id in this.EventIds
				select id.ToString());
			}
			stringBuilder2.AppendFormat(format, arg);
			stringBuilder.AppendFormat("ResourceNameIndex={0}", this.ResourceNameIndex);
			stringBuilder.AppendFormat("EvalMessage={0}", this.EvaluateMessage);
			stringBuilder.AppendFormat("PopulateProp={0}", this.PopulatePropertiesXml);
			stringBuilder.AppendFormat("OnMatching={0}", (this.OnMatching == null) ? "NULL" : "Defined");
			stringBuilder.AppendFormat("OnNotify={0}", (this.OnNotify == null) ? "NULL" : "Defined");
			return stringBuilder.ToString();
		}

		public const int NoResourceName = -1;

		public const string AllProviders = "*";

		public delegate bool CustomMatching(EventLogNotification.EventRecordInternal record);

		public delegate void CustomNotification(EventLogNotification.EventRecordInternal record, ref EventLogNotification.EventNotificationMetadata enm);
	}
}
