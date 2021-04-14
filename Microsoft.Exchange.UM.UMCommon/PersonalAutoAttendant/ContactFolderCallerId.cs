using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class ContactFolderCallerId : CallerIdBase
	{
		internal ContactFolderCallerId() : base(CallerIdTypeEnum.DefaultContactFolder)
		{
		}

		internal override int EvaluationCost
		{
			get
			{
				return PAAConstants.ContactFolderCallerIdEvaluationCost;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidateContactFolderCallerId(out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		internal override bool Evaluate(IRuleEvaluator evaluator)
		{
			CallerIdRuleEvaluator callerIdRuleEvaluator = evaluator as CallerIdRuleEvaluator;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactFolderCallerId:Evaluate() MatchedContacts.Length == {0}", new object[]
			{
				callerIdRuleEvaluator.MatchedPersonalContacts.Length
			});
			if (callerIdRuleEvaluator.MatchedPersonalContacts.Length > 0)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactFolderCallerId:Evaluate() Evaluation PASSED", new object[0]);
				return true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactFolderCallerId:Evaluate() Evaluation FAILED", new object[0]);
			return false;
		}
	}
}
