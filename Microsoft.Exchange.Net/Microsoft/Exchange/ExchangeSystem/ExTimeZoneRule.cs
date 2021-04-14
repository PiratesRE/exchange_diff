using System;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExTimeZoneRule
	{
		public ExTimeZoneRule(string id, string displayName, TimeSpan bias, ExYearlyRecurringTime observanceEnd)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
			if (displayName == null)
			{
				throw new ArgumentNullException("displayName");
			}
			if (id == string.Empty)
			{
				throw new ArgumentException("id");
			}
			if (displayName == string.Empty)
			{
				throw new ArgumentException("displayName");
			}
			if (Math.Abs(bias.Ticks) > TimeLibConsts.MaxBias.Ticks)
			{
				throw new ArgumentOutOfRangeException("bias");
			}
			this.Id = id;
			this.DisplayName = displayName;
			this.Bias = bias;
			this.ObservanceEnd = observanceEnd;
		}

		public ExYearlyRecurringTime ObservanceEnd
		{
			get
			{
				return this.observanceEnd;
			}
			internal set
			{
				this.observanceEnd = value;
			}
		}

		public override string ToString()
		{
			return string.Format("Rule: Id={0}; DisplayName={1}", this.Id, this.DisplayName);
		}

		internal void Validate()
		{
			if (this.observanceEnd != null)
			{
				this.observanceEnd.Validate();
			}
		}

		internal DateTime ToUtc(DateTime dateTime)
		{
			if (dateTime == DateTime.MinValue)
			{
				return TimeLibConsts.MinSystemDateTimeValue;
			}
			if (dateTime == DateTime.MaxValue)
			{
				return TimeLibConsts.MaxSystemDateTimeValue;
			}
			return DateTime.SpecifyKind(dateTime.Subtract(this.Bias), DateTimeKind.Utc);
		}

		internal DateTime FromUtc(DateTime dateTime)
		{
			return DateTime.SpecifyKind(dateTime.Add(this.Bias), DateTimeKind.Unspecified);
		}

		internal ExTimeZoneRuleGroup RuleGroup
		{
			get
			{
				return this.ruleGroup;
			}
			set
			{
				if (this.ruleGroup != null)
				{
					throw new InvalidOperationException("Cannot change rule set");
				}
				this.ruleGroup = value;
			}
		}

		internal ExTimeZone TimeZone
		{
			get
			{
				return this.ruleGroup.TimeZone;
			}
		}

		public readonly string Id;

		public readonly string DisplayName;

		public readonly TimeSpan Bias;

		private ExYearlyRecurringTime observanceEnd;

		private ExTimeZoneRuleGroup ruleGroup;
	}
}
