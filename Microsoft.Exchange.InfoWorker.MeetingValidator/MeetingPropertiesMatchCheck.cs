using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MeetingPropertiesMatchCheck : ConsistencyCheckBase<ConsistencyCheckResult>
	{
		internal MeetingPropertiesMatchCheck(CalendarValidationContext context)
		{
			SeverityType severity = context.AreItemsOccurrences ? SeverityType.Warning : SeverityType.Error;
			this.Initialize(ConsistencyCheckType.MeetingPropertiesMatchCheck, "Checks to make sure that the attendee has the correct critical properties for the meeting.", severity, context, null);
		}

		protected override ConsistencyCheckResult DetectInconsistencies()
		{
			ConsistencyCheckResult result = ConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			ExDateTime startTime = base.Context.OrganizerItem.StartTime;
			ExDateTime endTime = base.Context.OrganizerItem.EndTime;
			string location = base.Context.OrganizerItem.Location;
			ExDateTime startTime2 = base.Context.AttendeeItem.StartTime;
			ExDateTime endTime2 = base.Context.AttendeeItem.EndTime;
			string location2 = base.Context.AttendeeItem.Location;
			int appointmentSequenceNumber = base.Context.OrganizerItem.AppointmentSequenceNumber;
			int appointmentLastSequenceNumber = base.Context.OrganizerItem.AppointmentLastSequenceNumber;
			ExDateTime ownerCriticalChangeTime = base.Context.OrganizerItem.OwnerCriticalChangeTime;
			ExDateTime attendeeCriticalChangeTime = base.Context.OrganizerItem.AttendeeCriticalChangeTime;
			int appointmentSequenceNumber2 = base.Context.AttendeeItem.AppointmentSequenceNumber;
			ExDateTime ownerCriticalChangeTime2 = base.Context.AttendeeItem.OwnerCriticalChangeTime;
			ExDateTime attendeeCriticalChangeTime2 = base.Context.AttendeeItem.AttendeeCriticalChangeTime;
			bool flag = false;
			if (appointmentSequenceNumber == appointmentSequenceNumber2 || (appointmentSequenceNumber != appointmentLastSequenceNumber && appointmentLastSequenceNumber >= appointmentSequenceNumber2))
			{
				flag = true;
			}
			else if (ExDateTime.Compare(ownerCriticalChangeTime, ownerCriticalChangeTime2, MeetingPropertiesMatchCheck.DateTimeComparisonTreshold) == 0)
			{
				flag = true;
			}
			else if (ExDateTime.Compare(attendeeCriticalChangeTime, attendeeCriticalChangeTime2, MeetingPropertiesMatchCheck.DateTimeComparisonTreshold) == 0)
			{
				flag = true;
			}
			else
			{
				this.FailCheck(result, CalendarInconsistencyFlag.VersionInfo, "SequenceNumber", appointmentSequenceNumber, appointmentSequenceNumber2);
				this.FailCheck(result, CalendarInconsistencyFlag.VersionInfo, "OwnerCriticalChangeTime", ownerCriticalChangeTime, ownerCriticalChangeTime2);
				this.FailCheck(result, CalendarInconsistencyFlag.VersionInfo, "AttendeeCriticalChangeTime", attendeeCriticalChangeTime, attendeeCriticalChangeTime2);
			}
			if (!flag)
			{
				ExDateTime utcNow = ExDateTime.UtcNow;
				if (ExDateTime.Compare(ownerCriticalChangeTime, utcNow, TimeSpan.FromMinutes(120.0)) != 0)
				{
					this.FailCheck(result, CalendarInconsistencyFlag.VersionInfo, "DelayedUpdates", ownerCriticalChangeTime.ToUtc().ToString(), utcNow.ToUtc().ToString());
				}
			}
			bool flag2 = MeetingPropertiesMatchCheck.CheckForMeetingOverlapInconsistency(startTime, endTime, startTime2, endTime2);
			if (flag2)
			{
				this.FailCheck(result, CalendarInconsistencyFlag.TimeOverlap, "MeetingOverlap", (startTime - endTime).TotalMinutes, 0);
			}
			bool flag3 = false;
			this.CheckTimeConsistency(result, MeetingPropertiesMatchCheck.TimeProperty.StartTime, ref flag3);
			this.CheckTimeConsistency(result, MeetingPropertiesMatchCheck.TimeProperty.EndTime, ref flag3);
			if (location != null && location2 != null && !location2.Contains(location))
			{
				try
				{
					ClientIntentFlags? clientIntentFlags;
					if (base.Context.BaseRole == RoleType.Attendee)
					{
						ICalendarItemStateDefinition initialState = new LocationBasedCalendarItemStateDefinition(location);
						ICalendarItemStateDefinition targetState = new LocationBasedCalendarItemStateDefinition(location2);
						ClientIntentQuery clientIntentQuery = new TransitionalClientIntentQuery(base.Context.AttendeeItem.GlobalObjectId, initialState, targetState);
						clientIntentFlags = clientIntentQuery.Execute((MailboxSession)base.Context.AttendeeItem.Session, base.Context.CvsGateway).Intent;
					}
					else
					{
						clientIntentFlags = base.Context.CalendarInstance.GetLocationIntent(base.Context, base.Context.AttendeeItem.GlobalObjectId, location, location2);
					}
					if (!ClientIntentQuery.CheckDesiredClientIntent(clientIntentFlags, new ClientIntentFlags[]
					{
						ClientIntentFlags.ModifiedLocation
					}))
					{
						this.FailCheck(result, CalendarInconsistencyFlag.Location, "Location", location, location2, clientIntentFlags);
					}
				}
				catch (CalendarVersionStoreNotPopulatedException exc)
				{
					this.FailCheck(result, Inconsistency.CreateMissingCvsInconsistency(RoleType.Attendee, exc, base.Context));
				}
			}
			return result;
		}

		protected override void ProcessResult(ConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		private static bool CheckForMeetingOverlapInconsistency(ExDateTime organizerStartTime, ExDateTime organizerEndTime, ExDateTime attendeeStartTime, ExDateTime attendeeEndTime)
		{
			bool result;
			if (organizerStartTime.Equals(organizerEndTime))
			{
				result = (attendeeEndTime < organizerStartTime || attendeeStartTime > organizerEndTime);
			}
			else if (attendeeStartTime < organizerStartTime)
			{
				result = (attendeeEndTime <= organizerStartTime);
			}
			else
			{
				result = (attendeeStartTime >= organizerEndTime && attendeeEndTime > organizerEndTime);
			}
			return result;
		}

		private void CheckTimeConsistency(ConsistencyCheckResult result, MeetingPropertiesMatchCheck.TimeProperty propertyToCheck, ref bool timeZoneCheckFailed)
		{
			ExDateTime exDateTime;
			ExDateTime exDateTime2;
			switch (propertyToCheck)
			{
			case MeetingPropertiesMatchCheck.TimeProperty.StartTime:
				exDateTime = base.Context.OrganizerItem.StartTime;
				exDateTime2 = base.Context.AttendeeItem.StartTime;
				break;
			case MeetingPropertiesMatchCheck.TimeProperty.EndTime:
				exDateTime = base.Context.OrganizerItem.EndTime;
				exDateTime2 = base.Context.AttendeeItem.EndTime;
				break;
			default:
				throw new ArgumentException(string.Format("Time property ({0}) is not valid for consistency check.", propertyToCheck));
			}
			if (!exDateTime.Equals(exDateTime2))
			{
				if (Math.Abs(exDateTime.Subtract(exDateTime2).TotalMinutes) > 120.0)
				{
					this.FailCheck(result, CalendarInconsistencyFlag.StartTime, propertyToCheck.ToString(), exDateTime, exDateTime2);
					return;
				}
				if (!timeZoneCheckFailed)
				{
					REG_TIMEZONE_INFO? effectiveTimeZoneRule = this.GetEffectiveTimeZoneRule(base.Context.OrganizerItem);
					if (effectiveTimeZoneRule != null)
					{
						REG_TIMEZONE_INFO? effectiveTimeZoneRule2 = this.GetEffectiveTimeZoneRule(base.Context.AttendeeItem);
						if (effectiveTimeZoneRule2 == null || !effectiveTimeZoneRule2.Equals(effectiveTimeZoneRule))
						{
							this.FailCheck(result, CalendarInconsistencyFlag.StartTimeZone, propertyToCheck.ToString(), exDateTime, exDateTime2);
							timeZoneCheckFailed = true;
						}
					}
				}
			}
		}

		private REG_TIMEZONE_INFO? GetEffectiveTimeZoneRule(CalendarItemBase item)
		{
			REG_TIMEZONE_INFO? result = null;
			byte[] valueOrDefault = item.GetValueOrDefault<byte[]>(ItemSchema.TimeZoneDefinitionStart);
			if (valueOrDefault != null)
			{
				ExTimeZone timeZone = null;
				if (O12TimeZoneFormatter.TryParseTimeZoneBlob(valueOrDefault, string.Empty, out timeZone))
				{
					result = new REG_TIMEZONE_INFO?(TimeZoneHelper.RegTimeZoneInfoFromExTimeZone(timeZone, item.StartTime));
				}
			}
			return result;
		}

		private void FailCheck(ConsistencyCheckResult result, CalendarInconsistencyFlag inconsistencyFlag, string propertyName, object expectedValue, object actualValue)
		{
			this.FailCheck(result, inconsistencyFlag, propertyName, expectedValue, actualValue, null);
		}

		private void FailCheck(ConsistencyCheckResult result, CalendarInconsistencyFlag inconsistencyFlag, string propertyName, object expectedValue, object actualValue, ClientIntentFlags? inconsistencyIntent)
		{
			PropertyInconsistency propertyInconsistency = PropertyInconsistency.CreateInstance(RoleType.Attendee, inconsistencyFlag, propertyName, expectedValue, actualValue, base.Context);
			propertyInconsistency.Intent = inconsistencyIntent;
			this.FailCheck(result, propertyInconsistency);
		}

		private void FailCheck(ConsistencyCheckResult result, Inconsistency inconsistency)
		{
			result.Status = CheckStatusType.Failed;
			result.AddInconsistency(base.Context, inconsistency);
		}

		internal const string CheckDescription = "Checks to make sure that the attendee has the correct critical properties for the meeting.";

		private const double MaxTravelTimeThresholdInMinutes = 120.0;

		private static readonly TimeSpan DateTimeComparisonTreshold = TimeSpan.FromSeconds(1.0);

		private enum TimeProperty
		{
			StartTime,
			EndTime
		}
	}
}
