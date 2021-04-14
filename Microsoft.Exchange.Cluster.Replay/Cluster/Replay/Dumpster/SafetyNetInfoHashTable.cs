using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Exchange.Cluster.Replay.Dumpster
{
	internal class SafetyNetInfoHashTable : Dictionary<SafetyNetRequestKey, SafetyNetInfo>
	{
		public SafetyNetInfoHashTable() : base(5, SafetyNetRequestKeyComparer.Instance)
		{
		}

		public bool RedeliveryRequired
		{
			get
			{
				foreach (SafetyNetInfo safetyNetInfo in base.Values)
				{
					if (safetyNetInfo.RedeliveryRequired)
					{
						return true;
					}
				}
				return false;
			}
		}

		public string RedeliveryServers
		{
			get
			{
				HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (SafetyNetInfo safetyNetInfo in base.Values)
				{
					if (safetyNetInfo.RedeliveryRequired)
					{
						List<string> list = safetyNetInfo.PrimaryHubServers;
						if (safetyNetInfo.ShadowHubServers.Count > 0)
						{
							list = safetyNetInfo.ShadowHubServers;
						}
						foreach (string item in list)
						{
							if (!hashSet.Contains(item))
							{
								hashSet.Add(item);
							}
						}
					}
				}
				string[] value = (from server in hashSet
				select server).ToArray<string>();
				return string.Join(",", value);
			}
		}

		public DateTime RedeliveryStartTime
		{
			get
			{
				DateTime dateTime = DateTime.MinValue;
				foreach (SafetyNetInfo safetyNetInfo in base.Values)
				{
					if (safetyNetInfo.RedeliveryRequired)
					{
						if (dateTime.Equals(DateTime.MinValue))
						{
							dateTime = safetyNetInfo.StartTimeUtc;
						}
						else if (dateTime > safetyNetInfo.StartTimeUtc)
						{
							dateTime = safetyNetInfo.StartTimeUtc;
						}
					}
				}
				return dateTime;
			}
		}

		public DateTime RedeliveryEndTime
		{
			get
			{
				DateTime dateTime = DateTime.MinValue;
				foreach (SafetyNetInfo safetyNetInfo in base.Values)
				{
					if (safetyNetInfo.RedeliveryRequired)
					{
						if (dateTime.Equals(DateTime.MinValue))
						{
							dateTime = safetyNetInfo.EndTimeUtc;
						}
						if (dateTime < safetyNetInfo.EndTimeUtc)
						{
							dateTime = safetyNetInfo.EndTimeUtc;
						}
					}
				}
				return dateTime;
			}
		}
	}
}
