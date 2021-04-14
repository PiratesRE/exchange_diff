using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.MailboxReplicationService
{
	[DataContract]
	[Serializable]
	public class ProviderInfo : XMLSerializableBase
	{
		public ProviderInfo()
		{
			this.Durations = new List<DurationInfo>();
		}

		[XmlElement(ElementName = "Durations")]
		[DataMember(Name = "Duration", IsRequired = false)]
		public List<DurationInfo> Durations { get; set; }

		public TimeSpan TotalDuration
		{
			get
			{
				long num = 0L;
				foreach (DurationInfo durationInfo in this.Durations)
				{
					num += durationInfo.DurationTicks;
				}
				return new TimeSpan(num);
			}
		}

		public static ProviderInfo operator +(ProviderInfo providerInfo1, ProviderInfo providerInfo2)
		{
			ProviderInfo providerInfo3 = new ProviderInfo();
			providerInfo3.Durations.AddRange(providerInfo1.Durations);
			Dictionary<string, DurationInfo> dictionary = new Dictionary<string, DurationInfo>();
			for (int i = 0; i < providerInfo1.Durations.Count; i++)
			{
				dictionary.Add(providerInfo1.Durations[i].Name, providerInfo1.Durations[i]);
			}
			SortedList<string, DurationInfo> sortedList = new SortedList<string, DurationInfo>(dictionary);
			foreach (DurationInfo durationInfo in providerInfo2.Durations)
			{
				DurationInfo durationInfo2;
				if (sortedList.TryGetValue(durationInfo.Name, out durationInfo2))
				{
					durationInfo2.Duration += durationInfo.Duration;
				}
				else
				{
					providerInfo3.Durations.Add(durationInfo);
				}
			}
			return providerInfo3;
		}

		public override string ToString()
		{
			return "Total Execution Time: " + this.TotalDuration.ToString();
		}
	}
}
