using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.MessagingPolicies.Rules;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal class SentToScopePredicate : PredicateCondition
	{
		public ScopeType Type { get; private set; }

		public SentToScopePredicate(Property property, ScopeType type, RulesCreationContext creationContext) : base(property, new ShortList<string>(), creationContext)
		{
			this.Type = type;
		}

		public override string Name
		{
			get
			{
				return "SentToScope";
			}
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			return null;
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			BaseTransportRulesEvaluationContext baseTransportRulesEvaluationContext = (BaseTransportRulesEvaluationContext)baseContext;
			if (baseTransportRulesEvaluationContext == null)
			{
				throw new ArgumentException("context is either null or not of type: BaseTransportRulesEvaluationContext");
			}
			baseTransportRulesEvaluationContext.PredicateName = this.Name;
			ShortList<string> shortList = (ShortList<string>)base.Property.GetValue(baseTransportRulesEvaluationContext);
			List<string> matches = new List<string>();
			bool flag = false;
			if (shortList != null && shortList.Count > 0)
			{
				List<RoutingAddress> list = new List<RoutingAddress>(shortList.Count);
				foreach (string address in shortList)
				{
					if (RoutingAddress.IsValidAddress(address))
					{
						list.Add((RoutingAddress)address);
					}
				}
				switch (this.Type)
				{
				case ScopeType.Internal:
					flag = ADUtils.IsAnyInternal(list, ((OwaRulesEvaluationContext)baseTransportRulesEvaluationContext).OrganizationId, base.EvaluationMode, ref matches);
					break;
				case ScopeType.External:
					flag = ADUtils.IsAnyExternal(list, ((OwaRulesEvaluationContext)baseTransportRulesEvaluationContext).OrganizationId, base.EvaluationMode, ref matches);
					break;
				case ScopeType.ExternalPartner:
					flag = ADUtils.IsAnyExternalPartner(list, ((OwaRulesEvaluationContext)baseTransportRulesEvaluationContext).OrganizationId, base.EvaluationMode, ref matches);
					break;
				case ScopeType.ExternalNonPartner:
					flag = ADUtils.IsAnyExternalNonPartner(list, ((OwaRulesEvaluationContext)baseTransportRulesEvaluationContext).OrganizationId, base.EvaluationMode, ref matches);
					break;
				default:
					throw new ArgumentException(string.Format("this.Type:{0} is not a valid ScopeType.", this.Type));
				}
			}
			base.UpdateEvaluationHistory(baseContext, flag, matches, 0);
			return flag;
		}
	}
}
