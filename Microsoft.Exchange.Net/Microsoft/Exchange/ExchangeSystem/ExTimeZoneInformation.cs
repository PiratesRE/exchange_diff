using System;
using System.Collections.Generic;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExTimeZoneInformation
	{
		public ExTimeZoneInformation(string timeZoneId, string displayName) : this(timeZoneId, displayName, new LocalizedString(displayName))
		{
		}

		public ExTimeZoneInformation(string timeZoneId, string displayName, LocalizedString localizedDisplayName) : this(timeZoneId, displayName, localizedDisplayName, string.Empty)
		{
		}

		internal ExTimeZoneInformation(string timeZoneId, string displayName, LocalizedString localizedDisplayName, string muiStandardName)
		{
			if (timeZoneId == null)
			{
				throw new InvalidTimeZoneException("timeZoneId = null");
			}
			if (displayName == null)
			{
				throw new InvalidTimeZoneException("displayName = null");
			}
			if (timeZoneId.Length == 0)
			{
				throw new InvalidTimeZoneException("timeZoneId.Length = 0");
			}
			this.Id = timeZoneId;
			this.DisplayName = displayName;
			this.LocalizedDisplayName = (localizedDisplayName.IsEmpty ? new LocalizedString(this.DisplayName) : localizedDisplayName);
			this.MuiStandardName = muiStandardName;
		}

		public void AddGroup(ExTimeZoneRuleGroup group)
		{
			if (group == null)
			{
				throw new ArgumentNullException("group");
			}
			this.groups.Add(group);
		}

		public IList<ExTimeZoneRuleGroup> Groups
		{
			get
			{
				return this.ReadOnlyGroups;
			}
		}

		private ReadOnlyCollection<ExTimeZoneRuleGroup> ReadOnlyGroups
		{
			get
			{
				if (this.readOnlyGroups == null)
				{
					this.readOnlyGroups = new ReadOnlyCollection<ExTimeZoneRuleGroup>(this.groups);
				}
				return this.readOnlyGroups;
			}
		}

		public IList<TimeSpan> Biases
		{
			get
			{
				return this.biases;
			}
		}

		internal void Validate()
		{
			if (this.groups.Count == 0)
			{
				throw new InvalidTimeZoneException("No rules");
			}
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup in this.groups)
			{
				exTimeZoneRuleGroup.Validate();
				exTimeZoneRuleGroup.TimeZoneInfo = this;
			}
			this.groups.Sort(GroupComparer.Instance);
			if (this.groups[this.groups.Count - 1].EndTransition != null)
			{
				throw new InvalidTimeZoneException("Last group should have no transition");
			}
			for (int i = 0; i < this.groups.Count - 1; i++)
			{
				ExTimeZoneRuleGroup exTimeZoneRuleGroup2 = this.groups[i];
				if (exTimeZoneRuleGroup2.EndTransition == null)
				{
					throw new InvalidTimeZoneException("Only last group is allowed to have no transition");
				}
				DateTime value = exTimeZoneRuleGroup2.EndTransition.Value;
				ExTimeZoneRuleGroup exTimeZoneRuleGroup3 = this.groups[i + 1];
				exTimeZoneRuleGroup3.EffectiveUtcStart = exTimeZoneRuleGroup2.EffectiveUtcEnd;
			}
			this.biases = new List<TimeSpan>();
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup4 in this.groups)
			{
				foreach (ExTimeZoneRule exTimeZoneRule in exTimeZoneRuleGroup4.Rules)
				{
					int num = this.biases.IndexOf(exTimeZoneRule.Bias);
					if (num < 0)
					{
						this.biases.Add(exTimeZoneRule.Bias);
					}
				}
			}
			this.biases.Sort();
			this.biases.Reverse();
		}

		internal TimeSpan StandardBias
		{
			get
			{
				return this.biases[this.biases.Count - 1];
			}
		}

		internal ExTimeZone TimeZone
		{
			get
			{
				return this.timeZone;
			}
			set
			{
				if (this.timeZone != null)
				{
					throw new InvalidOperationException("Cannot change time zone in rule set");
				}
				this.timeZone = value;
			}
		}

		internal ExTimeZoneRule GetRuleForUtcTime(DateTime utcDateTime)
		{
			ExTimeZoneRule exTimeZoneRule = null;
			foreach (ExTimeZoneRuleGroup exTimeZoneRuleGroup in this.ReadOnlyGroups)
			{
				if (exTimeZoneRuleGroup.EffectiveUtcStart <= utcDateTime && utcDateTime < exTimeZoneRuleGroup.EffectiveUtcEnd)
				{
					exTimeZoneRule = exTimeZoneRuleGroup.GetRuleForUtcTime(utcDateTime);
					break;
				}
			}
			if (exTimeZoneRule == null)
			{
				throw new InvalidOperationException(string.Format("Time zone {0} failed to find a rule for an UTC value", this.Id));
			}
			return exTimeZoneRule;
		}

		internal IEnumerable<ExTimeZoneRule> GetRulesForLocalTime(DateTime dateTime)
		{
			foreach (TimeSpan bias in this.biases)
			{
				DateTime utcCandidate = dateTime.Add(-bias);
				ExTimeZoneRule rule = this.GetRuleForUtcTime(utcCandidate);
				if (rule.FromUtc(utcCandidate) == dateTime)
				{
					yield return rule;
				}
			}
			yield break;
		}

		internal bool FindLeastBiasForLocalTime(DateTime dateTime, out TimeSpan bestBias)
		{
			bool flag = false;
			bestBias = TimeSpan.MinValue;
			foreach (TimeSpan t in this.biases)
			{
				DateTime dateTime2 = dateTime.Add(-t);
				ExTimeZoneRule ruleForUtcTime = this.GetRuleForUtcTime(dateTime2);
				if (ruleForUtcTime.FromUtc(dateTime2) == dateTime)
				{
					if (!flag)
					{
						flag = true;
						bestBias = ruleForUtcTime.Bias;
					}
					else if (ruleForUtcTime.Bias > bestBias)
					{
						bestBias = ruleForUtcTime.Bias;
					}
				}
			}
			return flag;
		}

		public readonly string Id;

		public readonly string DisplayName;

		public readonly LocalizedString LocalizedDisplayName;

		internal string AlternativeId;

		internal readonly string MuiStandardName;

		private readonly List<ExTimeZoneRuleGroup> groups = new List<ExTimeZoneRuleGroup>(2);

		private ReadOnlyCollection<ExTimeZoneRuleGroup> readOnlyGroups;

		private ExTimeZone timeZone;

		private List<TimeSpan> biases;
	}
}
