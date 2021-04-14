using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.CalendarDiagnostics;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal sealed class MeetingExistenceCheck : ConsistencyCheckBase<MeetingExistenceConsistencyCheckResult>
	{
		internal MeetingExistenceCheck(CalendarValidationContext context)
		{
			SeverityType severity = (context.BaseItem != null && !context.BaseItem.IsCancelled) ? SeverityType.FatalError : SeverityType.Information;
			this.Initialize(ConsistencyCheckType.MeetingExistenceCheck, "Checkes whether the item can be found in the owner's calendar or not.", severity, context, null);
		}

		private UserObject UserToCheck
		{
			get
			{
				if (base.Context.OppositeRole != RoleType.Organizer)
				{
					return base.Context.Attendee;
				}
				return base.Context.Organizer;
			}
		}

		private bool FailOnAbsence
		{
			get
			{
				return base.Context.BaseRole == RoleType.Attendee || base.Context.Attendee.ResponseType != ResponseType.Decline;
			}
		}

		protected override MeetingExistenceConsistencyCheckResult DetectInconsistencies()
		{
			MeetingExistenceConsistencyCheckResult meetingExistenceConsistencyCheckResult = MeetingExistenceConsistencyCheckResult.CreateInstance(base.Type, base.Description);
			meetingExistenceConsistencyCheckResult.Status = CheckStatusType.CheckError;
			if (base.Context.BaseItem != null)
			{
				try
				{
					if (this.CheckFoundItem(null, base.Context.OppositeItem, meetingExistenceConsistencyCheckResult, base.Context.ErrorString) && (meetingExistenceConsistencyCheckResult.ItemIsFound || !this.FailOnAbsence))
					{
						meetingExistenceConsistencyCheckResult.Status = CheckStatusType.Passed;
					}
				}
				catch (ArgumentException ex)
				{
					Globals.ConsistencyChecksTracer.TraceError((long)this.GetHashCode(), "{0}: Could not open item store session or calendar for {1} ({2}), exception = {3}", new object[]
					{
						base.Type,
						base.Context.OppositeRole,
						this.UserToCheck,
						ex
					});
				}
			}
			return meetingExistenceConsistencyCheckResult;
		}

		private bool CheckFoundItem(CalendarFolder folder, CalendarItemBase item, MeetingExistenceConsistencyCheckResult result, string itemQueryError)
		{
			bool flag = false;
			if (item != null)
			{
				CalendarItemType calendarItemType = base.Context.BaseItem.CalendarItemType;
				CalendarItemType calendarItemType2 = item.CalendarItemType;
				if ((calendarItemType == CalendarItemType.Single || calendarItemType == CalendarItemType.RecurringMaster) && (calendarItemType2 == CalendarItemType.Occurrence || calendarItemType2 == CalendarItemType.Exception))
				{
					result.ItemIsFound = false;
					string description = string.Format("Item type mismatch. [BaseType|BaseGoid - FoundType|FoundGoid] ({0}|{1} - {2}|{3})", new object[]
					{
						calendarItemType,
						base.Context.BaseItem.GlobalObjectId,
						calendarItemType2,
						item.GlobalObjectId
					});
					this.FailCheck(result, this.GetInconsistencyFullDescription(description, itemQueryError));
					flag = true;
				}
				else
				{
					result.ItemIsFound = true;
				}
			}
			else
			{
				result.ItemIsFound = false;
				if (this.FailOnAbsence)
				{
					string description2 = string.Format("Could not find the matching meeting in {0}'s calendar.", base.Context.OppositeRole.ToString().ToLower());
					try
					{
						string inconsistencyFullDescription = this.GetInconsistencyFullDescription(description2, itemQueryError);
						this.FailCheck(result, base.Context.CalendarInstance.GetInconsistency(base.Context, inconsistencyFullDescription));
					}
					catch (CalendarVersionStoreNotPopulatedException ex)
					{
						this.FailCheck(result, Inconsistency.CreateInstance(base.Context.OppositeRole, string.Format("The Calendar Version Store is not fully populated yet (Wait Time: {0}).", ex.WaitTimeBeforeThrow), CalendarInconsistencyFlag.MissingCvs, base.Context));
					}
					flag = true;
				}
			}
			return !flag;
		}

		private string GetInconsistencyFullDescription(string description, string errorString)
		{
			return string.Format("{0} Error: {1}", description, errorString);
		}

		private void FailCheck(MeetingExistenceConsistencyCheckResult result, Inconsistency inconsistency)
		{
			result.Status = CheckStatusType.Failed;
			if (inconsistency != null)
			{
				result.AddInconsistency(base.Context, inconsistency);
			}
		}

		private void FailCheck(MeetingExistenceConsistencyCheckResult result, string errorString)
		{
			result.Status = CheckStatusType.Failed;
			result.ErrorString = errorString;
		}

		protected override void ProcessResult(MeetingExistenceConsistencyCheckResult result)
		{
			if (base.Context.CalendarInstance == null)
			{
				result.ShouldBeReported = false;
				return;
			}
			if (result.Status == CheckStatusType.Passed)
			{
				result.ShouldBeReported = true;
				return;
			}
			if (base.Context.BaseItem.IsCancelled)
			{
				result.ShouldBeReported = false;
				return;
			}
			if (base.Context.OppositeRole == RoleType.Organizer)
			{
				result.ShouldBeReported = true;
				return;
			}
			if (!base.Context.OrganizerItem.GetValueOrDefault<bool>(ItemSchema.IsResponseRequested, true))
			{
				result.ShouldBeReported = false;
				return;
			}
			result.ShouldBeReported = (base.Context.Attendee.ResponseType == ResponseType.Accept || base.Context.Attendee.ResponseType == ResponseType.Tentative);
		}

		internal const string CheckDescription = "Checkes whether the item can be found in the owner's calendar or not.";
	}
}
