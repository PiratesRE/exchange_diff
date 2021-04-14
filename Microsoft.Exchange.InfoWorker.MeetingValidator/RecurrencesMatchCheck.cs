using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class RecurrencesMatchCheck : ConsistencyCheckBase<ConsistencyCheckResult>
	{
		internal RecurrencesMatchCheck(CalendarValidationContext context)
		{
			this.Initialize(ConsistencyCheckType.RecurrencesMatchCheck, "Checkes to make sure that the attendee has the correct recurrence pattern.", SeverityType.Error, context, RecurrencesMatchCheck.dependsOnCheckPassList);
		}

		protected override ConsistencyCheckResult DetectInconsistencies()
		{
			ConsistencyCheckResult consistencyCheckResult = ConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			consistencyCheckResult.OrganizerRecurrence = base.Context.OrganizerRecurrence;
			consistencyCheckResult.AttendeeRecurrence = base.Context.AttendeeRecurrence;
			if (base.Context.OrganizerRecurrence.Group != base.Context.AttendeeRecurrence.Group)
			{
				this.FailCheck(consistencyCheckResult, "RecurrenceGroup", base.Context.OrganizerRecurrence.Group, base.Context.AttendeeRecurrence.Group);
			}
			if (base.Context.OrganizerRecurrence.Type != base.Context.AttendeeRecurrence.Type)
			{
				this.FailCheck(consistencyCheckResult, "RecurrenceTypeInBlob", base.Context.OrganizerRecurrence.Type, base.Context.AttendeeRecurrence.Type);
			}
			if (base.Context.OrganizerRecurrence.Period != base.Context.AttendeeRecurrence.Period)
			{
				this.FailCheck(consistencyCheckResult, "Period", base.Context.OrganizerRecurrence.Period, base.Context.AttendeeRecurrence.Period);
			}
			if (base.Context.OrganizerRecurrence.DayMask != base.Context.AttendeeRecurrence.DayMask && (base.Context.OrganizerRecurrence.Type == RecurrenceTypeInBlob.Week || base.Context.OrganizerRecurrence.Type == RecurrenceTypeInBlob.MonthNth))
			{
				this.FailCheck(consistencyCheckResult, "DayMask", base.Context.OrganizerRecurrence.DayMask, base.Context.AttendeeRecurrence.DayMask);
			}
			if (base.Context.OrganizerRecurrence.DayOfMonth != base.Context.AttendeeRecurrence.DayOfMonth && base.Context.OrganizerRecurrence.Type == RecurrenceTypeInBlob.Month)
			{
				this.FailCheck(consistencyCheckResult, "DayOfMonth", base.Context.OrganizerRecurrence.DayOfMonth, base.Context.AttendeeRecurrence.DayOfMonth);
			}
			if (base.Context.OrganizerRecurrence.NthOccurrence != base.Context.AttendeeRecurrence.NthOccurrence && base.Context.OrganizerRecurrence.Type == RecurrenceTypeInBlob.MonthNth)
			{
				this.FailCheck(consistencyCheckResult, "NthOccurrence", base.Context.OrganizerRecurrence.NthOccurrence, base.Context.AttendeeRecurrence.NthOccurrence);
			}
			if (base.Context.OrganizerRecurrence.Range != base.Context.AttendeeRecurrence.Range)
			{
				this.FailCheck(consistencyCheckResult, "RecurrenceRange", base.Context.OrganizerRecurrence.Range, base.Context.AttendeeRecurrence.Range);
				consistencyCheckResult.Status = CheckStatusType.Failed;
			}
			if (base.Context.OrganizerRecurrence.StartDate != base.Context.AttendeeRecurrence.StartDate)
			{
				this.FailCheck(consistencyCheckResult, "StartDate", base.Context.OrganizerRecurrence.StartDate, base.Context.AttendeeRecurrence.StartDate);
			}
			if (base.Context.OrganizerRecurrence.EndDate != base.Context.AttendeeRecurrence.EndDate && base.Context.OrganizerRecurrence.Range == RecurrenceRangeType.End)
			{
				this.FailCheck(consistencyCheckResult, "EndDate", base.Context.OrganizerRecurrence.EndDate, base.Context.AttendeeRecurrence.EndDate);
			}
			if (base.Context.OrganizerRecurrence.NumberOfOccurrences != base.Context.AttendeeRecurrence.NumberOfOccurrences && base.Context.OrganizerRecurrence.Range == RecurrenceRangeType.AfterNOccur)
			{
				this.FailCheck(consistencyCheckResult, "NumberOfOccurrences", base.Context.OrganizerRecurrence.NumberOfOccurrences, base.Context.AttendeeRecurrence.NumberOfOccurrences);
			}
			if (base.Context.OrganizerRecurrence.FirstDayOfWeek != base.Context.AttendeeRecurrence.FirstDayOfWeek)
			{
				this.FailCheck(consistencyCheckResult, "FirstDayOfWeek", base.Context.OrganizerRecurrence.FirstDayOfWeek, base.Context.AttendeeRecurrence.FirstDayOfWeek);
			}
			return consistencyCheckResult;
		}

		protected override void ProcessResult(ConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		private void FailCheck(ConsistencyCheckResult result, string propertyName, object expectedValue, object actualValue)
		{
			result.Status = CheckStatusType.Failed;
			result.AddInconsistency(base.Context, PropertyInconsistency.CreateInstance(RoleType.Attendee, CalendarInconsistencyFlag.RecurrenceBlob, propertyName, expectedValue, actualValue, base.Context));
		}

		internal const string CheckDescription = "Checkes to make sure that the attendee has the correct recurrence pattern.";

		private static IEnumerable<ConsistencyCheckType> dependsOnCheckPassList = new ConsistencyCheckType[]
		{
			ConsistencyCheckType.RecurrenceBlobsConsistentCheck
		};
	}
}
