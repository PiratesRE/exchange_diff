using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class MeetingCancellationCheck : ConsistencyCheckBase<ConsistencyCheckResult>
	{
		internal MeetingCancellationCheck(CalendarValidationContext context)
		{
			IEnumerable<ConsistencyCheckType> dependsOnCheckPassList = null;
			if (context.BaseRole == RoleType.Attendee && context.AttendeeItem.IsCancelled)
			{
				dependsOnCheckPassList = MeetingCancellationCheck.cancelledAttendeeDependsOnCheckPassList;
			}
			this.Initialize(ConsistencyCheckType.MeetingCancellationCheck, "Checkes to make sure that the meeting cancellations statuses match.", SeverityType.Error, context, dependsOnCheckPassList);
		}

		protected override ConsistencyCheckResult DetectInconsistencies()
		{
			ConsistencyCheckResult consistencyCheckResult = ConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			bool isCancelled = base.Context.BaseItem.IsCancelled;
			bool isCancelled2 = base.Context.OppositeItem.IsCancelled;
			if (isCancelled != isCancelled2)
			{
				consistencyCheckResult.Status = CheckStatusType.Failed;
				consistencyCheckResult.AddInconsistency(base.Context, PropertyInconsistency.CreateInstance(base.Context.OppositeRole, CalendarInconsistencyFlag.Cancellation, "IsCancelled", isCancelled, isCancelled2, base.Context));
			}
			return consistencyCheckResult;
		}

		protected override void ProcessResult(ConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		internal const string CheckDescription = "Checkes to make sure that the meeting cancellations statuses match.";

		protected static IEnumerable<ConsistencyCheckType> cancelledAttendeeDependsOnCheckPassList = new ConsistencyCheckType[]
		{
			ConsistencyCheckType.AttendeeOnListCheck
		};
	}
}
