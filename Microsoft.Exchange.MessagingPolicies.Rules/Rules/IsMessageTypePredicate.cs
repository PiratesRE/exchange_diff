using System;
using Microsoft.Exchange.Extensibility.Internal;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IsMessageTypePredicate : PredicateCondition
	{
		public IsMessageTypePredicate(ShortList<string> entries, RulesCreationContext creationContext) : base(null, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "isMessageType";
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return Value.CreateValue(typeof(string), entries);
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			string text = (string)base.Value.GetValue(transportRulesEvaluationContext);
			string key;
			switch (key = text)
			{
			case "OOF":
				return TransportUtils.IsOof(transportRulesEvaluationContext.MailItem.Message);
			case "AutoForward":
				return TransportUtils.IsAutoForward(transportRulesEvaluationContext.MailItem.Message);
			case "Encrypted":
				return TransportUtils.IsEncrypted(transportRulesEvaluationContext.MailItem.Message);
			case "Calendaring":
				return TransportUtils.IsCalendaring(transportRulesEvaluationContext.MailItem.Message);
			case "PermissionControlled":
				return TransportUtils.IsPermissionControlled(transportRulesEvaluationContext.MailItem);
			case "Voicemail":
				return TransportFacades.IsVoicemail(transportRulesEvaluationContext.MailItem.Message);
			case "Signed":
				return TransportUtils.IsSigned(transportRulesEvaluationContext.MailItem.Message);
			case "ApprovalRequest":
				return TransportUtils.IsApprovalRequest(transportRulesEvaluationContext.MailItem.Message);
			case "ReadReceipt":
				return TransportUtils.IsReadReceipt(transportRulesEvaluationContext.MailItem.Message);
			}
			return false;
		}
	}
}
