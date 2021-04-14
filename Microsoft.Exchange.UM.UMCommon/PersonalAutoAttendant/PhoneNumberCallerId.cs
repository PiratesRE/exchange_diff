using System;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PhoneNumberCallerId : CallerId<string>
	{
		internal PhoneNumberCallerId(string callerid) : base(CallerIdTypeEnum.Number, callerid)
		{
		}

		internal string PhoneNumberString
		{
			get
			{
				return base.GetData();
			}
		}

		internal override int EvaluationCost
		{
			get
			{
				return PAAConstants.PhoneNumberCallerIdEvaluationCost;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidatePhoneNumberCallerId(this.PhoneNumberString, out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		internal override bool Evaluate(IRuleEvaluator evaluator)
		{
			CallerIdRuleEvaluator callerIdRuleEvaluator = evaluator as CallerIdRuleEvaluator;
			PhoneNumber callerId = callerIdRuleEvaluator.GetCallerId();
			PIIMessage piimessage = PIIMessage.Create(PIIType._Caller, callerId);
			PIIMessage piimessage2 = PIIMessage.Create(PIIType._PhoneNumber, this.PhoneNumberString);
			PIIMessage[] data = new PIIMessage[]
			{
				piimessage,
				piimessage2
			};
			CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "PhoneNumberCallerId:Evaluate() CallerId = '_Caller' Condition = '_PhoneNumber'", new object[0]);
			PhoneNumber phoneNumber = null;
			if (!PhoneNumber.TryParse(this.PhoneNumberString, out phoneNumber))
			{
				CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage2, "PhoneNumberCallerId:Evaluate() PhoneNumberCallerId condition '_PhoneNumber' failed to parse as a PhoneNumber. Condition will fail evaluation", new object[0]);
				return false;
			}
			UMSubscriber umsubscriber = callerIdRuleEvaluator.GetUMSubscriber();
			bool flag = callerId.IsMatch(phoneNumber.Number, callerId.GetOptionalPrefixes(umsubscriber.DialPlan));
			CallIdTracer.TraceError(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PhoneNumberCallerId:Evaluate() {2} Comparing Parsed CallerId = '{0}' Parsed Condition = '{1}'", new object[]
			{
				callerId.Number,
				phoneNumber.Number,
				flag ? "PASS" : "FAIL"
			});
			return flag;
		}
	}
}
