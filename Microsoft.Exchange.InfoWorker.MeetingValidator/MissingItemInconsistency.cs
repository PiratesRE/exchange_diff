using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	public class MissingItemInconsistency : Inconsistency
	{
		protected MissingItemInconsistency(RoleType owner, string description, CalendarInconsistencyFlag flag, CalendarValidationContext context) : base(owner, description, flag, context)
		{
		}

		internal static MissingItemInconsistency CreateAttendeeMissingItemInstance(string description, ClientIntentFlags? intent, int? deletedItemVersion, CalendarValidationContext context)
		{
			return new MissingItemInconsistency(RoleType.Attendee, description, CalendarInconsistencyFlag.MissingItem, context)
			{
				Intent = intent,
				DeletedItemVersion = deletedItemVersion
			};
		}

		internal static MissingItemInconsistency CreateOrganizerMissingItemInstance(string description, CalendarValidationContext context)
		{
			return new MissingItemInconsistency(RoleType.Organizer, description, CalendarInconsistencyFlag.OrphanedMeeting, context);
		}

		public int? DeletedItemVersion { get; private set; }

		internal override RumInfo CreateRumInfo(CalendarValidationContext context, IList<Attendee> attendees)
		{
			CalendarInconsistencyFlag flag = base.Flag;
			if (flag != CalendarInconsistencyFlag.OrphanedMeeting)
			{
				return MissingAttendeeItemRumInfo.CreateMasterInstance(attendees, base.Flag, this.DeletedItemVersion);
			}
			if (context.OppositeRole == RoleType.Organizer && !context.OppositeRoleOrganizerIsValid)
			{
				return NullOpRumInfo.CreateInstance();
			}
			MeetingInquiryAction predictedRepairAction;
			bool wouldRepair = context.CalendarInstance.WouldTryToRepairIfMissing(context, out predictedRepairAction);
			return AttendeeInquiryRumInfo.CreateMasterInstance(wouldRepair, predictedRepairAction);
		}
	}
}
