using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class CalendarInstanceContext : IDisposable
	{
		internal CalendarInstanceContext(MeetingValidationResult validationResult, CalendarValidationContext validationContext)
		{
			if (validationResult == null)
			{
				throw new ArgumentNullException("validationResult");
			}
			if (validationContext == null)
			{
				throw new ArgumentNullException("validationContext");
			}
			this.ValidationResult = validationResult;
			this.ValidationContext = validationContext;
		}

		internal bool IsValidationDone { get; private set; }

		internal MeetingValidationResult ValidationResult { get; private set; }

		internal CalendarValidationContext ValidationContext { get; private set; }

		public void Dispose()
		{
			if (this.ValidationContext != null)
			{
				this.ValidationContext.Dispose();
				this.ValidationContext = null;
			}
		}

		private static MeetingComparisonResult GetResultPerAttendee(MeetingValidationResult validationResult, UserObject user)
		{
			MeetingComparisonResult meetingComparisonResult = null;
			string key = user.EmailAddress.ToLower();
			if (!validationResult.ResultsPerAttendee.TryGetValue(key, out meetingComparisonResult))
			{
				meetingComparisonResult = MeetingComparisonResult.CreateInstance(user, validationResult.MeetingData);
				validationResult.ResultsPerAttendee.Add(key, meetingComparisonResult);
			}
			return meetingComparisonResult;
		}

		internal void Validate(Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent, List<GlobalObjectId> inquiredMasterGoids, Action<long> onItemRepaired)
		{
			MeetingComparisonResult resultPerAttendee = CalendarInstanceContext.GetResultPerAttendee(this.ValidationResult, this.ValidationContext.Attendee);
			MeetingComparer meetingComparer = new MeetingComparer(this.ValidationContext);
			meetingComparer.Run(resultPerAttendee, ref organizerRumsSent);
			if (resultPerAttendee.InquiredMeeting && this.ValidationContext.BaseItem.CalendarItemType == CalendarItemType.RecurringMaster)
			{
				inquiredMasterGoids.Add(this.ValidationContext.BaseItem.GlobalObjectId);
			}
			if (onItemRepaired != null)
			{
				onItemRepaired(meetingComparer.NumberOfSuccessfulRepairAttempts);
			}
			foreach (ConsistencyCheckResult consistencyCheckResult in resultPerAttendee)
			{
				if (consistencyCheckResult.Status != CheckStatusType.Passed)
				{
					this.ValidationResult.IsConsistent = false;
					break;
				}
			}
			this.IsValidationDone = true;
			this.ValidationResult.WasValidationSuccessful = true;
		}
	}
}
