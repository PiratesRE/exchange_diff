using System;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class ADContactCallerId : CallerId<string>
	{
		internal ADContactCallerId(string legacyExchangeDN) : base(CallerIdTypeEnum.ADContact, legacyExchangeDN)
		{
			if (legacyExchangeDN == null)
			{
				throw new ArgumentNullException("legacyExchangeDN");
			}
		}

		internal string LegacyExchangeDN
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
				return PAAConstants.ADContactCallerIdEvaluationCost;
			}
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidateADContactCallerId(this.LegacyExchangeDN, out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		internal override bool Evaluate(IRuleEvaluator evaluator)
		{
			CallerIdRuleEvaluator callerIdRuleEvaluator = evaluator as CallerIdRuleEvaluator;
			PIIMessage piimessage = PIIMessage.Create(PIIType._PII, this.LegacyExchangeDN);
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "ADContactCallerId:Evaluate() ExchangeLegacyDN = \"PII\"", new object[0]);
			if (callerIdRuleEvaluator.MatchedADContact == null)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ADContactCallerId:Evaluate() There was no ADContact match for callerid. This evaluation will fail", new object[0]);
				return false;
			}
			ADContactInfo matchedADContact = callerIdRuleEvaluator.MatchedADContact;
			PIIMessage piimessage2 = PIIMessage.Create(PIIType._User, matchedADContact.LegacyExchangeDN);
			PIIMessage[] data = new PIIMessage[]
			{
				piimessage,
				piimessage2
			};
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, data, "ADContactCallerId:Evaluate() Comparing ExchangeLegacyDN = \"_PII\" with ADContact \"_User\"", new object[0]);
			IADRecipientLookup iadrecipientLookup = ADRecipientLookupFactory.CreateFromADRecipient(matchedADContact.ADOrgPerson, true);
			ADRecipient adrecipient = iadrecipientLookup.LookupByLegacyExchangeDN(this.LegacyExchangeDN);
			if (adrecipient != null && adrecipient.Guid == matchedADContact.ADOrgPerson.Guid)
			{
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "ADContactCallerId:Evaluate() PASSED ExchangeLegacyDN = \"_PII\"", new object[0]);
				return true;
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, piimessage, "ADContactCallerId:Evaluate() FAILED ExchangeLegacyDN = \"_PII\"", new object[0]);
			return false;
		}
	}
}
