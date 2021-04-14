using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;

namespace Microsoft.Exchange.Infoworker.MeetingValidator
{
	[ClassAccessLevel(AccessLevel.Implementation)]
	internal class ValidateStoreObjectCheck : ConsistencyCheckBase<PrimaryConsistencyCheckResult>
	{
		internal ValidateStoreObjectCheck(CalendarValidationContext context)
		{
			this.Initialize(ConsistencyCheckType.ValidateStoreObjectCheck, "Calls Validate() on the base calendar item.", SeverityType.Error, context, null);
		}

		protected override PrimaryConsistencyCheckResult DetectInconsistencies()
		{
			PrimaryConsistencyCheckResult result;
			try
			{
				StoreObjectValidationError[] errorList;
				switch (base.Context.BaseRole)
				{
				case RoleType.Organizer:
					errorList = base.Context.OrganizerItem.Validate();
					break;
				case RoleType.Attendee:
					errorList = base.Context.AttendeeItem.Validate();
					break;
				default:
					errorList = new StoreObjectValidationError[0];
					break;
				}
				result = this.GetResult(errorList);
			}
			catch (ObjectValidationException ex)
			{
				result = this.GetResult(ex.Errors);
				result.Status = CheckStatusType.Failed;
			}
			return result;
		}

		protected override void ProcessResult(PrimaryConsistencyCheckResult result)
		{
			result.ShouldBeReported = true;
		}

		private PrimaryConsistencyCheckResult GetResult(ICollection<StoreObjectValidationError> errorList)
		{
			PrimaryConsistencyCheckResult primaryConsistencyCheckResult = PrimaryConsistencyCheckResult.CreateInstance(base.Type, base.Description, true);
			if (errorList != null && errorList.Count != 0)
			{
				primaryConsistencyCheckResult.Status = CheckStatusType.Failed;
				foreach (StoreObjectValidationError storeObjectValidationError in errorList)
				{
					primaryConsistencyCheckResult.AddInconsistency(base.Context, Inconsistency.CreateInstance(base.Context.BaseRole, storeObjectValidationError.ToString(), CalendarInconsistencyFlag.StoreObjectValidation, base.Context));
				}
			}
			return primaryConsistencyCheckResult;
		}

		internal const string CheckDescription = "Calls Validate() on the base calendar item.";
	}
}
