using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMDialingRuleEntry
	{
		[DataMember]
		public string Comment { get; set; }

		[DataMember]
		public string DialedNumber { get; set; }

		[DataMember]
		public string GroupName { get; set; }

		[DataMember]
		public string NumberMask { get; set; }

		public UMDialingRuleEntry()
		{
		}

		public UMDialingRuleEntry(DialGroupEntry entry)
		{
			this.Comment = ((entry.Comment == null) ? string.Empty : entry.Comment);
			this.DialedNumber = entry.DialedNumber;
			this.GroupName = entry.Name;
			this.NumberMask = entry.NumberMask;
		}

		public static IEnumerable<UMDialingRuleEntry> GetUMDialingRuleEntry(MultiValuedProperty<DialGroupEntry> entries)
		{
			if (entries != null)
			{
				return from entry in entries
				select new UMDialingRuleEntry(entry);
			}
			return null;
		}

		public static string[] GetStringArray(IEnumerable<UMDialingRuleEntry> entries)
		{
			if (entries != null)
			{
				return (from entry in entries
				select new DialGroupEntry(entry.GroupName, entry.NumberMask, entry.DialedNumber, entry.Comment).ToString()).ToArray<string>();
			}
			return null;
		}
	}
}
