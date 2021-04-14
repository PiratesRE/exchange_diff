using System;
using System.Collections.Generic;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Core.RuleTasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;

namespace Microsoft.Exchange.MessagingPolicies.Rules.Tasks
{
	[Serializable]
	public abstract class SmtpAddressesPredicate : TransportRulePredicate
	{
		protected SmtpAddressesPredicate(string internalPredicateName, string internalPropertyName)
		{
			this.internalPredicateName = internalPredicateName;
			this.internalPropertyName = internalPropertyName;
		}

		public abstract SmtpAddress[] Addresses { get; set; }

		internal override string Description
		{
			get
			{
				return this.LocalizedStringDescription(RuleDescription.BuildDescriptionStringFromStringArray(Utils.BuildSmtpAddressStringList(this.Addresses, false), RulesTasksStrings.RuleDescriptionOrDelimiter, base.MaxDescriptionListLength));
			}
		}

		protected abstract SmtpAddressesPredicate.LocalizedStringDescriptionDelegate LocalizedStringDescription { get; }

		internal static TransportRulePredicate CreateFromInternalCondition<T>(Condition condition, string internalPredicateName, string internalPropertyName) where T : SmtpAddressesPredicate, new()
		{
			if (condition.ConditionType != ConditionType.Predicate)
			{
				return null;
			}
			PredicateCondition predicateCondition = (PredicateCondition)condition;
			if (!predicateCondition.Name.Equals(internalPredicateName) || !predicateCondition.Property.Name.Equals(internalPropertyName))
			{
				return null;
			}
			List<SmtpAddress> list = new List<SmtpAddress>(predicateCondition.Value.RawValues.Count);
			for (int i = 0; i < predicateCondition.Value.RawValues.Count; i++)
			{
				SmtpAddress item = new SmtpAddress(predicateCondition.Value.RawValues[i]);
				if (item.IsValidAddress)
				{
					list.Add(item);
				}
			}
			T t = Activator.CreateInstance<T>();
			if (list.Count > 0)
			{
				t.Addresses = list.ToArray();
			}
			return t;
		}

		internal override void Reset()
		{
			this.addresses = null;
			base.Reset();
		}

		protected override void ValidateRead(List<ValidationError> errors)
		{
			if (this.Addresses == null || this.Addresses.Length == 0)
			{
				errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.ArgumentNotSet, base.Name));
				return;
			}
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				if (!smtpAddress.IsValidAddress)
				{
					errors.Add(new RulePhrase.RulePhraseValidationError(RulesTasksStrings.InvalidRecipient(smtpAddress.ToString()), base.Name));
					return;
				}
			}
			base.ValidateRead(errors);
		}

		internal override Condition ToInternalCondition()
		{
			ShortList<string> shortList = new ShortList<string>();
			foreach (SmtpAddress smtpAddress in this.Addresses)
			{
				string text = smtpAddress.ToString();
				if (string.IsNullOrEmpty(text))
				{
					throw new ArgumentException(RulesTasksStrings.InvalidRecipient(text), "Address");
				}
				shortList.Add(text);
			}
			return TransportRuleParser.Instance.CreatePredicate(this.internalPredicateName, TransportRuleParser.Instance.CreateProperty(this.internalPropertyName), shortList);
		}

		internal override string GetPredicateParameters()
		{
			return string.Join(", ", Utils.BuildSmtpAddressStringList(this.Addresses, true));
		}

		internal override void SuppressPiiData()
		{
			string[] array;
			string[] array2;
			this.Addresses = SuppressingPiiData.Redact(this.Addresses, out array, out array2);
		}

		private readonly string internalPredicateName;

		private readonly string internalPropertyName;

		protected SmtpAddress[] addresses;

		protected delegate LocalizedString LocalizedStringDescriptionDelegate(string addresses);
	}
}
