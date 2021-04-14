using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Common;

namespace Microsoft.Exchange.ExchangeSystem
{
	public sealed class ExTimeZone
	{
		static ExTimeZone()
		{
			ExTimeZoneRuleGroup exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(null);
			exTimeZoneRuleGroup.AddRule(ExTimeZone.UtcTimeZoneRule);
			ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation("tzone://Microsoft/Utc", "UTC");
			exTimeZoneInformation.AddGroup(exTimeZoneRuleGroup);
			ExTimeZone.UtcTimeZone = new ExTimeZone(exTimeZoneInformation);
			ExTimeZone.CurrentTimeZone = ExTimeZone.GetCurrentTimeZone();
			ExTimeZone.UnspecifiedTimeZone = ExTimeZone.BuildUnspecifiedTimeZone();
		}

		public ExTimeZone(ExTimeZoneInformation timeZoneInfo)
		{
			timeZoneInfo.Validate();
			timeZoneInfo.TimeZone = this;
			this.TimeZoneInformation = timeZoneInfo;
		}

		public static TimeSpan MaxBias
		{
			get
			{
				return TimeLibConsts.MaxBias;
			}
		}

		public static ExTimeZone UtcTimeZone { get; private set; }

		public static ExTimeZoneRule UtcTimeZoneRule { get; private set; } = new ExTimeZoneRule("tzrule://Microsoft/UtcRule", "UTC rule", TimeSpan.FromTicks(0L), null);

		public static ExTimeZone CurrentTimeZone { get; private set; }

		public static ExTimeZone UnspecifiedTimeZone { get; private set; }

		public bool IsCustomTimeZone
		{
			get
			{
				return this.Id == "tzone://Microsoft/Custom";
			}
		}

		public string Id
		{
			get
			{
				return this.TimeZoneInformation.Id;
			}
		}

		public LocalizedString LocalizableDisplayName
		{
			get
			{
				return this.TimeZoneInformation.LocalizedDisplayName;
			}
		}

		public string DisplayName
		{
			get
			{
				return this.TimeZoneInformation.DisplayName;
			}
		}

		public ExTimeZoneInformation TimeZoneInformation { get; private set; }

		internal string AlternativeId
		{
			get
			{
				if (!this.IsCustomTimeZone)
				{
					return this.Id;
				}
				return this.TimeZoneInformation.AlternativeId;
			}
		}

		public static ExTimeZone TimeZoneFromKind(DateTimeKind kind)
		{
			switch (kind)
			{
			case DateTimeKind.Utc:
				return ExTimeZone.UtcTimeZone;
			case DateTimeKind.Local:
				return ExTimeZone.CurrentTimeZone;
			}
			return ExTimeZone.UnspecifiedTimeZone;
		}

		public DaylightTime GetDaylightChanges(int year)
		{
			ExDateTime exDateTime = new ExDateTime(this, year, 1, 1);
			ExTimeZoneRule ruleForUtcTime = this.TimeZoneInformation.GetRuleForUtcTime(exDateTime.UniversalTime);
			if (ruleForUtcTime == null)
			{
				throw new InvalidTimeZoneException("no rule covers year: " + year);
			}
			if (ruleForUtcTime.RuleGroup.Rules.Count <= 1)
			{
				return new DaylightTime(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero);
			}
			ExTimeZoneRule exTimeZoneRule = null;
			ExTimeZoneRule exTimeZoneRule2 = null;
			foreach (ExTimeZoneRule exTimeZoneRule3 in ruleForUtcTime.RuleGroup.Rules)
			{
				if (exTimeZoneRule3.Bias == this.TimeZoneInformation.StandardBias)
				{
					exTimeZoneRule = exTimeZoneRule3;
				}
				else
				{
					exTimeZoneRule2 = exTimeZoneRule3;
				}
			}
			if (exTimeZoneRule == null || exTimeZoneRule2 == null)
			{
				ExTraceGlobals.CommonTracer.TraceError((long)this.GetHashCode(), "No different Bias for standard time and daylight saving time. Treat it as No DST.");
				return new DaylightTime(DateTime.MinValue, DateTime.MinValue, TimeSpan.Zero);
			}
			DateTime instance = exTimeZoneRule.ObservanceEnd.GetInstance(year);
			DateTime instance2 = exTimeZoneRule2.ObservanceEnd.GetInstance(year);
			TimeSpan timeSpan = exTimeZoneRule2.Bias - exTimeZoneRule.Bias;
			if (timeSpan < TimeSpan.Zero)
			{
				ExTraceGlobals.CommonTracer.TraceError((long)this.GetHashCode(), "Rare time zone rule found, the DST bias is less than standard bias.");
			}
			return new DaylightTime(instance, instance2, timeSpan);
		}

		public bool IsDaylightSavingTime(ExDateTime dateTime)
		{
			return dateTime.Bias != this.TimeZoneInformation.StandardBias;
		}

		public ExDateTime ConvertDateTime(ExDateTime exDateTime)
		{
			if (!exDateTime.HasTimeZone)
			{
				ExTimeZoneHelperForMigrationOnly.CheckValidationLevel(false, ExTimeZoneHelperForMigrationOnly.ValidationLevel.VeryHigh, "ConvertDateTime: UnspecifiedTimeZone: UnspecifiedTimeZone", new object[0]);
				return new ExDateTime(this, exDateTime.LocalTime);
			}
			return new ExDateTime(this, exDateTime.UniversalTime, null);
		}

		public ExDateTime Assign(ExDateTime exDateTime)
		{
			ExTimeZoneHelperForMigrationOnly.CheckValidationLevel<string>(!exDateTime.HasTimeZone, ExTimeZoneHelperForMigrationOnly.ValidationLevel.VeryHigh, "ExTimeZone.Assign:. ExDateTime alreayd has time zone: {0}.", exDateTime.TimeZone.Id);
			if (exDateTime.LocalTime <= TimeLibConsts.MinSystemDateTimeValue)
			{
				return this.ConvertDateTime(ExDateTime.MinValue);
			}
			if (exDateTime.LocalTime >= TimeLibConsts.MaxSystemDateTimeValue)
			{
				return this.ConvertDateTime(ExDateTime.MaxValue);
			}
			return new ExDateTime(this, exDateTime.LocalTime);
		}

		public override string ToString()
		{
			return string.Format("Time zone: Id={0}; DisplayName={1}", this.Id, this.DisplayName);
		}

		internal TimeSpan GetBiasForUtcTime(DateTime utcDateTime)
		{
			return this.TimeZoneInformation.GetRuleForUtcTime(utcDateTime).Bias;
		}

		internal IEnumerable<TimeSpan> GetBiasesForLocalTime(DateTime dateTime)
		{
			foreach (ExTimeZoneRule rule in this.TimeZoneInformation.GetRulesForLocalTime(dateTime))
			{
				yield return rule.Bias;
			}
			yield break;
		}

		private static ExTimeZone GetCurrentTimeZone()
		{
			ExTimeZone exTimeZone = null;
			string currentTimeZoneName = ExRegistryReader.GetCurrentTimeZoneName();
			if (!string.IsNullOrEmpty(currentTimeZoneName))
			{
				ExTimeZoneEnumerator.Instance.TryGetTimeZoneByName(currentTimeZoneName, out exTimeZone);
			}
			if (exTimeZone == null)
			{
				string currentTimeZoneMuiStandardName = ExRegistryReader.GetCurrentTimeZoneMuiStandardName();
				if (!string.IsNullOrEmpty(currentTimeZoneMuiStandardName))
				{
					ExTraceGlobals.CommonTracer.TraceInformation<string, string>(0, 0L, "Current time zone name '{0}' from registry is invalid, fallback to use MUI standard name '{1}' to look up system time zone list.", currentTimeZoneName, currentTimeZoneMuiStandardName);
					foreach (ExTimeZone exTimeZone2 in ExTimeZoneEnumerator.Instance)
					{
						if (currentTimeZoneMuiStandardName.Equals(exTimeZone2.TimeZoneInformation.MuiStandardName, StringComparison.OrdinalIgnoreCase))
						{
							exTimeZone = exTimeZone2;
							break;
						}
					}
				}
			}
			if (exTimeZone == null)
			{
				ExTraceGlobals.CommonTracer.TraceInformation(0, 0L, "Unable to get current time zone according to registry, UTC time zone is being used instead.");
				exTimeZone = ExTimeZone.UtcTimeZone;
			}
			return exTimeZone;
		}

		private static ExTimeZone BuildUnspecifiedTimeZone()
		{
			string id = "tzrule://Microsoft/UnspecifiedRule";
			string displayName = "Unspecified time zone rule";
			string timeZoneId = "tzone://Microsoft/Unspecified";
			string displayName2 = "UnspecifiedTimeZone time zone";
			ExTimeZoneRule ruleInfo = new ExTimeZoneRule(id, displayName, TimeSpan.FromTicks(0L), null);
			ExTimeZoneRuleGroup exTimeZoneRuleGroup = new ExTimeZoneRuleGroup(null);
			exTimeZoneRuleGroup.AddRule(ruleInfo);
			ExTimeZoneInformation exTimeZoneInformation = new ExTimeZoneInformation(timeZoneId, displayName2);
			exTimeZoneInformation.AddGroup(exTimeZoneRuleGroup);
			return new ExTimeZone(exTimeZoneInformation);
		}

		public const string CustomTimeZoneId = "tzone://Microsoft/Custom";

		public const string CustomTimeZoneName = "Customized Time Zone";

		public const string UtcRuleId = "tzrule://Microsoft/UtcRule";

		public const string UtcRuleName = "UTC rule";

		public const string UtcZoneId = "tzone://Microsoft/Utc";

		public const string UtcZoneName = "UTC";
	}
}
