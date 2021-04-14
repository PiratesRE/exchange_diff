using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.ExchangeSystem;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CorrectResponseCheck : ConsistencyCheckBase<ConsistencyCheckResult>
	{
		internal CorrectResponseCheck(CalendarValidationContext context)
		{
			SeverityType severity = context.AreItemsOccurrences ? SeverityType.Warning : SeverityType.Error;
			this.Initialize(ConsistencyCheckType.CorrectResponseCheck, "Checkes to make sure that the organizaer has the correct response recorded for the attendee.", severity, context, (context.BaseRole == RoleType.Attendee) ? CorrectResponseCheck.attendeeDependsOnCheckPassList : null);
		}

		protected override ConsistencyCheckResult DetectInconsistencies()
		{
			ConsistencyCheckResult consistencyCheckResult = ConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			ResponseType responseType = base.Context.AttendeeItem.ResponseType;
			ResponseType responseType2 = base.Context.Attendee.ResponseType;
			ExDateTime replyTime = base.Context.Attendee.ReplyTime;
			ExDateTime appointmentReplyTime = base.Context.AttendeeItem.AppointmentReplyTime;
			if (!ExDateTime.MinValue.Equals(replyTime) && responseType != ResponseType.NotResponded && responseType2 != responseType)
			{
				consistencyCheckResult.Status = CheckStatusType.Failed;
				consistencyCheckResult.AddInconsistency(base.Context, ResponseInconsistency.CreateInstance(responseType, responseType2, appointmentReplyTime, replyTime, base.Context));
			}
			return consistencyCheckResult;
		}

		protected override void ProcessResult(ConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		internal const string CheckDescription = "Checkes to make sure that the organizaer has the correct response recorded for the attendee.";

		protected static IEnumerable<ConsistencyCheckType> attendeeDependsOnCheckPassList = new ConsistencyCheckType[]
		{
			ConsistencyCheckType.AttendeeOnListCheck
		};
	}
}
