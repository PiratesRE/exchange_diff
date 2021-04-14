using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	internal class MeetingComparer
	{
		internal MeetingComparer(CalendarValidationContext context)
		{
			this.NumberOfSuccessfulRepairAttempts = 0L;
			this.context = context;
		}

		internal long NumberOfSuccessfulRepairAttempts { get; private set; }

		internal void Run(MeetingComparisonResult comparisonResult, ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			if (!this.SkipItem(ref organizerRumsSent))
			{
				PrimaryConsistencyCheckChain primaryConsistencyCheckChain = ConsistencyCheckFactory.Instance.CreatePrimaryConsistencyCheckChain(this.context, comparisonResult);
				primaryConsistencyCheckChain.PerformChecks(this.context);
				if (primaryConsistencyCheckChain.ShouldTerminate)
				{
					if (RumFactory.Instance.Policy.RepairMode == CalendarRepairType.ValidateOnly && this.context.BaseItem.CalendarItemType == CalendarItemType.RecurringMaster)
					{
						RecurrenceBlobsConsistentCheck recurrenceBlobsConsistentCheck = new RecurrenceBlobsConsistentCheck(this.context);
						ConsistencyCheckResult consistencyCheckResult = recurrenceBlobsConsistentCheck.Run();
						if (consistencyCheckResult.ShouldBeReported)
						{
							comparisonResult.AddCheckResult(consistencyCheckResult);
						}
					}
				}
				else
				{
					ConsistencyCheckChain<ConsistencyCheckResult> consistencyCheckChain = ConsistencyCheckFactory.Instance.CreateGeneralConsistencyCheckChain(this.context, comparisonResult, RumFactory.Instance.Policy.RepairMode == CalendarRepairType.ValidateOnly);
					consistencyCheckChain.PerformChecks();
				}
				if (RumFactory.Instance.Policy.RepairMode == CalendarRepairType.RepairAndValidate && comparisonResult.RepairInfo.SendRums(this.context.BaseItem, ref organizerRumsSent))
				{
					this.NumberOfSuccessfulRepairAttempts += 1L;
				}
			}
		}

		private bool SkipItem(ref Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			bool result;
			if (organizerRumsSent == null)
			{
				organizerRumsSent = new Dictionary<GlobalObjectId, List<Attendee>>();
				this.context.HasSentUpdateForItemOrMaster = false;
				result = false;
			}
			else if (this.context.BaseRole == RoleType.Organizer && RumFactory.Instance.Policy.RepairMode == CalendarRepairType.RepairAndValidate && organizerRumsSent.Count != 0)
			{
				if (this.HasSentUpdateForItem(this.context.OrganizerItem.GlobalObjectId, this.context.Attendee, organizerRumsSent))
				{
					this.context.HasSentUpdateForItemOrMaster = true;
					result = true;
				}
				else if (this.context.OrganizerItem.CalendarItemType == CalendarItemType.Occurrence || this.context.OrganizerItem.CalendarItemType == CalendarItemType.Exception)
				{
					this.context.HasSentUpdateForItemOrMaster = this.HasSentUpdateForMaster(this.context.OrganizerItem, this.context.Attendee, organizerRumsSent);
					result = (this.context.HasSentUpdateForItemOrMaster && this.context.OrganizerItem.CalendarItemType == CalendarItemType.Exception);
				}
				else
				{
					this.context.HasSentUpdateForItemOrMaster = false;
					result = false;
				}
			}
			else
			{
				this.context.HasSentUpdateForItemOrMaster = false;
				result = false;
			}
			return result;
		}

		private bool HasSentUpdateForMaster(CalendarItemBase organizerItem, UserObject attendee, Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			return this.HasSentUpdateForItem(new GlobalObjectId(organizerItem.CleanGlobalObjectId), attendee, organizerRumsSent);
		}

		private bool HasSentUpdateForItem(GlobalObjectId goid, UserObject attendee, Dictionary<GlobalObjectId, List<Attendee>> organizerRumsSent)
		{
			bool result = false;
			if (organizerRumsSent.ContainsKey(goid) && RumAgent.Instance.AttendeeListContainsParticipant(organizerRumsSent[goid], attendee.Participant))
			{
				result = true;
			}
			return result;
		}

		private CalendarValidationContext context;
	}
}
