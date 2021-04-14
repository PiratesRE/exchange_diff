using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class PersonaContactCallerId : CallerId<EmailAddress>
	{
		internal PersonaContactCallerId(EmailAddress emailAddress) : base(CallerIdTypeEnum.PersonaContact, emailAddress)
		{
			if (emailAddress == null)
			{
				throw new ArgumentNullException("emailAddress");
			}
			if (!SmtpAddress.IsValidSmtpAddress(emailAddress.Address))
			{
				throw new InvalidSmtpAddressException(emailAddress.Address);
			}
		}

		internal EmailAddress EmailAddress
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
				return PAAConstants.PersonaContactCallerIdEvaluationCost;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidatePersonaContactCallerId(this.EmailAddress.Address, out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		public override string ToString()
		{
			return this.EmailAddress.Address + ":" + this.EmailAddress.Name;
		}

		internal override bool Evaluate(IRuleEvaluator evaluator)
		{
			CallerIdRuleEvaluator callerIdRuleEvaluator = evaluator as CallerIdRuleEvaluator;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonaContactCallerId:Evaluate() MatchedPersonaEmails.Length == {0}", new object[]
			{
				callerIdRuleEvaluator.MatchedPersonaEmails.Count
			});
			if (callerIdRuleEvaluator.MatchedPersonaEmails != null && callerIdRuleEvaluator.MatchedPersonaEmails.Count > 0)
			{
				if (callerIdRuleEvaluator.MatchedPersonaEmails.Contains(this.EmailAddress.Address.ToLower()))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonaContactCallerId:Evaluate() Evaluation PASSED", new object[0]);
					return true;
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonaContactCallerId:Evaluate() Evaluation FAILED", new object[0]);
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "PersonaContactCallerId:Evaluate() Evaluation FAILED", new object[0]);
			return false;
		}
	}
}
