using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExTimeZoneRuleGroup
	{
		public ExTimeZoneRuleGroup(DateTime? endTransition)
		{
			this.endTransition = endTransition;
		}

		public void AddRule(ExTimeZoneRule ruleInfo)
		{
			if (ruleInfo == null)
			{
				throw new ArgumentNullException("ruleInfo");
			}
			this.rules.Add(ruleInfo);
		}

		public IList<ExTimeZoneRule> Rules
		{
			get
			{
				if (this.readOnlyRules == null)
				{
					this.readOnlyRules = new ReadOnlyCollection<ExTimeZoneRule>(this.rules);
				}
				return this.readOnlyRules;
			}
		}

		public override string ToString()
		{
			return string.Format("RuleGroup; rule count={0}; transition = {1}", this.rules.Count, (this.EndTransition != null) ? this.EndTransition.Value.ToString() : "none");
		}

		public DateTime? EndTransition
		{
			get
			{
				return this.endTransition;
			}
		}

		internal ExTimeZone TimeZone
		{
			get
			{
				if (this.timeZoneInfo == null)
				{
					return null;
				}
				return this.timeZoneInfo.TimeZone;
			}
		}

		internal void CalculateEffectiveUtcEnd()
		{
			if (this.EndTransition == null)
			{
				this.effectiveUtcEnd = DateTime.MaxValue;
				return;
			}
			DateTime value = this.EndTransition.Value;
			ExTimeZoneRule exTimeZoneRule = this.Rules[0];
			if (this.Rules.Count > 1)
			{
				int num = value.Year;
				if (value == new DateTime(num, 1, 1))
				{
					num--;
				}
				for (int i = this.Rules.Count - 1; i >= 0; i--)
				{
					DateTime instance = this.rules[i].ObservanceEnd.GetInstance(num);
					if (instance >= value)
					{
						exTimeZoneRule = this.rules[i];
						break;
					}
				}
			}
			this.effectiveUtcEnd = exTimeZoneRule.ToUtc(value);
		}

		internal ExTimeZoneRule GetRuleForUtcTime(DateTime utcDateTime)
		{
			if (this.rules.Count == 1)
			{
				return this.rules[0];
			}
			int? num = null;
			for (int i = 0; i < this.rules.Count; i++)
			{
				ExTimeZoneRule exTimeZoneRule = this.rules[i];
				DateTime t = exTimeZoneRule.FromUtc(utcDateTime);
				if (num == null)
				{
					num = new int?(t.Year);
				}
				DateTime instance = exTimeZoneRule.ObservanceEnd.GetInstance(num.Value);
				if (t < instance)
				{
					return exTimeZoneRule;
				}
			}
			return this.rules[0];
		}

		internal void Validate()
		{
			if (this.rules.Count == 0)
			{
				throw new InvalidTimeZoneException("Empty group");
			}
			this.rules.Sort(RuleComparer.Instance);
			int num = -1;
			foreach (ExTimeZoneRule exTimeZoneRule in this.rules)
			{
				if (exTimeZoneRule.ObservanceEnd != null)
				{
					int sortIndex = exTimeZoneRule.ObservanceEnd.SortIndex;
					if (sortIndex == num)
					{
						throw new InvalidTimeZoneException("Rules are too close");
					}
					num = sortIndex;
				}
				try
				{
					exTimeZoneRule.Validate();
				}
				catch (ArgumentOutOfRangeException innerException)
				{
					throw new InvalidTimeZoneException("Invalid rule", innerException);
				}
				exTimeZoneRule.RuleGroup = this;
			}
			this.CalculateEffectiveUtcEnd();
		}

		public ExTimeZoneInformation TimeZoneInfo
		{
			get
			{
				return this.timeZoneInfo;
			}
			internal set
			{
				this.timeZoneInfo = value;
			}
		}

		internal DateTime EffectiveUtcStart
		{
			get
			{
				return this.effectiveUtcStart;
			}
			set
			{
				this.effectiveUtcStart = value;
			}
		}

		internal DateTime EffectiveUtcEnd
		{
			get
			{
				return this.effectiveUtcEnd;
			}
			set
			{
				this.effectiveUtcEnd = value;
			}
		}

		private DateTime effectiveUtcStart = DateTime.MinValue;

		private DateTime effectiveUtcEnd = DateTime.MaxValue;

		private ExTimeZoneInformation timeZoneInfo;

		private DateTime? endTransition;

		private List<ExTimeZoneRule> rules = new List<ExTimeZoneRule>(2);

		private ReadOnlyCollection<ExTimeZoneRule> readOnlyRules;
	}
}
