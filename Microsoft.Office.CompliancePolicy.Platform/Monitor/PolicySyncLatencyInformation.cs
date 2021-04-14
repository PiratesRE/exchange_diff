using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Office.CompliancePolicy.PolicyConfiguration;

namespace Microsoft.Office.CompliancePolicy.Monitor
{
	internal class PolicySyncLatencyInformation
	{
		public PolicySyncLatencyInformation(ConfigurationObjectType objectType, int count) : this(objectType, count, null)
		{
		}

		internal PolicySyncLatencyInformation(ConfigurationObjectType objectType, int count, Func<TimeSpan, TimeSpan> getLatencyValueDelegate)
		{
			this.Latencies = new Dictionary<LatencyType, TimeSpan>();
			this.ObjectType = objectType;
			this.Count = count;
			this.getLatencyValueDelegate = getLatencyValueDelegate;
		}

		public ConfigurationObjectType ObjectType { get; set; }

		public int Count { get; set; }

		public Dictionary<LatencyType, TimeSpan> Latencies { get; private set; }

		public override string ToString()
		{
			ArgumentValidator.ThrowIfCollectionNullOrEmpty<KeyValuePair<LatencyType, TimeSpan>>("Latencies", this.Latencies);
			string[] value = (from entry in this.Latencies
			select string.Format("{0}={1}", entry.Key, (int)this.GetLatencyValue(entry.Value).TotalSeconds)).ToArray<string>();
			return string.Format("Object={0},Count={1},{2};", this.ObjectType, this.Count, string.Join(",", value));
		}

		private TimeSpan GetLatencyValue(TimeSpan latency)
		{
			if (this.getLatencyValueDelegate == null)
			{
				return latency;
			}
			return this.getLatencyValueDelegate(latency);
		}

		private readonly Func<TimeSpan, TimeSpan> getLatencyValueDelegate;
	}
}
