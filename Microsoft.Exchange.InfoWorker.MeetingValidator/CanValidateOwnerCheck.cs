using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class CanValidateOwnerCheck : ConsistencyCheckBase<PrimaryConsistencyCheckResult>
	{
		internal CanValidateOwnerCheck(CalendarValidationContext context)
		{
			SeverityType severity = SeverityType.FatalError;
			if (context.BaseItem == null || context.BaseItem.IsCancelled)
			{
				severity = SeverityType.Information;
			}
			else if (context.BaseRole == RoleType.Organizer && (context.Attendee.ResponseType == ResponseType.NotResponded || context.Attendee.ResponseType == ResponseType.None))
			{
				severity = SeverityType.Warning;
			}
			this.Initialize(ConsistencyCheckType.CanValidateOwnerCheck, "Checks whether the counterpart user can be validated or not.", severity, context, null);
		}

		protected override PrimaryConsistencyCheckResult DetectInconsistencies()
		{
			PrimaryConsistencyCheckResult primaryConsistencyCheckResult = PrimaryConsistencyCheckResult.CreateInstance(base.Type, base.Description, true);
			UserObject userObject;
			if (base.Context.BaseRole == RoleType.Organizer)
			{
				userObject = base.Context.Attendee;
				if (!Participant.HasSameEmail(base.Context.Organizer.Participant, base.Context.OrganizerItem.Organizer, base.Context.LocalUser.ExchangePrincipal))
				{
					this.FailCheck(primaryConsistencyCheckResult, base.Context.BaseRole, CalendarInconsistencyFlag.Organizer, string.Format("The calendar item is an organizer item, while the mailbox owner ({0}) is not the organizer of the item ({1}).", base.Context.Organizer, base.Context.OrganizerItem.Organizer));
				}
			}
			else
			{
				userObject = base.Context.Organizer;
			}
			if (primaryConsistencyCheckResult.Status == CheckStatusType.Passed)
			{
				if (DistributionList.IsDL(userObject.RecipientType))
				{
					this.FailCheck(primaryConsistencyCheckResult, base.Context.OppositeRole, CalendarInconsistencyFlag.LargeDL, "Distribution list too large to expand");
				}
				else if (userObject.ExchangePrincipal == null)
				{
					this.FailCheck(primaryConsistencyCheckResult, base.Context.BaseRole, CalendarInconsistencyFlag.UserNotFound, "Could not get Exchange principal for " + base.Context.OppositeRole.ToString().ToLower());
				}
				else if (userObject.ExchangePrincipal.MailboxInfo.Location.IsLegacyServer())
				{
					this.FailCheck(primaryConsistencyCheckResult, base.Context.OppositeRole, CalendarInconsistencyFlag.LegacyUser, base.Context.OppositeRole.ToString() + " is on a legacy server.");
				}
				else if (base.Context.CalendarInstance != null)
				{
					if (!base.Context.CalendarInstance.ShouldProcessMailbox)
					{
						primaryConsistencyCheckResult.TerminateCheck("Skipping mailbox that should not be processed by CRA.");
					}
				}
				else
				{
					this.FailCheck(primaryConsistencyCheckResult, "Unable to initialize the target session.");
				}
			}
			return primaryConsistencyCheckResult;
		}

		protected override void ProcessResult(PrimaryConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
			if (base.Context.CalendarInstance != null)
			{
				result.ShouldBeReported = base.Context.CalendarInstance.ShouldProcessMailbox;
			}
			if (result.ShouldBeReported)
			{
				result.ShouldBeReported = (!base.Context.BaseItem.IsCancelled || result.Status == CheckStatusType.Passed);
			}
		}

		private void FailCheck(ConsistencyCheckResult result, RoleType inconsistentRole, CalendarInconsistencyFlag flag, string fullDescription)
		{
			result.Status = CheckStatusType.Failed;
			result.AddInconsistency(base.Context, Inconsistency.CreateInstance(inconsistentRole, fullDescription, flag, base.Context));
		}

		private void FailCheck(ConsistencyCheckResult result, string errorString)
		{
			result.Status = CheckStatusType.Failed;
			result.ErrorString = errorString;
		}

		internal const string CheckDescription = "Checks whether the counterpart user can be validated or not.";
	}
}
