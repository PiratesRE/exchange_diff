using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Exchange.Data.ContentTypes.iCalendar;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Win32;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class VTimeZone : CalendarComponentBase
	{
		internal VTimeZone(CalendarComponentBase root) : base(root)
		{
			this.standardRules = new Dictionary<ushort, TimeZoneRule>();
			this.daylightRules = new Dictionary<ushort, TimeZoneRule>();
		}

		internal VTimeZone(CalendarComponentBase root, REG_TIMEZONE_INFO tzInfo, string timeZoneId) : base(root)
		{
			this.timeZoneInfo = tzInfo;
			this.timeZoneId = timeZoneId;
		}

		internal void Demote()
		{
			TimeSpan transitionBiasTimeSpan = VTimeZone.GetTransitionBiasTimeSpan(this.timeZoneInfo.Bias, this.timeZoneInfo.StandardBias);
			TimeSpan transitionBiasTimeSpan2 = VTimeZone.GetTransitionBiasTimeSpan(this.timeZoneInfo.Bias, this.timeZoneInfo.DaylightBias);
			Recurrence recurrence = null;
			Recurrence recurrence2 = null;
			ExDateTime exDateTime;
			VTimeZone.GetStartAndRuleFromTimeZoneTransition(this.timeZoneInfo.StandardDate, out exDateTime, out recurrence);
			ExDateTime exDateTime2;
			VTimeZone.GetStartAndRuleFromTimeZoneTransition(this.timeZoneInfo.DaylightDate, out exDateTime2, out recurrence2);
			base.OutboundContext.Writer.StartComponent(ComponentId.VTimeZone);
			base.OutboundContext.Writer.WriteProperty(PropertyId.TimeZoneId, this.timeZoneId);
			base.OutboundContext.Writer.StartComponent(ComponentId.Standard);
			base.OutboundContext.Writer.StartProperty(PropertyId.DateTimeStart);
			base.OutboundContext.Writer.WritePropertyValue((DateTime)exDateTime);
			base.OutboundContext.Writer.StartProperty(PropertyId.TimeZoneOffsetFrom);
			base.OutboundContext.Writer.WritePropertyValue(transitionBiasTimeSpan2);
			base.OutboundContext.Writer.StartProperty(PropertyId.TimeZoneOffsetTo);
			base.OutboundContext.Writer.WritePropertyValue(transitionBiasTimeSpan);
			if (recurrence != null)
			{
				base.OutboundContext.Writer.StartProperty(PropertyId.RecurrenceRule);
				base.OutboundContext.Writer.WritePropertyValue(recurrence);
			}
			base.OutboundContext.Writer.EndComponent();
			base.OutboundContext.Writer.StartComponent(ComponentId.Daylight);
			base.OutboundContext.Writer.StartProperty(PropertyId.DateTimeStart);
			base.OutboundContext.Writer.WritePropertyValue((DateTime)exDateTime2);
			base.OutboundContext.Writer.StartProperty(PropertyId.TimeZoneOffsetFrom);
			base.OutboundContext.Writer.WritePropertyValue(transitionBiasTimeSpan);
			base.OutboundContext.Writer.StartProperty(PropertyId.TimeZoneOffsetTo);
			base.OutboundContext.Writer.WritePropertyValue(transitionBiasTimeSpan2);
			if (recurrence2 != null)
			{
				base.OutboundContext.Writer.StartProperty(PropertyId.RecurrenceRule);
				base.OutboundContext.Writer.WritePropertyValue(recurrence2);
			}
			base.OutboundContext.Writer.EndComponent();
			base.OutboundContext.Writer.EndComponent();
		}

		internal ExTimeZone Promote()
		{
			ExTimeZone result = null;
			TimeZoneRule timeZoneRule = null;
			TimeZoneRule timeZoneRule2 = null;
			SortedSet<ushort> sortedSet = new SortedSet<ushort>();
			VTimeZone.CollectAllRuleYears(this.standardRules, sortedSet);
			VTimeZone.CollectAllRuleYears(this.daylightRules, sortedSet);
			List<RegistryTimeZoneRule> list = new List<RegistryTimeZoneRule>(sortedSet.Count);
			foreach (ushort num in sortedSet)
			{
				TimeZoneRule timeZoneRule3 = VTimeZone.UpdateAndGetCurrentRule(num, this.daylightRules, ref timeZoneRule2);
				TimeZoneRule timeZoneRule4 = VTimeZone.UpdateAndGetCurrentRule(num, this.standardRules, ref timeZoneRule);
				if (timeZoneRule3 != null || timeZoneRule4 != null)
				{
					list.Add(new RegistryTimeZoneRule((int)num, this.GetNativeTimeZoneRule(timeZoneRule4, timeZoneRule3)));
				}
			}
			try
			{
				result = TimeZoneHelper.CreateCustomExTimeZoneFromRegRules(list[0].RegTimeZoneInfo, this.timeZoneId, this.timeZoneId, list);
			}
			catch (InvalidTimeZoneException ex)
			{
				ExTraceGlobals.ICalTracer.TraceDebug<string>((long)this.GetHashCode(), "ToExTimeZone::ToExTimeZone. Following error found when construct customized time zone: {0}", ex.Message);
				base.Context.AddError(ServerStrings.InvalidICalElement("VTIMEZONE"));
			}
			return result;
		}

		protected override void ProcessProperty(CalendarPropertyBase calendarProperty)
		{
			PropertyId propertyId = calendarProperty.CalendarPropertyId.PropertyId;
			if (propertyId != PropertyId.TimeZoneId)
			{
				return;
			}
			this.timeZoneId = CalendarUtil.RemoveDoubleQuotes((string)calendarProperty.Value);
		}

		protected override bool ProcessSubComponent(CalendarComponentBase calendarComponent)
		{
			bool result = true;
			TimeZoneRule timeZoneRule = calendarComponent as TimeZoneRule;
			if (timeZoneRule != null)
			{
				ushort year = timeZoneRule.Year;
				ComponentId componentId = calendarComponent.ComponentId;
				if (componentId != ComponentId.Standard)
				{
					if (componentId == ComponentId.Daylight)
					{
						if (this.daylightRules.ContainsKey(year))
						{
							ExTraceGlobals.ICalTracer.TraceError<ushort>(0L, "VTimeZone::ProcessSubComponent:ComponentId.Daylight. Ignoring the repeated year timezone definition. Year: {0}", year);
						}
						else
						{
							this.daylightRules.Add(year, timeZoneRule);
						}
					}
				}
				else if (this.standardRules.ContainsKey(year))
				{
					ExTraceGlobals.ICalTracer.TraceError<ushort>(0L, "VTimeZone::ProcessSubComponent:ComponentId.Standard. Ignoring the repeated year timezone definition. Year: {0}", year);
				}
				else
				{
					this.standardRules.Add(year, timeZoneRule);
				}
			}
			return result;
		}

		protected override bool ValidateProperties()
		{
			if (string.IsNullOrEmpty(this.timeZoneId))
			{
				return false;
			}
			if (this.standardRules.Count == 0 && this.daylightRules.Count == 0)
			{
				return false;
			}
			foreach (TimeZoneRule timeZoneRule in this.standardRules.Values.Union(this.daylightRules.Values))
			{
				if (!timeZoneRule.Validate())
				{
					return false;
				}
			}
			return true;
		}

		private static TimeSpan GetTransitionBiasTimeSpan(int bias, int transitionBias)
		{
			TimeSpan t = new TimeSpan(600000000L * (long)(bias + transitionBias));
			if (t == TimeSpan.MinValue)
			{
				ExTraceGlobals.ICalTracer.TraceError<int, int>(0L, "VTimeZone::GetTransitionBiasTimeSpan. The timezone is invalid.\r\nBias: {0}, TransitionBias: {1}", bias, transitionBias);
				throw new CorruptDataException(ServerStrings.ExCorruptedTimeZone);
			}
			return t.Negate();
		}

		private static void GetStartAndRuleFromTimeZoneTransition(NativeMethods.SystemTime systemTime, out ExDateTime start, out Recurrence rule)
		{
			rule = null;
			int num;
			int num2;
			int num3;
			if (systemTime.Year == 0)
			{
				if (systemTime.Month != 0)
				{
					rule = VTimeZone.CreateTimeZoneRRule(systemTime);
				}
				DateTime minSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MinSupportedDateTime;
				DateTime maxSupportedDateTime = CultureInfo.CurrentCulture.Calendar.MaxSupportedDateTime;
				if (minSupportedDateTime.Year < 1601 && maxSupportedDateTime.Year >= 1601)
				{
					num = 1601;
					num2 = 1;
					num3 = 1;
				}
				else
				{
					num = minSupportedDateTime.Year;
					num2 = minSupportedDateTime.Month;
					num3 = minSupportedDateTime.Day;
				}
			}
			else
			{
				num = (int)systemTime.Year;
				num2 = (int)systemTime.Month;
				num3 = (int)systemTime.Day;
			}
			try
			{
				start = new ExDateTime(ExTimeZone.UnspecifiedTimeZone, num, num2, num3, (int)systemTime.Hour, (int)systemTime.Minute, (int)systemTime.Second, (int)systemTime.Milliseconds);
			}
			catch (ArgumentOutOfRangeException innerException)
			{
				ExTraceGlobals.ICalTracer.TraceError(0L, "VTimezone::GetStartAndRuleFromTimeZoneTransition. The timezone is invalid.\r\nYear: {0}, Month: {1}, Day: {2}, Hour: {3}, Minute: {4}, Second: {5}, Milliseconds: {6}.", new object[]
				{
					num,
					num2,
					num3,
					systemTime.Hour,
					systemTime.Minute,
					systemTime.Second,
					systemTime.Milliseconds
				});
				throw new CorruptDataException(ServerStrings.ExCorruptedTimeZone, innerException);
			}
		}

		private static Recurrence CreateTimeZoneRRule(NativeMethods.SystemTime systemTime)
		{
			int num = (int)systemTime.Day;
			if (num == 5)
			{
				num = -1;
			}
			DayOfWeek dayOfWeek = (DayOfWeek)systemTime.DayOfWeek;
			Recurrence.ByDay byDay = new Recurrence.ByDay(dayOfWeek, num);
			return new Recurrence
			{
				Frequency = Frequency.Yearly,
				Interval = 1,
				ByMonth = new int[]
				{
					(int)systemTime.Month
				},
				ByDayList = new Recurrence.ByDay[]
				{
					byDay
				}
			};
		}

		private static void CollectAllRuleYears(Dictionary<ushort, TimeZoneRule> rules, SortedSet<ushort> allRuleYears)
		{
			foreach (ushort num in rules.Keys)
			{
				allRuleYears.Add(num);
				TimeZoneRule timeZoneRule = rules[num];
				if (timeZoneRule.RuleHasRecurrenceUntilField && timeZoneRule.RecurrenceRule.UntilDateTime.Year < DateTime.MaxValue.Year)
				{
					allRuleYears.Add((ushort)(timeZoneRule.RecurrenceRule.UntilDateTime.Year + 1));
				}
			}
		}

		private static TimeZoneRule UpdateAndGetCurrentRule(ushort currentYear, Dictionary<ushort, TimeZoneRule> rules, ref TimeZoneRule recentRule)
		{
			TimeZoneRule timeZoneRule;
			if (!rules.TryGetValue(currentYear, out timeZoneRule))
			{
				timeZoneRule = VTimeZone.ValidateRecentRule(recentRule, currentYear);
			}
			recentRule = timeZoneRule;
			return timeZoneRule;
		}

		private static TimeZoneRule ValidateRecentRule(TimeZoneRule recentRule, ushort currentYear)
		{
			if (recentRule == null)
			{
				return null;
			}
			if (!recentRule.RuleHasRecurrenceUntilField || recentRule.RecurrenceRule.UntilDateTime.Year >= (int)currentYear)
			{
				return recentRule;
			}
			return null;
		}

		private REG_TIMEZONE_INFO GetNativeTimeZoneRule(TimeZoneRule standard, TimeZoneRule daylight)
		{
			if (standard != null && daylight != null && standard.Offset == daylight.Offset)
			{
				daylight = null;
			}
			REG_TIMEZONE_INFO result = default(REG_TIMEZONE_INFO);
			if (standard == null)
			{
				result.Bias = -daylight.Offset;
				result.DaylightBias = 0;
			}
			else
			{
				result.Bias = -standard.Offset;
				result.StandardBias = 0;
				if (daylight != null)
				{
					result.StandardDate = standard.TransitionDate;
					result.DaylightDate = daylight.TransitionDate;
					result.DaylightBias = standard.Offset - daylight.Offset;
				}
			}
			return result;
		}

		private string timeZoneId;

		private REG_TIMEZONE_INFO timeZoneInfo;

		private Dictionary<ushort, TimeZoneRule> standardRules;

		private Dictionary<ushort, TimeZoneRule> daylightRules;
	}
}
