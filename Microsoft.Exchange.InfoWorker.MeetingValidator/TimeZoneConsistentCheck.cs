using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class TimeZoneConsistentCheck : ConsistencyCheckBase<ConsistencyCheckResult>
	{
		internal TimeZoneConsistentCheck(CalendarValidationContext context)
		{
			SeverityType severity = context.AreItemsOccurrences ? SeverityType.Warning : SeverityType.Error;
			this.Initialize(ConsistencyCheckType.TimeZoneMatchCheck, "Checks to make sure that the attendee has correct recurring time zone information with the organizer.", severity, context, null);
		}

		protected override ConsistencyCheckResult DetectInconsistencies()
		{
			ConsistencyCheckResult consistencyCheckResult = ConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			if (base.Context.OrganizerItem == null || base.Context.AttendeeItem == null)
			{
				consistencyCheckResult.Status = CheckStatusType.CheckError;
				consistencyCheckResult.ErrorString = "Either organizer or attendee's calendar item is null";
			}
			else
			{
				ExTimeZone recurringTimeZoneFromPropertyBag = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(base.Context.OrganizerItem.PropertyBag);
				if (recurringTimeZoneFromPropertyBag != null)
				{
					ExTimeZone recurringTimeZoneFromPropertyBag2 = TimeZoneHelper.GetRecurringTimeZoneFromPropertyBag(base.Context.AttendeeItem.PropertyBag);
					if (recurringTimeZoneFromPropertyBag2 == null)
					{
						this.FailCheck(consistencyCheckResult, this.GetLogValue(recurringTimeZoneFromPropertyBag, null), this.GetLogValue(recurringTimeZoneFromPropertyBag2, null));
					}
					else
					{
						REG_TIMEZONE_INFO value = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(recurringTimeZoneFromPropertyBag);
						REG_TIMEZONE_INFO reg_TIMEZONE_INFO = TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(recurringTimeZoneFromPropertyBag2);
						if (!value.Equals(reg_TIMEZONE_INFO))
						{
							this.FailCheck(consistencyCheckResult, this.GetLogValue(recurringTimeZoneFromPropertyBag, new REG_TIMEZONE_INFO?(value)), this.GetLogValue(recurringTimeZoneFromPropertyBag2, new REG_TIMEZONE_INFO?(reg_TIMEZONE_INFO)));
						}
					}
				}
			}
			return consistencyCheckResult;
		}

		protected override void ProcessResult(ConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		private string GetLogValue(ExTimeZone timeZone, REG_TIMEZONE_INFO? timeZoneRule)
		{
			string result = null;
			if (timeZone != null)
			{
				if (timeZoneRule != null)
				{
					result = string.Format("Bias: {0} - {1}", timeZoneRule.Value.Bias, timeZone.DisplayName);
				}
				else
				{
					result = timeZone.DisplayName;
				}
			}
			return result;
		}

		private void FailCheck(ConsistencyCheckResult result, string organizerValueToLog, string attendeeValueToLog)
		{
			result.Status = CheckStatusType.Failed;
			result.AddInconsistency(base.Context, PropertyInconsistency.CreateInstance(RoleType.Attendee, CalendarInconsistencyFlag.RecurringTimeZone, "TimeZone", organizerValueToLog, attendeeValueToLog, base.Context));
		}

		internal const string CheckDescription = "Checks to make sure that the attendee has correct recurring time zone information with the organizer.";
	}
}
