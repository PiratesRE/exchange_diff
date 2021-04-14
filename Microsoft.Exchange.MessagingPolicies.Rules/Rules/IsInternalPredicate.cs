using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Extensibility.Internal;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.MessagingPolicies.Rules
{
	internal class IsInternalPredicate : PredicateCondition
	{
		public IsInternalPredicate(Property property, ShortList<string> entries, RulesCreationContext creationContext) : base(property, entries, creationContext)
		{
		}

		public override string Name
		{
			get
			{
				return "isInternal";
			}
		}

		public override bool Evaluate(RulesEvaluationContext baseContext)
		{
			TransportRulesEvaluationContext transportRulesEvaluationContext = (TransportRulesEvaluationContext)baseContext;
			transportRulesEvaluationContext.PredicateName = this.Name;
			object value = base.Property.GetValue(transportRulesEvaluationContext);
			ITransportMailItemFacade transportMailItemFacade = TransportUtils.GetTransportMailItemFacade(transportRulesEvaluationContext.MailItem);
			OrganizationId orgId = (OrganizationId)transportMailItemFacade.OrganizationIdAsObject;
			List<string> list = new List<string>();
			IEnumerable<string> enumerable = value as IEnumerable<string>;
			bool flag = false;
			if (enumerable != null)
			{
				foreach (string text in enumerable)
				{
					if (IsInternalPredicate.IsInternal(transportRulesEvaluationContext.Server, text, orgId))
					{
						if (base.EvaluationMode == ConditionEvaluationMode.Optimized)
						{
							return true;
						}
						list.Add(text);
						flag = true;
					}
				}
				base.UpdateEvaluationHistory(baseContext, flag, list, 0);
				return flag;
			}
			flag = IsInternalPredicate.IsInternal(transportRulesEvaluationContext.Server, (string)value, orgId);
			if (base.EvaluationMode == ConditionEvaluationMode.Full)
			{
				if (flag)
				{
					list.Add((string)value);
				}
				base.UpdateEvaluationHistory(baseContext, flag, list, 0);
			}
			return flag;
		}

		protected override Value BuildValue(ShortList<string> entries, RulesCreationContext creationContext)
		{
			if (entries.Count != 0)
			{
				throw new RulesValidationException(RulesStrings.ValueIsNotAllowed(this.Name));
			}
			return null;
		}

		internal static bool IsInternal(SmtpServer server, string recipient, OrganizationId orgId)
		{
			if (string.IsNullOrEmpty(recipient))
			{
				return false;
			}
			RoutingAddress address = (RoutingAddress)recipient;
			AddressBookImpl addressBookImpl = server.AddressBook as AddressBookImpl;
			if (addressBookImpl != null)
			{
				return addressBookImpl.IsInternalTo(address, orgId, false);
			}
			return server.AddressBook.IsInternal(address);
		}
	}
}
