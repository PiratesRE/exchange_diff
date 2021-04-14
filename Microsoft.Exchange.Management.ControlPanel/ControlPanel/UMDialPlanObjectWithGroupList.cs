using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMDialPlanObjectWithGroupList : BaseRow
	{
		public UMDialPlanObjectWithGroupList(UMDialPlan dialPlan) : base(dialPlan)
		{
			this.ConfiguredInCountryOrRegionGroupNameList = this.GetDialingRuleGroupNameList(dialPlan.ConfiguredInCountryOrRegionGroups);
			this.ConfiguredInternationalGroupNameList = this.GetDialingRuleGroupNameList(dialPlan.ConfiguredInternationalGroups);
		}

		public IEnumerable<string> ConfiguredInCountryOrRegionGroupNameList { get; private set; }

		public IEnumerable<string> ConfiguredInternationalGroupNameList { get; private set; }

		private IEnumerable<string> GetDialingRuleGroupNameList(MultiValuedProperty<DialGroupEntry> dialingRuleGroups)
		{
			List<string> list = new List<string>(dialingRuleGroups.Count);
			foreach (DialGroupEntry dialGroupEntry in dialingRuleGroups)
			{
				if (!list.Contains(dialGroupEntry.Name))
				{
					list.Add(dialGroupEntry.Name);
				}
			}
			list.Sort(StringComparer.OrdinalIgnoreCase);
			return list;
		}
	}
}
