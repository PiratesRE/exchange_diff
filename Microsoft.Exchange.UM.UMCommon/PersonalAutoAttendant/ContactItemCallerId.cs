using System;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;
using Microsoft.Exchange.UM.UMCommon;

namespace Microsoft.Exchange.UM.PersonalAutoAttendant
{
	internal class ContactItemCallerId : CallerId<StoreObjectId>
	{
		internal ContactItemCallerId(StoreObjectId id) : base(CallerIdTypeEnum.ContactItem, id)
		{
			if (id == null)
			{
				throw new ArgumentNullException("id");
			}
		}

		internal StoreObjectId StoreObjectId
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
				return PAAConstants.ContactItemCallerIdEvaluationCost;
			}
		}

		public static ContactItemCallerId Parse(string representation)
		{
			byte[] entryId = Convert.FromBase64String(representation);
			StoreObjectId id = StoreObjectId.FromProviderSpecificId(entryId, StoreObjectType.Contact);
			return new ContactItemCallerId(id);
		}

		public override string ToString()
		{
			byte[] providerLevelItemId = this.StoreObjectId.ProviderLevelItemId;
			return Convert.ToBase64String(providerLevelItemId, Base64FormattingOptions.None);
		}

		public override bool Validate(IDataValidator validator)
		{
			IDataValidationResult dataValidationResult;
			bool result = validator.ValidateContactItemCallerId(this.StoreObjectId, out dataValidationResult);
			base.ValidationResult = dataValidationResult.PAAValidationResult;
			return result;
		}

		internal override bool Evaluate(IRuleEvaluator evaluator)
		{
			CallerIdRuleEvaluator callerIdRuleEvaluator = evaluator as CallerIdRuleEvaluator;
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactItemCallerId:Evaluate() MatchedPersonalContacts.Length == {0}", new object[]
			{
				callerIdRuleEvaluator.MatchedPersonalContacts.Length
			});
			if (callerIdRuleEvaluator.MatchedPersonalContacts.Length == 0)
			{
				return false;
			}
			for (int i = 0; i < callerIdRuleEvaluator.MatchedPersonalContacts.Length; i++)
			{
				PersonalContactInfo personalContactInfo = callerIdRuleEvaluator.MatchedPersonalContacts[i];
				CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactItemCallerId:Evaluate() Comparing this = \"{0}\" that = \"{1}\"", new object[]
				{
					this.StoreObjectId.ToBase64String(),
					personalContactInfo.Id
				});
				if (personalContactInfo.Id.Equals(this.StoreObjectId.ToBase64String()))
				{
					CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactItemCallerId:Evaluate() Evaluation PASSED", new object[0]);
					return true;
				}
			}
			CallIdTracer.TraceDebug(ExTraceGlobals.PersonalAutoAttendantTracer, this, "ContactItemCallerId:Evaluate() Evaluation FAILED", new object[0]);
			return false;
		}
	}
}
