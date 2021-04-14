using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Availability;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class TimeOfDayRuleEvaluator : IRuleEvaluator
	{
		public TimeOfDayRuleEvaluator(TimeOfDayEnum timeofday, WorkingPeriod period)
		{
			this.timeOfDay = timeofday;
			this.customWorkingPeriod = period;
		}

		public bool Evaluate(IDataLoader dataLoader)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:Evaluate() TimeOfDay={0}", new object[]
			{
				this.timeOfDay
			});
			if (this.timeOfDay == TimeOfDayEnum.None)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:Evaluate() no conditions defined. Returning true", new object[0]);
				return true;
			}
			if (this.timeOfDay == TimeOfDayEnum.Custom)
			{
				return this.EvaluateCustom(dataLoader);
			}
			if (this.timeOfDay == TimeOfDayEnum.WorkingHours || this.timeOfDay == TimeOfDayEnum.NonWorkingHours)
			{
				return this.DoesItSatisfyWorkingHoursRule(dataLoader);
			}
			return (this.timeOfDay == TimeOfDayEnum.CompanyNonWorkingHours || this.timeOfDay == TimeOfDayEnum.CompanyWorkingHours) && this.DoesItSatisfyCompanyWorkingHoursRule(dataLoader);
		}

		private static bool IsSameWorkDay(DaysOfWeek daysOfWeek, DayOfWeek dayToTest)
		{
			int num = (int)WorkingHours.DayToDays(dayToTest);
			return (daysOfWeek & (DaysOfWeek)num) != (DaysOfWeek)0;
		}

		private bool DoesItSatisfyCompanyWorkingHoursRule(IDataLoader dataLoader)
		{
			bool flag = false;
			return dataLoader.TryIsWithinCompanyWorkingHours(out flag) && ((flag && this.timeOfDay == TimeOfDayEnum.CompanyWorkingHours) || (!flag && this.timeOfDay == TimeOfDayEnum.CompanyNonWorkingHours));
		}

		private bool DoesItSatisfyWorkingHoursRule(IDataLoader dataLoader)
		{
			WorkingHours workingHours = null;
			try
			{
				workingHours = dataLoader.GetWorkingHours();
			}
			catch (StorageTransientException ex)
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:DoesItSatisfyWorkingHoursRule() Got exception getting WorkingHours", new object[0]);
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "Exception : {0}", new object[]
				{
					ex
				});
				return false;
			}
			if (workingHours == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:DoesItSatisfyWorkingHoursRule() Got NULL Calendar Working Hours, Returning false", new object[0]);
				return false;
			}
			ExDateTime utcNow = ExDateTime.UtcNow;
			bool flag = workingHours.InWorkingHours(utcNow, utcNow);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:DoesItSatisfyWorkingHoursRule() Calendar Working Hours: {0}", new object[]
			{
				workingHours
			});
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:DoesItSatisfyWorkingHoursRule() For UTCTime: {0}[Local = {1}] IsWorkingHours = {2}", new object[]
			{
				utcNow,
				dataLoader.GetUserTimeZone().ConvertDateTime(utcNow),
				flag
			});
			return (flag && this.timeOfDay == TimeOfDayEnum.WorkingHours) || (!flag && this.timeOfDay == TimeOfDayEnum.NonWorkingHours);
		}

		private bool EvaluateCustom(IDataLoader dataLoader)
		{
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:EvaluateCustom() Period={0}", new object[]
			{
				this.customWorkingPeriod
			});
			ExTimeZone userTimeZone = dataLoader.GetUserTimeZone();
			ExDateTime now = ExDateTime.GetNow(userTimeZone);
			if (!TimeOfDayRuleEvaluator.IsSameWorkDay(this.customWorkingPeriod.DayOfWeek, now.DayOfWeek))
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:EvaluateCustom() User Time Zone ={0} UserLocalTime = {1} returning FALSE", new object[]
				{
					userTimeZone.DisplayName,
					now
				});
				return false;
			}
			int num = (int)now.TimeOfDay.TotalMinutes;
			if (num >= this.customWorkingPeriod.StartTimeInMinutes && num <= this.customWorkingPeriod.EndTimeInMinutes)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:EvaluateCustom() User Time Zone ={0} UserLocalTime = {1} returning TRUE", new object[]
				{
					userTimeZone.DisplayName,
					now
				});
				return true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "TimeOfDayRuleEvaluator:EvaluateCustom() User Time Zone ={0} UserLocalTime = {1} returning FALSE", new object[]
			{
				userTimeZone.DisplayName,
				now
			});
			return false;
		}

		private TimeOfDayEnum timeOfDay;

		private WorkingPeriod customWorkingPeriod;
	}
}
