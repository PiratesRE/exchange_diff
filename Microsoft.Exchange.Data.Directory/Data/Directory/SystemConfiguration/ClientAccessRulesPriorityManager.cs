using System;
using System.Collections.Generic;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class ClientAccessRulesPriorityManager
	{
		public List<ADClientAccessRule> ADClientAccessRules { get; private set; }

		public ClientAccessRulesPriorityManager(IEnumerable<ADClientAccessRule> adClientAccessRules)
		{
			this.ADClientAccessRules = new List<ADClientAccessRule>(adClientAccessRules);
			this.ADClientAccessRules.Sort(delegate(ADClientAccessRule x, ADClientAccessRule y)
			{
				int num = -x.DatacenterAdminsOnly.CompareTo(y.DatacenterAdminsOnly);
				if (num == 0)
				{
					num = x.InternalPriority.CompareTo(y.InternalPriority);
				}
				if (num == 0)
				{
					num = x.Guid.CompareTo(y.Guid);
				}
				return num;
			});
		}

		public int GetInternalPriority(int wantedPriority, ADClientAccessRule rule, out bool normalized)
		{
			int ruleRelativePosition = this.GetRuleRelativePosition(rule);
			int num = 0;
			if (ruleRelativePosition != -1)
			{
				this.ADClientAccessRules.RemoveAt(ruleRelativePosition);
			}
			return this.GetInternalPriority(wantedPriority, rule.DatacenterAdminsOnly, out num, out normalized);
		}

		public int GetInternalPriority(int wantedPriority, bool isDatacenterAdminsOnly, out int relativePosition, out bool normalized)
		{
			if (wantedPriority > 0 && wantedPriority <= this.ADClientAccessRules.Count)
			{
				return this.CalculateInternalPriority(this.GetCorrectedPriority(wantedPriority, isDatacenterAdminsOnly), out relativePosition, out normalized);
			}
			return this.CalculateInternalPriority(this.GetCorrectedPriority(this.ADClientAccessRules.Count + 1, isDatacenterAdminsOnly), out relativePosition, out normalized);
		}

		private int GetCorrectedPriority(int wantedPriority, bool isDatacenterAdminsOnly)
		{
			if (!isDatacenterAdminsOnly)
			{
				while (wantedPriority <= this.ADClientAccessRules.Count)
				{
					if (!this.ADClientAccessRules[wantedPriority - 1].DatacenterAdminsOnly)
					{
						break;
					}
					wantedPriority++;
				}
			}
			else
			{
				while (wantedPriority > 1 && !this.ADClientAccessRules[wantedPriority - 2].DatacenterAdminsOnly)
				{
					wantedPriority--;
				}
			}
			return wantedPriority;
		}

		private int CalculateInternalPriority(int wantedPriority, out int relativePosition, out bool normalized)
		{
			normalized = false;
			relativePosition = wantedPriority;
			if (this.ADClientAccessRules.Count == 0)
			{
				return 1000000;
			}
			if (wantedPriority > this.ADClientAccessRules.Count)
			{
				int internalPriority = this.ADClientAccessRules[this.ADClientAccessRules.Count - 1].InternalPriority;
				if (internalPriority + 1000000 > 1000000000)
				{
					normalized = true;
					return this.NormalizePriorities(wantedPriority * 1000000);
				}
				return internalPriority + 1000000;
			}
			else
			{
				if (wantedPriority != 1)
				{
					return this.GetMiddlePriority(wantedPriority, out normalized);
				}
				if (this.ADClientAccessRules[0].InternalPriority == 1)
				{
					normalized = true;
					return this.NormalizePriorities(1000000);
				}
				return this.ADClientAccessRules[0].InternalPriority >> 1;
			}
		}

		private int GetMiddlePriority(int wantedPriority, out bool normalized)
		{
			int num = wantedPriority - 2;
			int internalPriority = this.ADClientAccessRules[num].InternalPriority;
			int internalPriority2 = this.ADClientAccessRules[num + 1].InternalPriority;
			normalized = (internalPriority2 - internalPriority < 2);
			if (normalized)
			{
				return this.NormalizePriorities(wantedPriority * 1000000);
			}
			return internalPriority + internalPriority2 >> 1;
		}

		private int NormalizePriorities(int alreadyAssigned)
		{
			int i = 0;
			int num = 1000000;
			while (i < this.ADClientAccessRules.Count)
			{
				if (num == alreadyAssigned)
				{
					num += 1000000;
				}
				this.ADClientAccessRules[i].InternalPriority = num;
				i++;
				num += 1000000;
			}
			return alreadyAssigned;
		}

		private int GetRuleRelativePosition(ADClientAccessRule rule)
		{
			for (int i = 0; i < this.ADClientAccessRules.Count; i++)
			{
				if (this.ADClientAccessRules[i].Identity.Equals(rule.Identity))
				{
					return i;
				}
			}
			return -1;
		}

		private const int priorityStep = 1000000;

		private const int priorityLimit = 1000000000;
	}
}
